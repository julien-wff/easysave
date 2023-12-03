using EasyLib.Files;

namespace EasySave.Tests.EasyLib.Files;

public class BackupFileTests
{
    [Fact]
    void TestBackupFile()
    {
        // Arrange
        string tempFilePath = Path.GetTempFileName();
        FileInfo file = new(tempFilePath);
        string expectedName = Path.GetFileName(tempFilePath);
        ulong expectedSize =(ulong) file.Length;

        // Act
        var backupFile = new BackupFile(tempFilePath);
        var backupFile2 = new BackupFile(tempFilePath);

        // Assert
        Assert.Equal(expectedName, backupFile.Name);
        Assert.Equal(expectedSize, backupFile.Size);
        backupFile.Hash.Should().Be(backupFile2.Hash);

        // Cleanup
        File.Delete(tempFilePath);
    }
    
}
