namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// selects only the finished backup folders.
/// </summary>
public class DifferentialBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, string jobName,
        string destinationPath)
    {
        if (!folders[0].Any())
        {
            var finalDestinationPath = BackupFolderSelector.GetDestinationPath(jobName, destinationPath);
            var destFolder = new List<List<string>>() { new List<string>() { finalDestinationPath } };
            return destFolder;
        }
        else
        {
            if (folders[0].Count == 1)
            {
                return new List<List<string>>() { new List<string>() { folders[0][0] } };
            }
            else if (folders[0].Count == 2)
            {
                return new List<List<string>>()
                    { new List<string>() { folders[0][0] }, new List<string>(), new List<string>() { folders[0][1] } };
            }
            else
            {
                return new List<List<string>>()
                {
                    new List<string>() { folders[0][0] }, new List<string>() { folders[0][folders.Count() - 1] },
                    new List<string>() { folders[0].Last() }
                };
            }
        }
    }
}
