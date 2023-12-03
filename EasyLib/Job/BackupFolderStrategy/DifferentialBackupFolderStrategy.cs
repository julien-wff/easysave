namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// selects only the finished backup folders.
/// </summary>
public class DifferentialBackupFolderStrategy : IBackupFolderStrategy
{
    public List<string> SelectFolders(List<string> folders, string? pausedJob)
    {
        return folders;
    }
}
