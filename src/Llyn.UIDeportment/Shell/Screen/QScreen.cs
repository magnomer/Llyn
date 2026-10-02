using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

internal sealed class QScreen
{
    private const int QScreenDelay = 500;

    internal static readonly DependencyProperty QScreenAddressProperty = DependencyProperty.RegisterAttached(
        "QScreenAddress",
        typeof(Uri),
        typeof(QScreen),
        new PropertyMetadata(null, QScreenSourceRefine));

    internal static readonly DependencyProperty QScreenFilmProperty = DependencyProperty.RegisterAttached(
        "QScreenFilm",
        typeof(string),
        typeof(QScreen),
        new PropertyMetadata(null));

    internal static readonly DependencyProperty QScreenFromProperty = DependencyProperty.RegisterAttached(
        "QScreenFrom",
        typeof(TimeSpan),
        typeof(QScreen),
        new PropertyMetadata(TimeSpan.Zero, QScreenSourceRefine));

    internal static readonly DependencyProperty QScreenUntilProperty = DependencyProperty.RegisterAttached(
        "QScreenUntil",
        typeof(TimeSpan?),
        typeof(QScreen),
        new PropertyMetadata(null, QScreenSourceRefine));

    internal static readonly DependencyProperty QScreenVolumeProperty = DependencyProperty.RegisterAttached(
        "QScreenVolume",
        typeof(double),
        typeof(QScreen),
        new PropertyMetadata(1d, QScreenVolumeRefine));

    internal static readonly DependencyProperty QScreenPlayingProperty = DependencyProperty.RegisterAttached(
        "QScreenPlaying",
        typeof(bool),
        typeof(QScreen),
        new FrameworkPropertyMetadata(
            false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, QScreenPlayingRefine));

    private static readonly ConditionalWeakTable<DependencyObject, QScreen> QScreenHold = [];

    private readonly FrameworkElement _qScreenSurface;

    private readonly DispatcherTimer _qScreenPending;

    private readonly QScreenFile _qScreenFile;

    private readonly QScreenBrowser _qScreenBrowser;

    private bool _qScreenWeb;

    private bool _qScreenOpened;

    private QScreen(FrameworkElement surface)
    {
        _qScreenSurface = surface;
        _qScreenFile = new QScreenFile(this, surface);
        _qScreenBrowser = new QScreenBrowser(this, surface);

        QScreenStage.SizeChanged += QScreenSizeRefine;
        QScreenSwitch.Click += QScreenSwitchRefine;

        _qScreenPending = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(QScreenDelay),
        };
        _qScreenPending.Tick += QScreenPendingRefine;

        surface.Loaded += QScreenOpenRefine;
        surface.Unloaded += QScreenDropRefine;
    }

    internal string? QScreenFilm => (string?)_qScreenSurface.GetValue(QScreenFilmProperty);

    internal TimeSpan QScreenFrom => (TimeSpan)_qScreenSurface.GetValue(QScreenFromProperty);

    internal TimeSpan? QScreenUntil => (TimeSpan?)_qScreenSurface.GetValue(QScreenUntilProperty);

    internal double QScreenVolume => (double)_qScreenSurface.GetValue(QScreenVolumeProperty);

    internal bool QScreenPlaying => (bool)_qScreenSurface.GetValue(QScreenPlayingProperty);

    private Uri? QScreenAddress => (Uri?)_qScreenSurface.GetValue(QScreenAddressProperty);

    private Border QScreenStage => QContract.QContractFind<Border>(_qScreenSurface, "PScreenStage");

    private Grid QScreenPage => QContract.QContractFind<Grid>(_qScreenSurface, "PScreenPage");

    private TextBlock QScreenNotice => QContract.QContractFind<TextBlock>(_qScreenSurface, "PScreenNotice");

    private ToggleButton QScreenSwitch => QContract.QContractFind<ToggleButton>(_qScreenSurface, "PScreenSwitch");

    internal static void QScreenIntroduce(FrameworkElement surface)
    {
        ArgumentNullException.ThrowIfNull(surface);

        if (!QScreenHold.TryGetValue(surface, out _))
        {
            QScreenHold.Add(surface, new QScreen(surface));
        }
    }

    internal static QScreen? QScreenFind(DependencyObject node)
    {
        return QScreenHold.TryGetValue(node, out QScreen? screen) ? screen : null;
    }

    internal void QScreenNoticeRefine()
    {
        QScreenPage.Visibility = Visibility.Collapsed;
        QScreenNotice.Text = QLocalizationCatalog.QLocalizationTextRead("Card.VideoFailed");
        QScreenNotice.Visibility = Visibility.Visible;
    }

    private static void QScreenSourceRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (QScreenFind(holder) is QScreen screen && (screen._qScreenOpened || screen.QScreenPlaying))
        {
            screen._qScreenPending.Stop();
            screen._qScreenPending.Start();
        }
    }

    private static void QScreenPlayingRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (QScreenFind(holder) is not QScreen screen)
        {
            return;
        }

        screen.QScreenSwitch.IsChecked = screen.QScreenPlaying;
        if (screen.QScreenPlaying && !screen._qScreenOpened && screen._qScreenSurface.IsLoaded)
        {
            screen._qScreenPending.Stop();
            screen.QScreenRefine();
            return;
        }

        screen.QScreenSyncRefine();
    }

    private static void QScreenVolumeRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (QScreenFind(holder) is not QScreen screen)
        {
            return;
        }

        screen._qScreenFile.QScreenLevelRefine();
        screen._qScreenBrowser.QScreenPageSend(
            "volume:" + screen.QScreenVolume.ToString("0.###", CultureInfo.InvariantCulture));
    }

    private void QScreenSizeRefine(object sender, SizeChangedEventArgs e)
    {
        if (!e.WidthChanged)
        {
            return;
        }

        double height = e.NewSize.Width * 9 / 16;
        if (double.IsNaN(QScreenStage.Height) || Math.Abs(QScreenStage.Height - height) > 0.5)
        {
            QScreenStage.Height = height;
        }
    }

    private void QScreenOpenRefine(object sender, RoutedEventArgs e)
    {
        if (QScreenPlaying && QScreenAddress is not null)
        {
            _qScreenPending.Stop();
            _qScreenPending.Start();
        }
    }

    private void QScreenPendingRefine(object? sender, EventArgs e)
    {
        _qScreenPending.Stop();
        QScreenRefine();
    }

    private void QScreenSwitchRefine(object sender, RoutedEventArgs e)
    {
        _qScreenSurface.SetValue(QScreenPlayingProperty, QScreenSwitch.IsChecked == true);
    }

    private void QScreenDropRefine(object sender, RoutedEventArgs e)
    {
        _qScreenPending.Stop();
        QScreenStopRefine();
        _qScreenBrowser.QScreenBrowserDispose();
    }

    private void QScreenRefine()
    {
        QScreenStopRefine();

        if (QScreenAddress is not Uri address)
        {
            return;
        }

        _qScreenOpened = true;
        if (address.IsFile)
        {
            _qScreenWeb = false;
            _qScreenFile.QScreenMediaRefine(address);
            return;
        }

        _qScreenWeb = true;
        _qScreenBrowser.QScreenPageRefine(address);
    }

    private void QScreenSyncRefine()
    {
        if (_qScreenWeb)
        {
            _qScreenBrowser.QScreenPageSend(QScreenPlaying ? "play" : "pause");
            return;
        }

        _qScreenFile.QScreenPlaybackRefine();
    }

    private void QScreenStopRefine()
    {
        _qScreenOpened = false;
        QScreenNotice.Visibility = Visibility.Collapsed;
        _qScreenFile.QScreenSilenceRefine();
        _qScreenBrowser.QScreenBlankRefine();
    }
}
