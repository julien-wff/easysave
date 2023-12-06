namespace EasyCLI.Localization;

public static class JobCheckRules
{
    /// <summary>
    /// Converts a JobCheckRule enum to human a readable string
    /// </summary>
    /// <param name="rule">Rule to translate</param>
    /// <returns>Readable rule</returns>
    /// <exception cref="ArgumentException">In case the provided rule is unknown</exception>
    public static string GetString(EasyLib.Enums.JobCheckRule rule)
    {
        return rule switch
        {
            EasyLib.Enums.JobCheckRule.Valid => Loc.T("Checks.Valid"),
            EasyLib.Enums.JobCheckRule.SourcePathInvalid => Loc.T("Checks.SourcePathInvalid"),
            EasyLib.Enums.JobCheckRule.DestinationPathInvalid => Loc.T("Checks.DestinationPathInvalid"),
            EasyLib.Enums.JobCheckRule.SourcePathDoesNotExist => Loc.T("Checks.SourcePathDoesNotExist"),
            EasyLib.Enums.JobCheckRule.DestinationPathDoesNotExist => Loc.T("Checks.DestinationPathDoesNotExist"),
            EasyLib.Enums.JobCheckRule.SharedRoot => Loc.T("Checks.SharedRoot"),
            EasyLib.Enums.JobCheckRule.DuplicateId => Loc.T("Checks.DuplicateId"),
            EasyLib.Enums.JobCheckRule.DuplicateName => Loc.T("Checks.DuplicateName"),
            EasyLib.Enums.JobCheckRule.DuplicatePaths => Loc.T("Checks.DuplicatePaths"),
            EasyLib.Enums.JobCheckRule.DestinationNotEmpty => Loc.T("Checks.DestinationNotEmpty"),
            _ => throw new ArgumentException("Invalid job check rule.")
        };
    }
}
