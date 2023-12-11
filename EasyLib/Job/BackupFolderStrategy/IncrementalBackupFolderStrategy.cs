namespace EasyLib.Job.BackupFolderStrategy;

public class IncrementalBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        return
        [
            folders[0],
            [lastFolderPath],
            []
        ];
    }
}
