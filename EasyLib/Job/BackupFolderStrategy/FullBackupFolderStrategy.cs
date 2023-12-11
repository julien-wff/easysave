namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// All the files of a backup are stored in the same folder.
/// So the list of selected folders is empty.
/// </summary>
public class FullBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        return
        [
            [],
            [lastFolderPath],
            []
        ];
    }
}
