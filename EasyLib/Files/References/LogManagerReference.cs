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
        var stateDirectory = Path.Combine(appDataPath, "easysave", "logs");
        LogFilePath = Path.Combine(stateDirectory, DateTime.Now.ToString("yyyy-MM-dd") + ".json");

        // Create directory if it doesn't exist
        if (!Directory.Exists(stateDirectory))
        {
            Directory.CreateDirectory(stateDirectory);
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
    /// <param name="jsonLog"></param>
    public void AppendLog(JsonLogElement jsonLog)
    {
        JsonFileUtils.AppendJsonToList(LogFilePath, jsonLog);
    }
}
