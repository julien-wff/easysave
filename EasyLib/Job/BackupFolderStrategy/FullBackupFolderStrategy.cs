namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// All the files of a backup are stored in the same folder.
/// So the list of selected folders is empty.
/// </summary>
public class FullBackupFolderStrategy : IBackupFolderStrategy
{
    public List<string> SelectFolders(List<string> folders, string? pausedJob)
    {
        return new List<string>();
    }
}
