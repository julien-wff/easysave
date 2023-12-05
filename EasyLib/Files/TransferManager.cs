using EasyLib.Enums;
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

    public void ComputeDifference(List<List<string>> folders)
    {
        _job.State = JobState.DifferenceCalculation;
        InstructionsFolder = _sourceFolder;
        if (folders.Count == 1)
        {
            foreach (var folder in folders[0])
            {
                var tempFolder = new BackupFolder(folder);
                tempFolder.Walk(folder);
            }
        }
    }

    private void _compareFolders(BackupFolder actualFolder, BackupFolder destinationFolder)
    {
        _notifySubscribersForChange();
        foreach (var sourceFile in actualFolder.Files)
        {
            foreach (var destinationFile in destinationFolder.Files)
            {
                if (sourceFile.Hash == destinationFile.Hash)
                {
                    destinationFolder.Files.Remove(destinationFile);
                }
            }
        }

        _notifySubscribersForChange();
        foreach (var actualSubFolder in actualFolder.SubFolders)
        {
            foreach (var subFolder in destinationFolder.SubFolders)
            {
                if (actualSubFolder.Name == subFolder.Name)
                {
                    _compareFolders(actualSubFolder, subFolder);
                }
            }
        }
    }

    /// <summary>
    /// This class take the destination folder path and create the folder structure for the backup
    /// </summary>
    /// <param name="destinationFolderPath"></param>
    /// <returns></returns>
    public void CreateDestinationStructure(string destinationFolderPath)
    {
        _job.State = JobState.DestinationStructureCreation;
        Directory.CreateDirectory(destinationFolderPath);
        _createTree(destinationFolderPath, InstructionsFolder);
    }

    private void _createTree(string parentPath, BackupFolder folder)
    {
        foreach (var subFolder in folder.SubFolders)
        {
            _notifySubscribersForChange();
            Directory.CreateDirectory(parentPath + Path.DirectorySeparatorChar + subFolder.Name);
            _createTree(parentPath + Path.DirectorySeparatorChar + subFolder.Name, subFolder);
        }
    }

    public void TransferFiles(string destinationFolder)
    {
        _job.State = JobState.Copy;
        var sourceFolder = _job.SourceFolder;
        _transferFile(sourceFolder, destinationFolder, InstructionsFolder);
    }

    private void _transferFile(string sourceFolder, string destinationFolderPath, BackupFolder folder)
    {
        foreach (var file in folder.Files)
        {
            _job.CurrentFileSource = Path.Combine(sourceFolder, file.Name);
            _job.CurrentFileDestination = Path.Combine(destinationFolderPath, file.Name);

            var copyStart = DateTime.Now;
            File.Copy(_job.CurrentFileSource, _job.CurrentFileDestination, true);
            var copyEnd = DateTime.Now;

            _job.FilesCopied++;
            _job.FilesBytesCopied += file.Size;
            _notifySubscribersForChange();

            LogManager.Instance.AppendLog(new JsonLogElement
            {
                JobName = _job.Name,
                SourcePath = Path.Combine(sourceFolder, file.Name),
                DestinationPath = Path.Combine(destinationFolderPath, file.Name),
                FileSize = file.Size,
                TransferTime = (int)(copyEnd - copyStart).TotalMilliseconds
            });
        }

        foreach (var subFolder in folder.SubFolders)
        {
            _transferFile(
                sourceFolder + Path.DirectorySeparatorChar + subFolder.Name,
                destinationFolderPath + Path.DirectorySeparatorChar + subFolder.Name,
                subFolder
            );
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
