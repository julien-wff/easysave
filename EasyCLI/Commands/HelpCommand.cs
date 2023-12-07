using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;
using EasyCLI.Localization;

namespace EasyCLI.Commands;

public class HelpCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("help")
        .SetDescription(Loc.T("Commands.Help.Description"))
        .SetAliases(new[] { "h", "-h", "--help" })
        .AddArg(new CommandArg()
            .SetName("command")
            .SetDescription(Loc.T("Commands.Help.Args.Command.Description"))
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
                Console.WriteLine(Loc.T("Commands.CommandNotFound", commandName));
                return;
            }

            command.ShowHelp();
        }
        else
        {
            PrintGeneralHelp();
        }
    }

    private static void PrintGeneralHelp()
    {
        Console.WriteLine(Loc.T("Commands.Help.GeneralHelp.Header"));

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

            // ReSharper disable once LocalizableElement
            Console.Write($"  {commandName}");
            Console.Write(new string(' ', longestCommandName - commandName.Length + 2));
            Console.WriteLine(commandDescription);
        }
    }
}
