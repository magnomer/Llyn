using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Llyn.Core;
using Microsoft.Web.WebView2.Core;

namespace Llyn.Core.Windows;

internal sealed class LPressController : LPressBoard
{
    private readonly IntPtr _lPressControllerFrame;

    private CoreWebView2Controller? _lPressControllerCore;

    private readonly string _lPressControllerPage;

    private readonly SemaphoreSlim _lPressControllerTurn = new(1, 1);

    internal LPressController(IntPtr frame, CoreWebView2Controller core, string page)
    {
        _lPressControllerFrame = frame;
        _lPressControllerCore = core;
        _lPressControllerPage = page;
    }

    public async Task<LPressProof> LPressBoardDraw(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        await _lPressControllerTurn.WaitAsync();

        try
        {
            CoreWebView2Controller core = _lPressControllerCore
                ?? throw new ObjectDisposedException(nameof(LPressController));
            CoreWebView2PrintSettings printing =
                LPressPrintingCreate(core.CoreWebView2.Environment, setting);
            Stream stream = await core.CoreWebView2.PrintToPdfStreamAsync(printing);
            return await LPressDocument.LPressDocumentOpen(stream);
        }
        finally
        {
            _lPressControllerTurn.Release();
        }
    }

    public async Task<LPressStatus> LPressBoardPrint(LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(setting);

        await _lPressControllerTurn.WaitAsync();

        try
        {
            CoreWebView2Controller core = _lPressControllerCore
                ?? throw new ObjectDisposedException(nameof(LPressController));
            CoreWebView2PrintSettings printing =
                LPressPrintingCreate(core.CoreWebView2.Environment, setting);
            LPressPrintingApply(printing, setting);

            CoreWebView2PrintStatus status = await core.CoreWebView2.PrintAsync(printing);
            return status switch
            {
                CoreWebView2PrintStatus.Succeeded => LPressStatus.LPressStatusDone,
                CoreWebView2PrintStatus.PrinterUnavailable => LPressStatus.LPressStatusOffline,
                CoreWebView2PrintStatus.OtherError => LPressStatus.LPressStatusError,
                _ => throw new InvalidOperationException("The browser answered an unknown print status."),
            };
        }
        catch (ArgumentException)
        {
            return LPressStatus.LPressStatusError;
        }
        finally
        {
            _lPressControllerTurn.Release();
        }
    }

    public async Task LPressBoardExport(LPressSetting setting, string path)
    {
        ArgumentNullException.ThrowIfNull(setting);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await _lPressControllerTurn.WaitAsync();

        try
        {
            CoreWebView2Controller core = _lPressControllerCore
                ?? throw new ObjectDisposedException(nameof(LPressController));
            CoreWebView2PrintSettings printing =
                LPressPrintingCreate(core.CoreWebView2.Environment, setting);
            printing.PageRanges = setting.LPressSettingRange.LPressRangeText;

            if (!await core.CoreWebView2.PrintToPdfAsync(path, printing))
            {
                throw new IOException("The document could not be printed to the chosen path.");
            }
        }
        finally
        {
            _lPressControllerTurn.Release();
        }
    }

    public async Task LPressBoardShow()
    {
        await _lPressControllerTurn.WaitAsync();

        try
        {
            CoreWebView2Controller core = _lPressControllerCore
                ?? throw new ObjectDisposedException(nameof(LPressController));
            await LPressDialogShow(core.CoreWebView2);
        }
        finally
        {
            _lPressControllerTurn.Release();
        }
    }

    internal static async Task LPressDialogShow(CoreWebView2 core)
    {
        await core.CallDevToolsProtocolMethodAsync(
            "Runtime.evaluate",
            "{\"expression\":\"window.llynPrinted = new Promise(done => window.addEventListener("
            + "'afterprint', () => done(true), { once: true })); true\"}");

        core.ShowPrintUI(CoreWebView2PrintDialogKind.System);

        await core.CallDevToolsProtocolMethodAsync(
            "Runtime.evaluate", "{\"expression\":\"window.llynPrinted\",\"awaitPromise\":true}");
    }

    private static CoreWebView2PrintSettings LPressPrintingCreate(
        CoreWebView2Environment environment, LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(environment);
        ArgumentNullException.ThrowIfNull(setting);

        LPressPaper paper = setting.LPressSettingPaper;
        LPressMargin margin = setting.LPressSettingMargin;
        CoreWebView2PrintSettings printing = environment.CreatePrintSettings();
        printing.PageWidth = Math.Min(paper.LPressPaperWidth, paper.LPressPaperHeight);
        printing.PageHeight = Math.Max(paper.LPressPaperWidth, paper.LPressPaperHeight);
        printing.Orientation = setting.LPressSettingOrientation == LPressOrientation.LPressOrientationLandscape
            ? CoreWebView2PrintOrientation.Landscape
            : CoreWebView2PrintOrientation.Portrait;
        printing.MarginTop = margin.LPressMarginTop;
        printing.MarginBottom = margin.LPressMarginBottom;
        printing.MarginLeft = margin.LPressMarginLeft;
        printing.MarginRight = margin.LPressMarginRight;
        printing.ScaleFactor = setting.LPressSettingScale / 100.0;
        printing.ShouldPrintBackgrounds = setting.LPressSettingBackground;
        return printing;
    }

    private static void LPressPrintingApply(CoreWebView2PrintSettings printing, LPressSetting setting)
    {
        ArgumentNullException.ThrowIfNull(printing);
        ArgumentNullException.ThrowIfNull(setting);

        printing.PrinterName = setting.LPressSettingPrinter
            ?? throw new InvalidOperationException("Save as PDF has no printer to print to.");
        printing.Copies = setting.LPressSettingCopies;
        printing.Collation = setting.LPressSettingCollation
            ? CoreWebView2PrintCollation.Collated
            : CoreWebView2PrintCollation.Uncollated;
        printing.ColorMode = setting.LPressSettingColor switch
        {
            LPressColor.LPressColorDefault => CoreWebView2PrintColorMode.Default,
            LPressColor.LPressColorFull => CoreWebView2PrintColorMode.Color,
            LPressColor.LPressColorGray => CoreWebView2PrintColorMode.Grayscale,
            _ => throw new InvalidOperationException("The setting holds an unknown colour."),
        };
        printing.Duplex = setting.LPressSettingSide switch
        {
            LPressSide.LPressSideDefault => CoreWebView2PrintDuplex.Default,
            LPressSide.LPressSideSingle => CoreWebView2PrintDuplex.OneSided,
            LPressSide.LPressSideLong => CoreWebView2PrintDuplex.TwoSidedLongEdge,
            LPressSide.LPressSideShort => CoreWebView2PrintDuplex.TwoSidedShortEdge,
            _ => throw new InvalidOperationException("The setting holds an unknown side."),
        };
        printing.PageRanges = setting.LPressSettingRange.LPressRangeText;
        printing.MediaSize = CoreWebView2PrintMediaSize.Custom;
    }

    public async ValueTask DisposeAsync()
    {
        await _lPressControllerTurn.WaitAsync();

        try
        {
            CoreWebView2Controller? core = _lPressControllerCore;
            _lPressControllerCore = null;

            if (core is null)
            {
                return;
            }

            core.Close();
            LPressBrowser.LPressWindowDispose(_lPressControllerFrame);
            LPressBrowser.LPressPageDelete(_lPressControllerPage);
        }
        finally
        {
            _lPressControllerTurn.Release();
        }
    }
}
