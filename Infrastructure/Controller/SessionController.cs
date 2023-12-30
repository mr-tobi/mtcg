using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Security;
using Infrastructure.Controller.DTO;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("/sessions")]
public class SessionController
{
    [RequestMapping(RequestMethod.POST, contentType: "text/plain")]
    public string Login([RequestBody] LoginDTO dto)
    {
        var loginPort = new LoginUseCase(new UserWithAuthenticationRepository(PgRepository.CreateAndOpenConnection()));
        return loginPort.Login(new LoginData(dto.Username, dto.Password));
    }
}
