using System.Net;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Package;
using Infrastructure.Controller.DTO;
using Infrastructure.Repository;
using Infrastructure.Repository.Exceptions;

namespace Infrastructure.Controller;

[Controller("/packages")]
public class CreatePackageController
{
    private readonly ICreatePackagePort _createPackagePort;

    public CreatePackageController()
    {
        var connection = PgRepository.CreateAndOpenConnection();
        _createPackagePort = new CreatePackageUseCase(new PackageRepository(connection));
    }

    [RequestMapping(RequestMethod.POST)]
    public void CreatePackage([RequestBody] List<CardDTO> cards)
    {
        try
        {
            _createPackagePort.CreatePackage(cards.Select(card => new CardData(card.Id, card.Name, card.Damage))
                .ToList());
        }
        catch (AlreadyExistsException)
        {
            throw new ExceptionWithStatusCode(
                "At least one card in the packages already exists",
                HttpStatusCode.Conflict
            );
        }
    }
}