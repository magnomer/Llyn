using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using Llyn.Conduct;

namespace Llyn.UIDemeanor;

internal sealed class BWindow
{
    private const int BWindowWidthMinimum = 80;
    private const int BWindowHeightMinimum = 24;
    private const int BWindowLegendWidth = 8;

    private readonly BConsole _bWindowConsole;
    private readonly BLocalizationCatalog _bWindowCatalog;
    private readonly CAtelier _bWindowAtelier;
    private readonly CEnvoy _bWindowEnvoy;
    private readonly TaskCompletionSource _bWindowExit = new();
    private (int BConsoleWidth, int BConsoleHeight) _bWindowSize;
    private BCanvas? _bWindowCanvas;
    private bool _bWindowChanged;

    internal BWindow(BConsole console, BLocalizationCatalog catalog, CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(console);
        ArgumentNullException.ThrowIfNull(catalog);
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _bWindowConsole = console;
        _bWindowCatalog = catalog;
        _bWindowAtelier = atelier;
        _bWindowEnvoy = envoy;
    }

    internal Task BWindowExit => _bWindowExit.Task;

    internal void BWindowOpen()
    {
        (int width, int height) = _bWindowConsole.BConsoleSizeRead();
        BWindowSizeObserve(width, height);
        _bWindowAtelier.CAtelierOpen(_bWindowEnvoy, BObserver.BObserverCreate<Action>(static work => work()));
    }

    internal void BWindowSizeObserve(int width, int height)
    {
        (width, height) = (Math.Max(width, 0), Math.Max(height, 0));
        if (_bWindowCanvas is not null && (width, height) == _bWindowSize)
        {
            return;
        }

        _bWindowSize = (width, height);
        _bWindowCanvas = new BCanvas(_bWindowConsole, new BCanvasWidth(false), width, height);
        _bWindowChanged = true;
    }

    internal void BWindowBatchObserve()
    {
        if (!_bWindowChanged || _bWindowCanvas is not BCanvas canvas)
        {
            return;
        }

        _bWindowChanged = false;
        (int width, int height) = _bWindowSize;
        bool small = width < BWindowWidthMinimum || height < BWindowHeightMinimum;
        canvas.BCanvasFrameDraw([.. small ? BWindowNoticeBuild(width, height) : BWindowFrameBuild(width, height)]);
        canvas.BCanvasSync();
    }

    internal void BWindowKeyObserve(ConsoleKeyInfo key)
    {
        bool quit = key.Key == ConsoleKey.F10
            || (key.Key == ConsoleKey.C && key.Modifiers == ConsoleModifiers.Control)
            || key.KeyChar == '\u0003';
        if (quit && _bWindowAtelier.CAtelierQuitConfirm(_bWindowEnvoy))
        {
            _bWindowExit.TrySetResult();
        }
    }

    private IEnumerable<BCanvasText> BWindowFrameBuild(int width, int height)
    {
        int left = width / 2;
        foreach (BCanvasText side in BWindowSideBuild(0, left, height))
        {
            yield return side;
        }

        foreach (BCanvasText side in BWindowSideBuild(left, width - left, height))
        {
            yield return side;
        }

        yield return new BCanvasText(
            height - 2, 1, width - 1, _bWindowCatalog.BLocalizationTextRead("Window.Opening"), "text");
        int quit = 9 * BWindowLegendWidth;
        yield return new BCanvasText(height - 1, quit, 2, "10", "legend-number");
        yield return new BCanvasText(
            height - 1,
            quit + 2,
            BWindowLegendWidth - 2,
            _bWindowCatalog.BLocalizationTextRead("Legend.Quit"),
            "legend-label");
    }

    private static IEnumerable<BCanvasText> BWindowSideBuild(int column, int width, int height)
    {
        int inner = Math.Max(width - 2, 0);
        yield return new BCanvasText(1, column, width, $"╔{new string('═', inner)}╗", "side");
        for (int row = 2; row < height - 3; row++)
        {
            string body = row == height - 5
                ? $"╟{new string('─', inner)}╢"
                : $"║{new string(' ', inner)}║";
            yield return new BCanvasText(row, column, width, body, "side");
        }

        yield return new BCanvasText(height - 3, column, width, $"╚{new string('═', inner)}╝", "side");
    }

    private IEnumerable<BCanvasText> BWindowNoticeBuild(int width, int height)
    {
        string notice = string.Format(
            CultureInfo.CurrentCulture,
            _bWindowCatalog.BLocalizationTextRead("Window.Small"),
            BWindowWidthMinimum,
            BWindowHeightMinimum,
            width,
            height);
        IReadOnlyList<string> lines = new BCanvasWidth(false).BCanvasWidthDivide(notice, width);
        for (int row = 0; row < lines.Count; row++)
        {
            yield return new BCanvasText(row, 0, width, lines[row], "text");
        }
    }
}
