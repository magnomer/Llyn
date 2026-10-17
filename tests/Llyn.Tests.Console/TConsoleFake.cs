using System.Collections.Concurrent;
using System.Globalization;
using Llyn.UIDemeanor;

namespace Llyn.Tests;

internal sealed class TConsoleFake : BConsole
{
    private readonly ConcurrentQueue<(int BConsoleWidth, int BConsoleHeight)> _tConsoleFakeSizes = new();
    private string[][]? _tConsoleFakeCells;

    public List<string> TConsoleFakeOutput { get; } = [];

    public List<string> TConsoleFakeError { get; } = [];

    public List<string> TConsoleFakeTrail { get; } = [];

    public bool TConsoleFakeTerminal { get; set; }

    public string? TConsoleFakeAnswer { get; set; }

    public int TConsoleFakeWidth { get; set; } = 80;

    public int TConsoleFakeHeight { get; set; } = 24;

    public BlockingCollection<ConsoleKeyInfo> TConsoleFakeKeys { get; } = [];

    public bool? TConsoleFakeWindow { get; private set; }

    public List<string> TConsoleFakeFrames { get; } = [];

    public string[] TConsoleFakeGrid =>
        _tConsoleFakeCells?.Select(static row => string.Concat(row)).ToArray() ?? [];

    public override void BConsoleNoticePrint(string text)
    {
        TConsoleFakeError.Add(text);
        TConsoleFakeTrail.Add(text);
    }

    public override void BConsoleResultPrint(string text)
    {
        TConsoleFakeOutput.Add(text);
    }

    public override string? BConsoleLineRead()
    {
        return TConsoleFakeAnswer;
    }

    public override bool BConsoleTerminalCheck()
    {
        return TConsoleFakeTerminal;
    }

    public override void BConsoleModeApply(Action interruption)
    {
    }

    public override void BConsoleModeRestore()
    {
        TConsoleFakeTrail.Add(nameof(BConsoleModeRestore));
    }

    public void TConsoleFakeAdjust(int width, int height)
    {
        _tConsoleFakeSizes.Enqueue((width, height));
        TConsoleFakeKeys.Add(default);
    }

    public override void BConsoleFramePrint(string frame)
    {
        TConsoleFakeFrames.Add(frame);
        _tConsoleFakeCells ??= Enumerable.Range(0, TConsoleFakeHeight)
            .Select(_ => Enumerable.Repeat(" ", TConsoleFakeWidth).ToArray())
            .ToArray();
        BCanvasWidth measure = new(false);
        (int row, int column) = (0, 0);
        int index = 0;
        while (index < frame.Length)
        {
            if (frame[index] == '\e')
            {
                int end = index + 2;
                while (frame[end] is >= '0' and <= '?')
                {
                    end++;
                }

                if (frame[end] == 'H')
                {
                    string[] place = frame[(index + 2)..end].Split(';');
                    (row, column) = (int.Parse(place[0], CultureInfo.InvariantCulture) - 1,
                        int.Parse(place[1], CultureInfo.InvariantCulture) - 1);
                }

                index = end + 1;
                continue;
            }

            int length = StringInfo.GetNextTextElementLength(frame, index);
            string cluster = frame.Substring(index, length);
            int cells = measure.BCanvasWidthResolve(cluster);
            index += length;
            if (row < TConsoleFakeHeight && column + cells <= TConsoleFakeWidth)
            {
                _tConsoleFakeCells[row][column] = cluster;
                if (cells == 2)
                {
                    _tConsoleFakeCells[row][column + 1] = string.Empty;
                }
            }

            column += cells;
        }

        lock (_tConsoleFakeSizes)
        {
            Monitor.PulseAll(_tConsoleFakeSizes);
        }
    }

    public override (int BConsoleWidth, int BConsoleHeight) BConsoleSizeRead()
    {
        lock (_tConsoleFakeSizes)
        {
            return (TConsoleFakeWidth, TConsoleFakeHeight);
        }
    }

    public override ConsoleKeyInfo BConsoleKeyRead()
    {
        lock (_tConsoleFakeSizes)
        {
            while (TConsoleFakeFrames.Count == 0 && Monitor.Wait(_tConsoleFakeSizes, 5000))
            {
            }
        }

        ConsoleKeyInfo key = TConsoleFakeKeys.Take();
        while (key == default && _tConsoleFakeSizes.TryDequeue(out (int BConsoleWidth, int BConsoleHeight) size))
        {
            lock (_tConsoleFakeSizes)
            {
                int frames = TConsoleFakeFrames.Count;
                (TConsoleFakeWidth, TConsoleFakeHeight) = size;
                _tConsoleFakeCells = null;
                while (TConsoleFakeFrames.Count == frames && Monitor.Wait(_tConsoleFakeSizes, 5000))
                {
                }
            }

            key = TConsoleFakeKeys.Take();
        }

        return key;
    }

    public override void BConsoleWindowApply()
    {
        TConsoleFakeWindow = true;
    }

    public override void BConsoleWindowRestore()
    {
        if (TConsoleFakeWindow is true)
        {
            TConsoleFakeWindow = false;
            TConsoleFakeTrail.Add(nameof(BConsoleWindowRestore));
        }
    }
}
