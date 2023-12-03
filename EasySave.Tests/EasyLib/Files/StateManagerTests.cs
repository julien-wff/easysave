using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.Job;
using EasyLib.Json;

namespace EasySave.Tests.EasyLib.Files;

public class StateManagerTests
{
    private string GetStateFilePath()
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var stateDirectory = Path.Combine(appDataDir, "easysave");
        return Path.Combine(stateDirectory, "state.json");
    }

    [Fact]
    public void Instance_ShouldBeSingleton()
    {
        // Arrange

        // Act
        var instance1 = StateManager.Instance;
        var instance2 = StateManager.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void StateFileCreation_ShouldCreateEmptyFile()
    {
        // Arrange

        // Act
        var stateManager = StateManager.Instance;
        var stateFilePath = stateManager.StateFilePath;

        // Assert
        Assert.True(File.Exists(stateFilePath));
    }

    [Fact]
    public void StateFileCreation_ShouldCreateJsonArray()
    {
        // Arrange

        // Act
        var stateManager = StateManager.Instance;
        var stateFilePath = stateManager.StateFilePath;
        var fileContent = File.ReadAllText(stateFilePath).Trim();

        // Assert
        Assert.StartsWith("[", fileContent);
        Assert.EndsWith("]", fileContent);
    }

    [Fact]
    public void StateFilePath_ShouldBeCorrect()
    {
        // Arrange

        // Act
        var stateManager = StateManager.Instance;
        var stateFilePath = stateManager.StateFilePath;

        // Assert
        Assert.Equal(GetStateFilePath(), stateFilePath);
    }

    [Fact]
    public void ReadJobs_ShouldReturnCorrectJobs()
    {
        // Arrange
        var jsonJob = new JsonJob
        {
            id = 1,
            name = "job1",
            source_folder = "C:\\",
            destination_folder = "D:\\",
            type = EnumConverter<JobType>.ConvertToString(JobType.Full),
            state = EnumConverter<JobState>.ConvertToString(JobState.End)
        };
        var stateFilePath = StateManager.Instance.StateFilePath;
        JsonFileUtils.WriteJson(stateFilePath, new List<JsonJob> { jsonJob });

        // Act
        var jobs = StateManager.Instance.ReadJobs();

        // Assert
        jobs.Should().NotBeNullOrEmpty();
        Assert.Single(jobs);
        var job = jobs[0];
        Assert.Equal(jsonJob.name, job.Name);
        Assert.Equal(jsonJob.source_folder, job.SourceFolder);
        Assert.Equal(jsonJob.destination_folder, job.DestinationFolder);
    }

    [Fact]
    public void WriteJobs_ShouldWriteCorrectJobs()
    {
        // Arrange
        var job = new Job("job1", "C:\\", "D:\\", JobType.Full)
        {
            Id = 1,
        };

        // Act
        StateManager.Instance.WriteJobs(new List<Job> { job });

        // Assert
        var stateFilePath = StateManager.Instance.StateFilePath;
        var jsonJobs = JsonFileUtils.ReadJson<List<JsonJob>>(stateFilePath);
        jsonJobs.Should().NotBeNullOrEmpty();
        Assert.Single(jsonJobs!);
        var jsonJob = jsonJobs![0];
        Assert.Equal(job.Name, jsonJob.name);
        Assert.Equal(job.SourceFolder, jsonJob.source_folder);
        Assert.Equal(job.DestinationFolder, jsonJob.destination_folder);
    }
}
