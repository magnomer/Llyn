using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Llyn.Core;
using Microsoft.Web.WebView2.Core;

namespace Llyn.Core.Windows;

public sealed class LPressBrowser : LPress
{
    private const int LPressBrowserStyle = unchecked((int)0x80000000);

    private const int LPressBrowserWidth = 1240;

    private const int LPressBrowserHeight = 1754;

    private const double LPressBrowserMargin = 12.0 / 25.4;

    private static readonly string LPressBrowserFolder = Path.Combine(Path.GetTempPath(), "Llyn", "Press");

    private static Task<CoreWebView2Environment>? _lPressSetting;

    public async Task LPressSave(string html, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await LPressBoardRun(html, async (setting, core) =>
        {
            CoreWebView2PrintSettings printing = LPressSettingCreate(setting, LPressPaper.LPressPaperLocal);

            if (!await core.PrintToPdfAsync(path, printing))
            {
                throw new IOException("The document could not be printed to the chosen path.");
            }
        });
    }

    public async Task LPressPrint(string html, LPressTicket ticket)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html);
        ArgumentNullException.ThrowIfNull(ticket);

        await LPressBoardRun(html, async (setting, core) =>
        {
            CoreWebView2PrintSettings printing = LPressSettingCreate(setting, ticket.LPressTicketPaper);
            printing.PrinterName = ticket.LPressTicketPrinter;
            printing.Orientation = ticket.LPressTicketLandscape
                ? CoreWebView2PrintOrientation.Landscape
                : CoreWebView2PrintOrientation.Portrait;
            printing.Copies = Math.Max(1, ticket.LPressTicketCopies);
            printing.Collation = ticket.LPressTicketCollated
                ? CoreWebView2PrintCollation.Collated
                : CoreWebView2PrintCollation.Uncollated;
            printing.Duplex = ticket.LPressTicketSide switch
            {
                LPressSide.LPressSideSingle => CoreWebView2PrintDuplex.OneSided,
                LPressSide.LPressSideLong => CoreWebView2PrintDuplex.TwoSidedLongEdge,
                LPressSide.LPressSideShort => CoreWebView2PrintDuplex.TwoSidedShortEdge,
                _ => CoreWebView2PrintDuplex.Default,
            };
            printing.ColorMode = ticket.LPressTicketInk switch
            {
                LPressInk.LPressInkColor => CoreWebView2PrintColorMode.Color,
                LPressInk.LPressInkGray => CoreWebView2PrintColorMode.Grayscale,
                _ => CoreWebView2PrintColorMode.Default,
            };

            CoreWebView2PrintStatus status = await core.PrintAsync(printing);
            if (status == CoreWebView2PrintStatus.PrinterUnavailable)
            {
                throw new IOException("The chosen printer could not be reached.");
            }

            if (status != CoreWebView2PrintStatus.Succeeded)
            {
                throw new IOException("The document could not be printed.");
            }
        });
    }

    private static CoreWebView2PrintSettings LPressSettingCreate(CoreWebView2Environment setting, LPressPaper paper)
    {
        CoreWebView2PrintSettings printing = setting.CreatePrintSettings();
        printing.ShouldPrintBackgrounds = true;
        printing.PageWidth = paper.LPressPaperWidth;
        printing.PageHeight = paper.LPressPaperHeight;
        printing.MarginTop = LPressBrowserMargin;
        printing.MarginBottom = LPressBrowserMargin;
        printing.MarginLeft = LPressBrowserMargin;
        printing.MarginRight = LPressBrowserMargin;
        return printing;
    }

    private static async Task LPressBoardRun(string html, Func<CoreWebView2Environment, CoreWebView2, Task> act)
    {
        string folder = Path.Combine(LPressBrowserFolder, "Page");
        Directory.CreateDirectory(folder);

        string page = Path.Combine(folder, Guid.NewGuid().ToString("N") + ".html");
        File.WriteAllText(page, html, new UTF8Encoding(false));

        IntPtr frame = IntPtr.Zero;
        CoreWebView2Controller? board = null;

        try
        {
            CoreWebView2Environment setting = await LPressSettingRead();

            frame = LPressWindowCreate();
            board = await setting.CreateCoreWebView2ControllerAsync(frame);
            board.Bounds = new Rectangle(0, 0, LPressBrowserWidth, LPressBrowserHeight);

            await LPressPageLoad(board.CoreWebView2, page);
            await act(setting, board.CoreWebView2);
        }
        finally
        {
            board?.Close();

            if (frame != IntPtr.Zero)
            {
                LPressWindowDispose(frame);
            }

            LPressPageDelete(page);
        }
    }

    private static async Task<CoreWebView2Environment> LPressSettingRead()
    {
        Task<CoreWebView2Environment> pending = _lPressSetting ??=
            CoreWebView2Environment.CreateAsync(null, Path.Combine(LPressBrowserFolder, "Profile"));

        try
        {
            return await pending;
        }
        catch (WebView2RuntimeNotFoundException exception)
        {
            _lPressSetting = null;
            throw new InvalidOperationException(
                "The Microsoft Edge WebView2 Runtime that prints the page is not installed.", exception);
        }
        catch (Exception)
        {
            _lPressSetting = null;
            throw;
        }
    }

    private static void LPressPageDelete(string page)
    {
        try
        {
            File.Delete(page);
        }
        catch (IOException)
        {
            return;
        }
        catch (UnauthorizedAccessException)
        {
            return;
        }
    }

    private static async Task LPressPageLoad(CoreWebView2 core, string page)
    {
        TaskCompletionSource<bool> ready =
            new TaskCompletionSource<bool>(TaskCreationOptions.RunContinuationsAsynchronously);

        void LPressReadyHandle(object? sender, CoreWebView2NavigationCompletedEventArgs e)
        {
            ready.TrySetResult(e.IsSuccess);
        }

        core.NavigationCompleted += LPressReadyHandle;

        try
        {
            core.Navigate(new Uri(page).AbsoluteUri);

            if (!await ready.Task)
            {
                throw new IOException("The rendered page could not be opened for printing.");
            }
        }
        finally
        {
            core.NavigationCompleted -= LPressReadyHandle;
        }
    }

    private static IntPtr LPressWindowCreate()
    {
        IntPtr frame = CreateWindowExW(
            0,
            "STATIC",
            null,
            LPressBrowserStyle,
            -32000,
            -32000,
            LPressBrowserWidth,
            LPressBrowserHeight,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero);

        if (frame == IntPtr.Zero)
        {
            throw new InvalidOperationException("No surface could be opened for printing.");
        }

        return frame;
    }

    private static void LPressWindowDispose(IntPtr frame)
    {
        DestroyWindow(frame);
    }

    [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern IntPtr CreateWindowExW(
        int exStyle,
        string className,
        string? windowName,
        int style,
        int x,
        int y,
        int width,
        int height,
        IntPtr parent,
        IntPtr menu,
        IntPtr instance,
        IntPtr param);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool DestroyWindow(IntPtr window);
}
