namespace EasyCLI.Localization;

public static class JobCheckRules
{
    /// <summary>
    /// Converts a JobCheckRule enum to human a readable string
    /// </summary>
    /// <param name="rule">Rule to translate</param>
    /// <returns>Readable rule</returns>
    /// <exception cref="ArgumentException">In case the provided rule is unknown</exception>
    public static string GetString(EasyLib.Enums.JobCheckRules rule)
    {
        return rule switch
        {
            EasyLib.Enums.JobCheckRules.Valid => "Check successful, no errors found.",
            EasyLib.Enums.JobCheckRules.SourcePathInvalid => "Source path is not valid.",
            EasyLib.Enums.JobCheckRules.DestinationPathInvalid => "Destination path is not valid.",
            EasyLib.Enums.JobCheckRules.SourcePathDoesNotExist => "Source path does not exist.",
            EasyLib.Enums.JobCheckRules.DestinationPathDoesNotExist => "Destination path does not exist.",
            EasyLib.Enums.JobCheckRules.SharedRoot => "Source and destination paths share the same root.",
            EasyLib.Enums.JobCheckRules.DuplicateId => "Job with the same ID already exists.",
            EasyLib.Enums.JobCheckRules.DuplicateName => "Job with the same name already exists.",
            EasyLib.Enums.JobCheckRules.DuplicatePaths =>
                "Job with the same source and destination paths already exists.",
            EasyLib.Enums.JobCheckRules.DestinationNotEmpty => "Destination folder is not empty.",
            _ => throw new ArgumentException("Invalid job check rule.")
        };
    }
}
