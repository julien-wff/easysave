using EasyLib.Files.References;

namespace EasyLib.Files;

/// <summary>
/// Write daily logs for file transfers
/// </summary>
public abstract class LogManager
{
    private static LogManagerReference? _instance;
    private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// Expose the singleton instance
    /// </summary>
    public static LogManagerReference Instance => _instance ??= new LogManagerReference(AppDataPath);
}
