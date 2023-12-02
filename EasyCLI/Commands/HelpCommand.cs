using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;

namespace EasyCLI.Commands;

public class HelpCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("help")
        .SetDescription("Display help on other commands")
        .SetAliases(new[] { "h", "-h", "--help" })
        .AddArg(new CommandArg()
            .SetName("command")
            .SetDescription("Display help of a specific command")
            .SetType(new CommandArgTypeString())
            .SetRequired(false));

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();

        if (argsList.Count == 2)
        {
            var commandName = argsList[1];
            var command = CommandRunner
                .CommandRunner
                .GetInstance()
                .GetCommandFromArgs(argsList.GetRange(1, 1));

            if (command is null)
            {
                Console.WriteLine($"Command {commandName} not found. Try 'easysave help' for more information.");
                return;
            }

            command.ShowHelp();
        }
        else
        {
            PrintGeneralHelp();
        }
    }

    private void PrintGeneralHelp()
    {
        Console.WriteLine("EasySave CLI Help Page");
        Console.WriteLine();
        Console.WriteLine("Usage: easysave [command] [arguments]");
        Console.WriteLine();
        Console.WriteLine("A CLI tool to backup your files");
        Console.WriteLine();
        Console.WriteLine("Available commands:");

        var commands = CommandRunner
            .CommandRunner
            .GetInstance()
            .Commands;

        var longestCommandName = commands
            .Select(command => command.Params.Name.Length)
            .Max();

        foreach (var cmdParams in commands.Select(cmd => cmd.Params))
        {
            var commandName = cmdParams.Name;
            var commandDescription = cmdParams.Description;

            Console.Write($"  {commandName}");
            Console.Write(new string(' ', longestCommandName - commandName.Length + 2));
            Console.WriteLine(commandDescription);
        }
    }
}
