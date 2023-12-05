using EasyCLI.Localization;
using EasyLib.Enums;
using EasyLib.Job;

namespace EasyCLI.Display;

public static class JobProgression
{
    public static void PrintJobProgression(Job job)
    {
        switch (job.State)
        {
            case JobState.End:
            case JobState.Paused:
                Console.WriteLine($"Job #{job.Id} - {job.Name} waiting to start");
                break;

            case JobState.SourceScan:
                Console.WriteLine($"Job #{job.Id} - {job.Name} scanning source folder...");
                break;

            case JobState.DifferenceCalculation:
                Console.WriteLine($"Job #{job.Id} - {job.Name} calculating differences for {job.FilesCount} files...");
                break;

            case JobState.DestinationStructureCreation:
                Console.WriteLine(
                    $"Job #{job.Id} - {job.Name} creating destination structure for {job.FilesCount} files...");
                break;

            case JobState.Copy:
                _printCopyProgression(job);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(job), "Invalid state");
        }
    }

    private static void _printCopyProgression(Job job)
    {
        var width = Console.WindowWidth;
        var barWidth = width - 10;
        var progression = Math.Clamp(job.FilesCopied / (double)job.FilesCount, 0, 1);
        var result = $"Job #{job.Id} - {job.Name} copying files {job.FilesCopied}/{job.FilesCount}";
        result += " ";
        result += $"({FileSizeFormatter.Format(job.FilesBytesCopied)}/{FileSizeFormatter.Format(job.FilesSizeBytes)})";
        result += "\n";
        result += "[";
        result += new string('=', (int)(barWidth * progression));
        result += new string(' ', (int)(barWidth * (1 - progression)));
        result += $"] {progression * 100:0.0}%";
        result += "\n\n";
        result += $"Source       {job.CurrentFileSource}";
        result += "\n";
        result += $"Destination  {job.CurrentFileDestination}";
        Console.WriteLine(result);
    }
}
