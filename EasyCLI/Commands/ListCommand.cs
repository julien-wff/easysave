using EasyCLI.Commands.CommandFeatures;
using EasyLib;
using EasyLib.Enums;

namespace EasyCLI.Commands;

public class ListCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("list")
        .SetDescription("List all the jobs")
        .SetAliases(new[] { "ls" })
        .AddArg(new CommandArg()
            .SetName("jobs")
            .SetDescription("Jobs to check. Use format selector (1,2 or 1-3 or job1,2-4). Optional")
            .SetRequired(false));

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();
        var jm = new LocalJobManager();
        var fetchSuccess = jm.FetchJobs();

        if (!fetchSuccess)
        {
            Console.WriteLine("Failed to fetch jobs");
            return;
        }

        var jobs = jm.GetJobs();

        if (argsList.Count == 2)
        {
            jobs = jm.GetJobsFromString(argsList[1]);
        }

        if (jobs.Count == 0)
        {
            if (argsList.Count == 2)
            {
                Console.WriteLine($"No jobs found in the state file for selection '{argsList[1]}'");
            }
            else
            {
                Console.WriteLine("No jobs found in the state file");
            }

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
