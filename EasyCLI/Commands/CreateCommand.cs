using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;
using EasyCLI.Localization;
using EasyLib.Enums;
using EasyLib.JobManager;

namespace EasyCLI.Commands;

public class CreateCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("create")
        .SetDescription(Loc.T("Commands.Create.Description"))
        .SetAliases(new[] { "new", "add" })
        .AddArg(new CommandArg()
            .SetName("name")
            .SetDescription(Loc.T("Commands.Create.Args.Name.Description"))
            .SetType(new CommandArgTypeString())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("source")
            .SetDescription(Loc.T("Commands.Create.Args.Source.Description"))
            .SetType(new CommandArgTypePath())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("destination")
            .SetDescription(Loc.T("Commands.Create.Args.Destination.Description"))
            .SetType(new CommandArgTypePath())
            .SetRequired(true))
        .AddFlag(new CommandFlag()
            .SetName("type")
            .SetDescription(Loc.T("Commands.Create.Flags.Type.Description"))
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

        if (argsList.Count <= 3)
        {
            Console.WriteLine(Loc.T("Commands.WrongArgumentCount", 3, argsList.Count - 1, "create"));
            return;
        }

        var name = argsList[1];
        Params.Args[1].Type.RawValue = argsList[2]; // Source path
        Params.Args[2].Type.RawValue = argsList[3]; // Destination path

        if (!Params.Args[1].Type.CheckValue())
        {
            Console.WriteLine(Loc.T("Checks.SourcePathInvalid"));
            return;
        }

        var source = (string)Params.Args[1].Type.ParseValue();

        if (!Params.Args[2].Type.CheckValue())
        {
            Console.WriteLine(Loc.T("Checks.DestinationPathInvalid"));
            return;
        }

        var destination = (string)Params.Args[2].Type.ParseValue();

        var jobType = JobType.Full;
        if (argsList.Count > 5 && argsList[4] is "-t" or "--type")
        {
            Params.Flags[0].Type.RawValue = argsList[5];
            if (!Params.Flags[0].Type.CheckValue())
            {
                Console.WriteLine(Loc.T("Checks.InvalidJobType"));
                return;
            }

            jobType = (JobType)Params.Flags[0].Type.ParseValue();
        }

        var jm = new LocalJobManager();

        var jobCheck = jm.CheckJobRules(-1, name, source, destination);
        if (jobCheck != JobCheckRule.Valid)
        {
            // Translates the error message from the JobCheckRules enum
            Console.WriteLine(JobCheckRules.GetString(jobCheck));
            return;
        }

        var job = jm.CreateJob(name, source, destination, jobType);

        Console.WriteLine(Loc.T("Job.Created", job.Name, job.Id));
    }
}
