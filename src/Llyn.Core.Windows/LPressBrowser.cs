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

    private static readonly string LPressBrowserFolder = Path.Combine(Path.GetTempPath(), "Llyn", "Press");

    private static Task<CoreWebView2Environment>? _lPressSetting;

    public async Task LPressSave(string html, string path)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html);
        ArgumentException.ThrowIfNullOrWhiteSpace(path);

        await using LPressBoard board = await LPressBoardOpen(html);
        await board.LPressBoardExport(LPressSetting.LPressSettingCreate(null), path);
    }

    public async Task<LPressBoard> LPressBoardOpen(string html)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(html);

        string page = LPressPageCreate(html);
        IntPtr frame = IntPtr.Zero;

        try
        {
            CoreWebView2Environment setting = await LPressSettingRead();

            frame = LPressWindowCreate();
            CoreWebView2Controller board = await LPressBoardLoad(setting, frame, page);
            return new LPressController(frame, board, page);
        }
        catch (Exception)
        {
            if (frame != IntPtr.Zero)
            {
                LPressWindowDispose(frame);
            }

            LPressPageDelete(page);
            throw;
        }
    }

    private static async Task<CoreWebView2Controller> LPressBoardLoad(
        CoreWebView2Environment setting, IntPtr frame, string page)
    {
        CoreWebView2Controller board = await setting.CreateCoreWebView2ControllerAsync(frame);

        try
        {
            board.Bounds = new Rectangle(0, 0, LPressBrowserWidth, LPressBrowserHeight);
            await LPressPageLoad(board.CoreWebView2, page);
            return board;
        }
        catch (Exception)
        {
            board.Close();
            throw;
        }
    }

    private static string LPressPageCreate(string html)
    {
        string folder = Path.Combine(LPressBrowserFolder, "Page");
        Directory.CreateDirectory(folder);

        string page = Path.Combine(folder, Guid.NewGuid().ToString("N") + ".html");
        File.WriteAllText(page, html, new UTF8Encoding(false));
        return page;
    }

    internal static async Task<CoreWebView2Environment> LPressSettingRead()
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

    internal static void LPressPageDelete(string page)
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

    internal static async Task LPressPageLoad(CoreWebView2 core, string page)
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

            await core.CallDevToolsProtocolMethodAsync(
                "Runtime.evaluate",
                "{\"expression\":\"Promise.all(Array.from(document.images, image => { image.loading = 'eager'; "
                + "return image.decode().catch(() => null); })).then(() => document.fonts.ready).then(() => true)\","
                + "\"awaitPromise\":true}");
        }
        finally
        {
            core.NavigationCompleted -= LPressReadyHandle;
        }
    }

    internal static IntPtr LPressWindowCreate()
    {
        IntPtr owner = GetForegroundWindow();
        int[] place = new int[4];

        if (owner == IntPtr.Zero || !GetWindowRect(owner, place))
        {
            owner = IntPtr.Zero;
            place = new int[4];
        }

        IntPtr frame = CreateWindowExW(
            0,
            "STATIC",
            null,
            LPressBrowserStyle,
            place[0],
            place[1],
            LPressBrowserWidth,
            LPressBrowserHeight,
            owner,
            IntPtr.Zero,
            IntPtr.Zero,
            IntPtr.Zero);

        if (frame == IntPtr.Zero)
        {
            throw new InvalidOperationException("No surface could be opened for printing.");
        }

        return frame;
    }

    internal static void LPressWindowDispose(IntPtr frame)
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

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool GetWindowRect(IntPtr window, [Out] int[] rect);
}
