namespace EasySave.Tests.EasyLib.JobTests.BackupFolderStrategyTests;

public class BackupFolderSelectorTests
{
    /*
    [Fact]
    public void SelectFoldersFullNew_ShouldSelectCorrectFolders()
    {
        // Arrange

        string tempDirPath = Path.GetTempPath();
        Directory.CreateDirectory(tempDirPath + @"BackUpFolderSelector\");
        string testPath = @"BackUpFolderSelector\";
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir2\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\dir3\");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\file0.txt", "fil0");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\file1.txt", "file1");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir2\fil2.txt", "file2");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\dir3\file3.txt", "file3");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\dir1\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\dir2\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\dir1\dir3\");
        File.WriteAllText(tempDirPath + testPath + @"DestinationPath\file0.txt", "fil0");
        File.WriteAllText(tempDirPath + testPath + @"DestinationPath\dir1\file1.txt", "file1");

        // Act

        var typeFull = new FullBackupFolderStrategy();
        var stateNew = new NewBackupFolderStrategy();

        var selectorFullNew = new BackupFolderSelector(typeFull, stateNew);

        var folders = new List<string> { "dir1", "dir2"};
        var path = new List<string> { "F:\\" };

        var result = selectorFullNew.SelectFolders(new List<string>(), null, "job1", tempDirPath + testPath + @"DestinationPath\");
        var result2 = selectorFullNew.SelectFolders(folders, null,"job1", tempDirPath + testPath + @"DestinationPath\");
        var result3 = selectorFullNew.SelectFolders(new List<string>(), path[0],"job1", tempDirPath + testPath + @"DestinationPath\");
        var result4 = selectorFullNew.SelectFolders(folders, path[0],"job1", tempDirPath + testPath + @"DestinationPath\");

        // Assert
        Assert.Single(result);
        Assert.Single(result2);
        Assert.Single(result3);
        Assert.Single(result4);
    }

    [Fact]
    public void SelectFoldersFullResume_ShouldSelectCorrectFolders()
    {
        // Arrange

        // Act

        var typeFull = new FullBackupFolderStrategy();
        var stateNew = new ResumeBackupFolderStrategy();

        var selectorFullResume = new BackupFolderSelector(typeFull, stateNew);

        var folders = new List<string> { "C:\\", "D:\\", "E:\\" };
        var path = new List<string> { "F:\\" };

        var result = selectorFullResume.SelectFolders(new List<string>(), null, "job1", "G:\\");
        var result2 = selectorFullResume.SelectFolders(folders, null,"job1", "G:\\");
        var result3 = selectorFullResume.SelectFolders(new List<string>(), path[0],"job1", "G:\\");
        var result4 = selectorFullResume.SelectFolders(folders, path[0],"job1", "G:\\");

        // Assert
        Assert.Empty(result);
        Assert.Empty(result2);
        result3.Should().BeEquivalentTo(path);
        result4.Should().BeEquivalentTo(path);
    }

    [Fact]
    public void SelectFoldersDiffNew_ShouldSelectCorrectFolders()
    {
        // Arrange

        string tempDirPath = Path.GetTempPath();
        Directory.CreateDirectory(tempDirPath + @"BackUpFolderSelector\");
        string testPath = @"BackUpFolderSelector\";
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir2\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\dir3\");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\file0.txt", "fil0");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\file1.txt", "file1");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir2\fil2.txt", "file2");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\dir3\file3.txt", "file3");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\dir1\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\dir2\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\dir1\dir3\");
        File.WriteAllText(tempDirPath + testPath + @"DestinationPath\file0.txt", "fil0");
        File.WriteAllText(tempDirPath + testPath + @"DestinationPath\dir1\file1.txt", "file1");

        // Act

        var typeDiff = new DifferentialBackupFolderStrategy();
        var stateNew = new NewBackupFolderStrategy();

        var selectorDiffNew = new BackupFolderSelector(typeDiff, stateNew);

        var folders = new List<string> { "C:\\", "D:\\", "E:\\" };
        var path = new List<string> { "F:\\" };

        var result = selectorDiffNew.SelectFolders(new List<string>(), null, "job1", tempDirPath + testPath + @"DestinationPath\");
        var result2 = selectorDiffNew.SelectFolders(folders, null,"job1", tempDirPath + testPath + @"DestinationPath\");
        var result3 = selectorDiffNew.SelectFolders(new List<string>(), path[0],"job1", tempDirPath + testPath + @"DestinationPath\");
        var result4 = selectorDiffNew.SelectFolders(folders, path[0],"job1", tempDirPath + testPath + @"DestinationPath\");

        // Assert
        Assert.Single(result);
        result2.Count.Should().Be(4);
        Assert.Single(result3);
        result4.Count.Should().Be(4);
    }

    [Fact]
    public void SelectFoldersDiffResume_ShouldSelectCorrectFolders()
    {
        // Arrange
        var typeDiff = new DifferentialBackupFolderStrategy();
        var stateNew = new ResumeBackupFolderStrategy();

        var selectorDiffResume = new BackupFolderSelector(typeDiff, stateNew);

        var folders = new List<string> { "C:\\", "D:\\", "E:\\" };
        var path = new List<string> { "F:\\" };
        var folderAndpPath = new List<string> { "C:\\", "D:\\", "E:\\", "F:\\" };
        // Act
        var result = selectorDiffResume.SelectFolders(new List<string>(), null, "job1", "G:\\");
        var result2 = selectorDiffResume.SelectFolders(folders, null,"job1", "G:\\");
        var result3 = selectorDiffResume.SelectFolders(new List<string>(), path[0],"job1", "G:\\");
        var result4 = selectorDiffResume.SelectFolders(folders, path[0],"job1", "G:\\");

        // Assert
        Assert.Empty(result);
        result2.Should().BeEquivalentTo(folders);
        result3.Should().BeEquivalentTo(path);
        result4.Should().BeEquivalentTo(folderAndpPath);
    }
    */
}
