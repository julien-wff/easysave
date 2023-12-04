using EasyLib;
using EasyLib.Enums;

namespace EasySave.Tests.EasyLib;

public class JobManagerTests
{
    [Theory]
    [InlineData("aze")]
    [InlineData("123")]
    [InlineData("10-11")]
    [InlineData("-1")]
    [InlineData("--1")]
    public void GetJobsFromString_ShouldReturnNoJob(string input)
    {
        // Arrange
        var jobManager = new JobManager(true);
        if (jobManager.GetJobs().Count > 0)
        {
            jobManager.GetJobs().Clear();
        }

        jobManager.CreateJob("job1", "C:\\", "D:\\", JobType.Full);
        jobManager.CreateJob("job2", "E:\\", "F:\\", JobType.Full);

        // Act
        var jobs = jobManager.GetJobsFromString(input);

        // Assert
        jobs.Should().BeEmpty();
    }

    [Theory]
    [InlineData("2")]
    [InlineData("job2")]
    public void GetJobsFromString_ShouldReturnIndividualJob(string input)
    {
        // Arrange
        var jobManager = new JobManager(true);
        if (jobManager.GetJobs().Count > 0)
        {
            jobManager.GetJobs().Clear();
        }

        jobManager.CreateJob("job1", "C:\\", "D:\\", JobType.Full);
        jobManager.CreateJob("job2", "E:\\", "F:\\", JobType.Full);

        // Act
        var jobs = jobManager.GetJobsFromString(input);

        // Assert
        jobs.Should().HaveCount(1);
        jobs[0].Name.Should().Be("job2");
    }

    [Theory]
    [InlineData("1,2,3")]
    [InlineData("job1,job2,job3")]
    [InlineData("job1,2,job3")]
    [InlineData("1-3")]
    [InlineData("3-1")]
    [InlineData("1,2-3")]
    public void GetJobsFromString_ShouldReturnThreeJobsJob(string input)
    {
        // Arrange
        var jobManager = new JobManager(true);
        if (jobManager.GetJobs().Count > 0)
        {
            jobManager.GetJobs().Clear();
        }

        jobManager.CreateJob("job1", "C:\\", "D:\\", JobType.Full);
        jobManager.CreateJob("job2", "E:\\", "F:\\", JobType.Full);
        jobManager.CreateJob("job3", "G:\\", "H:\\", JobType.Full);
        jobManager.CreateJob("job4", "I:\\", "J:\\", JobType.Full);

        // Act
        var jobs = jobManager.GetJobsFromString(input);

        // Assert
        jobs.Should().HaveCount(3);
        jobs[0].Name.Should().Be("job1");
        jobs[1].Name.Should().Be("job2");
        jobs[2].Name.Should().Be("job3");
    }
}
