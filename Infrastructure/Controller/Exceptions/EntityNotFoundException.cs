using System.Net;
using Application.HTTPServer.Exception;

namespace Infrastructure.Controller.Exceptions;

public class EntityNotFoundException : ExceptionWithStatusCode
{
    public EntityNotFoundException(string entityType) : base($"{entityType} not found", HttpStatusCode.NotFound)
    {
    }
}
