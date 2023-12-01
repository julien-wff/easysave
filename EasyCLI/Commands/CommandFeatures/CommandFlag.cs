namespace EasyCLI.Commands.CommandFeatures;

/// <summary>
/// Represents a flag of a command. A flag is a special type of argument that is passed to a command,
/// of the form "command --flag".
/// </summary>
public class CommandFlag : CommandArg
{
    /// <summary>
    /// Aliases of the flag. These are alternative ways to call the flag.
    /// </summary>
    public List<string> Aliases = new();

    /// <summary>
    /// Short hands of the flag. These are alternative ways to call the flag in one character (eg: -f).
    public List<string> ShortHands = new();

    /// <summary>
    /// Sets the short hands of the flag. These are alternative ways to call the flag in one character (eg: -f).
    /// </summary>
    /// <param name="shortHands">The short hands to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public CommandFlag SetShortHands(List<string> shortHands)
    {
        ShortHands.Clear();
        ShortHands.AddRange(shortHands);
        return this;
    }

    /// <summary>
    /// Sets the aliases of the flag. These are alternative ways to call the flag.
    /// </summary>
    /// <param name="alias">The alias to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public CommandFlag SetAliases(string alias)
    {
        Aliases.Clear();
        Aliases.Add(alias);
        return this;
    }
}
