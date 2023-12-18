using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Localization;
using EasyLib.Enums;
using EasyLib.JobManager;

namespace EasyCLI.Commands;

public class CheckCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("check")
        .SetDescription(Loc.T("Commands.Check.Description"))
        .SetAliases(new[] { "validate", "verify" })
        .AddArg(new CommandArg()
            .SetName("jobs")
            .SetDescription(Loc.T("Commands.Check.Args.Jobs.Description"))
            .SetRequired(false));

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();
        var jm = new LocalJobManager();
        var fetchJobsResult = jm.FetchJobs();

        if (!fetchJobsResult)
        {
            Console.WriteLine(Loc.T("Job.FetchError"));
            return;
        }

        var jobs = jm.GetJobs();

        if (argsList.Count == 2)
        {
            jobs = jm.GetJobsFromString(argsList[1]);
        }

        if (jobs.Count == 0)
        {
            Console.WriteLine(Loc.T("Job.NoJobsFound"));
            return;
        }

        var invalidCount = 0;
        foreach (var job in jobs)
        {
            var checkResult = jm.CheckJobRules((int)job.Id, job.Name, job.SourceFolder, job.DestinationFolder);
            if (checkResult != JobCheckRule.Valid)
            {
                Console.WriteLine(Loc.T("Job.CheckError", job.Name, job.Id, JobCheckRules.GetString(checkResult)));
                Console.WriteLine(Loc.T(
                    "Job.JobInformation.Attributes",
                    job.SourceFolder,
                    job.DestinationFolder,
                    EnumConverter<JobType>.ConvertToString(job.Type),
                    EnumConverter<JobState>.ConvertToString(job.State)
                ));
                Console.WriteLine();
                invalidCount++;
            }
        }

        Console.WriteLine(Loc.T("Commands.Check.Result", invalidCount, jobs.Count));

        if (invalidCount > 0)
        {
            Environment.Exit(1);
        }
    }
}
