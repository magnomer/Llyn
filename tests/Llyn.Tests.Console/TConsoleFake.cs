using System.Collections.Concurrent;
using System.Globalization;
using System.Runtime.ExceptionServices;
using Llyn.UIDemeanor;
using Xunit;

namespace Llyn.Tests;

internal sealed class TConsoleFake : BConsole
{
    private readonly ConcurrentQueue<(int BConsoleWidth, int BConsoleHeight)> _tConsoleFakeSizes = new();
    private readonly Dictionary<string, string> _tConsoleFakePalette = new(StringComparer.Ordinal);
    private string[][]? _tConsoleFakeCells;
    private string?[][]? _tConsoleFakeTints;
    private int _tConsoleFakeReads;

    public TConsoleFake()
    {
        List<string> roles = [];
        TConsoleFakeTheme = new BTheme(role =>
        {
            if (!roles.Contains(role))
            {
                roles.Add(role);
            }

            return string.Create(
                CultureInfo.InvariantCulture, $"white #{roles.IndexOf(role) + 1:X6} on black #000000");
        });
        foreach (string role in roles)
        {
            _tConsoleFakePalette[TConsoleFakeTheme.BThemeRead(role)] = role;
        }
    }

    public BTheme TConsoleFakeTheme { get; }

    public List<string> TConsoleFakeOutput { get; } = [];

    public List<string> TConsoleFakeError { get; } = [];

    public List<string> TConsoleFakeTrail { get; } = [];

    public bool TConsoleFakeTerminal { get; set; }

    public bool TConsoleFakeAmbiguous { get; set; }

    public bool TConsoleFakeBatch { get; set; }

    public string? TConsoleFakeAnswer { get; set; }

    public int TConsoleFakeWidth { get; set; } = 80;

    public int TConsoleFakeHeight { get; set; } = 24;

    public int TConsoleFakeFaults { get; set; }

    public BlockingCollection<ConsoleKeyInfo> TConsoleFakeKeys { get; } = [];

    public bool? TConsoleFakeWindow { get; private set; }

    public List<string> TConsoleFakeFrames { get; } = [];

    public List<string[]> TConsoleFakeShots { get; } = [];

    public string[] TConsoleFakeGrid =>
        _tConsoleFakeCells?.Select(static row => string.Concat(row)).ToArray() ?? [];

    public string?[][] TConsoleFakeRoles =>
        _tConsoleFakeTints?.Select(static row => row.ToArray()).ToArray() ?? [];

    public int TConsoleFakeReads => Volatile.Read(ref _tConsoleFakeReads);

    public static void TConsoleFakeRun(Action run, int seconds = 60)
    {
        ExceptionDispatchInfo? failure = null;
        Thread runner = new(() =>
        {
            try
            {
                run();
            }
            catch (Exception exception)
            {
                failure = ExceptionDispatchInfo.Capture(exception);
            }
        })
        {
            IsBackground = true,
        };
        runner.Start();

        Assert.True(runner.Join(TimeSpan.FromSeconds(seconds)), $"The loop did not end within {seconds} seconds.");
        failure?.Throw();
    }

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

    public void TConsoleFakeSettle(ConsoleKeyInfo key, BLoop loop, Action batch, CancellationToken stop)
    {
        int reads = TConsoleFakeReads;
        TConsoleFakeKeys.Add(key);
        if (!SpinWait.SpinUntil(() => TConsoleFakeReads > reads || stop.IsCancellationRequested, 5000)
            || stop.IsCancellationRequested)
        {
            return;
        }

        TaskCompletionSource handled = new(TaskCreationOptions.RunContinuationsAsynchronously);
        loop.BLoopDefer(() => loop.BLoopDefer(() =>
        {
            batch();
            handled.SetResult();
        }));
        _ = SpinWait.SpinUntil(() => handled.Task.IsCompleted || stop.IsCancellationRequested, 5000);
    }

    public override void BConsoleFramePrint(string frame)
    {
        TConsoleFakeFrames.Add(frame);
        _tConsoleFakeCells ??= Enumerable.Range(0, TConsoleFakeHeight)
            .Select(_ => Enumerable.Repeat(" ", TConsoleFakeWidth).ToArray())
            .ToArray();
        _tConsoleFakeTints ??= Enumerable.Range(0, TConsoleFakeHeight)
            .Select(_ => new string?[TConsoleFakeWidth])
            .ToArray();
        string? tint = null;
        BCanvasWidth measure = new(TConsoleFakeAmbiguous);
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
                else if (frame[end] == 'm')
                {
                    tint = _tConsoleFakePalette.GetValueOrDefault(frame[index..(end + 1)]);
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
                _tConsoleFakeTints[row][column] = tint;
                if (cells == 2)
                {
                    _tConsoleFakeCells[row][column + 1] = string.Empty;
                    _tConsoleFakeTints[row][column + 1] = tint;
                }
            }

            column += cells;
        }

        TConsoleFakeShots.Add(TConsoleFakeGrid);
        lock (_tConsoleFakeSizes)
        {
            Monitor.PulseAll(_tConsoleFakeSizes);
        }

        if (TConsoleFakeFaults > 0)
        {
            TConsoleFakeFaults--;
            throw new IOException("The console refused the frame.");
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
        Interlocked.Increment(ref _tConsoleFakeReads);
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
                _tConsoleFakeTints = null;
                while (TConsoleFakeFrames.Count == frames && Monitor.Wait(_tConsoleFakeSizes, 5000))
                {
                }
            }

            key = TConsoleFakeKeys.Take();
        }

        return key;
    }

    public override bool BConsoleKeyCheck()
    {
        return TConsoleFakeBatch && TConsoleFakeKeys.Count > 0;
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
