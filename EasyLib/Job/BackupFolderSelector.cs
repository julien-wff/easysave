using EasyLib.Job.BackupFolderStrategy;

namespace EasyLib.Job;

public class BackupFolderSelector
{
    private IBackupFolderStrategy _typeSelector;
    private IBackupFolderStrategy _stateSelector;
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
    /// <returns></returns>
    public List<string> SelectFolders(List<string> folders, string? pausedJob)
    {
        List<string> typeSelectedFolders = _typeSelector.SelectFolders(folders, pausedJob);
        List<string> stateSelectedFolders = _stateSelector.SelectFolders(typeSelectedFolders, pausedJob);
        return stateSelectedFolders;
    }
}