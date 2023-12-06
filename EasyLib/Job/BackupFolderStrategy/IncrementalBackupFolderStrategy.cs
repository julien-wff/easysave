namespace EasyLib.Job.BackupFolderStrategy;

public class IncrementalBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, string jobName,
        string destinationPath)
    {
        return folders;
    }
}
