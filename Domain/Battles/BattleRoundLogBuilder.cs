using System.Text;

namespace Domain.Battles;

public class BattleRoundLogBuilder : IBattleRoundLogBuilder
{
    private readonly StringBuilder _logBuilder = new();

    public void Append(string message)
    {
        _logBuilder.Append($"{(_logBuilder.Length <= 0 ? "" : " => ")}{message}");
    }

    public string Build()
    {
        var result = _logBuilder.ToString();
        _logBuilder.Clear();
        return result;
    }
}
