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
        if (folders[0].Any() && folders[2].Any())
        {
            return new List<List<string>>()
            {
                folders[0],
                new List<string>() { destinationPath },
                folders[2]
            };
        }
        else if (folders[0].Any())
        {
            return new List<List<string>>()
            {
                folders[0],
                new List<string>() { destinationPath },
                new List<string>()
            };
        }
        else
        {
            return new List<List<string>>()
            {
                new List<string>(),
                new List<string>() { destinationPath },
                new List<string>()
            };
        }
    }
}
