using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;
using EasyLib;
using EasyLib.Enums;

namespace EasyCLI.Commands;

public class CreateCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("create")
        .SetDescription("Create a new job in the state file")
        .SetAliases(new[] { "new", "add" })
        .AddArg(new CommandArg()
            .SetName("name")
            .SetDescription("Name of the job to create")
            .SetType(new CommandArgTypeString())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("source")
            .SetDescription("Source folder to backup")
            .SetType(new CommandArgTypePath())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("destination")
            .SetDescription("Destination folder to backup to")
            .SetType(new CommandArgTypePath())
            .SetRequired(true))
        .AddFlag(new CommandFlag()
            .SetName("type")
            .SetDescription("Type of backup to perform. Valid options and full and differential")
            .SetType(new CommandArgTypeJobType())
            .SetDefault("full")
            .SetShortHands(new[] { "t" }));

    public override bool ValidateArgs(IEnumerable<string> args)
    {
        throw new NotImplementedException();
    }

    public override void Run(IEnumerable<string> args)
    {
        var argsList = args.ToList();

        if (argsList.Count < 3)
        {
            Console.WriteLine("Not enough arguments provided. Use 'help create' for more information.");
            return;
        }

        var name = argsList[1];
        Params.Args[1].Type.RawValue = argsList[2]; // Source path
        Params.Args[2].Type.RawValue = argsList[3]; // Destination path

        if (!Params.Args[1].Type.CheckValue())
        {
            Console.WriteLine("Source path is not valid.");
            return;
        }

        var source = (string)Params.Args[1].Type.ParseValue();

        if (!Params.Args[2].Type.CheckValue())
        {
            Console.WriteLine("Destination path is not valid.");
            return;
        }

        var destination = (string)Params.Args[2].Type.ParseValue();

        var jobType = JobType.Full;
        if (argsList.Count > 5 && argsList[4] is "-t" or "--type")
        {
            Params.Flags[0].Type.RawValue = argsList[5];
            if (!Params.Flags[0].Type.CheckValue())
            {
                Console.WriteLine("Invalid job type. Valid options are 'full' and 'differential'.");
                return;
            }

            jobType = (JobType)Params.Flags[0].Type.ParseValue();
        }

        var jm = new JobManager();
        jm.FetchJobs();

        var jobCheck = jm.CheckJobRules(-1, name, source, destination);
        if (jobCheck != JobCheckRule.Valid)
        {
            // Translates the error message from the JobCheckRules enum
            Console.WriteLine(Localization.JobCheckRules.GetString(jobCheck));
            return;
        }

        var job = jm.CreateJob(name, source, destination, jobType);

        Console.WriteLine($"Job '{name}' created successfully with ID {job.Id}.");
    }
}
