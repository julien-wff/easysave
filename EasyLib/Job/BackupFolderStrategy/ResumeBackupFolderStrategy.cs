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
            return new List<List<string>>()
            {
                new List<string>(),
                new List<string>() { destinationPath },
                new List<string>()
            };
        }
        else
        {
            folders[0].Append(lastFolderPath);
            Console.WriteLine(folders[0]);
            return new List<List<string>>()
            {
                folders[0],
                new List<string>() { lastFolderPath },
                new List<string>()
            };
        }
    }
}
