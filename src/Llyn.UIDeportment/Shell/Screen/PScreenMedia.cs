using System;
using System.Windows;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

public partial class PScreen
{
    private const int PScreenClockDelay = 250;

    private readonly DispatcherTimer _pScreenClock = new(DispatcherPriority.Background)
    {
        Interval = TimeSpan.FromMilliseconds(PScreenClockDelay),
    };

    private void PScreenMediaRefine(Uri address)
    {
        PScreenMedia.Visibility = Visibility.Visible;
        PScreenMedia.Source = address;
        PScreenMedia.Play();
        _pScreenClock.Start();
    }

    private void PScreenPlaybackRefine()
    {
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

    private void PScreenSilenceRefine()
    {
        _pScreenClock.Stop();

        PScreenMedia.Visibility = Visibility.Collapsed;
        PScreenMedia.Stop();
        PScreenMedia.Source = null;
    }

    private void PScreenReadyRefine(object sender, RoutedEventArgs e)
    {
        PScreenMedia.Volume = PScreenVolume;
        PScreenMedia.Position = PScreenFrom;
        if (!PScreenPlaying)
        {
            PScreenMedia.Pause();
        }
    }

    private void PScreenFinishRefine(object sender, RoutedEventArgs e)
    {
        PScreenMedia.Position = PScreenFrom;
        PScreenMedia.Play();
    }

    private void PScreenFailureRefine(object? sender, ExceptionRoutedEventArgs e)
    {
        PScreenMedia.Visibility = Visibility.Collapsed;
        PScreenNoticeRefine();
    }

    private void PScreenSpanRefine(object? sender, EventArgs e)
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
}
