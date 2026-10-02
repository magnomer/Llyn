using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Llyn.UIDeportment;

internal sealed class QScreenFile
{
    private const int QScreenClockDelay = 250;

    private readonly DispatcherTimer _qScreenClock = new(DispatcherPriority.Background)
    {
        Interval = TimeSpan.FromMilliseconds(QScreenClockDelay),
    };

    private readonly QScreen _qScreenFileDriver;

    private readonly FrameworkElement _qScreenFileSurface;

    internal QScreenFile(QScreen driver, FrameworkElement surface)
    {
        _qScreenFileDriver = driver;
        _qScreenFileSurface = surface;

        QScreenMedia.MediaOpened += QScreenReadyRefine;
        QScreenMedia.MediaEnded += QScreenFinishRefine;
        QScreenMedia.MediaFailed += QScreenFailureRefine;
        _qScreenClock.Tick += QScreenSpanRefine;
    }

    private MediaElement QScreenMedia => QContract.QContractFind<MediaElement>(_qScreenFileSurface, "PScreenMedia");

    internal void QScreenMediaRefine(Uri address)
    {
        QScreenMedia.Visibility = Visibility.Visible;
        QScreenMedia.Source = address;
        QScreenMedia.Play();
        _qScreenClock.Start();
    }

    internal void QScreenPlaybackRefine()
    {
        if (QScreenMedia.Source is null)
        {
            return;
        }

        if (_qScreenFileDriver.QScreenPlaying)
        {
            QScreenMedia.Play();
            return;
        }

        QScreenMedia.Pause();
    }

    internal void QScreenSilenceRefine()
    {
        _qScreenClock.Stop();

        QScreenMedia.Visibility = Visibility.Collapsed;
        QScreenMedia.Stop();
        QScreenMedia.Source = null;
    }

    internal void QScreenLevelRefine()
    {
        QScreenMedia.Volume = _qScreenFileDriver.QScreenVolume;
    }

    private void QScreenReadyRefine(object sender, RoutedEventArgs e)
    {
        QScreenMedia.Volume = _qScreenFileDriver.QScreenVolume;
        QScreenMedia.Position = _qScreenFileDriver.QScreenFrom;
        if (!_qScreenFileDriver.QScreenPlaying)
        {
            QScreenMedia.Pause();
        }
    }

    private void QScreenFinishRefine(object sender, RoutedEventArgs e)
    {
        QScreenMedia.Position = _qScreenFileDriver.QScreenFrom;
        QScreenMedia.Play();
    }

    private void QScreenFailureRefine(object? sender, ExceptionRoutedEventArgs e)
    {
        QScreenMedia.Visibility = Visibility.Collapsed;
        _qScreenFileDriver.QScreenNoticeRefine();
    }

    private void QScreenSpanRefine(object? sender, EventArgs e)
    {
        if (QScreenMedia.Source is null
            || _qScreenFileDriver.QScreenUntil is not TimeSpan until
            || until <= _qScreenFileDriver.QScreenFrom
            || QScreenMedia.Position < until)
        {
            return;
        }

        QScreenMedia.Position = _qScreenFileDriver.QScreenFrom;
    }
}
