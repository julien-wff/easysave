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
            EasyLib.Enums.JobCheckRule.Valid => "Check successful, no errors found.",
            EasyLib.Enums.JobCheckRule.SourcePathInvalid => "Source path is not valid.",
            EasyLib.Enums.JobCheckRule.DestinationPathInvalid => "Destination path is not valid.",
            EasyLib.Enums.JobCheckRule.SourcePathDoesNotExist => "Source path does not exist.",
            EasyLib.Enums.JobCheckRule.DestinationPathDoesNotExist => "Destination path does not exist.",
            EasyLib.Enums.JobCheckRule.SharedRoot => "Source and destination paths share the same root.",
            EasyLib.Enums.JobCheckRule.DuplicateId => "Job with the same ID already exists.",
            EasyLib.Enums.JobCheckRule.DuplicateName => "Job with the same name already exists.",
            EasyLib.Enums.JobCheckRule.DuplicatePaths =>
                "Job with the same source and destination paths already exists.",
            EasyLib.Enums.JobCheckRule.DestinationNotEmpty => "Destination folder is not empty.",
            _ => throw new ArgumentException("Invalid job check rule.")
        };
    }
}
