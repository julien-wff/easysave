namespace EasyCLI.Commands.CommandFeatures.CommandArgType;

public class CommandArgTypePath() : CommandArgType("path")
{
    public override bool CheckValue()
    {
        var invalidChars = Path.GetInvalidPathChars();
        return RawValue.IndexOfAny(invalidChars) < 0;
    }

    public override object ParseValue()
    {
        return Path.GetFullPath(RawValue);
    }
}
