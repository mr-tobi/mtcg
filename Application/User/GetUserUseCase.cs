using Domain.Users;

namespace Application.User;

public class GetUserUseCase : IGetUserPort
{
    private readonly IUserRepository _userRepository;

    public GetUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public Domain.Users.User? GetUser(string username)
    {
        return _userRepository.Get(username);
    }
}
