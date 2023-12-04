﻿using EasyLib.Enums;

namespace EasyLib.Files;

/// <summary>
/// this class is used to handle the transfer of files from a source to a destination
/// </summary>
public class TransferManager
{
    private BackupFolder _instructionsFolder;
    private BackupFolder _sourceFolder;
    public Job.Job Job;

    public TransferManager(Job.Job job)
    {
        Job = job;
        _sourceFolder = new BackupFolder(Job.SourceFolder);
        _instructionsFolder = new BackupFolder(Job.DestinationFolder);
    }

    public BackupFolder InstructionsFolder => _instructionsFolder;

    /// <summary>
    /// Update the instance source folder and the files count and size from the job instance
    /// </summary>
    public void ScanSource()
    {
        Job.State = JobState.SourceScan;
        _sourceFolder.Walk(Job.SourceFolder);
        Job.FilesCount = 0;
        Job.FilesSizeBytes = 0;
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
            Job.FilesSizeBytes += file.Size;
            Job.FilesCount++;
        }

        foreach (var subFolder in folder.SubFolders)
        {
            _getFileInfo(subFolder);
        }
    }

    public void ComputeDifference(List<string> folders)
    {
        Job.State = JobState.DifferenceCalculation;
        _instructionsFolder = _sourceFolder;
        foreach (var folder in folders)
        {
            BackupFolder tempFolder = new BackupFolder(folder);
            tempFolder.Walk(folder);
            CompareFolders(tempFolder, _instructionsFolder);
        }
    }

    private void CompareFolders(BackupFolder actualFolder, BackupFolder destinationFolder)
    {
        foreach (var sourceFile in actualFolder.Files)
        {
            foreach (var destinationFile in destinationFolder.Files)
            {
                if (sourceFile == destinationFile)
                {
                    destinationFolder.Files.Remove(destinationFile);
                }
            }
        }

        foreach (var actualSubFolder in actualFolder.SubFolders)
        {
            foreach (var subFolder in destinationFolder.SubFolders)
            {
                if (actualSubFolder == subFolder)
                {
                    CompareFolders(actualSubFolder, subFolder);
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
        Job.State = JobState.DestinationStructureCreation;
        Directory.CreateDirectory(destinationFolderPath);
        _createTree(destinationFolderPath, _instructionsFolder);
    }

    private void _createTree(string parentPath, BackupFolder folder)
    {
        foreach (var subFolder in folder.SubFolders)
        {
            Directory.CreateDirectory(parentPath + @"\" + subFolder.Name);
            _createTree(parentPath + @"\" + subFolder.Name, subFolder);
        }
    }

    public void TransferFiles(string destinationFolder)
    {
        Job.State = JobState.Copy;
        var sourceFolder = Job.SourceFolder;
        _transferFile(sourceFolder, destinationFolder, _instructionsFolder);
    }

    private void _transferFile(string sourceFolder, string destinationFolderPath, BackupFolder folder)
    {
        foreach (var file in folder.Files)
        {
            File.Copy(sourceFolder + @"\" + file.Name, destinationFolderPath + @"\" + file.Name);
        }

        foreach (var subFolder in folder.SubFolders)
        {
            _transferFile(sourceFolder + @"\" + subFolder.Name, destinationFolderPath + @"\" + subFolder.Name,
                subFolder);
        }
    }
}