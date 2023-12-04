using EasyCLI.Commands.CommandFeatures;
using EasyLib;

namespace EasyCLI.Commands;

public class DeleteCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("delete")
        .SetDescription("Delete a job from the state file")
        .SetAliases(new[] { "remove", "del", "rm" })
        .AddArg(new CommandArg()
            .SetName("jobs")
            .SetDescription("Jobs to delete. Use format selector (1,2 or 1-3 or job1,2-4)")
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
            Console.WriteLine("Invalid number of arguments. See 'easysave help delete' for more information.");
            return;
        }

        var jobManager = new JobManager();

        if (jobManager.GetJobs().Count == 0)
        {
            Console.WriteLine("No job found in state file.");
            return;
        }

        var jobs = jobManager.GetJobsFromString(argsList[1]);

        if (jobs.Count == 0)
        {
            Console.WriteLine("No job found from selection. See 'easysave help delete' for more information.");
            return;
        }

        foreach (var job in jobs)
        {
            jobManager.DeleteJob(job);
        }

        Console.WriteLine($"Successfully deleted {jobs.Count} job(s).");
    }
}
