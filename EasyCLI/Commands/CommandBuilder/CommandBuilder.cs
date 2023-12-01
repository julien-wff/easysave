using EasyCLI.Commands.CommandFeatures;

namespace EasyCLI.Commands.CommandBuilder;

/// <summary>
/// Provides a builder for creating commands. Used by the CommandRunner and the help commands.
/// </summary>
public sealed class CommandBuilder
{
    /// <summary>
    /// Aliases of the command (other keywords to call the command).
    /// </summary>
    public List<string> Aliases = new();

    /// <summary>
    /// Arguments of the command (dynamic params after the name).
    /// </summary>
    public List<CommandArg> Args = new();

    /// <summary>
    /// Description of the command. Printed in the help command.
    /// </summary>
    public string Description = string.Empty;

    /// <summary>
    /// Flags of the command (--flag).
    /// </summary>
    public List<CommandFlag> Flags = new();

    /// <summary>
    /// Name of the command. This is the keyword to call the command.
    /// </summary>
    public string Name = string.Empty;

    /// <summary>
    /// Sub-commands of the command. Subcommands are nested commands.
    /// Eg: easysave save help, help is a sub-command of save.
    /// </summary>
    public List<Command> SubCommands = new();

    /// <summary>
    /// Sets the name of the command. This is the keyword to call the command.
    /// </summary>
    /// <param name="name">The name to set.</param>
    /// <returns>The CommandBuilder instance.</returns>
    public CommandBuilder SetName(string name)
    {
        Name = name;
        return this;
    }

    /// <summary>
    /// Sets the description of the command. Printed in the help command.
    /// </summary>
    /// <param name="description">The description to set.</param>
    /// <returns>The CommandBuilder instance.</returns>
    public CommandBuilder SetDescription(string description)
    {
        Description = description;
        return this;
    }

    /// <summary>
    /// Sets the aliases of the command. Aliases are other keywords to call the command.
    /// </summary>
    /// <param name="aliases">The aliases to set.</param>
    /// <returns>The CommandBuilder instance.</returns>
    public CommandBuilder SetAliases(List<string> aliases)
    {
        Aliases.Clear();
        Aliases.AddRange(aliases);
        return this;
    }

    /// <summary>
    /// Adds an argument to the command. Arguments are dynamic params after the name.
    /// </summary>
    /// <param name="arg">The argument to add.</param>
    /// <returns>The CommandBuilder instance.</returns>
    public CommandBuilder AddArg(CommandArg arg)
    {
        Args.Add(arg);
        return this;
    }

    /// <summary>
    /// Adds a flag to the command. Flags are --flag.
    /// </summary>
    /// <param name="flag">The flag to add.</param>
    /// <returns>The CommandBuilder instance.</returns>
    public CommandBuilder AddFlag(CommandFlag flag)
    {
        Flags.Add(flag);
        return this;
    }

    /// <summary>
    /// Adds a sub-command to the command. Subcommands are nested commands.
    /// Eg: easysave save help, help is a sub-command of save.
    /// </summary>
    /// <param name="subCommand">The sub-command to add.</param>
    /// <returns>The CommandBuilder instance.</returns>
    public CommandBuilder AddSubCommand(Command subCommand)
    {
        SubCommands.Add(subCommand);
        return this;
    }
}
