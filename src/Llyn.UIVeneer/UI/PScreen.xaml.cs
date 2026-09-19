using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Llyn.UIVeneer;

public partial class PScreen : UserControl
{
    private const int PScreenDelay = 500;

    private readonly DispatcherTimer _pScreenPending;

    private bool _pScreenWeb;

    private bool _pScreenOpened;

    public PScreen()
    {
        InitializeComponent();

        _pScreenPending = new DispatcherTimer(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(PScreenDelay),
        };
        _pScreenPending.Tick += PScreenPendingHandle;

        PScreenClockPrepare();

        Loaded += PScreenOpenHandle;
        Unloaded += PScreenDropHandle;
    }

    public static readonly DependencyProperty PScreenAddressProperty = DependencyProperty.Register(
        nameof(PScreenAddress),
        typeof(Uri),
        typeof(PScreen),
        new PropertyMetadata(null, PScreenSourceHandle));

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

    public Uri? PScreenAddress
    {
        get => (Uri?)GetValue(PScreenAddressProperty);
        set => SetValue(PScreenAddressProperty, value);
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
        if (holder is PScreen screen && (screen._pScreenOpened || screen.PScreenPlaying))
        {
            screen._pScreenPending.Stop();
            screen._pScreenPending.Start();
        }
    }

    private static void PScreenPlayingHandle(DependencyObject holder, DependencyPropertyChangedEventArgs e)
    {
        if (holder is not PScreen screen)
        {
            return;
        }

        screen.PScreenSwitch.IsChecked = screen.PScreenPlaying;
        if (screen.PScreenPlaying && !screen._pScreenOpened && screen.IsLoaded)
        {
            screen._pScreenPending.Stop();
            screen.PScreenShow();
            return;
        }

        screen.PScreenSync();
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
        PScreenPageSend("volume:" + PScreenVolume.ToString("0.###", CultureInfo.InvariantCulture));
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
        if (PScreenPlaying && PScreenAddress is not null)
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

    private void PScreenDropHandle(object sender, RoutedEventArgs e)
    {
        _pScreenPending.Stop();
        PScreenStop();
        PScreenBrowserDispose();
    }

    private void PScreenShow()
    {
        PScreenStop();

        if (PScreenAddress is not Uri address)
        {
            return;
        }

        _pScreenOpened = true;
        if (address.IsFile)
        {
            _pScreenWeb = false;
            PScreenMediaPlay(address);
            return;
        }

        _pScreenWeb = true;
        PScreenPageShow(address);
    }

    private void PScreenSync()
    {
        if (_pScreenWeb)
        {
            PScreenPageSend(PScreenPlaying ? "play" : "pause");
            return;
        }

        PScreenMediaSync();
    }

    private void PScreenStop()
    {
        _pScreenOpened = false;
        PScreenNotice.Visibility = Visibility.Collapsed;
        PScreenMediaStop();
        PScreenPageStop();
    }

    private void PScreenNoticeShow()
    {
        PScreenPage.Visibility = Visibility.Collapsed;
        PScreenNotice.Text = PLocalizationCatalog.PLocalizationCatalogCurrent["Card.VideoFailed"];
        PScreenNotice.Visibility = Visibility.Visible;
    }
}
