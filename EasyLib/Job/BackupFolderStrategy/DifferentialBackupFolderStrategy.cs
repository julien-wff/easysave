namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// selects only the finished backup folders.
/// </summary>
public class DifferentialBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        var finalDestinationPath = BackupFolderSelector.GetDestinationPath(jobType, destinationFolder);
        var folderCount = folders[0].Count;
        return folderCount switch
        {
            0 => [[], [finalDestinationPath], []],
            1 =>
            [
                folders[0],
                [finalDestinationPath + Path.DirectorySeparatorChar + Path.GetDirectoryName(finalDestinationPath)],
                [finalDestinationPath]
            ],
            _ =>
            [
                Directory.GetDirectories(folders[0][folderCount - 1]).Append(folders[0][0]).ToList(),
                [finalDestinationPath + Path.DirectorySeparatorChar + Path.GetDirectoryName(finalDestinationPath)],
                [folders[0][folderCount - 2] + Path.DirectorySeparatorChar]
            ]
        };
    }
}
