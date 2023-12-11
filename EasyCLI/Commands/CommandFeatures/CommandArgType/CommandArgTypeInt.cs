namespace EasyCLI.Commands.CommandFeatures.CommandArgType;

public class CommandArgTypeInt() : CommandArgType("int")
{
    public override bool CheckValue()
    {
        return int.TryParse(RawValue, out _);
    }

    public override object ParseValue()
    {
        return int.Parse(RawValue);
    }
}
