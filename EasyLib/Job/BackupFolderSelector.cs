﻿using EasyLib.Job.BackupFolderStrategy;

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
    /// <param name="lastFolderPath"></param>
    /// <param name="jobType"></param>
    /// <param name="destinationFolder"></param>
    /// <returns></returns>
    public List<List<string>> SelectFolders(List<List<string>> folders, string lastFolderPath, Enum jobType,
        string destinationFolder)
    {
        List<List<string>> typeSelectedFolders = _typeSelector.SelectFolders(
            folders,
            lastFolderPath,
            jobType,
            destinationFolder);
        List<List<string>> stateSelectedFolders =
            _stateSelector.SelectFolders(typeSelectedFolders, lastFolderPath, jobType, destinationFolder);
        return stateSelectedFolders;
    }

    /// <summary>
    /// Create a destination path name for the backup job.
    /// </summary>
    /// <param name="jobType"></param>
    /// <param name="destinationPath"></param>
    /// <returns></returns>
    public static string GetDestinationPath(Enum jobType, string destinationPath)
    {
        string date = DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss");
        string finalDestinationPath =
            Path.Join(destinationPath, date + "_" + jobType, Path.DirectorySeparatorChar.ToString());
        return finalDestinationPath;
    }
}
