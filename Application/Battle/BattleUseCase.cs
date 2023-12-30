using Domain.Battles;
using Domain.Users;

namespace Application.Battle;

public class BattleUseCase : IBattlePort
{
    private static Task<Task<string[]>>? _currentBattleTask;
    private static TaskCompletionSource<Domain.Users.User>? _queueEvent;
    private static readonly object QueueLock = new();

    private readonly IBattleService _battleService;
    private readonly IUserRepository _userRepository;

    public BattleUseCase(IBattleService battleService, IUserRepository userRepository)
    {
        _battleService = battleService;
        _userRepository = userRepository;
    }

    private Task<Task<string[]>> QueueUserAndGetBattleTask(Domain.Users.User user)
    {
        lock (QueueLock)
        {
            if (_currentBattleTask == null)
            {
                _queueEvent = new TaskCompletionSource<Domain.Users.User>();
                return _currentBattleTask = Task.Factory.StartNew(async () =>
                {
                    var secondUser = await _queueEvent.Task;

                    if (secondUser.Equals(user))
                    {
                        throw new InvalidOperationException("User cannot battle against himself");
                    }

                    return _battleService.StartBattle(user, secondUser).ToArray();
                });
            }

            var currentBattleTask = _currentBattleTask;
            _currentBattleTask = null;
            _queueEvent.SetResult(user);
            return currentBattleTask;
        }
    }

    public string[] QueueForBattle(string username)
    {
        var user = _userRepository.GetOrElseThrow(username);
        if (user.Deck.Count != Domain.Users.User.DeckSize)
        {
            throw new InvalidOperationException("Deck of user is not complete");
        }

        try
        {
            return QueueUserAndGetBattleTask(user).Result.Result;
        }
        catch (AggregateException e)
        {
            if (e.InnerException != null)
            {
                throw e.InnerException;
            }

            throw new Exception($"Something went wrong: {e.Message}");
        }
    }
}