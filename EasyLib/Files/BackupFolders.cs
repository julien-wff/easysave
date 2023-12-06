namespace EasyLib.Files;

/// <summary>
/// Recursively reproduce the file tree given by the file path
/// </summary>
public class BackupFolder
{
    public string Name;

    /// <summary>
    /// Constructor of the BackupFolder class
    /// </summary>
    /// <param name="path"></param>
    public BackupFolder(string path)
    {
        Name = Path.GetFileName(Path.GetDirectoryName(path))!;
    }

    public List<BackupFile> Files { get; set; } = new();
    public List<BackupFolder> SubFolders { get; set; } = new();

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
            var backupFolder = new BackupFolder(subDirectory.FullName + Path.DirectorySeparatorChar);
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
