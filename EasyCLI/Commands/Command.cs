namespace EasyCLI.Commands;

/// <summary>
/// Represents a command that can be executed by the application.
/// This is an abstract class that should be inherited by specific command classes.
/// </summary>
public abstract class Command
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CommandBuilder"/> class, that gives all the necessary information
    /// about the command.
    /// </summary>
    public abstract CommandBuilder.CommandBuilder Params { get; }

    /// <summary>
    /// Validates the arguments for the command.
    /// This method should be overridden in derived classes to provide specific validation logic.
    /// </summary>
    /// <returns>True if the arguments are valid, false otherwise.</returns>
    /// <param name="args">Arguments to validate.</param>
    public abstract bool ValidateArgs(List<string> args);

    /// <summary>
    /// Executes the command.
    /// This method should be overridden in derived classes to provide specific execution logic.
    /// </summary>
    /// <param name="args">Arguments to execute the command with.</param>
    public abstract void Run(List<string> args);
}
