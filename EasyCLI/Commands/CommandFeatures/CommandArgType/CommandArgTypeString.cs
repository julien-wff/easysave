namespace EasyCLI.Commands.CommandFeatures.CommandArgType;

public class CommandArgTypeString() : CommandArgType("string")
{
    public override bool CheckValue()
    {
        return true;
    }

    public override object ParseValue()
    {
        return RawValue;
    }
}
