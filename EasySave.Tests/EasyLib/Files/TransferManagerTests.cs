using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.Job;
using EasyLib.Job.BackupFolderStrategy;

namespace EasySave.Tests.EasyLib.Files;

public class TransferManagerTests
{
    [Fact]
    public void TestTransferManagerSourceSan()
    {
        // Arrange
        string tempDirPath = Path.GetTempPath();
        Directory.CreateDirectory(tempDirPath + @"TransferManagerTests\");
        string testPath = @"TransferManagerTests\";
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir2\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\dir3\");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\file0.txt", "fil0");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\file1.txt", "file1");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir2\fil2.txt", "file2");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\dir3\file3.txt", "file3");
        
        const string jobName = "job1";
        string sourcePath = tempDirPath + testPath + @"SourcePath\";
        string destinationPath = tempDirPath + testPath + @"DestinationPath\";
        const JobType jobType = JobType.Full;

        // Act
        var job = new Job(jobName, sourcePath, destinationPath, jobType);
        var transferManager = new TransferManager(job);
        transferManager.ScanSource();
        
        // Assert
        Assert.Equal((uint)4, job.FilesCount);
    }

    [Fact]
    public void TestTransferManagerComputeDifferenceFull()
    {
        // Arrange
        string tempDirPath = Path.GetTempPath();
        Directory.CreateDirectory(tempDirPath + @"TransferManagerTests2\");
        string testPath = @"TransferManagerTests2\";
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"DestinationPath\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir2\");
        Directory.CreateDirectory(tempDirPath + testPath + @"SourcePath\dir1\dir3\");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\file0.txt", "fil0");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\file1.txt", "file1");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir2\fil2.txt", "file2");
        File.WriteAllText(tempDirPath + testPath + @"SourcePath\dir1\dir3\file3.txt", "file3");
        
        const string jobName = "job1";
        string sourcePath = tempDirPath + testPath + @"SourcePath\";
        string destinationPath = tempDirPath + testPath + @"DestinationPath\";
        const JobType jobType = JobType.Full;
        
        // Act
        var job = new Job(jobName, sourcePath, destinationPath, jobType);
        var transferManager = new TransferManager(job);
        transferManager.ScanSource();
        var method = new BackupFolderSelector(new FullBackupFolderStrategy(),new NewBackupFolderStrategy());
        List<string> folders = method.SelectFolders( new List<string>(), null);
        transferManager.ComputeDifference(folders);
        
        // Assert
        Assert.Single(transferManager.InstructionsFolder.Files);
        Assert.Equal("file0.txt", transferManager.InstructionsFolder.Files[0].Name);
        Assert.Equal("file1.txt", transferManager.InstructionsFolder.SubFolders[0].Files[0].Name);
        Assert.Equal("file3.txt", transferManager.InstructionsFolder.SubFolders[0].SubFolders[0].Files[0].Name);
    }
    [Fact]
    public void TestTransferManagerComputeDifferenceDifferential()
    {
        // Arrange
        string tempDirPath = Path.GetTempPath();
        Directory.CreateDirectory(tempDirPath + @"TransferManagerTests2\");
        string testPath = @"TransferManagerTests2\";
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
        
        const string jobName = "job1";
        string sourcePath = tempDirPath + testPath + @"SourcePath\";
        string destinationPath = tempDirPath + testPath + @"DestinationPath\";
        const JobType jobType = JobType.Full;
        
        // Act
        var job = new Job(jobName, sourcePath, destinationPath, jobType);
        var transferManager = new TransferManager(job);
        transferManager.ScanSource();
        var method = new BackupFolderSelector(new FullBackupFolderStrategy(),new NewBackupFolderStrategy());
        List<string> folders = method.SelectFolders( new List<string>(), null);
        transferManager.ComputeDifference(folders);
        
        // Assert
        Assert.Single(transferManager.InstructionsFolder.Files);
        Assert.Equal("file0.txt", transferManager.InstructionsFolder.Files[0].Name);
        Assert.Equal("file1.txt", transferManager.InstructionsFolder.SubFolders[0].Files[0].Name);
        Assert.Equal("file3.txt", transferManager.InstructionsFolder.SubFolders[0].SubFolders[0].Files[0].Name);
        Assert.Equal(2, transferManager.InstructionsFolder.SubFolders.Count);
    }
}
