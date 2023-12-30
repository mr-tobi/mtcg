using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Application.HTTPServer.Config;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.HTTPServer.Router.Util;
using Application.HTTPServer.Security;
using Domain.Exceptions;

namespace Application.HTTPServer;

public class Server
{
    private static Regex _requestRegex = new(@"^(GET|POST|PUT|DELETE)\s(\/[\w|\/|-]*)(?:\?(\S+))?\s(HTTPS?\/\S+)");

    public const string SupportedHttpProtocol = "HTTP/1.1";

    private ServerConfig _config;
    private SecurityChecker _securityChecker;

    public Server(ServerConfig config)
    {
        _config = config;
        _securityChecker = new SecurityChecker(config.Security);
    }

    public void Start()
    {
        var serverSocket = new TcpListener(IPAddress.Loopback, _config.Port);
        serverSocket.Start();
        Console.WriteLine("Started listening...");
        while (true)
        {
            HandleRequestAsync(serverSocket.AcceptTcpClient());
        }
    }

    private void HandleRequestAsync(TcpClient client)
    {
        Task.Run(() =>
        {
            try
            {
                HandleRequest(client);
            }
            finally
            {
                client.Dispose();
            }
        });
    }

    private void HandleRequest(TcpClient client)
    {
        using var reader = new StreamReader(client.GetStream());
        using var writer = new StreamWriter(client.GetStream()) { AutoFlush = true };
        var request = GetRequestFromStream(reader);

        if (request == null)
        {
            Console.WriteLine("Invalid request");
            return;
        }

        Console.WriteLine(request.ToString());

        if (request.ProtocolVersion != SupportedHttpProtocol)
        {
            Console.WriteLine($"HTTP version is not supported. Supported version: {SupportedHttpProtocol}");
            return;
        }

        writer.Write(BuildResponse(request).ToString());
    }

    private Response BuildResponse(Request request)
    {
        var responseBuilder = new Response.Builder();
        try
        {
            _securityChecker.CheckAllowed(request);
            var content = InvokeCallbackForRequest(request, out var responseContentType);
            if (content != null)
            {
                switch (content)
                {
                    case ResultWithRequestMetadata resultWithMetadata:
                        if (resultWithMetadata.Result != null)
                        {
                            responseBuilder.Content(JsonSerializer.Serialize(resultWithMetadata.Result));
                        }

                        responseBuilder
                            .StatusCode(resultWithMetadata.StatusCode)
                            .ContentType(resultWithMetadata.ContentType);
                        break;
                    default:
                        responseBuilder
                            .Content(JsonSerializer.Serialize(content))
                            .ContentType(responseContentType);
                        break;
                }
            }
        }
        catch (ExceptionWithStatusCode e)
        {
            responseBuilder
                .Reset()
                .StatusCode(e.StatusCode)
                .ContentType("text/plain")
                .Content(e.Message);
        }
        catch (NotFoundException e)
        {
            responseBuilder
                .Reset()
                .StatusCode(HttpStatusCode.NotFound)
                .ContentType("text/plain")
                .Content(e.Message);
        }
        catch (InvalidDataException e)
        {
            responseBuilder
                .Reset()
                .StatusCode(HttpStatusCode.BadRequest)
                .ContentType("text/plain")
                .Content(e.Message);
        }
        catch (System.Exception e)
        {
            responseBuilder
                .Reset()
                .StatusCode(HttpStatusCode.InternalServerError)
                .ContentType("text/plain")
                .Content(e.Message);
        }

        return responseBuilder.Build();
    }

    private object? InvokeCallbackForRequest(Request request, out string responseContentType)
    {
        var callbackInfo = Router.Router.GetCallbackInfoForRequest(request);
        responseContentType = callbackInfo.ContentType;
        try
        {
            return callbackInfo.Callback.Invoke(
                Activator.CreateInstance(callbackInfo.Callback.DeclaringType),
                GetCallbackParameter(request, callbackInfo)
            );
        }
        catch (TargetInvocationException e)
        {
            throw e.InnerException;
        }
    }

    private object?[] GetCallbackParameter(Request request, PathCallbackInfo callbackInfo)
    {
        var parameterList = new List<object?>();
        var paramAttributes = new List<Type>
            { typeof(RequestParamAttribute), typeof(RequestBodyAttribute), typeof(PathVariableAttribute) };
        foreach (var parameterInfo in callbackInfo.Callback.GetParameters())
        {
            if (parameterInfo.ParameterType == typeof(CurrentUser))
            {
                var token = SecurityConfig.TokenHandler.GetToken(request.GetToken());
                parameterList.Add(new CurrentUser(token.GetUsername()));
                continue;
            }

            var requestParamAttr = parameterInfo.GetCustomAttributes(false)
                .First(attribute => paramAttributes.Contains(attribute.GetType()));

            switch (requestParamAttr)
            {
                case RequestParamAttribute paramAttribute:
                {
                    request.RequestParameter.TryGetValue(
                        paramAttribute.Name ?? parameterInfo.Name,
                        out var requestParamValue
                    );
                    if (requestParamValue == null && !paramAttribute.Optional)
                    {
                        throw new InvalidDataException("Request parameter is missing");
                    }

                    parameterList.Add(requestParamValue);
                    break;
                }
                case RequestBodyAttribute:
                {
                    var requestBody = request.Content;
                    if (requestBody == null)
                    {
                        throw new InvalidDataException("Request body is missing");
                    }

                    object? content;
                    try
                    {
                        content = JsonSerializer.Deserialize(requestBody, parameterInfo.ParameterType);
                    }
                    catch (JsonException e)
                    {
                        throw new InvalidDataException("Request body has wrong data");
                    }

                    if (content == null)
                    {
                        throw new InvalidDataException("Request body is invalid");
                    }

                    parameterList.Add(content);
                    break;
                }
                case PathVariableAttribute pathVariable:
                {
                    var pathVariables = GetPathVariables(request, callbackInfo);
                    pathVariables.TryGetValue(pathVariable.Name ?? parameterInfo.Name, out var pathVariableValue);

                    if (pathVariableValue == null)
                    {
                        throw new InvalidDataException("Path variable is missing");
                    }

                    parameterList.Add(pathVariableValue);
                    break;
                }
            }
        }

        return parameterList.ToArray();
    }

    private Dictionary<string, string> GetPathVariables(Request request, PathCallbackInfo callbackInfo)
    {
        var requestPathParts = PathUtil.SplitPath(request.Target);
        var callbackPathParts = PathUtil.SplitPath(callbackInfo.Path);

        // This should never happen
        if (requestPathParts.Count != callbackPathParts.Count)
        {
            throw new InvalidOperationException("Request path and callback path are not the same");
        }

        var pathVariables = new Dictionary<string, string>();
        for (int i = 0; i < requestPathParts.Count; i++)
        {
            var callbackPart = callbackPathParts[i];
            var requestPart = requestPathParts[i];
            if (!callbackPart.StartsWith(':'))
            {
                continue;
            }

            pathVariables.Add(callbackPart.Substring(1), requestPart);
        }

        return pathVariables;
    }

    private string GetStringRequest(StreamReader reader)
    {
        var data = new StringBuilder();
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            data.Append(line + "\r\n");
            if (String.IsNullOrEmpty(line))
            {
                break;
            }
        }

        return data.ToString();
    }

    private Request? GetRequestFromStream(StreamReader requestReader)
    {
        var requestString = GetStringRequest(requestReader);

        if (String.IsNullOrEmpty(requestString))
        {
            return null;
        }

        var requestInfoMatches = _requestRegex.Match(requestString);

        if (!requestInfoMatches.Success)
        {
            return null;
        }

        var requestInfoValues = requestInfoMatches.Groups.Values.Skip(1).Select(group => group.Value).ToArray();
        var hasRequestParameter = requestInfoValues.Length == 4;

        var method = Enum.Parse<RequestMethod>(requestInfoValues[0]);
        var target = requestInfoValues[1];
        var requestParams = hasRequestParameter
            ? GetRequestParameter(requestInfoValues[2])
            : new Dictionary<string, string>();
        var version = requestInfoValues[hasRequestParameter ? 3 : 2];
        var headers = GetHeaders(requestString);
        return new Request(
            method,
            target,
            requestParams,
            version,
            headers,
            GetRequestBody(method, headers, requestReader)
        );
    }

    private Dictionary<string, string> GetRequestParameter(string parametersString)
    {
        return parametersString
            .Split('&')
            .Select(parameter => parameter.Split('='))
            .Where(pair => pair.Length == 2)
            .ToDictionary(pair => pair[0], pair => pair[1]);
    }

    private string? GetRequestBody(RequestMethod method, Dictionary<string, string> header, StreamReader reader)
    {
        if (method != RequestMethod.POST && method != RequestMethod.PUT)
        {
            return null;
        }

        header.TryGetValue("Content-Length", out var contentLengthString);
        if (contentLengthString == null)
        {
            return null;
        }

        var contentLength = int.Parse(contentLengthString);

        if (contentLength == 0)
        {
            return null;
        }

        var chars = new char[contentLength];
        var offset = 0;
        while (offset < contentLength)
        {
            int readBytes = reader.Read(chars, offset, contentLength - offset);
            offset += readBytes;

            if (readBytes == 0)
            {
                break;
            }
        }

        return new string(chars);
    }

    private Dictionary<string, string> GetHeaders(string requestString)
    {
        Dictionary<string, string> headers = new Dictionary<string, string>();
        var headerMatches = new Regex(@"(\S+):\s(.+)\r\n").Matches(requestString);

        if (headerMatches.Count <= 0)
        {
            return headers;
        }

        return headerMatches
            .Select(match => match.Groups.Values.Skip(1))
            .Select(groups =>
                groups
                    .Select(group => group.Value)
                    .ToArray()
            )
            .Where(values => values.Length == 2)
            .ToDictionary(values => values[0], values => values[1]);
    }
}
