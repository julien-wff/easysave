namespace EasyLib.Files;

/// <summary>
/// Recursively reproduce the file tree given by the file path
/// </summary>
public class BackupFolder
{
    public readonly string Name;
    public List<BackupFolder> SubFolders;
    public List<BackupFile> Files;
    /// <summary>
    /// Constructor of the BackupFolder class
    /// </summary>
    /// <param name="path"></param>
    public BackupFolder(string path)
    {
        this.Name = Path.GetFileName(Path.GetDirectoryName(path));
        this.SubFolders = new();
        this.Files = new();
    }
    /// <summary>
    /// This method recursively walks through the file tree
    /// </summary>
    /// <param name="path"></param>
    public void Walk(string path)
    {
        var directoryInfo = new DirectoryInfo(path);
        var subDirectories = directoryInfo.GetDirectories();
        var files = directoryInfo.GetFiles();
        
        foreach (var subDirectory in subDirectories)
        {
            var backupFolder = new BackupFolder(subDirectory.FullName+@"\");
            backupFolder.Walk(subDirectory.FullName);
            SubFolders.Add(backupFolder);
        }
        
        foreach (var file in files)
        {
            var backupFile = new BackupFile(file.FullName);
            Files.Add(backupFile);
        }
    }
}
