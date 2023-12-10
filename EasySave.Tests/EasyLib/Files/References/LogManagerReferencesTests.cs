using EasyLib.Files;
using EasyLib.Files.References;
using EasyLib.Json;

namespace EasySave.Tests.EasyLib.Files.References;

public class LogManagerReferencesTests
{
    private static string GetLogFilePath(string appDataDir)
    {
        var stateDirectory = Path.Combine(appDataDir, "easysave");
        return Path.Combine(stateDirectory, "logs",
            DateTime.Now.ToString("yyyy-MM-dd") + ConfigManager.Instance.LogFormat);
    }

    [Fact]
    public void LogFileCreation_ShouldCreateEmptyFile()
    {
        // Arrange
        var appDataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());

        // Act
        var logManager = new LogManagerReference(appDataPath);

        // Assert
        Assert.True(File.Exists(logManager.LogFilePath));
    }

    [Fact]
    public void LogFilePath_ShouldBeCorrect()
    {
        // Arrange
        var appDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var expectedLogFilePath = GetLogFilePath(appDataDir);

        // Act
        var logManager = new LogManagerReference(appDataDir);

        // Assert
        Assert.Equal(expectedLogFilePath, logManager.LogFilePath);
    }

    [Fact]
    public void WriteLogs_ShouldWriteCorrectLogs()
    {
        // Arrange
        var appDataDir = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var log = new LogElement
        {
            JobName = "test",
            TransferTime = 15,
            SourcePath = "C:\\test",
            DestinationPath = "C:\\test",
            FileSize = 15
        };
        var logManager = new LogManagerReference(appDataDir);

        // Act
        logManager.AppendLog(log);

        // Assert
        var logFilePath = logManager.LogFilePath;
        var jsonLogs = JsonFileUtils.ReadJson<List<LogElement>>(logFilePath);
        jsonLogs.Should().NotBeNullOrEmpty();
        var jsonLogElement = jsonLogs![0];
        Assert.Equal(log.JobName, jsonLogElement.JobName);
    }
}
