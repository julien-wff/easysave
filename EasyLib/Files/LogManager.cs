using EasyLib.Json;

namespace EasyLib.Files;

public class LogManager
{
    public readonly string LogFilePath;

    private LogManager()
    {
        // AppData dir and append easysave/state.json
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

    public static LogManager Instance { get; } = new LogManager();

    public void AppendLog(JsonLogElement jsonLog)
    {
        JsonFileUtils.AppendJsonToList(LogFilePath, jsonLog);
    }
}
