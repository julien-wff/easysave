using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.Files.References;
using EasyLib.Job;
using EasyLib.Json;

namespace EasySave.Tests.EasyLib.Files.References;

public class StateManagerReferenceTests
{
    private static string GetLogFilePath(string appDataDir)
    {
        var stateDirectory = Path.Combine(appDataDir, "easysave");
        return Path.Combine(stateDirectory, "state.json");
    }

    [Fact]
    public void LogFileCreation_ShouldCreateEmptyFile()
    {
        // Arrange
        var appDataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act
        var stateManager = new StateManagerReference(appDataPath);

        // Assert
        Assert.True(File.Exists(stateManager.StateFilePath));
    }

    [Fact]
    public void LogFilePath_ShouldBeCorrect()
    {
        // Arrange
        var appDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var expectedStateFilePath = GetLogFilePath(appDataDir);

        // Act
        var stateManager = new StateManagerReference(appDataDir);

        // Assert
        Assert.Equal(expectedStateFilePath, stateManager.StateFilePath);
    }

    [Fact]
    public void WriteJogs_ShouldWriteCorrectJogs()
    {
        // Arrange
        var appDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var stateManager = new StateManagerReference(appDataDir);
        var job = new Job("job1", "C:\\", "D:\\", JobType.Full)
        {
            Id = 1,
        };

        // Act
        stateManager.WriteJobs(new List<Job> { job });

        // Assert
        var jsonJobs = JsonFileUtils.ReadJson<List<JsonJob>>(stateManager.StateFilePath);
        jsonJobs.Should().NotBeNullOrEmpty();
        Assert.Single(jsonJobs!);
        var jsonJob = jsonJobs![0];
        Assert.Equal(job.Name, jsonJob.name);
        Assert.Equal(job.SourceFolder, jsonJob.source_folder);
        Assert.Equal(job.DestinationFolder, jsonJob.destination_folder);
    }
}
