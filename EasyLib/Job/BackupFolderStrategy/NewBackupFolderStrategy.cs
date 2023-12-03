namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// Because the strategy is new no backup folder is existing
/// </summary>
public class NewBackupFolderStrategy : IBackupFolderStrategy
{
    public List<string> SelectFolders(List<string> folders, string? pausedJob)
    {
        return new List<string>();
    }
}
