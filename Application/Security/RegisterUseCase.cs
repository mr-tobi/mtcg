using Application.HTTPServer.Config;

namespace Application.Security;

public class RegisterUseCase : IRegisterPort
{
    private readonly IUserWithAuthenticationRepository _userWithAuthenticationRepository;

    public RegisterUseCase(IUserWithAuthenticationRepository userWithAuthenticationRepository)
    {
        _userWithAuthenticationRepository = userWithAuthenticationRepository;
    }

    public string Register(RegisterData registerData)
    {
        var username = registerData.Username;
        if (_userWithAuthenticationRepository.Exists(registerData.Username))
        {
            throw new UsernameAlreadyExistsException(username);
        }

        var user = _userWithAuthenticationRepository.ExecuteInTransaction(() => _userWithAuthenticationRepository.Save(
            new UserWithAuthentication(
                registerData.Password.PasswordHash(),
                registerData.Role,
                new Domain.Users.User(username)
            )
        ));

        return SecurityConfig.TokenHandler.GenerateToken(user.User.Username, user.Role);
    }
}
