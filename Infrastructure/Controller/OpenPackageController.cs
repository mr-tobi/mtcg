using System.Net;
using Application.HTTPServer;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Package;
using Domain.Cards;
using Domain.Users;
using Domain.Users.Exceptions;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("")]
public class OpenPackageController
{
    private readonly IOpenPackagePort _openPackagePort;

    public OpenPackageController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        var userRepo = new UserRepository(connection);
        var packageRepo = new PackageRepository(connection);
        var userService = new UserService(userRepo, packageRepo);
        var packageService = new PackageService(packageRepo);
        _openPackagePort = new OpenPackageUseCase(userService, userRepo, packageService);
    }

    [RequestMapping(RequestMethod.POST, "/transactions/packages")]
    public void OpenPackage(CurrentUser currentUser)
    {
        try
        {
            _openPackagePort.OpenPackage(currentUser.Username);
        }
        catch (NotEnoughMoneyException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.Forbidden);
        }
    }
}
