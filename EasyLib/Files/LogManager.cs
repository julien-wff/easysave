using EasyLib.Files.References;

namespace EasyLib.Files;

/// <summary>
/// Write daily logs for file transfers
/// </summary>
public class LogManager : LogManagerReference
{
    private static LogManager? _instance;

    private LogManager() : base(AppDataPath)
    {
    }

    public static string AppDataPath { get; set; } =
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// Expose the singleton instance
    /// </summary>
    public static LogManager Instance => _instance ??= new LogManager();
}
