using System;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;

namespace Llyn.UIShell;

public partial class PScreen : UserControl
{
    private const string PScreenHost = "https://llyn.video/screen";

    private const int PScreenDelay = 500;

    private static Task<CoreWebView2Environment>? _pScreenSetting;

    private readonly DispatcherTimer _pScreenClock;

    private readonly DispatcherTimer _pScreenPending;

    private bool _pScreenWeb;

    private bool _pScreenReady;

    private WebView2? _pScreenBrowser;

    private string _pScreenPaper = string.Empty;

    public PScreen()
    {
        InitializeComponent();

        _pScreenClock = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(250),
        };
        _pScreenClock.Tick += PScreenSpanHandle;

        _pScreenPending = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(PScreenDelay),
        };
        _pScreenPending.Tick += PScreenPendingHandle;

        Loaded += PScreenOpenHandle;
        Unloaded += PScreenDropHandle;
    }

    public static readonly DependencyProperty PScreenLocationProperty = DependencyProperty.Register(
        nameof(PScreenLocation),
        typeof(string),
        typeof(PScreen),
        new PropertyMetadata(string.Empty, PScreenSourceHandle));

    public static readonly DependencyProperty PScreenFromProperty = DependencyProperty.Register(
        nameof(PScreenFrom),
        typeof(TimeSpan),
        typeof(PScreen),
        new PropertyMetadata(TimeSpan.Zero, PScreenSourceHandle));

    public static readonly DependencyProperty PScreenUntilProperty = DependencyProperty.Register(
        nameof(PScreenUntil),
        typeof(TimeSpan?),
        typeof(PScreen),
        new PropertyMetadata(null, PScreenSourceHandle));

    public static readonly DependencyProperty PScreenVolumeProperty = DependencyProperty.Register(
        nameof(PScreenVolume),
        typeof(double),
        typeof(PScreen),
        new PropertyMetadata(1d, PScreenVolumeHandle));

    public static readonly DependencyProperty PScreenPlayingProperty = DependencyProperty.Register(
        nameof(PScreenPlaying),
        typeof(bool),
        typeof(PScreen),
        new FrameworkPropertyMetadata(
            false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PScreenPlayingHandle));

    public string PScreenLocation
    {
        get => (string)GetValue(PScreenLocationProperty);
        set => SetValue(PScreenLocationProperty, value);
    }

    public TimeSpan PScreenFrom
    {
        get => (TimeSpan)GetValue(PScreenFromProperty);
        set => SetValue(PScreenFromProperty, value);
    }

    public TimeSpan? PScreenUntil
    {
        get => (TimeSpan?)GetValue(PScreenUntilProperty);
        set => SetValue(PScreenUntilProperty, value);
    }

    public bool PScreenPlaying
    {
        get => (bool)GetValue(PScreenPlayingProperty);
        set => SetValue(PScreenPlayingProperty, value);
    }

    public double PScreenVolume
    {
        get => (double)GetValue(PScreenVolumeProperty);
        set => SetValue(PScreenVolumeProperty, value);
    }

    private static void PScreenSourceHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is PScreen screen)
        {
            screen._pScreenPending.Stop();
            screen._pScreenPending.Start();
        }
    }

    private static void PScreenPlayingHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is PScreen screen)
        {
            screen.PScreenSwitch.IsChecked = screen.PScreenPlaying;
            screen.PScreenSync();
        }
    }

    private static void PScreenVolumeHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is PScreen screen)
        {
            screen.PScreenVolumeApply();
        }
    }

    private void PScreenVolumeApply()
    {
        PScreenMedia.Volume = PScreenVolume;

        if (_pScreenReady && _pScreenBrowser?.CoreWebView2 is CoreWebView2 core)
        {
            try
            {
                core.PostWebMessageAsString(
                    "volume:" + PScreenVolume.ToString("0.###", CultureInfo.InvariantCulture));
            }
            catch (Exception)
            {
            }
        }
    }

    private void PScreenSizeHandle(object sender, SizeChangedEventArgs e)
    {
        if (!e.WidthChanged)
        {
            return;
        }

        double height = e.NewSize.Width * 9 / 16;
        if (double.IsNaN(PScreenStage.Height) || Math.Abs(PScreenStage.Height - height) > 0.5)
        {
            PScreenStage.Height = height;
        }
    }

    private void PScreenOpenHandle(object sender, RoutedEventArgs e)
    {
        if (PScreenLocation.Length != 0)
        {
            _pScreenPending.Stop();
            _pScreenPending.Start();
        }
    }

    private void PScreenPendingHandle(object? sender, EventArgs e)
    {
        _pScreenPending.Stop();
        PScreenShow();
    }

    private void PScreenSwitchHandle(object sender, RoutedEventArgs e)
    {
        PScreenPlaying = PScreenSwitch.IsChecked == true;
    }

    private void PScreenShow()
    {
        PScreenStop();

        Uri? address = PImage.PImageAddressRead(PScreenLocation);
        if (address is null)
        {
            return;
        }

        if (address.IsFile)
        {
            _pScreenWeb = false;
            PScreenMedia.Visibility = Visibility.Visible;
            PScreenMedia.Source = address;
            PScreenMedia.Play();
            _pScreenClock.Start();
            return;
        }

        _pScreenWeb = true;
        _pScreenPaper = PScreenPaperBuild(address);
        PScreenPage.Visibility = Visibility.Visible;
        _ = PScreenPageShow();
    }

    private async Task PScreenPageShow()
    {
        string paper = _pScreenPaper;
        WebView2 browser = _pScreenBrowser ??= PScreenBrowserCreate();

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

    private WebView2 PScreenBrowserCreate()
    {
        WebView2 browser = new()
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

    private void PScreenNoticeShow()
    {
        PScreenPage.Visibility = Visibility.Collapsed;
        PScreenNotice.Text = PLocalizationCatalog.PLocalizationCatalogCurrent["Card.VideoFailed"];
        PScreenNotice.Visibility = Visibility.Visible;
    }

    private void PScreenSync()
    {
        if (_pScreenWeb)
        {
            if (_pScreenReady && _pScreenBrowser?.CoreWebView2 is CoreWebView2 core)
            {
                try
                {
                    core.PostWebMessageAsString(PScreenPlaying ? "play" : "pause");
                }
                catch (Exception)
                {
                }
            }

            return;
        }

        if (PScreenMedia.Source is null)
        {
            return;
        }

        if (PScreenPlaying)
        {
            PScreenMedia.Play();
            return;
        }

        PScreenMedia.Pause();
    }

    private void PScreenStop()
    {
        _pScreenClock.Stop();
        _pScreenPaper = string.Empty;

        PScreenNotice.Visibility = Visibility.Collapsed;
        PScreenMedia.Visibility = Visibility.Collapsed;
        PScreenMedia.Stop();
        PScreenMedia.Source = null;

        PScreenPage.Visibility = Visibility.Collapsed;
        if (_pScreenReady && _pScreenBrowser?.CoreWebView2 is CoreWebView2 core)
        {
            core.Navigate("about:blank");
        }
    }

    private void PScreenReadyHandle(object sender, RoutedEventArgs e)
    {
        PScreenMedia.Volume = PScreenVolume;
        PScreenMedia.Position = PScreenFrom;
        if (!PScreenPlaying)
        {
            PScreenMedia.Pause();
        }
    }

    private void PScreenFinishHandle(object sender, RoutedEventArgs e)
    {
        PScreenMedia.Position = PScreenFrom;
        PScreenMedia.Play();
    }

    private void PScreenFailureHandle(object sender, ExceptionRoutedEventArgs e)
    {
        PScreenMedia.Visibility = Visibility.Collapsed;
        PScreenNoticeShow();
    }

    private void PScreenSpanHandle(object? sender, EventArgs e)
    {
        if (PScreenMedia.Source is null
            || PScreenUntil is not TimeSpan until
            || until <= PScreenFrom
            || PScreenMedia.Position < until)
        {
            return;
        }

        PScreenMedia.Position = PScreenFrom;
    }

    private void PScreenDropHandle(object sender, RoutedEventArgs e)
    {
        _pScreenPending.Stop();
        PScreenStop();

        if (_pScreenBrowser is not WebView2 browser)
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

    private string PScreenPaperBuild(Uri address)
    {
        string from = ((long)PScreenFrom.TotalSeconds).ToString(CultureInfo.InvariantCulture);
        string until = (PScreenUntil is TimeSpan span && span > PScreenFrom
            ? (long)span.TotalSeconds
            : 0).ToString(CultureInfo.InvariantCulture);
        string playing = PScreenPlaying ? "1" : "0";
        string level = PScreenVolume.ToString("0.###", CultureInfo.InvariantCulture);
        string? film = PScreenFilmRead(address);

        if (film is null)
        {
            return PScreenPaperFormat(
                "<video id=\"reel\" src=\"" + PScreenTextFormat(address.AbsoluteUri) + "\" playsinline></video>\n"
                + "<script>\n"
                + "var START = " + from + ";\n"
                + "var END = " + until + ";\n"
                + "var reel = document.getElementById('reel');\n"
                + "reel.volume = " + level + ";\n"
                + "reel.addEventListener('loadedmetadata', function () { reel.currentTime = START; });\n"
                + "reel.addEventListener('timeupdate', function () {\n"
                + "    if (END > START && reel.currentTime >= END) { reel.currentTime = START; }\n"
                + "});\n"
                + "reel.addEventListener('ended', function () { reel.currentTime = START; reel.play(); });\n"
                + "window.chrome.webview.addEventListener('message', function (notice) {\n"
                + "    var order = String(notice.data);\n"
                + "    if (order.indexOf('volume:') === 0) { reel.volume = parseFloat(order.slice(7)); return; }\n"
                + "    if (order === 'play') { reel.play(); } else { reel.pause(); }\n"
                + "});\n"
                + "if (" + playing + ") { reel.play(); }\n"
                + "</script>");
        }

        return PScreenPaperFormat(
            "<div id=\"reel\"></div>\n"
            + "<script src=\"https://www.youtube.com/iframe_api\"></script>\n"
            + "<script>\n"
            + "var START = " + from + ";\n"
            + "var END = " + until + ";\n"
            + "var AUTO = " + playing + ";\n"
            + "var LEVEL = " + level + ";\n"
            + "var player = null;\n"
            + "function onYouTubeIframeAPIReady() {\n"
            + "    var settings = {\n"
            + "        start: START,\n"
            + "        autoplay: AUTO,\n"
            + "        rel: 0,\n"
            + "        modestbranding: 1,\n"
            + "        playsinline: 1,\n"
            + "        origin: 'https://llyn.video'\n"
            + "    };\n"
            + "    if (END > START) { settings.end = END; }\n"
            + "    player = new YT.Player('reel', {\n"
            + "        videoId: '" + PScreenTextFormat(film) + "',\n"
            + "        playerVars: settings,\n"
            + "        events: {\n"
            + "            onReady: function () {\n"
            + "                player.setVolume(LEVEL * 100);\n"
            + "                if (LEVEL > 0) { player.unMute(); } else { player.mute(); }\n"
            + "                if (AUTO) { player.playVideo(); }\n"
            + "            },\n"
            + "            onStateChange: function (notice) {\n"
            + "                if (notice.data === YT.PlayerState.ENDED) {\n"
            + "                    player.seekTo(START, true);\n"
            + "                    player.playVideo();\n"
            + "                }\n"
            + "            }\n"
            + "        }\n"
            + "    });\n"
            + "}\n"
            + "window.chrome.webview.addEventListener('message', function (notice) {\n"
            + "    if (!player) { return; }\n"
            + "    var order = String(notice.data);\n"
            + "    if (order.indexOf('volume:') === 0) {\n"
            + "        LEVEL = parseFloat(order.slice(7));\n"
            + "        player.setVolume(LEVEL * 100);\n"
            + "        if (LEVEL > 0) { player.unMute(); } else { player.mute(); }\n"
            + "        return;\n"
            + "    }\n"
            + "    if (order === 'play') {\n"
            + "        var moment = player.getCurrentTime();\n"
            + "        if (moment < START || (END > START && moment >= END)) { player.seekTo(START, true); }\n"
            + "        player.playVideo();\n"
            + "    } else {\n"
            + "        player.pauseVideo();\n"
            + "    }\n"
            + "});\n"
            + "</script>");
    }

    private static string PScreenPaperFormat(string body)
    {
        return "<!doctype html>\n<html>\n<head>\n<meta charset=\"utf-8\">\n<style>\n"
            + "html, body { margin: 0; height: 100%; background: #000; overflow: hidden; }\n"
            + "#reel, iframe, video { width: 100%; height: 100%; border: 0; display: block; }\n"
            + "</style>\n</head>\n<body>\n" + body + "\n</body>\n</html>";
    }

    internal static string? PScreenFilmRead(Uri address)
    {
        ArgumentNullException.ThrowIfNull(address);

        string house = address.Host.ToLowerInvariant();
        if (house.StartsWith("www.", StringComparison.Ordinal))
        {
            house = house[4..];
        }

        if (string.Equals(house, "youtu.be", StringComparison.Ordinal))
        {
            return PScreenFilmCheck(address.AbsolutePath.Trim('/'));
        }

        if (!string.Equals(house, "youtube.com", StringComparison.Ordinal)
            && !string.Equals(house, "youtube-nocookie.com", StringComparison.Ordinal))
        {
            return null;
        }

        string path = address.AbsolutePath;
        string[] openings = ["/embed/", "/shorts/", "/v/", "/live/"];
        foreach (string opening in openings)
        {
            if (path.StartsWith(opening, StringComparison.OrdinalIgnoreCase))
            {
                return PScreenFilmCheck(path[opening.Length..].Trim('/'));
            }
        }

        foreach (string field in address.Query.TrimStart('?').Split('&'))
        {
            if (field.StartsWith("v=", StringComparison.OrdinalIgnoreCase))
            {
                return PScreenFilmCheck(field[2..]);
            }
        }

        return null;
    }

    private static string? PScreenFilmCheck(string film)
    {
        if (film.Length == 0)
        {
            return null;
        }

        foreach (char letter in film)
        {
            if (!char.IsLetterOrDigit(letter) && letter != '-' && letter != '_')
            {
                return null;
            }
        }

        return film;
    }

    private static string PScreenTextFormat(string text)
    {
        return text
            .Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("'", "\\'", StringComparison.Ordinal)
            .Replace("\"", "&quot;", StringComparison.Ordinal)
            .Replace("<", "&lt;", StringComparison.Ordinal);
    }
}
