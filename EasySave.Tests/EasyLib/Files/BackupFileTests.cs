using EasyLib.Files;

namespace EasySave.Tests.EasyLib.Files;

public class BackupFileTests
{
    [Fact]
    void TestBackupFile()
    {
        // Arrange
        var tempFilePath = Path.GetTempFileName();
        FileInfo file = new(tempFilePath);
        var expectedName = Path.GetFileName(tempFilePath);
        var expectedSize = (ulong)file.Length;

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
