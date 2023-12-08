namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// if the backup job is paused, the backup folder selected is existing and we return it.
/// </summary>
public class ResumeBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        if (!folders[0].Any())
        {
            var destinationPath = BackupFolderSelector.GetDestinationPath(jobType, destinationFolder);
            return new List<List<string>>()
            {
                new List<string>(),
                new List<string>() { destinationPath },
                new List<string>()
            };
        }
        else if (folders.Count == 1)
        {
            var finalDestinationFolder = folders;
            finalDestinationFolder[0].Add(lastFolderPath);
            return finalDestinationFolder;
        }
        else
        {
            return folders;
        }
    }
}
