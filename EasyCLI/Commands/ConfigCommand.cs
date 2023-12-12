using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;
using EasyCLI.Localization;
using EasyLib.Files;

namespace EasyCLI.Commands;

public class ConfigCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("config")
        .SetDescription(Loc.T("Commands.Config.Description"))
        .SetAliases(new[] { "conf", "cfg", "c" })
        .AddArg(new CommandArg()
            .SetName("show")
            .SetDescription(Loc.T("Commands.Config.Args.Key.Description"))
            .SetType(new CommandArgTypeString())
            .SetRequired(true));

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();
        if (argsList.Count > 2)
        {
            Console.WriteLine(Loc.T("Commands.Config.Args.Key.TooManyArgs"));
            return;
        }

        if (argsList.Count == 1)
        {
            Console.WriteLine(Loc.T("Not enough arguments. See 'easysave help config' for more information."));
            return;
        }

        switch (argsList[1])
        {
            case "show":
                var settings = ConfigManager.Instance.GetStringProperties();
                Console.WriteLine("Config path: " + Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                    "easysave",
                    "config.json"
                ));
                Console.WriteLine(settings);
                break;
            default:
                Console.WriteLine("Invalid argument. See 'easysave help config' for more information.");
                break;
        }
    }
}
