namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// if the backup job is paused, the backup folder selected is existing and we return it.
/// </summary>
public class ResumeBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        if (lastFolderPath == "")
        {
            var destinationPath = BackupFolderSelector.GetDestinationPath(jobType, destinationFolder);
            return
            [
                [],
                [destinationPath],
                []
            ];
        }

        folders[0] = folders[0].Append(lastFolderPath).ToList();
        return
        [
            folders[0],
            [lastFolderPath],
            []
        ];
    }
}
