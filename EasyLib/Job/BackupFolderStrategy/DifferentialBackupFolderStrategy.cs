namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// 
/// </summary>
public class DifferentialBackupFolderStrategy : IBackupFolderStrategy
{
    public List<string> SelectFolders(List<string> folders, string? pausedJob)
    {
        return folders;
    }
}
