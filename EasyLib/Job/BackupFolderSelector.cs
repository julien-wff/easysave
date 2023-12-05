using EasyLib.Job.BackupFolderStrategy;

namespace EasyLib.Job;

public class BackupFolderSelector
{
    private readonly IBackupFolderStrategy _stateSelector;
    private readonly IBackupFolderStrategy _typeSelector;

    /// <summary>
    /// Constructor of the BackupFolderSelector class.
    /// </summary>
    /// <param name="typeSelector"></param>
    /// <param name="stateSelector"></param>
    public BackupFolderSelector(IBackupFolderStrategy typeSelector, IBackupFolderStrategy stateSelector)
    {
        _typeSelector = typeSelector;
        _stateSelector = stateSelector;
    }

    /// <summary>
    /// Call the SelectFolders method of the IBackupFolderStrategy object to filter the folders depending on the type and state of the backup job.
    /// </summary>
    /// <param name="folders"></param>
    /// <param name="pausedJob"></param>
    /// <param name="jobName"></param>
    /// <param name="destinationPath"></param>
    /// <returns></returns>
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, string jobName,
        string destinationPath)
    {
        List<List<string>> typeSelectedFolders =
            _typeSelector.SelectFolders(folders, lastFolderPath, jobName, destinationPath);
        List<List<string>> stateSelectedFolders =
            _stateSelector.SelectFolders(typeSelectedFolders, lastFolderPath, jobName, destinationPath);
        return stateSelectedFolders;
    }
}
