using Domain.Users;

namespace Application.User;

public class UpdateUserUseCase : IUpdateUserPort
{
    private readonly IUserRepository _userRepository;

    public UpdateUserUseCase(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public void UpdateUser(string username, UpdatableUserData userData)
    {
        var user = _userRepository.GetOrElseThrow(username);
        user.UpdateUser(userData);
        _userRepository.Update(user);
    }
}
