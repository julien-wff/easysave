using EasyLib.Files.References;

namespace EasyLib.Files;

/// <summary>
/// Singleton to get and write the jobs to the state.json file.
/// </summary>
public abstract class StateManager
{
    private static StateManagerReference? _instance;

    private static readonly string AppDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

    /// <summary>
    /// Expose the singleton instance
    /// </summary>
    public static StateManagerReference Instance => _instance ??= new StateManagerReference(AppDataPath);
}
