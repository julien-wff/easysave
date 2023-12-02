using EasyLib;
using EasyLib.Enums;

namespace EasySave.Tests.EasyLib;

public class JobManagerTests
{
    [Fact]
    public void CreateJob_ShouldCreateJob()
    {
        // Arrange
        var jobManager = new JobManager();

        // Act
        jobManager.CreateJob("job1", "C:\\", "D:\\", JobType.Full);

        // Assert
        Assert.Single(jobManager.GetJobs());
    }

    [Fact]
    public void DeleteJob_ShouldDeleteJob()
    {
        // Arrange
        var jobManager = new JobManager();
        jobManager.CreateJob("job1", "C:\\", "D:\\", JobType.Full);
        var job = jobManager.GetJobs().First();

        // Act
        jobManager.DeleteJob(job);

        // Assert
        Assert.Empty(jobManager.GetJobs());
    }
}
