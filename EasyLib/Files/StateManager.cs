namespace EasyLib.Files;

/// <summary>
/// Singleton to get and write the jobs to the state.json file.
/// </summary>
public class StateManager
{
    public readonly string StateFilePath;

    private StateManager()
    {
        // AppData dir and append easysave/state.json
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var stateDirectory = Path.Combine(appDataDir, "easysave");
        StateFilePath = Path.Combine(stateDirectory, "state.json");

        // Create directory if it doesn't exist
        if (!Directory.Exists(stateDirectory))
        {
            Directory.CreateDirectory(stateDirectory);
        }

        // Create file and write [] if it doesn't exist
        if (!File.Exists(StateFilePath))
        {
            File.WriteAllText(StateFilePath, "[]");
        }
    }

    public static StateManager Instance { get; } = new();
}
