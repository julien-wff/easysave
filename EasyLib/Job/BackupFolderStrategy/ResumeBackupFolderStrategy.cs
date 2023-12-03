namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// if the backup job is paused, the backup folder selected is existing and we return it.
/// </summary>
public class ResumeBackupFolderStrategy : IBackupFolderStrategy
{
    public List<string> SelectFolders(List<string> folders, string? pausedJob)
    {
        var selectedFolder = new List<string>();
        if (pausedJob != null)
        {
            selectedFolder.Add(pausedJob);
        }
        return selectedFolder;
    }
}
