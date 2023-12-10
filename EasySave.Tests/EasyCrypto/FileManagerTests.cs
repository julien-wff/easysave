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
        Program.Main(new[] { appDataPath, "key" });

        // Assert
        Assert.True(File.Exists(appDataPath));
        Assert.NotEqual("test", File.ReadAllText(appDataPath));

        // Cleanup
        File.Delete(appDataPath);
    }

    [Fact]
    public void DecryptFileTest()
    {
        // Arrange
        var appDataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.WriteAllText(appDataPath, "This is some test data");
        // Act

        Program.Main(new[] { appDataPath, "key" });
        Program.Main(new[] { appDataPath, "key" });

        // Assert
        Assert.True(File.Exists(appDataPath));
        Assert.Equal("This is some test data", File.ReadAllText(appDataPath));

        // Cleanup
        File.Delete(appDataPath);
    }

    [Fact]
    public void DecryptFileWithWrongKeyTest()
    {
        // Arrange
        var appDataPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        File.WriteAllText(appDataPath, "This is some test data");
        // Act

        Program.Main(new[] { appDataPath, "key" });
        Program.Main(new[] { appDataPath, "notTheKey" });

        // Assert
        Assert.True(File.Exists(appDataPath));
        Assert.NotEqual("This is some test data", File.ReadAllText(appDataPath));

        // Cleanup
        File.Delete(appDataPath);
    }
}
