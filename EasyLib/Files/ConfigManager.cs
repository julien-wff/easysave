using EasyLib.Files.References;

namespace EasyLib.Files;

public abstract class ConfigManager
{
    private static ConfigManagerReference? _instance;

    private static readonly string
        ConfigFilePath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    // expose the singleton instance
    public static ConfigManagerReference Instance => _instance ??= new ConfigManagerReference(ConfigFilePath);
}
