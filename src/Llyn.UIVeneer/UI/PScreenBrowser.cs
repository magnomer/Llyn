using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Llyn.UIVeneer;

public partial class PScreen
{
    private const string PScreenHost = "https://llyn.video/screen";

    private static Task<CoreWebView2Environment>? _pScreenSetting;

    private bool _pScreenReady;

    private WebView2CompositionControl? _pScreenBrowser;

    private string _pScreenPaper = string.Empty;

    private void PScreenPageShow(Uri address)
    {
        _pScreenPaper = PScreenPaperBuild(address);
        PScreenPage.Visibility = Visibility.Visible;
        _ = PScreenPageLoad();
    }

    private async Task PScreenPageLoad()
    {
        string paper = _pScreenPaper;
        WebView2CompositionControl browser = _pScreenBrowser ??= PScreenBrowserCreate();

        try
        {
            _pScreenSetting ??= CoreWebView2Environment.CreateAsync(
                null,
                null,
                new CoreWebView2EnvironmentOptions
                {
                    AdditionalBrowserArguments = "--autoplay-policy=no-user-gesture-required",
                });

            await browser.EnsureCoreWebView2Async(await _pScreenSetting.ConfigureAwait(true))
                .ConfigureAwait(true);
        }
        catch (Exception)
        {
            _pScreenSetting = null;
            PScreenNoticeShow();
            return;
        }

        if (!ReferenceEquals(paper, _pScreenPaper))
        {
            return;
        }

        if (!_pScreenReady)
        {
            _pScreenReady = true;
            browser.CoreWebView2.Settings.AreDefaultContextMenusEnabled = false;
            browser.CoreWebView2.Settings.AreDevToolsEnabled = false;
            browser.CoreWebView2.Settings.IsStatusBarEnabled = false;
            browser.CoreWebView2.AddWebResourceRequestedFilter(
                PScreenHost, CoreWebView2WebResourceContext.Document);
            browser.CoreWebView2.WebResourceRequested += PScreenPaperHandle;
        }

        browser.CoreWebView2.Navigate(PScreenHost);
    }

    private WebView2CompositionControl PScreenBrowserCreate()
    {
        WebView2CompositionControl browser = new()
        {
            DefaultBackgroundColor = System.Drawing.Color.Black,
        };

        PScreenPage.Children.Add(browser);
        return browser;
    }

    private void PScreenPaperHandle(object? sender, CoreWebView2WebResourceRequestedEventArgs e)
    {
        if (_pScreenBrowser?.CoreWebView2 is not CoreWebView2 core)
        {
            return;
        }

        e.Response = core.Environment.CreateWebResourceResponse(
            new MemoryStream(Encoding.UTF8.GetBytes(_pScreenPaper)),
            200,
            "OK",
            "Content-Type: text/html; charset=utf-8");
    }

    private void PScreenPageSend(string order)
    {
        if (!_pScreenReady || _pScreenBrowser?.CoreWebView2 is not CoreWebView2 core)
        {
            return;
        }

        try
        {
            core.PostWebMessageAsString(order);
        }
        catch (Exception)
        {
        }
    }

    private void PScreenPageStop()
    {
        _pScreenPaper = string.Empty;

        PScreenPage.Visibility = Visibility.Collapsed;
        if (_pScreenReady && _pScreenBrowser?.CoreWebView2 is CoreWebView2 core)
        {
            core.Navigate("about:blank");
        }
    }

    private void PScreenBrowserDispose()
    {
        if (_pScreenBrowser is not WebView2CompositionControl browser)
        {
            return;
        }

        if (_pScreenReady)
        {
            _pScreenReady = false;
            browser.CoreWebView2.WebResourceRequested -= PScreenPaperHandle;
        }

        _pScreenBrowser = null;
        PScreenPage.Children.Remove(browser);
        browser.Dispose();
    }
}
