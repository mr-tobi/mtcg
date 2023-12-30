namespace Domain.Battles;

public interface IBattleRoundLogBuilder
{
    /// <summary>
    /// Append message to log builder
    /// </summary>
    /// <param name="message">Message to append</param>
    void Append(string message);

    /// <summary>
    /// Build the log string and clear to builder.
    /// </summary>
    /// <returns>The build log</returns>
    string Build();
}