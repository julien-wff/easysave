using EasyCLI.Commands.CommandFeatures;
using EasyLib;
using EasyLib.Enums;

namespace EasyCLI.Commands;

public class DiscardCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("discard")
        .SetDescription("Discard paused jobs")
        .SetAliases(new[] { "abandon", "cancel" })
        .AddArg(new CommandArg()
            .SetName("jobs")
            .SetDescription("Jobs to check. Use format selector (1,2 or 1-3 or job1,2-4)")
            .SetRequired(true));


    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();

        if (argsList.Count != 2)
        {
            Console.WriteLine("Invalid arguments. Type 'easysave help discard' for more information");
        }

        var jm = new LocalJobManager();
        var fetchSuccess = jm.FetchJobs();

        if (!fetchSuccess)
        {
            Console.WriteLine("Failed to retrieve jobs from state file");
            return;
        }

        var jobs = jm.GetJobsFromString(argsList[1]);

        if (jobs.Count == 0)
        {
            Console.WriteLine("No jobs found");
            return;
        }

        var endedJob = jobs.Find(j => j.State == JobState.End);
        if (endedJob != null)
        {
            Console.WriteLine($"Job '{endedJob.Name}' (#{endedJob.Id}) is not running, cannot discard");
            return;
        }

        foreach (var job in jobs)
        {
            jm.CancelJob(job);
        }

        Console.WriteLine($"Successfully discarded {jobs.Count} job(s)");
    }
}
