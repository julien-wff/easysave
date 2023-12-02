using EasyLib.Enums;
using EasyLib.Job;

namespace EasySave.Tests.EasyLib.JobTests;

public class JobTests
{
    [Fact]
    public void CreateJob_ShouldCreateJob()
    {
        // Arrange
        const string jobName = "job1";
        const string sourcePath = "C:\\";
        const string targetPath = "D:\\";
        const JobType jobType = JobType.Full;

        // Act
        var job = new Job(jobName, sourcePath, targetPath, jobType);

        // Assert
        job.Name.Should().Be(jobName);
        job.SourceFolder.Should().Be(sourcePath);
        job.DestinationFolder.Should().Be(targetPath);
        job.Type.Should().Be(jobType);
        job.State.Should().Be(JobState.End);
    }
}
