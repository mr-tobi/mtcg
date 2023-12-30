using Application.HTTPServer.Config;
using Application.HTTPServer.Exception;

namespace Application.Security;

public class LoginUseCase : ILoginPort
{
    private readonly IUserWithAuthenticationRepository _userWithAuthenticationRepository;

    public LoginUseCase(IUserWithAuthenticationRepository userWithAuthenticationRepository)
    {
        _userWithAuthenticationRepository = userWithAuthenticationRepository;
    }

    public string Login(LoginData loginData)
    {
        var passwordHash = _userWithAuthenticationRepository.GetPasswordHashForUser(loginData.Username);

        if (passwordHash == null || loginData.PasswordHash() != passwordHash)
        {
            throw new UnauthorizedException();
        }

        var role = _userWithAuthenticationRepository.GetRoleOfUser(loginData.Username);
        return SecurityConfig.TokenHandler.GenerateToken(loginData.Username, role ?? SecurityConfig.DefaultRoleName);
    }
}