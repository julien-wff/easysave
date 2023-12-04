namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// Because the strategy is new no backup folder is existing
/// </summary>
public class NewBackupFolderStrategy : IBackupFolderStrategy
{
    public List<string> SelectFolders(List<string> folders, string? pausedJob, string jobName, string destinationPath)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        Directory.CreateDirectory(destinationPath + @"\" + date + jobName);
        folders.Add(destinationPath + @"\" + date + jobName);
        return folders;
    }
}
