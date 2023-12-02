using System.Reflection;

namespace EasyCLI.Commands;

public class VersionCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("version")
        .SetDescription("Get the version of the application")
        .SetAliases(new List<string> { "ver", "v", "--version", "-v" });

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        var argsList = args.ToList();
        var argsCount = argsList.Count;
        if (argsCount != 0)
        {
            Console.WriteLine($"Command expects 0 arguments, {argsCount} given.");
            ShowHelp();
            return false;
        }

        return true;
    }

    public override void Run(IEnumerable<string> args)
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        var os = Environment.OSVersion;
        Console.WriteLine($"EasySave CLI version {version} on {os.VersionString}");
    }
}
