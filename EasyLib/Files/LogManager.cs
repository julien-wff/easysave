using EasyLib.Json;

namespace EasyLib.Files;

/// <summary>
/// 
/// </summary>
public class LogManager
{
    public readonly string LogFilePath;

    /// <summary>
    /// Create the instance of the singleton if it doesn't exist
    /// Create the log directory if it doesn't exist
    /// Create the log file if it doesn't exist
    /// </summary>
    private LogManager()
    {
        // AppData dir and append easysave/logs/
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var stateDirectory = Path.Combine(appDataDir, "easysave", "logs");
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
    /// expose the singleton instance
    /// </summary>
    public static LogManager Instance { get; } = new LogManager();

    /// <summary>
    /// Append the log to the log file
    /// </summary>
    /// <param name="jsonLog"></param>
    public void AppendLog(JsonLogElement jsonLog)
    {
        JsonFileUtils.AppendJsonToList(LogFilePath, jsonLog);
    }
}
