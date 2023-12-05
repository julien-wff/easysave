namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// if the backup job is paused, the backup folder selected is existing and we return it.
/// </summary>
public class ResumeBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, string jobName,
        string destinationPath)
    {
        if (!folders[0].Any())
        {
            throw new Exception("Cant resume with no destination");
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
