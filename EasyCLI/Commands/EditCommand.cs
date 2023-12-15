using EasyCLI.Commands.CommandFeatures;
using EasyCLI.Commands.CommandFeatures.CommandArgType;
using EasyCLI.Localization;
using EasyLib;
using EasyLib.Enums;

namespace EasyCLI.Commands;

public class EditCommand : Command
{
    public override CommandBuilder.CommandBuilder Params { get; } = new CommandBuilder.CommandBuilder()
        .SetName("edit")
        .SetDescription(Loc.T("Commands.Edit.Description"))
        .SetAliases(new[] { "modify", "update" })
        .AddArg(new CommandArg()
            .SetName("id")
            .SetDescription(Loc.T("Commands.Edit.Args.Id.Description"))
            .SetType(new CommandArgTypeInt())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("name")
            .SetDescription(Loc.T("Commands.Edit.Args.Name.Description"))
            .SetType(new CommandArgTypeString())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("source")
            .SetDescription(Loc.T("Commands.Edit.Args.Source.Description"))
            .SetType(new CommandArgTypePath())
            .SetRequired(true))
        .AddArg(new CommandArg()
            .SetName("destination")
            .SetDescription(Loc.T("Commands.Edit.Args.Destination.Description"))
            .SetType(new CommandArgTypePath())
            .SetRequired(true))
        .AddFlag(new CommandFlag()
            .SetName("type")
            .SetDescription(Loc.T("Commands.Edit.Flags.Type.Description"))
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

        if (argsList.Count <= 4)
        {
            Console.WriteLine(Loc.T("Commands.WrongArgumentCount", 3, argsList.Count - 1, "create"));
            return;
        }

        Params.Args[0].Type.RawValue = argsList[1]; // ID
        var name = argsList[2];
        Params.Args[1].Type.RawValue = argsList[3]; // Source path
        Params.Args[2].Type.RawValue = argsList[4]; // Destination path

        if (!Params.Args[0].Type.CheckValue())
        {
            Console.WriteLine(Loc.T("Checks.InvalidJobId"));
            return;
        }

        var id = (int)Params.Args[0].Type.ParseValue();

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

        JobType? jobType = null;
        if (argsList.Count > 6 && argsList[5] is "-t" or "--type")
        {
            Params.Flags[0].Type.RawValue = argsList[6];
            if (!Params.Flags[0].Type.CheckValue())
            {
                Console.WriteLine(Loc.T("Checks.InvalidJobType"));
                return;
            }

            jobType = (JobType)Params.Flags[0].Type.ParseValue();
        }

        var jm = new LocalJobManager();

        var job = jm.GetJobsFromIds(new[] { id });

        if (job.Count == 0)
        {
            Console.WriteLine(Loc.T("Checks.JobNotFound", id));
            return;
        }

        var editResult = jm.EditJob(job[0], name, source, destination, jobType);
        if (editResult != JobCheckRule.Valid)
        {
            // Translates the error message from the JobCheckRules enum
            Console.WriteLine(JobCheckRules.GetString(editResult));
            return;
        }

        Console.WriteLine(Loc.T("Job.Edited", job[0].Id, job[0].Name));
    }
}
