namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// Interface for backup folder strategy
/// It is used to filter folders list for backup with multiple calls of SelectFolders method from different strategies
/// </summary>
public interface IBackupFolderStrategy
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="lastFolderPath"></param>
    /// <param name="jobName"></param>
    /// <param name="destinationFolder"></param>
    /// <returns></returns>
    List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder);
}
