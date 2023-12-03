namespace EasyLib.Enums;

/// <summary>
/// List of different rules to respect when creating or checking a job
/// </summary>
public enum JobCheckRules
{
    Valid,
    SourcePathInvalid,
    DestinationPathInvalid,
    SourcePathDoesNotExist,
    DestinationPathDoesNotExist,
    SharedRoot,
    DuplicateId,
    DuplicateName,
    DuplicatePaths,
    DestinationNotEmpty,
}
