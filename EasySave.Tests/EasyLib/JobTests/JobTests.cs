using EasyLib.Enums;
using EasyLib.Job;
using EasyLib.Json;

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

    [Fact]
    public void ToJsonJob_ShouldReturnCorrectJsonJob()
    {
        // Arrange
        var job = new Job("job1", "C:\\", "D:\\", JobType.Full)
        {
            Id = 1,
            State = JobState.Copy,
            FilesCount = 10,
            FilesSizeBytes = 1000,
            FilesCopied = 5,
            FilesBytesCopied = 500
        };

        // Act
        var jsonJob = job.ToJsonJob();

        // Assert
        Assert.Equal(job.Id, jsonJob.id);
        Assert.Equal(job.Name, jsonJob.name);
        Assert.Equal(job.SourceFolder, jsonJob.source_folder);
        Assert.Equal(job.DestinationFolder, jsonJob.destination_folder);
        Assert.Equal(EnumConverter<JobType>.ConvertToString(job.Type), jsonJob.type);
        Assert.Equal(EnumConverter<JobState>.ConvertToString(job.State), jsonJob.state);
        Assert.Equal(job.FilesCount, jsonJob.active_job_info?.total_file_count);
        Assert.Equal(job.FilesSizeBytes, jsonJob.active_job_info?.total_file_size);
        Assert.Equal(job.FilesCopied, jsonJob.active_job_info?.files_copied);
        Assert.Equal(job.FilesBytesCopied, jsonJob.active_job_info?.bytes_copied);
    }

    [Fact]
    public void JobConstructor_ShouldCreateCorrectJobFromJsonJob()
    {
        // Arrange
        var jsonJob = new JsonJob
        {
            id = 1,
            name = "job1",
            source_folder = "C:\\",
            destination_folder = "D:\\",
            type = EnumConverter<JobType>.ConvertToString(JobType.Full),
            state = EnumConverter<JobState>.ConvertToString(JobState.End),
            active_job_info = new JsonActiveJobInfo
            {
                total_file_count = 10,
                total_file_size = 1000,
                files_copied = 5,
                bytes_copied = 500
            }
        };

        // Act
        var job = new Job(jsonJob);

        // Assert
        Assert.Equal(jsonJob.id, job.Id);
        Assert.Equal(jsonJob.name, job.Name);
        Assert.Equal(jsonJob.source_folder, job.SourceFolder);
        Assert.Equal(jsonJob.destination_folder, job.DestinationFolder);
        Assert.Equal(EnumConverter<JobType>.ConvertToEnum(jsonJob.type), job.Type);
        Assert.Equal(EnumConverter<JobState>.ConvertToEnum(jsonJob.state), job.State);
        Assert.Equal(jsonJob.active_job_info?.total_file_count, job.FilesCount);
        Assert.Equal(jsonJob.active_job_info?.total_file_size, job.FilesSizeBytes);
        Assert.Equal(jsonJob.active_job_info?.files_copied, job.FilesCopied);
        Assert.Equal(jsonJob.active_job_info?.bytes_copied, job.FilesBytesCopied);
    }
}
