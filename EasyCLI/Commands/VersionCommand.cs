using System.Reflection;
using EasyCLI.Localization;

namespace EasyCLI.Commands;

public class VersionCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("version")
        .SetDescription(Loc.T("Commands.Version.Description"))
        .SetAliases(new List<string> { "ver", "v", "--version", "-v" });

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        var argsList = args.ToList();
        var argsCount = argsList.Count;
        if (argsCount != 0)
        {
            Console.WriteLine(Loc.T("Command.WrongArgumentCount", 0, argsCount, "version"));
            ShowHelp();
            return false;
        }

        return true;
    }

    public override void Run(IEnumerable<string> args)
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version?.ToString() ?? "Unknown";
        var os = Environment.OSVersion;
        Console.WriteLine(Loc.T("Commands.Version.Version", version, os.VersionString));
    }
}
