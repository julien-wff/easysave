namespace EasyCLI.Commands.CommandFeatures.CommandArgType;

/// <summary>
/// Represents the type of an argument of a command. The type is used to validate and parse the argument.
/// </summary>
public abstract class CommandArgType(string name)
{
    /// <summary>
    /// Name of the argument type. This is used to identify the type.
    /// </summary>
    public readonly string Name = name;

    /// <summary>
    /// Raw value of the argument. This is the value before it is parsed.
    /// </summary>
    public string RawValue = String.Empty;

    /// <summary>
    /// Checks if the raw value is valid for this type.
    /// </summary>
    /// <returns>True if the raw value is valid, false otherwise.</returns>
    public abstract bool CheckValue();

    /// <summary>
    /// Parses the raw value to the specific type.
    /// </summary>
    /// <returns>The parsed value.</returns>
    public abstract object ParseValue();
}
