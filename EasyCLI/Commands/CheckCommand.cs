using EasyLib;
using EasyLib.Enums;

namespace EasyCLI.Commands;

public class CheckCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("check")
        .SetDescription("Checks if the config is valid, and if the state matches all the rules")
        .SetAliases(new[] { "validate", "verify" });

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var jm = new JobManager();
        var fetchJobsResult = jm.FetchJobs();

        if (!fetchJobsResult)
        {
            Console.WriteLine("Failed to fetch jobs.");
            return;
        }

        var jobs = jm.GetJobs();

        if (jobs.Count == 0)
        {
            Console.WriteLine("No jobs found.");
            return;
        }

        var invalidCount = 0;
        foreach (var job in jobs)
        {
            var checkResult = jm.CheckJobRules((int)job.Id, job.Name, job.SourceFolder, job.DestinationFolder);
            if (checkResult != JobCheckRule.Valid)
            {
                Console.WriteLine(
                    $"Job {job.Name} ({job.Id}) is invalid: {Localization.JobCheckRules.GetString(checkResult)}");
                Console.WriteLine($"  Source: {job.SourceFolder}");
                Console.WriteLine($"  Destination: {job.DestinationFolder}");
                Console.WriteLine($"  Type: {EnumConverter<JobType>.ConvertToString(job.Type)}");
                Console.WriteLine();
                invalidCount++;
            }
        }

        Console.WriteLine($"Checks complete with {invalidCount} invalid out of {jobs.Count}.");

        if (invalidCount > 0)
        {
            Environment.Exit(1);
        }
    }
}
