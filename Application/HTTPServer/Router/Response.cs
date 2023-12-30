using System.Net;

namespace Application.HTTPServer.Router;

internal class Response
{
    public string ProtocolVersion { get; }
    public HttpStatusCode StatusCode { get; }
    public DateTime Timestamp { get; }
    public string ContentType { get; }
    public string Content { get; }

    public Response(string protocolVersion, HttpStatusCode statusCode, DateTime timestamp, string contentType,
        string content)
    {
        ProtocolVersion = protocolVersion;
        StatusCode = statusCode;
        Timestamp = timestamp;
        ContentType = contentType;
        Content = content;
    }

    public override string ToString()
    {
        string[] rows =
        {
            $"{ProtocolVersion} {StatusCode:D}",
            $"Date: {Timestamp}",
            $"Content-Length: {Content.Length}",
            $"Content-Type: {ContentType}",
            "",
            $"{Content}",
        };
        return String.Join("\r\n", rows);
    }

    public class Builder
    {
        private string _protocolVersion = Server.SupportedHttpProtocol;
        private HttpStatusCode _statusCode { get; set; } = HttpStatusCode.OK;
        private string _contentType { get; set; } = "application/json";
        private string? _content { get; set; }

        public Response Build()
        {
            return new Response(_protocolVersion, _statusCode, DateTime.Now, _contentType, _content ?? "");
        }

        public Builder Reset()
        {
            var defaultBuilder = new Builder();
            _protocolVersion = defaultBuilder._protocolVersion;
            _statusCode = defaultBuilder._statusCode;
            _contentType = defaultBuilder._contentType;
            _content = defaultBuilder._content;
            return this;
        }

        public Builder StatusCode(HttpStatusCode statusCode)
        {
            _statusCode = statusCode;
            return this;
        }

        public Builder Content(string content)
        {
            _content = content;
            return this;
        }

        public Builder ContentType(string contentType)
        {
            _contentType = contentType;
            return this;
        }
    }
}