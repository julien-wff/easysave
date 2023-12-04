namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// Interface for backup folder strategy
/// It is used to filter folders list for backup with multiple calls of SelectFolders method from different strategies
/// </summary>
public interface IBackupFolderStrategy
{
    /// <summary>
    /// This method is used to filter backup folder list for backup depending on strategy
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="pausedJob"></param>
    /// <returns></returns>
    List<string> SelectFolders(List<string> folders, string? pausedJob, string jobName, string destinationPath);
}
