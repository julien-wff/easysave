namespace EasyLib.Job.BackupFolderStrategy;

/// <summary>
/// Because the strategy is new no backup folder is existing
/// </summary>
public class NewBackupFolderStrategy : IBackupFolderStrategy
{
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, string jobName,
        string destinationPath)
    {
        if (!folders.Any())
        {
            var finalDestinationPath = CreateDir(jobName, destinationPath);
            var destFolder = new List<List<string>>() { new List<string>() { finalDestinationPath } };
            return destFolder;
        }
        else if (folders.Count == 1)
        {
            var finalDestinationPath = CreateDir(jobName, destinationPath);
            var finalDestinationFolder = folders;
            finalDestinationFolder[0].Add(finalDestinationPath);
            return finalDestinationFolder;
        }
        else
        {
            var finalDestinationFolder = folders;
            finalDestinationFolder[1] = folders[2];
            var finalDestinationPath = CreateDir(jobName, destinationPath);
            finalDestinationFolder[2] = new List<string>() { finalDestinationPath };
            return finalDestinationFolder;
        }
    }

    private string CreateDir(string jobName, string destinationPath)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string finalDestinationPath = Path.Join(destinationPath, date + "_" + jobName);
        Directory.CreateDirectory(finalDestinationPath);
        return finalDestinationPath;
    }
}
