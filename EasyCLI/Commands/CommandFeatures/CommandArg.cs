using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasyCLI.Commands.CommandFeatures;

/// <summary>
/// Represents an argument of a command. An argument is a value that is passed to a command,
/// of the form "command arg1 arg2 arg3".
/// </summary>
public class CommandArg
{
    /// <summary>
    /// Default value of the argument. Used if the argument is not required and not provided.
    /// </summary>
    public object? Default;

    /// <summary>
    /// Description of the argument. Printed in the help message.
    /// </summary>
    public string Description = String.Empty;

    /// <summary>
    /// Name of the argument. Printed in the help message.
    /// </summary>
    public string Name = String.Empty;

    /// <summary>
    /// Whether the argument is required or not.
    /// </summary>
    public bool Required;

    /// <summary>
    /// Type of the argument (string, number, path). Used to validate and parse the argument.
    /// </summary>
    public CommandArgType.CommandArgType Type = new CommandArgTypeString();

    /// <summary>
    /// Sets the name of the argument. This is printed in the help message.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The CommandArg instance.</returns>
    public CommandArg SetName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description of the argument. This is printed in the help message.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The CommandArg instance.</returns>
    public CommandArg SetDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    /// Sets the type of the argument. This is used to validate and parse the argument.
    /// </summary>
    /// <param name="type">The CommandArgType to set.</param>
    /// <returns>The CommandArg instance.</returns>
    public CommandArg SetType(CommandArgType.CommandArgType type)
    {
        Type = type;
        return this;
    }

    /// <summary>
    /// Sets whether the argument is required or not.
    /// </summary>
    /// <param name="required">The required status to set.</param>
    /// <returns>The CommandArg instance.</returns>
    public CommandArg SetRequired(bool required)
    {
        Required = required;
        return this;
    }

    /// <summary>
    /// Sets the default value of the argument. This is used if the argument is not required and not provided.
    /// </summary>
    /// <param name="defaultValue">The default value to set.</param>
    /// <returns>The CommandArg instance.</returns>
    public CommandArg SetDefault(object defaultValue)
    {
        Default = defaultValue;
        return this;
    }
}
