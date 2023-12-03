using EasyLib;
using EasyLib.Enums;

namespace EasyCLI.Commands;

public class ListCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("list")
        .SetDescription("List all the jobs")
        .SetAliases(new[] { "ls" });

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var jm = new JobManager();
        var fetchSuccess = jm.FetchJobs();

        if (!fetchSuccess)
        {
            Console.WriteLine("Failed to fetch jobs");
            return;
        }

        var jobs = jm.GetJobs();

        if (jobs.Count == 0)
        {
            Console.WriteLine("No jobs found in the state file");
            return;
        }

        Console.WriteLine($"{jobs.Count} job(s) found:");
        Console.WriteLine();

        foreach (var job in jobs)
        {
            Console.WriteLine($"#{job.Id} - {job.Name}");
            Console.WriteLine($"  Source: {job.SourceFolder}");
            Console.WriteLine($"  Destination: {job.DestinationFolder}");
            Console.WriteLine($"  Type: {EnumConverter<JobType>.ConvertToString(job.Type)}");
            Console.WriteLine($"  State: {EnumConverter<JobState>.ConvertToString(job.State)}");
            Console.WriteLine();
        }
    }
}
