using EasyLib.Enums;
using EasyLib.Job.BackupFolderStrategy;

namespace EasyLib.Job;

public static class BackupFolderSelectorFactory
{
    public static BackupFolderSelector Create(JobType type, JobState state)
    {
        IBackupFolderStrategy typeSelector = type switch
        {
            JobType.Full => new FullBackupFolderStrategy(),
            JobType.Differential => new DifferentialBackupFolderStrategy(),
            JobType.Incremental => new IncrementalBackupFolderStrategy(),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        IBackupFolderStrategy stateSelector;
        switch (state)
        {
            case JobState.Copy:
            case JobState.Paused:
                stateSelector = new ResumeBackupFolderStrategy();
                break;
            default:
                stateSelector = new NewBackupFolderStrategy();
                break;
        }

        return new BackupFolderSelector(typeSelector, stateSelector);
    }
}
