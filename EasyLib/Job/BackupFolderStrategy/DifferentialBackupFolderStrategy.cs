namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// selects only the finished backup folders.
/// </summary>
public class DifferentialBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        if (folders[0].Count == 0)
        {
            var finalDestinationPath = BackupFolderSelector.GetDestinationPath(jobType, destinationFolder);
            return
            [
                [],
                [finalDestinationPath],
                []
            ];
        }

        var folderCount = folders[0].Count;
        return folderCount switch
        {
            1 => [[folders[0][0]]],
            2 => [[folders[0][0]], [], [folders[0][1]]],
            _ =>
            [
                [folders[0][0]],
                [folders[0][folderCount - 1]],
                [folders[0][folderCount - 1]]
            ]
        };
    }
}
