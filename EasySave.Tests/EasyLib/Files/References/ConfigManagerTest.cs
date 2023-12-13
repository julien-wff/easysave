using EasyLib.Files;

namespace EasySave.Tests.EasyLib.Files.References;

public class ConfigManagerTest
{
    [Fact]
    public void TestSingleton()
    {
        var instance1 = ConfigManager.Instance;
        var instance2 = ConfigManager.Instance;

        Assert.Equal(instance1, instance2);
    }

    [Fact]
    public void TestConfigManager()
    {
        // Assert
        Assert.True(ConfigManager.Instance.EncryptedFileExtensions.Count == 0);
        Assert.True(ConfigManager.Instance.LogFormat == ".json");
    }
}
