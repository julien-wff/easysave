using System.Diagnostics;
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
        PriorityFileFolder = InstructionsFolder;
    }

    private BackupFolder InstructionsFolder { get; set; }
    private BackupFolder PriorityFileFolder { get; set; }

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
            if (_job.CancellationToken.IsCancellationRequested)
                return;
            _job.FilesSizeBytes += file.Size;
            _job.FilesCount++;
        }

        _notifySubscribersForChange();

        foreach (var subFolder in folder.SubFolders)
        {
            if (_job.CancellationToken.IsCancellationRequested)
                return;
            _getFileInfo(subFolder);
        }
    }

    /// <summary>
    /// Depending on the job type, this method will call the right method to compute the difference between the source and the destination
    /// </summary>
    /// <param name="folders"></param>
    public void ComputeDifference(List<List<string>> folders)
    {
        if (_job.CancellationToken.IsCancellationRequested)
            return;
        _job.State = JobState.DifferenceCalculation;
        InstructionsFolder = new BackupFolder(folders[1][0]);
        if (folders[2].Count != 0)
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
    /// <param name="instruction"></param>
    /// <param name="pathList"></param>
    private void _compareBackupPath(BackupFolder instruction, List<string> pathList)
    {
        if (_job.CancellationToken.IsCancellationRequested)
            return;
        instruction.SubFolders = _sourceFolder.SubFolders;
        instruction.Files = _sourceFolder.Files;
        foreach (var path in pathList)
        {
            if (_job.CancellationToken.IsCancellationRequested)
                return;
            var backupFolder = new BackupFolder(path + Path.DirectorySeparatorChar);
            backupFolder.Walk(path + Path.DirectorySeparatorChar);
            instruction.SubFolders = _compareFolders(instruction.SubFolders, backupFolder.SubFolders);
            instruction.Files = _compareFiles(instruction.Files, backupFolder.Files);
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
            if (_job.CancellationToken.IsCancellationRequested)
                return new List<BackupFolder>();
            var sourceFolder = source.Find(s => s.Name == folder.Name);

            if (sourceFolder == null)
                continue;

            sourceFolder.SubFolders = _compareFolders(sourceFolder.SubFolders, folder.SubFolders);
            sourceFolder.Files = _compareFiles(sourceFolder.Files, folder.Files);
        }

        return source;
    }

    /// <summary>
    /// Compare the files of two lists of BackupFile objects and returns the list of all the non existing files in the destination
    /// </summary>
    /// <param name="source"></param>
    /// <param name="destination"></param>
    /// <returns></returns>
    private static List<BackupFile> _compareFiles(List<BackupFile> source, List<BackupFile> destination)
    {
        source = source.Where(s => !destination.Exists(d => d.Hash == s.Hash)).ToList();
        return source;
    }

    /// <summary>
    /// This class take the destination folder path and create the folder structure for the backup
    /// </summary>
    /// <returns></returns>
    public void CreateDestinationStructure()
    {
        if (_job.CancellationToken.IsCancellationRequested)
            return;
        _job.State = JobState.DestinationStructureCreation;
        var actualJobPath = Path.Combine(_job.DestinationFolder, InstructionsFolder.Name);
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
        if (_job.CancellationToken.IsCancellationRequested)
            return;
        foreach (var subFolder in folder.SubFolders)
        {
            var actualSubFolderPath = Path.Combine(parentPath, subFolder.Name);
            _notifySubscribersForChange();
            Directory.CreateDirectory(actualSubFolderPath);
            _createTree(actualSubFolderPath, subFolder);
        }
    }

    public void TransferFiles()
    {
        if (_job.CancellationToken.IsCancellationRequested)
            return;
        _job.State = JobState.Copy;
        if (ConfigManager.Instance.PriorityFileExtensions.Any())
        {
            Interlocked.Increment(ref Job.Job.CurrentPriorityRunning);
            Job.Job.NotifyWaitingJobs.Reset();
            var filteredFiles = _filterPriorityFiles(InstructionsFolder);
            PriorityFileFolder = filteredFiles[0];
            InstructionsFolder = filteredFiles[1];
            _transferFile(PriorityFileFolder, "", PriorityFileFolder.Name);
            Interlocked.Decrement(ref Job.Job.CurrentPriorityRunning);
            Job.Job.NotifyWaitingJobs.Set();
        }

        while (Interlocked.Read(Job.Job.CurrentPriorityRunning) > 0)
        {
            Job.Job.NotifyWaitingJobs.WaitOne();
            Console.WriteLine("finished waiting");
            if (Interlocked.Read(Job.Job.CurrentPriorityRunning) > 0)
            {
                Job.Job.NotifyWaitingJobs.Reset();
            }
        }

        if (_job.CancellationToken.IsCancellationRequested)
            return;
        _transferFile(InstructionsFolder, "", InstructionsFolder.Name);
    }

    private void _transferFile(BackupFolder folder, string sourcePath, string destinationPath)
    {
        foreach (var file in folder.Files)
        {
            if (_job.CancellationToken.IsCancellationRequested)
                return;
            _job.CurrentFileSource = Path.Combine(_job.SourceFolder, sourcePath, file.Name);
            _job.CurrentFileDestination = Path.Combine(_job.DestinationFolder, destinationPath, file.Name);
            DateTime copyStart;
            DateTime copyEnd;
            if (file.Size >= ConfigManager.Instance.MaxFileSize)
            {
                Job.Job.MaxSizeFileCopying.WaitOne();
                copyStart = DateTime.Now;
                File.Copy(_job.CurrentFileSource, _job.CurrentFileDestination, true);
                copyEnd = DateTime.Now;
                Job.Job.MaxSizeFileCopying.Release();
            }
            else
            {
                copyStart = DateTime.Now;
                File.Copy(_job.CurrentFileSource, _job.CurrentFileDestination, true);
                copyEnd = DateTime.Now;
            }

            // check if the file has to be encrypted
            _job.FilesCopied++;
            _job.FilesBytesCopied += file.Size;
            _notifySubscribersForChange();

            var cryptoStart = DateTime.Now;
            var cryptoEnd = cryptoStart;
            if (ConfigManager.Instance.EncryptedFileExtensions
                .Contains(file.Extension)) // check if the file extension is in the list of encrypted file types
            {
                var fileEncryption = new Process() // create a new process to run the EasyCrypto.exe
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = ConfigManager.Instance.EasyCryptoPath, // take the easy crypto path from the config
                        ArgumentList = { _job.CurrentFileDestination, ConfigManager.Instance.XorKey },
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                cryptoStart = DateTime.Now; // start the timer for the crypto
                fileEncryption.Start();
                fileEncryption.WaitForExit();
                cryptoEnd = DateTime.Now; // end the timer for the crypto
            }

            LogManager.Instance.AppendLog(new LogElement
            {
                JobName = _job.Name,
                SourcePath = Path.Combine(_job.CurrentFileSource),
                DestinationPath = Path.Combine(_job.CurrentFileDestination),
                FileSize = file.Size,
                TransferTime = (int)(copyEnd - copyStart).TotalMilliseconds, // add the copy time to the log
                CryptoTime =
                    (int)(cryptoEnd - cryptoStart)
                    .TotalMilliseconds, // add the crypto time to the log 0 if the file is not encrypted
            });
        }

        foreach (var subFolder in folder.SubFolders)
        {
            _transferFile(subFolder, Path.Combine(sourcePath, subFolder.Name),
                Path.Combine(destinationPath, subFolder.Name));
        }
    }

    private List<BackupFolder> _filterPriorityFiles(BackupFolder instructionsFolder)
    {
        var tempPriorityFolders = new BackupFolder(instructionsFolder.Name + Path.DirectorySeparatorChar);
        var tempInstructionsFolders = new BackupFolder(instructionsFolder.Name + Path.DirectorySeparatorChar);
        foreach (var subFolder in instructionsFolder.SubFolders)
        {
            var selectedFiles = _filterPriorityFiles(subFolder);
            tempPriorityFolders.SubFolders.Add(selectedFiles[0]);
            tempInstructionsFolders.SubFolders.Add(selectedFiles[1]);
        }

        foreach (var file in instructionsFolder.Files)
        {
            if (ConfigManager.Instance.PriorityFileExtensions.Contains(file.Extension))
            {
                tempPriorityFolders.Files.Add(file);
            }
            else
            {
                tempInstructionsFolders.Files.Add(file);
            }
        }

        return new List<BackupFolder>() { tempPriorityFolders, tempInstructionsFolders };
    }

    private void _notifySubscribersForChange()
    {
        foreach (var subscriber in _subscribers)
        {
            subscriber.OnJobProgress(_job);
        }
    }
}
