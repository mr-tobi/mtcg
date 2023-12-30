using System.Net;
using Application.HTTPServer;
using Application.HTTPServer.Exception;
using Application.HTTPServer.Router;
using Application.HTTPServer.Router.Attributes;
using Application.Security;
using Application.User;
using Domain.Users;
using Infrastructure.Controller.DTO;
using Infrastructure.Controller.Exceptions;
using Infrastructure.Repository;

namespace Infrastructure.Controller;

[Controller("users")]
public class UserController
{
    [RequestMapping(RequestMethod.POST)]
    public ResultWithRequestMetadata Register([RequestBody] RegisterDTO dto)
    {
        IRegisterPort registerPort =
            new RegisterUseCase(new UserWithAuthenticationRepository(PgRepository.CreateAndOpenConnection()));
        try
        {
            return new ResultWithRequestMetadata(
                registerPort.Register(new RegisterData(dto.Username, dto.Password, dto.Role)),
                HttpStatusCode.Created,
                "text/plain"
            );
        }
        catch (UsernameAlreadyExistsException e)
        {
            throw new ExceptionWithStatusCode(e.Message, HttpStatusCode.Conflict);
        }
    }

    [RequestMapping(RequestMethod.GET, "/:username")]
    public UserDTO GetUser([PathVariable] string username, CurrentUser currentUser)
    {
        AssertAccessUserAllowed(username, currentUser);
        IGetUserPort getUserPort = new GetUserUseCase(new UserRepository(PgRepository.CreateAndOpenConnection()));

        var user = getUserPort.GetUser(username) ?? throw new EntityNotFoundException("User");

        return new UserDTO(user.DisplayName, user.Coins, user.Bio, user.Image);
    }

    [RequestMapping(RequestMethod.PUT, "/:username")]
    public void UpdateUser([PathVariable] string username, [RequestBody] UpdateUserDTO userData, CurrentUser currentUser)
    {
        AssertAccessUserAllowed(username, currentUser);
        IUpdateUserPort updateUserPort = new UpdateUserUseCase(
            new UserRepository(PgRepository.CreateAndOpenConnection())
        );

        updateUserPort.UpdateUser(username, new UpdatableUserData(userData.Name, userData.Bio, userData.Image));
    }

    private void AssertAccessUserAllowed(string usernameToAccess, CurrentUser currentUser)
    {
        if (usernameToAccess != currentUser.Username) throw new UnauthorizedException();
    }
}
