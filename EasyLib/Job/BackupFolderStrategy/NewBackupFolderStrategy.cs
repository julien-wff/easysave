namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// Because the strategy is new no backup folder is existing
/// </summary>
public class NewBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        var destinationPath = BackupFolderSelector.GetDestinationPath(jobType, destinationFolder);
        if (folders[0].Count != 0 && folders[2].Count != 0)
        {
            return
            [
                folders[0],
                [destinationPath],
                folders[2]
            ];
        }

        if (folders[0].Count != 0)
        {
            return
            [
                folders[0],
                [destinationPath],
                []
            ];
        }

        return
        [
            [],
            [destinationPath],
            []
        ];
    }
}
