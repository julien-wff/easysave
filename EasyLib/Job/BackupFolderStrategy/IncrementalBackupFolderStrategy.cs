namespace EasyLib.Job.BackupFolderStrategy;

public class IncrementalBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        return new List<List<string>>()
        {
            folders[0],
            new List<string>() { lastFolderPath },
            new List<string>()
        };
    }
}
