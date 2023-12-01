using EasyCLI.Commands;

namespace EasyCLI.CommandRunner;

/// <summary>
/// The CommandRunner class is responsible for managing and executing commands.
/// </summary>
public sealed class CommandRunner
{
    /// <summary>
    /// Gets the collection of commands registered with the CommandRunner.
    /// </summary>
    public List<Command> Commands { get; } = new();

    /// <summary>
    /// Registers a command with the CommandRunner.
    /// </summary>
    /// <param name="command">The command to register.</param>
    /// <returns>The CommandRunner instance.</returns>
    public CommandRunner RegisterCommand(Command command)
    {
        Commands.Add(command);
        return this;
    }

    /// <summary>
    /// Runs the appropriate command based on the provided arguments.
    /// </summary>
    /// <param name="args">The arguments to determine which command to run.</param>
    /// <returns>True if the command was successfully run, false otherwise.</returns>
    public bool RunWithArgs(IEnumerable<string> args)
    {
        return false;
    }
}
