using EasyLib.Json;

namespace EasyLib.Files.References;

public class StateManagerReference
{
    public readonly string StateFilePath;

    public StateManagerReference(string appDataPath)
    {
        // AppData dir and append easysave/state.json
        var stateDirectory = Path.Combine(appDataPath, "easysave");
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

    public List<Job.Job> ReadJobs()
    {
        var jsonJobs = JsonFileUtils.ReadJson<List<JsonJob>>(StateFilePath);

        return jsonJobs == null
            ? new List<Job.Job>()
            : jsonJobs.Select(job => new Job.Job(job)).ToList();
    }

    public void WriteJobs(List<Job.Job> jobs)
    {
        var jsonJobs = jobs.Select(job => job.ToJsonJob()).ToList();
        JsonFileUtils.WriteJson(StateFilePath, jsonJobs);
    }
}
