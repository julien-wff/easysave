
using EasyLib.Job;

namespace EasyLib.Files;

/// <summary>
/// this class is used to handle the transfer of files from a source to a destination
/// </summary>
public class TransferManager
{
    public Job.Job Job;
    private BackupFolder _sourceFolder;
    private BackupFolder _instructionsFolder;
    public BackupFolder InstructionsFolder => _instructionsFolder;
    
    public TransferManager(Job.Job job)
    {
        Job = job;
        _sourceFolder = new BackupFolder(Job.SourceFolder);
        _instructionsFolder = new BackupFolder(Job.DestinationFolder);
    }
    /// <summary>
    /// Update the instance source folder and the files count and size from the job instance
    /// </summary>
    public void ScanSource()
    {
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
    
    public void CreateDestStructure()
    {
        
    }
    
    public void TransferFile()
    {
        
    }
    
    public void TransferFiles()
    {
        
    }
    
}
