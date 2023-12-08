using EasyCrypto;

namespace EasySave.Tests.EasyCrypto;

public class FileManagerTests
{
    [Fact]
    public void EncryptFileTest()
    {
        // Arrange
        var appDataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.WriteAllText(appDataPath, "test");
        // Act
        var fileManager = new FileManager(appDataPath, "key");

        fileManager.TransformFile();

        // Assert
        Assert.True(File.Exists(appDataPath));
        Assert.NotEqual("test", File.ReadAllText(appDataPath));
    }

    [Fact]
    public void DecryptFileTest()
    {
        // Arrange
        var appDataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.WriteAllText(appDataPath, "This is some test data");
        // Act
        var fileManager = new FileManager(appDataPath, "key");

        fileManager.TransformFile();
        fileManager.TransformFile();

        // Assert
        Assert.True(File.Exists(appDataPath));
        Assert.Equal("This is some test data", File.ReadAllText(appDataPath));
    }
}
