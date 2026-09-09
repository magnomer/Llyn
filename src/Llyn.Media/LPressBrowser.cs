using System;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Llyn.Core;
using Microsoft.Web.WebView2.Core;

namespace Llyn.Media;

public sealed class LPressBrowser : LPress
{
    private const int LPressBrowserStyle = unchecked((int)0x80000000);

    private const int LPressBrowserWidth = 1240;

    private const int LPressBrowserHeight = 1754;

    public async Task LPressSave(string html, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        string folder = Path.Combine(Path.GetTempPath(), "Llyn", "Press");
        Directory.CreateDirectory(folder);

        string page = Path.Combine(folder, Guid.NewGuid().ToString("N") + ".html");
        File.WriteAllText(page, html, new UTF8Encoding(false));

        IntPtr frame = IntPtr.Zero;
        CoreWebView2Controller? board = null;

        try
        {
            frame = LPressWindowCreate();

            CoreWebView2Environment setting =
                await CoreWebView2Environment.CreateAsync(null, folder);
            board = await setting.CreateCoreWebView2ControllerAsync(frame);
            board.Bounds = new Rectangle(0, 0, LPressBrowserWidth, LPressBrowserHeight);

            await LPressPageLoad(board.CoreWebView2, page);

            CoreWebView2PrintSettings printing = setting.CreatePrintSettings();
            printing.ShouldPrintBackgrounds = true;
            printing.MarginTop = 0;
            printing.MarginBottom = 0;
            printing.MarginLeft = 0;
            printing.MarginRight = 0;

            if (!await board.CoreWebView2.PrintToPdfAsync(path, printing))
            {
                throw new IOException("The document could not be printed to the chosen path.");
            }
        }
        finally
        {
            board?.Close();

            if (frame != IntPtr.Zero)
            {
                LPressWindowDispose(frame);
            }

            try
            {
                File.Delete(page);
            }
            catch (IOException)
            {
                board = null;
            }
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
