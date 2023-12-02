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
    /// </summary>
    public List<string> ShortHands = new();

    /// <summary>
    /// Sets the name of the argument. This is printed in the help message.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public new CommandFlag SetName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description of the argument. This is printed in the help message.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public new CommandFlag SetDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    /// Sets the type of the argument. This is used to validate and parse the argument.
    /// </summary>
    /// <param name="type">The CommandArgType to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public new CommandFlag SetType(CommandArgType.CommandArgType type)
    {
        Type = type;
        return this;
    }

    /// <summary>
    /// Sets whether the argument is required or not.
    /// </summary>
    /// <param name="required">The required status to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public new CommandFlag SetRequired(bool required)
    {
        Required = required;
        return this;
    }

    /// <summary>
    /// Sets the default value of the argument. This is used if the argument is not required and not provided.
    /// </summary>
    /// <param name="defaultValue">The default value to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public new CommandFlag SetDefault(object defaultValue)
    {
        Default = defaultValue;
        return this;
    }

    /// <summary>
    /// Sets the short hands of the flag. These are alternative ways to call the flag in one character (eg: -f).
    /// </summary>
    /// <param name="shortHands">The short hands to set.</param>
    /// <returns>The CommandFlag instance.</returns>
    public CommandFlag SetShortHands(IEnumerable<string> shortHands)
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
    public CommandFlag SetAliases(IEnumerable<string> alias)
    {
        Aliases.Clear();
        Aliases.AddRange(alias);
        return this;
    }
}
