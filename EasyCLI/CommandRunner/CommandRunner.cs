using EasyCLI.Commands;

namespace EasyCLI.CommandRunner;

/// <summary>
/// The CommandRunner class is responsible for managing and executing commands.
/// </summary>
public sealed class CommandRunner
{
    /// <summary>
    /// Contains the singleton instance of the CommandRunner.
    /// </summary>
    private static CommandRunner? _instance;

    /// <summary>
    /// Makes the constructor private to prevent instantiation outside of the class.
    /// </summary>
    private CommandRunner()
    {
    }

    /// <summary>
    /// Gets the collection of commands registered with the CommandRunner.
    /// </summary>
    public List<Command> Commands { get; } = new();

    /// <summary>
    /// Gets the singleton instance of the CommandRunner. If the instance does not exist, it is created.
    /// </summary>
    /// <returns>CommandRunner instance</returns>
    public static CommandRunner GetInstance()
    {
        return _instance ??= new CommandRunner();
    }

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

    public Command? GetCommandFromArgs(IEnumerable<string> args)
    {
        return null;
    }
}
