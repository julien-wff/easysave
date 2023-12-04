using EasyLib.Files;
using EasyLib.Json;

namespace EasySave.Tests.EasyLib.Files;

public class LogManagerTests
{
    private static string GetLogFilePath()
    {
        var appDataDir = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var stateDirectory = Path.Combine(appDataDir, "easysave");
        return Path.Combine(stateDirectory, "logs", DateTime.Now.ToString("yyyy-MM-dd") + ".json");
    }

    [Fact]
    public void Instance_ShouldBeSingleton()
    {
        // Arrange

        // Act
        var instance1 = LogManager.Instance;
        var instance2 = LogManager.Instance;

        // Assert
        Assert.Same(instance1, instance2);
    }

    [Fact]
    public void LogFileCreation_ShouldCreateEmptyFile()
    {
        // Arrange

        // Act
        var logManager = LogManager.Instance;
        var logFilePath = logManager.LogFilePath;

        // Assert
        Assert.True(File.Exists(logFilePath));
    }

    [Fact]
    public void LogFileCreation_ShouldCreateJsonArray()
    {
        // Arrange

        // Act
        var logManager = LogManager.Instance;
        var logFilePath = logManager.LogFilePath;
        var fileContent = File.ReadAllText(logFilePath).Trim();

        // Assert
        Assert.StartsWith("[", fileContent);
        Assert.EndsWith("]", fileContent);
    }

    [Fact]
    public void LogFilePath_ShouldBeCorrect()
    {
        // Arrange

        // Act
        var logManager = LogManager.Instance;
        var logFilePath = logManager.LogFilePath;

        // Assert
        Assert.Equal(GetLogFilePath(), logFilePath);
    }


    [Fact]
    public void WriteLogs_ShouldWriteCorrectLogs()
    {
        // Arrange
        var log = new JsonLogElement
        {
            JobName = "test",
            TransferTime = 15,
            SourcePath = "C:\\test",
            DestinationPath = "C:\\test",
            FileSize = 15
        };

        // Act
        LogManager.Instance.AppendLog(log);

        // Assert
        var logFilePath = LogManager.Instance.LogFilePath;
        var jsonLogs = JsonFileUtils.ReadJson<List<JsonLogElement>>(logFilePath);
        jsonLogs.Should().NotBeNullOrEmpty();
        var jsonLogElement = jsonLogs![0];
        Assert.Equal(log.JobName, jsonLogElement.JobName);
    }
}
