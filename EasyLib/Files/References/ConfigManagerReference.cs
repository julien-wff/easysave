using EasyLib.Json;

namespace EasyLib.Files.References;

public class ConfigManagerReference
{
    private readonly string _configFilePath;

    /// <summary>
    /// Manage the config file
    /// </summary>
    /// <param name="appDataPath"></param>
    public ConfigManagerReference(string appDataPath)
    {
        // AppData dir and append easysave/logs/
        var configDirectory = Path.Combine(appDataPath, "easysave");
        _configFilePath = Path.Combine(configDirectory, "config.json");

        // Create directory if it doesn't exist
        if (!Directory.Exists(configDirectory))
        {
            Directory.CreateDirectory(configDirectory);
        }

        // Create file and write [] if it doesn't exist
        if (!File.Exists(_configFilePath))
        {
            JsonFileUtils.WriteJson(_configFilePath, new JsonConfig());
        }
        else
        {
            ReadConfig();
        }
    }

    public List<string> CryptedFileTypes { get; set; } = new();
    public List<string> BusinnesProcesses { get; set; } = new();

    /// <summary>
    /// Read the config file
    /// </summary>
    public void ReadConfig()
    {
        var jsonConfig = JsonFileUtils.ReadJson<JsonConfig>(_configFilePath);
        CryptedFileTypes = jsonConfig.CryptedFileTypes ?? new List<string>();
        BusinnesProcesses = jsonConfig.BusinnesProcesses ?? new List<string>();
    }

    /// <summary>
    /// Write the config file
    /// </summary>
    public void writeConfig()
    {
        var jsonConfig = new JsonConfig
        {
            CryptedFileTypes = CryptedFileTypes,
            BusinnesProcesses = BusinnesProcesses
        };
        JsonFileUtils.WriteJson(_configFilePath, jsonConfig);
    }
}
