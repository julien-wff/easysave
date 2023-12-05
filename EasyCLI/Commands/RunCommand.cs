using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Display;
using EasyCLI.Localization;
using EasyLib;
using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Job;

namespace EasyCLI.Commands;

public class RunCommand : Command, IJobStatusSubscriber
{
    private DateTime _lastConsoleUpdate = DateTime.UtcNow;

    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("run")
        .SetDescription("Run selected jobs")
        .SetAliases(new[] { "start", "execute", "exe", "launch" })
        .AddArg(new CommandArg()
            .SetName("jobs")
            .SetDescription("Jobs to check. Use format selector (1,2 or 1-3 or job1,2-4). Optional")
            .SetRequired(true));

    public void OnJobProgress(Job job)
    {
        if (DateTime.UtcNow - _lastConsoleUpdate < TimeSpan.FromMilliseconds(200))
            return;

        _lastConsoleUpdate = DateTime.UtcNow;
        Console.Clear();
        Console.WriteLine();

        JobProgression.PrintJobProgression(job);
    }

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();

        if (argsList.Count != 2)
        {
            Console.WriteLine("Invalid number of arguments. See 'easysave help run' for more information.");
            return;
        }

        var jm = new JobManager();
        var fetchSuccess = jm.FetchJobs();

        if (!fetchSuccess)
        {
            Console.WriteLine("Failed to retrieve jobs from state file");
            return;
        }

        var jobs = jm.GetJobsFromString(argsList[1]);

        if (jobs.Count == 0)
        {
            Console.WriteLine($"No jobs found in the state file for selection '{argsList[1]}'");
            return;
        }

        foreach (var job in jobs)
        {
            var jobValidity = jm.CheckJobRules((int)job.Id, job.Name, job.SourceFolder, job.DestinationFolder, false);

            if (jobValidity != JobCheckRule.Valid)
            {
                Console.WriteLine($"Job #{job.Id} - {job.Name} is invalid: {JobCheckRules.GetString(jobValidity)}");
                return;
            }

            if (job.State is not JobState.End)
            {
                Console.WriteLine(
                    $"Job #{job.Id} - {job.Name} is already running. Please resume or discard it instead.");
                return;
            }
        }

        jm.Subscribe(this);
        Console.WriteLine($"Starting {jobs.Count} job(s)...");
        var executedJobs = jm.ExecuteJobs(jobs);
        jm.Unsubscribe(this);

        Console.WriteLine();
        Console.WriteLine(executedJobs
            ? $"{jobs.Count} job(s) executed successfully"
            : "Failed to execute jobs");
    }
}
