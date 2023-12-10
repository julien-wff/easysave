using EasyLib.Enums;
using EasyLib.Files;
using EasyLib.Job;

namespace EasySave.Tests.EasyLib.Files;

public class TransferManagerTests
{
    public List<string> createTestFiles(string tempDirPath, string testPath)
    {
        string sourcePath = tempDirPath + testPath + @"SourcePath\";
        string destinationPath = tempDirPath + testPath + @"DestinationPath\";
        Directory.CreateDirectory(sourcePath);
        Directory.CreateDirectory(destinationPath);
        Directory.CreateDirectory(sourcePath + @"dir1\");
        Directory.CreateDirectory(sourcePath + @"dir2\");
        Directory.CreateDirectory(sourcePath + @"dir1\dir3\");
        File.WriteAllText(sourcePath + @"file0.txt", "fil0");
        File.WriteAllText(sourcePath + @"dir1\file1.txt", "file1");
        File.WriteAllText(sourcePath + @"dir2\fil2.txt", "file2");
        File.WriteAllText(sourcePath + @"dir1\dir3\file3.txt", "file3");

        return new List<string>() { sourcePath, destinationPath };
    }

    [Fact]
    public void TestTransferManager()
    {
        // Arrange
        string tempDirPath = Path.GetTempPath();
        string testPath = @"SourceScanTests\";

        const string jobName = "job1";
        const JobType jobType = JobType.Full;

        var paths = createTestFiles(tempDirPath, testPath);
        // Act
        var job = new Job(jobName, paths[0], paths[1], jobType);
        var transferManager = new TransferManager(job);
        var folderList = Directory.GetDirectories(paths[1]).ToList();
        var directories = new List<List<string>>() { folderList };
        var selector = BackupFolderSelectorFactory.Create(job.Type, JobState.End);
        var folders = selector.SelectFolders(directories, "", job.Type, paths[1]);
        transferManager.ScanSource();
        transferManager.ComputeDifference(folders);
        transferManager.CreateDestinationStructure();
        transferManager.TransferFiles();

        // Assert
        Assert.Equal((uint)4, job.FilesCount);
        Assert.Equal(Directory.GetDirectories(paths[0]).Length,
            Directory.GetDirectories(Directory.GetDirectories(paths[1]).Last()).Length);
        Assert.Equivalent(Directory.GetFiles(paths[0]).Length,
            Directory.GetFiles(Directory.GetDirectories(paths[1]).Last()).Length);
    }
}
