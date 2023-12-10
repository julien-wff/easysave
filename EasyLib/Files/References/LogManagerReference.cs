using EasyLib.Json;

namespace EasyLib.Files.References;

/// <summary>
/// Reference class for the LogManager singleton.
/// Manages the log file and the appending of logs
/// </summary>
public class LogManagerReference
{
    public readonly string LogFilePath;

    /// <summary>
    /// Create the instance of the singleton if it doesn't exist
    /// Create the log directory if it doesn't exist
    /// Create the log file if it doesn't exist
    /// </summary>
    public LogManagerReference(string appDataPath)
    {
        // AppData dir and append easysave/logs/
        var logDirectory = Path.Combine(appDataPath, "easysave", "logs");
        LogFilePath = Path.Combine(logDirectory,
            DateTime.Now.ToString("yyyy-MM-dd") + ConfigManager.Instance.LogFormat);

        // Create directory if it doesn't exist
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        // Create file and write [] if it doesn't exist
        if (!File.Exists(LogFilePath))
        {
            File.WriteAllText(LogFilePath, "");
        }
    }

    /// <summary>
    /// Append the log to the log file
    /// </summary>
    /// <param name="log"></param>
    public void AppendLog(LogElement log)
    {
        if (ConfigManager.Instance.LogFormat == ".xml")
        {
            XmlFileUtils.AddXmlLog(LogFilePath, log);
        }
        else
        {
            JsonFileUtils.AppendJsonToList(LogFilePath, log);
        }
    }
}
