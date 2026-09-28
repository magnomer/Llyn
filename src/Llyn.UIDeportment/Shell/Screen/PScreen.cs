using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public partial class PScreen : UserControl
{
    private const int PScreenDelay = 500;

    private readonly DispatcherTimer _pScreenPending;

    private bool _pScreenWeb;

    private bool _pScreenOpened;

    public PScreen()
    {
        UserControl surface = (UserControl)System.Windows.Application.LoadComponent(
            new Uri("/Llyn.UIVeneer;component/Shell/Screen/PScreen.xaml", UriKind.Relative));
        Content = surface;
        NameScope.SetNameScope(this, NameScope.GetNameScope(surface));

        PScreenStage.SizeChanged += PScreenSizeRefine;
        PScreenMedia.MediaOpened += PScreenReadyRefine;
        PScreenMedia.MediaEnded += PScreenFinishRefine;
        PScreenMedia.MediaFailed += PScreenFailureRefine;
        PScreenSwitch.Click += PScreenSwitchRefine;

        _pScreenPending = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(PScreenDelay),
        };
        _pScreenPending.Tick += PScreenPendingRefine;
        _pScreenClock.Tick += PScreenSpanRefine;

        Loaded += PScreenOpenRefine;
        Unloaded += PScreenDropRefine;
    }

    private Border PScreenStage => (Border)FindName(nameof(PScreenStage));

    private MediaElement PScreenMedia => (MediaElement)FindName(nameof(PScreenMedia));

    private Grid PScreenPage => (Grid)FindName(nameof(PScreenPage));

    private TextBlock PScreenNotice => (TextBlock)FindName(nameof(PScreenNotice));

    private ToggleButton PScreenSwitch => (ToggleButton)FindName(nameof(PScreenSwitch));

    public static readonly DependencyProperty PScreenAddressProperty = DependencyProperty.Register(
        nameof(PScreenAddress),
        typeof(Uri),
        typeof(PScreen),
        new PropertyMetadata(null, PScreenSourceRefine));

    public static readonly DependencyProperty PScreenFilmProperty = DependencyProperty.Register(
        nameof(PScreenFilm),
        typeof(string),
        typeof(PScreen),
        new PropertyMetadata(null));

    public static readonly DependencyProperty PScreenFromProperty = DependencyProperty.Register(
        nameof(PScreenFrom),
        typeof(TimeSpan),
        typeof(PScreen),
        new PropertyMetadata(TimeSpan.Zero, PScreenSourceRefine));

    public static readonly DependencyProperty PScreenUntilProperty = DependencyProperty.Register(
        nameof(PScreenUntil),
        typeof(TimeSpan?),
        typeof(PScreen),
        new PropertyMetadata(null, PScreenSourceRefine));

    public static readonly DependencyProperty PScreenVolumeProperty = DependencyProperty.Register(
        nameof(PScreenVolume),
        typeof(double),
        typeof(PScreen),
        new PropertyMetadata(1d, PScreenVolumeRefine));

    public static readonly DependencyProperty PScreenPlayingProperty = DependencyProperty.Register(
        nameof(PScreenPlaying),
        typeof(bool),
        typeof(PScreen),
        new FrameworkPropertyMetadata(
            false, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, PScreenPlayingRefine));

    public Uri? PScreenAddress
    {
        get => (Uri?)GetValue(PScreenAddressProperty);
        set => SetValue(PScreenAddressProperty, value);
    }

    public string? PScreenFilm
    {
        get => (string?)GetValue(PScreenFilmProperty);
        set => SetValue(PScreenFilmProperty, value);
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

    private static void PScreenSourceRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is PScreen screen && (screen._pScreenOpened || screen.PScreenPlaying))
        {
            screen._pScreenPending.Stop();
            screen._pScreenPending.Start();
        }
    }

    private static void PScreenPlayingRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is not PScreen screen)
        {
            return;
        }

        screen.PScreenSwitch.IsChecked = screen.PScreenPlaying;
        if (screen.PScreenPlaying && !screen._pScreenOpened && screen.IsLoaded)
        {
            screen._pScreenPending.Stop();
            screen.PScreenRefine();
            return;
        }

        screen.PScreenSyncRefine();
    }

    private static void PScreenVolumeRefine(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is not PScreen screen)
        {
            return;
        }

        screen.PScreenMedia.Volume = screen.PScreenVolume;
        screen.PScreenPageSend("volume:" + screen.PScreenVolume.ToString("0.###", CultureInfo.InvariantCulture));
    }

    private void PScreenSizeRefine(object sender, SizeChangedEventArgs e)
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

    private void PScreenOpenRefine(object sender, RoutedEventArgs e)
    {
        if (PScreenPlaying && PScreenAddress is not null)
        {
            _pScreenPending.Stop();
            _pScreenPending.Start();
        }
    }

    private void PScreenPendingRefine(object? sender, EventArgs e)
    {
        _pScreenPending.Stop();
        PScreenRefine();
    }

    private void PScreenSwitchRefine(object sender, RoutedEventArgs e)
    {
        PScreenPlaying = PScreenSwitch.IsChecked == true;
    }

    private void PScreenDropRefine(object sender, RoutedEventArgs e)
    {
        _pScreenPending.Stop();
        PScreenStopRefine();
        PScreenBrowserDispose();
    }

    private void PScreenRefine()
    {
        PScreenStopRefine();

        if (PScreenAddress is not Uri address)
        {
            return;
        }

        _pScreenOpened = true;
        if (address.IsFile)
        {
            _pScreenWeb = false;
            PScreenMediaRefine(address);
            return;
        }

        _pScreenWeb = true;
        PScreenPageRefine(address);
    }

    private void PScreenSyncRefine()
    {
        if (_pScreenWeb)
        {
            PScreenPageSend(PScreenPlaying ? "play" : "pause");
            return;
        }

        PScreenPlaybackRefine();
    }

    private void PScreenStopRefine()
    {
        _pScreenOpened = false;
        PScreenNotice.Visibility = Visibility.Collapsed;
        PScreenSilenceRefine();
        PScreenBlankRefine();
    }

    private void PScreenNoticeRefine()
    {
        PScreenPage.Visibility = Visibility.Collapsed;
        PScreenNotice.Text = QLocalizationCatalog.QLocalizationCatalogCurrent["Card.VideoFailed"];
        PScreenNotice.Visibility = Visibility.Visible;
    }
}
