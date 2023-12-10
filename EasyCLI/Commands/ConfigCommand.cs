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
            .SetName("setting")
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
        switch (argsList[1])
        {
            case "list":
                var settings = ConfigManager.Instance.GetStringProperties();
                Console.WriteLine(settings);
                break;
            case "CryptedFileTypes":
                if (argsList.Count == 2)
                {
                    Console.WriteLine(ConfigManager.Instance.CryptedFileTypes);
                }
                else
                {
                    ConfigManager.Instance.CryptedFileTypes = argsList[2].Split(',').ToList();
                    ConfigManager.Instance.WriteConfig();
                }

                break;
        }
    }
}
