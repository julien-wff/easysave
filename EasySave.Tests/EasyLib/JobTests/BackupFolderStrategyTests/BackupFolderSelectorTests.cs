using EasyLib.Job;
using EasyLib.Job.BackupFolderStrategy;

namespace EasySave.Tests.EasyLib.JobTests.BackupFolderStrategyTests;

public class BackupFolderSelectorTests
{
    [Fact]
    public void SelectFoldersFullNew_ShouldSelectCorrectFolders()
    {
        // Arrange
        var typeFull = new FullBackupFolderStrategy();
        var stateNew = new NewBackupFolderStrategy();
        
        var selectorFullNew = new BackupFolderSelector(typeFull, stateNew);

        var folders = new List<string> { "C:\\", "D:\\", "E:\\" };
        var path = new List<string> { "F:\\" };
        // Act
        var result = selectorFullNew.SelectFolders(new List<string>(), null);
        var result2 = selectorFullNew.SelectFolders(folders, null);
        var result3 = selectorFullNew.SelectFolders(new List<string>(), path[0]);
        var result4 = selectorFullNew.SelectFolders(folders, path[0]);
        
        // Assert
        Assert.Empty(result);
        Assert.Empty(result2);
        Assert.Empty(result3);
        Assert.Empty(result4);
    }
    
    [Fact]
    public void SelectFoldersFullResume_ShouldSelectCorrectFolders()
    {
        // Arrange
        var typeFull = new FullBackupFolderStrategy();
        var stateNew = new ResumeBackupFolderStrategy();
        
        var selectorFullResume = new BackupFolderSelector(typeFull, stateNew);

        var folders = new List<string> { "C:\\", "D:\\", "E:\\" };
        var path = new List<string> { "F:\\" };
        // Act
        var result = selectorFullResume.SelectFolders(new List<string>(), null);
        var result2 = selectorFullResume.SelectFolders(folders, null);
        var result3 = selectorFullResume.SelectFolders(new List<string>(), path[0]);
        var result4 = selectorFullResume.SelectFolders(folders, path[0]);
        
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
        var typeDiff = new DifferentialBackupFolderStrategy();
        var stateNew = new NewBackupFolderStrategy();
        
        var selectorDiffNew = new BackupFolderSelector(typeDiff, stateNew);

        var folders = new List<string> { "C:\\", "D:\\", "E:\\" };
        var path = new List<string> { "F:\\" };
        // Act
        var result = selectorDiffNew.SelectFolders(new List<string>(), null);
        var result2 = selectorDiffNew.SelectFolders(folders, null);
        var result3 = selectorDiffNew.SelectFolders(new List<string>(), path[0]);
        var result4 = selectorDiffNew.SelectFolders(folders, path[0]);
        
        // Assert
        Assert.Empty(result);
        result2.Should().BeEquivalentTo(folders);
        Assert.Empty(result3);
        result4.Should().BeEquivalentTo(folders);
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
        var result = selectorDiffResume.SelectFolders(new List<string>(), null);
        var result2 = selectorDiffResume.SelectFolders(folders, null);
        var result3 = selectorDiffResume.SelectFolders(new List<string>(), path[0]);
        var result4 = selectorDiffResume.SelectFolders(folders, path[0]);
        
        // Assert
        Assert.Empty(result);
        result2.Should().BeEquivalentTo(folders);
        result3.Should().BeEquivalentTo(path);
        result4.Should().BeEquivalentTo(folderAndpPath);
    }
}
