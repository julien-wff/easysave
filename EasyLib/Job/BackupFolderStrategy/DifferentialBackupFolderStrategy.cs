namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// selects only the finished backup folders.
/// </summary>
public class DifferentialBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        if (!folders[0].Any())
        {
            var finalDestinationPath = BackupFolderSelector.GetDestinationPath(jobType, destinationFolder);
            return new List<List<string>>()
            {
                new List<string>(),
                new List<string>() { finalDestinationPath },
                new List<string>(),
            };
        }
        else
        {
            var folderCount = folders[0].Count;
            switch (folderCount)
            {
                case 1:
                    return new List<List<string>>()
                    {
                        new List<string>() { folders[0][0] }
                    };
                case 2:
                    return new List<List<string>>()
                    {
                        new List<string>() { folders[0][0] },
                        new List<string>(),
                        new List<string>() { folders[0][1] }
                    };
                default:
                    return new List<List<string>>()
                    {
                        new List<string>() { folders[0][0] },
                        new List<string>() { folders[0][folderCount - 1] },
                        new List<string>() { folders[0][folderCount - 1] }
                    };
            }
        }
    }
}
