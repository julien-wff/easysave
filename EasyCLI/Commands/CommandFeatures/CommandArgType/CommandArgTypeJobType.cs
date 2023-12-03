using EasyLib.Enums;

namespace EasyCLI.Commands.CommandFeatures.CommandArgType;

public class CommandArgTypeJobType() : CommandArgType("jobType")
{
    public override bool CheckValue()
    {
        try
        {
            EnumConverter<JobType>.ConvertToEnum(RawValue);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public override object ParseValue()
    {
        return EnumConverter<JobType>.ConvertToEnum(RawValue);
    }
}
