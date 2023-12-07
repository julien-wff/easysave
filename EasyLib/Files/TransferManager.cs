﻿using EasyLib.Enums;
using EasyLib.Events;
using EasyLib.Json;

namespace EasyLib.Files;

/// <summary>
/// this class is used to handle the transfer of files from a source to a destination
/// </summary>
public class TransferManager : IJobStatusPublisher
{
    private readonly Job.Job _job;
    private readonly BackupFolder _sourceFolder;
    private readonly List<IJobStatusSubscriber> _subscribers = new();

    public TransferManager(Job.Job job)
    {
        _job = job;
        _sourceFolder = new BackupFolder(_job.SourceFolder);
        InstructionsFolder = new BackupFolder(_job.DestinationFolder);
    }

    public BackupFolder InstructionsFolder { get; private set; }

    public void Subscribe(IJobStatusSubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public void Unsubscribe(IJobStatusSubscriber subscriber)
    {
        _subscribers.Remove(subscriber);
    }

    /// <summary>
    /// Update the instance source folder and the files count and size from the job instance
    /// </summary>
    public void ScanSource()
    {
        _notifySubscribersForChange();
        _job.State = JobState.SourceScan;
        _sourceFolder.Walk(_job.SourceFolder);
        _notifySubscribersForChange();
        _job.FilesCount = 0;
        _job.FilesSizeBytes = 0;
        _getFileInfo(_sourceFolder);
    }

    /// <summary>
    /// Used in ScanSource to go trough the file tree and get the size and number of files
    /// </summary>
    /// <param name="folder"></param>
    private void _getFileInfo(BackupFolder folder)
    {
        foreach (var file in folder.Files)
        {
            _job.FilesSizeBytes += file.Size;
            _job.FilesCount++;
        }

        _notifySubscribersForChange();

        foreach (var subFolder in folder.SubFolders)
        {
            _getFileInfo(subFolder);
        }
    }

    /// <summary>
    /// Depending on the job type, this method will call the right method to compute the difference between the source and the destination
    /// </summary>
    /// <param name="folders"></param>
    public void ComputeDifference(List<List<string>> folders)
    {
        _job.State = JobState.DifferenceCalculation;
        InstructionsFolder = new BackupFolder(folders[1][0]);
        if (folders[2].Any())
        {
            InstructionsFolder.SubFolders.Add(new BackupFolder(folders[3][0]));
            _compareBackupPath(InstructionsFolder.SubFolders[0], folders[0]);
        }
        else
        {
            _compareBackupPath(InstructionsFolder, folders[0]);
        }
    }

    /// <summary>
    /// Compare the source with the existing backup folders if there is any
    /// The result is stored in the InstructionsFolder property
    /// </summary>
    /// <param name="Instruction"></param>
    /// <param name="pathList"></param>
    private void _compareBackupPath(BackupFolder Instruction, List<string> pathList)
    {
        Instruction.SubFolders = _sourceFolder.SubFolders;
        Instruction.Files = _sourceFolder.Files;
        foreach (var path in pathList)
        {
            var backupFolder = new BackupFolder(path + Path.DirectorySeparatorChar);
            backupFolder.Walk(path + Path.DirectorySeparatorChar);
            Instruction.SubFolders = _compareFolders(Instruction.SubFolders, backupFolder.SubFolders);
            Instruction.Files = _compareFiles(Instruction.Files, backupFolder.Files);
        }
    }

    /// <summary>
    /// Recursively compare the subfolders of two lists of BackupFolder objects and returns the list of all the folders and non existing files in the destination folders
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    private List<BackupFolder> _compareFolders(List<BackupFolder> source, List<BackupFolder> destination)
    {
        foreach (var folder in destination)
        {
            var sourceFolder = source.Find(s => s.Name == folder.Name);
            if (sourceFolder != null)
            {
                sourceFolder.SubFolders = _compareFolders(sourceFolder.SubFolders, folder.SubFolders);
                sourceFolder.Files = _compareFiles(sourceFolder.Files, folder.Files);
            }
        }

        return source;
    }

    /// <summary>
    /// Compare the files of two lists of BackupFile objects and returns the list of all the non existing files in the destination
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    private List<BackupFile> _compareFiles(List<BackupFile> source, List<BackupFile> destination)
    {
        source = source.Where(s => !destination.Exists(d => d.Hash == s.Hash)).ToList();
        return source;
    }

    /// <summary>
    /// This class take the destination folder path and create the folder structure for the backup
    /// </summary>
    /// <param name="destinationFolderPath"></param>
    /// <returns></returns>
    public void CreateDestinationStructure()
    {
        _job.State = JobState.DestinationStructureCreation;
        string actualJobPath = Path.Combine(_job.DestinationFolder, InstructionsFolder.Name);
        if (!Directory.Exists(actualJobPath))
        {
            Directory.CreateDirectory(actualJobPath);
        }

        _createTree(actualJobPath, InstructionsFolder);
    }

    /// <summary>
    /// Create the folder structure for the backup
    /// </summary>
    /// <param name="parentPath"></param>
    /// <param name="folder"></param>
    private void _createTree(string parentPath, BackupFolder folder)
    {
        foreach (var subFolder in folder.SubFolders)
        {
            string actualSubFolderPath = Path.Combine(parentPath, subFolder.Name);
            _notifySubscribersForChange();
            Directory.CreateDirectory(actualSubFolderPath);
            _createTree(actualSubFolderPath, subFolder);
        }
    }

    public void TransferFiles()
    {
        _job.State = JobState.Copy;
        _transferFile(InstructionsFolder, "", InstructionsFolder.Name);
    }

    private void _transferFile(BackupFolder folder, string sourcePath, string destinationPath)
    {
        foreach (var file in folder.Files)
        {
            _job.CurrentFileSource = Path.Combine(_job.SourceFolder, sourcePath, file.Name);
            _job.CurrentFileDestination = Path.Combine(_job.DestinationFolder, destinationPath, file.Name);

            var copyStart = DateTime.Now;
            File.Copy(_job.CurrentFileSource, _job.CurrentFileDestination, true);
            var copyEnd = DateTime.Now;
            _job.FilesCopied++;
            _job.FilesBytesCopied += file.Size;
            _notifySubscribersForChange();

            LogManager.Instance.AppendLog(new JsonLogElement
            {
                JobName = _job.Name,
                SourcePath = Path.Combine(_job.SourceFolder, file.Name),
                DestinationPath = Path.Combine(_job.DestinationFolder, file.Name),
                FileSize = file.Size,
                TransferTime = (int)(copyEnd - copyStart).TotalMilliseconds
            });
        }

        foreach (var subFolder in folder.SubFolders)
        {
            _transferFile(subFolder, Path.Combine(sourcePath, subFolder.Name),
                Path.Combine(destinationPath, subFolder.Name));
        }
    }

    private void _notifySubscribersForChange()
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.OnJobProgress(_job);
        }
    }
}
