using EasyLib.Files;

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
}
