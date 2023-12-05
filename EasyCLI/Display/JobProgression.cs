using EasyCLI.Localization;
using EasyLib.Enums;
using EasyLib.Job;

namespace EasyCLI.Display;

public static class JobProgression
{
    private static int _lastConsoleDown;
    private static int _lastLinesPrinted;

    public static void PrintJobProgression(Job job)
    {
        switch (job.State)
        {
            case JobState.End:
            case JobState.Paused:
                _printMessage($"Job #{job.Id} - {job.Name} | Waiting to start...");
                break;

            case JobState.SourceScan:
                _printMessage($"Job #{job.Id} - {job.Name} | Scanning source folder...");
                break;

            case JobState.DifferenceCalculation:
                _printMessage($"Job #{job.Id} - {job.Name} | Calculating differences for {job.FilesCount} files...");
                break;

            case JobState.DestinationStructureCreation:
                _printMessage(
                    $"Job #{job.Id} - {job.Name} | Creating destination structure for {job.FilesCount} files...");
                break;

            case JobState.Copy:
                _printCopyProgression(job);
                break;

            default:
                throw new ArgumentOutOfRangeException(nameof(job), "Invalid state");
        }
    }

    public static void Reset()
    {
        _lastConsoleDown = 0;
        _lastLinesPrinted = 0;
    }

    private static void _moveCursorToTop(int up)
    {
        Console.SetCursorPosition(0, Console.CursorTop - _lastConsoleDown);
        _lastConsoleDown = up;
    }

    private static void _printMessage(string message)
    {
        var consoleWidth = Console.WindowWidth;

        var consoleLines = new List<string>();

        foreach (var line in message.Split('\n'))
        {
            var lineLength = line.Length;

            var lineCount = (int)Math.Max(1, Math.Ceiling((float)lineLength / consoleWidth));

            for (var i = 0; i < lineCount; i++)
            {
                var start = i * consoleWidth;
                var end = Math.Min(start + consoleWidth, lineLength);
                var subLine = line[start..end];
                subLine = subLine
                    .Replace('\r', ' ')
                    .PadRight(consoleWidth);

                consoleLines.Add(subLine);
            }
        }

        var linesPrinted = consoleLines.Count;
        if (consoleLines.Count < _lastLinesPrinted)
        {
            consoleLines.AddRange(Enumerable.Repeat(new string(' ', consoleWidth),
                _lastLinesPrinted - consoleLines.Count));
        }

        _lastLinesPrinted = linesPrinted;

        _moveCursorToTop(consoleLines.Count);
        Console.WriteLine(string.Join(Environment.NewLine, consoleLines));
    }

    private static void _printCopyProgression(Job job)
    {
        var width = Console.WindowWidth;
        var barWidth = width - 10;
        var progression = Math.Clamp(job.FilesCopied / (double)job.FilesCount, 0, 1);
        var result = "";
        result += $"Job #{job.Id} - {job.Name} | Copying files {job.FilesCopied}/{job.FilesCount}";
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
        _printMessage(result);
    }
}
