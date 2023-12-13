using System.Diagnostics;
using System.Globalization;
using System.Text;
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

    public CultureInfo Language { get; set; } = CultureInfo.CurrentCulture;
    public List<string> EncryptedFileExtensions { get; set; } = [];
    public List<string> PriorityFileExtensions { get; set; } = [];
    public string XorKey { get; set; } = GenerateRandomKey();
    public string LogFormat { get; set; } = ".json";
    public string? EasyCryptoPath { get; set; }
    public string? CompanySoftwareProcessPath { get; set; }
    public ulong MaxFileSize { get; set; } = 1000000;

    private static string GenerateRandomKey()
    {
        var random = new Random();
        var sb = new StringBuilder();
        for (var i = 0; i < 63; i++)
        {
            var c = random.Next(48, 48 + 62);
            // First 10 are numbers
            // Then we have 7 unwanted chars, followed by 26 uppercase letters
            // Then we have 6 unwanted chars, followed by 26 lowercase letters
            if (c >= 58)
            {
                c += 7;
            }

            if (c >= 91)
            {
                c += 6;
            }

            sb.Append((char)c);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Read the config file
    /// </summary>
    private void ReadConfig()
    {
        var jsonConfig = JsonFileUtils.ReadJson<ConfigElement>(_configFilePath);

        var xorKey = jsonConfig.XorKey;

        EncryptedFileExtensions = jsonConfig.EncryptedFileExtensions ?? [];
        XorKey = jsonConfig.XorKey ?? GenerateRandomKey();
        LogFormat = jsonConfig.LogFormat ?? ".json";
        EasyCryptoPath = jsonConfig.EasyCryptoPath;
        CompanySoftwareProcessPath = jsonConfig.CompanySoftwareProcessPath;
        Language = CultureInfo.GetCultureInfo(jsonConfig.Language);
        MaxFileSize = jsonConfig.MaxFileSize;
        PriorityFileExtensions = jsonConfig.PriorityFileExtensions;

        // If the key was null, write the new key
        if (xorKey == null)
        {
            WriteConfig();
        }
    }

    /// <summary>
    /// Write the config file
    /// </summary>
    public void WriteConfig()
    {
        var jsonConfig = new ConfigElement
        {
            EncryptedFileExtensions = EncryptedFileExtensions,
            PriorityFileExtensions = PriorityFileExtensions,
            XorKey = XorKey,
            LogFormat = LogFormat,
            EasyCryptoPath = EasyCryptoPath,
            CompanySoftwareProcessPath = CompanySoftwareProcessPath,
            Language = Language.ToString(),
            MaxFileSize = MaxFileSize
        };
        JsonFileUtils.WriteJson(_configFilePath, jsonConfig);
    }

    /// <summary>
    /// Checks if the company software is running
    /// </summary>
    /// <returns>True if it's running</returns>
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

    public string GetStringProperties()
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Language: {Language}");
        sb.AppendLine($"EncryptedFileExtensions: {string.Join(", ", EncryptedFileExtensions)}");
        sb.AppendLine($"PriorityFileExtensions: {string.Join(", ", PriorityFileExtensions)}");
        sb.AppendLine($"XorKey: {XorKey}");
        sb.AppendLine($"LogFormat: {LogFormat}");
        sb.AppendLine($"EasyCryptoPath: {EasyCryptoPath ?? "<null>"}");
        sb.AppendLine($"CompanySoftwareProcessPath: {CompanySoftwareProcessPath ?? "<null>"}");
        return sb.ToString();
    }
}
