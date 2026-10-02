using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Llyn.UIDeportment;

internal sealed class QScreenBrowser
{
    private const string QScreenHost = "https://llyn.video/screen";

    private const int QScreenClosed = unchecked((int)0x8007139F);

    private readonly QScreen _qScreenBrowserDriver;

    private readonly FrameworkElement _qScreenBrowserSurface;

    private bool _qScreenReady;

    private WebView2CompositionControl? _qScreenBrowser;

    private string _qScreenPaper = string.Empty;

    internal QScreenBrowser(QScreen driver, FrameworkElement surface)
    {
        _qScreenBrowserDriver = driver;
        _qScreenBrowserSurface = surface;
    }

    private Grid QScreenPage => QContract.QContractFind<Grid>(_qScreenBrowserSurface, "PScreenPage");

    internal void QScreenPageRefine(Uri address)
    {
        _qScreenPaper = QScreenPaper.QScreenPaperBuild(_qScreenBrowserDriver, address);
        QScreenPage.Visibility = Visibility.Visible;
        _ = QScreenLoadRefine();
    }

    internal void QScreenPageSend(string order)
    {
        if (!_qScreenReady || _qScreenBrowser?.CoreWebView2 is not CoreWebView2 core)
        {
            return;
        }

        try
        {
            core.PostWebMessageAsString(order);
        }
        catch (InvalidOperationException exception)
            when (exception.InnerException is COMException { HResult: QScreenClosed })
        {
        }
    }

    internal void QScreenBlankRefine()
    {
        _qScreenPaper = string.Empty;

        QScreenPage.Visibility = Visibility.Collapsed;
        if (_qScreenReady && _qScreenBrowser?.CoreWebView2 is CoreWebView2 core)
        {
            core.Navigate("about:blank");
        }
    }

    internal void QScreenBrowserDispose()
    {
        if (_qScreenBrowser is not WebView2CompositionControl browser)
        {
            return;
        }

        if (_qScreenReady)
        {
            _qScreenReady = false;
            browser.CoreWebView2.WebResourceRequested -= QScreenPaperRefine;
        }

        _qScreenBrowser = null;
        QScreenPage.Children.Remove(browser);
        browser.Dispose();
    }

    private async Task QScreenLoadRefine()
    {
        string paper = _qScreenPaper;
        WebView2CompositionControl browser = _qScreenBrowser ??= QScreenBrowserCreate();

        if (Window.GetWindow(_qScreenBrowserSurface)?.Tag is not QWindow host)
        {
            _qScreenBrowserDriver.QScreenNoticeRefine();
            return;
        }

        try
        {
            await browser.EnsureCoreWebView2Async(await host.QWindowScreen.QScreenSettingRead().ConfigureAwait(true))
                .ConfigureAwait(true);
        }
        catch (Exception)
        {
            host.QWindowScreen.QScreenSettingReset();
            _qScreenBrowserDriver.QScreenNoticeRefine();
            return;
        }

        if (!ReferenceEquals(paper, _qScreenPaper))
        {
            return;
        }

        if (!_qScreenReady)
        {
            _qScreenReady = true;
            browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            browser.CoreWebView2.Settings.AreDevToolsEnabled = false;
            browser.CoreWebView2.Settings.IsStatusBarEnabled = false;
            browser.CoreWebView2.AddWebResourceRequestedFilter(
                QScreenHost, CoreWebView2WebResourceContext.Document);
            browser.CoreWebView2.WebResourceRequested += QScreenPaperRefine;
        }

        browser.CoreWebView2.Navigate(QScreenHost);
    }

    private WebView2CompositionControl QScreenBrowserCreate()
    {
        WebView2CompositionControl browser = new()
        {
            DefaultBackgroundColor = System.Drawing.Color.Black,
        };

        QScreenPage.Children.Add(browser);
        return browser;
    }

    private void QScreenPaperRefine(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        if (_qScreenBrowser?.CoreWebView2 is not CoreWebView2 core)
        {
            return;
        }

        e.Response = core.Environment.CreateWebResourceResponse(
            new MemoryStream(Encoding.UTF8.GetBytes(_qScreenPaper)),
            200,
            "OK",
            "Content-Type: text/html; charset=utf-8");
    }
}
