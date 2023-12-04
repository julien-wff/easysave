namespace EasyCLI.Commands.CommandFeatures.CommandArgType;

public class CommandArgTypePath() : CommandArgType("path")
{
    public override bool CheckValue()
    {
        try
        {
            Path.GetFullPath(RawValue);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public override object ParseValue()
    {
        return Path.GetFullPath(RawValue);
    }
}
