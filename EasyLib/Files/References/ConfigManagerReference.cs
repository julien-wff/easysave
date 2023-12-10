using System.Diagnostics;
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
            WriteConfig();
        }
        else
        {
            ReadConfig();
        }
    }

    public List<string> CryptedFileTypes { get; set; } = new();
    public List<string> BusinnesProcesses { get; set; } = new();
    public string XorKey { get; set; } = "cryptokey";
    public string LogFormat { get; set; } = ".json";
    public string EasyCryptoPath { get; set; } = @"C:\EasyCrypto.exe";
    public string CompanySoftwareProcessPath { get; set; }

    /// <summary>
    /// Read the config file
    /// </summary>
    private void ReadConfig()
    {
        var jsonConfig = JsonFileUtils.ReadJson<ConfigElement>(_configFilePath);
        CryptedFileTypes = jsonConfig.CryptedFileTypes ?? new List<string>();
        BusinnesProcesses = jsonConfig.BusinnesProcesses ?? new List<string>();
        XorKey = jsonConfig.XorKey ?? "cryptokey";
        LogFormat = jsonConfig.LogFormat ?? ".json";
        EasyCryptoPath = jsonConfig.EasyCryptoPath ?? @"C:\EasyCrypto.exe";
        CompanySoftwareProcessPath = jsonConfig.CompanySoftwareProcessPath ?? "";
    }

    /// <summary>
    /// Write the config file
    /// </summary>
    public void WriteConfig()
    {
        var jsonConfig = new ConfigElement
        {
            CryptedFileTypes = CryptedFileTypes,
            BusinnesProcesses = BusinnesProcesses,
            XorKey = XorKey,
            LogFormat = LogFormat,
            EasyCryptoPath = EasyCryptoPath,
            CompanySoftwareProcessPath = CompanySoftwareProcessPath
        };
        JsonFileUtils.WriteJson(_configFilePath, jsonConfig);
    }

    public bool CheckProcessRunning()
    {
        if (string.IsNullOrEmpty(CompanySoftwareProcessPath))
        {
            return false;
        }

        var processName = Path.GetFileNameWithoutExtension(CompanySoftwareProcessPath);
        var processes = Process.GetProcessesByName(processName);
        return processes.Length > 0;
    }
}
