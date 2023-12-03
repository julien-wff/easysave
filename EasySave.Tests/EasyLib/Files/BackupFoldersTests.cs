using EasyLib.Files;

namespace EasySave.Tests.EasyLib.Files;

public class BackupFoldersTests
{
    [Fact]
    public void TestBackupFolder()
    {
        // Arrange
        string tempDirPath = Path.GetTempPath();
        Directory.CreateDirectory(tempDirPath + @"BackupFolderTests\");
        tempDirPath += @"BackupFolderTests\";
        var backupFolder = new BackupFolder(tempDirPath);
        string tempDirName = Path.GetFileName(Path.GetDirectoryName(tempDirPath))!;
        Directory.CreateDirectory(tempDirPath + "dir1");
        Directory.CreateDirectory(tempDirPath + "dir2");
        Directory.CreateDirectory(tempDirPath + @"dir1\dir3");
        File.WriteAllText(tempDirPath + "file0.txt", "test");
        File.WriteAllText(tempDirPath + "dir1\\file1.txt", "file1");
        File.WriteAllText(tempDirPath + "dir2\\fil2.txt", "file2");
        File.WriteAllText(tempDirPath + "dir1\\dir3\\file3.txt", "file3");
        
        // Act
        backupFolder.Walk(tempDirPath);

        // Assert
        Assert.Equal(tempDirName, backupFolder.Name);
        Assert.Equal(2, backupFolder.SubFolders.Count);
        Assert.Single(backupFolder.SubFolders[0].SubFolders);
        Assert.Equal("dir1", backupFolder.SubFolders[0].Name);
        Assert.Equal("dir3", backupFolder.SubFolders[0].SubFolders[0].Name);
        Assert.Equal("file0.txt", backupFolder.Files[0].Name);

    }
}
