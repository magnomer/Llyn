using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Llyn.UIShell;

public partial class PEditor
{
    private readonly Dictionary<PVideo, MediaElement> _pVideoPreview = [];

    private DispatcherTimer? _pVideoClock;

    internal void PVideoLoadHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not MediaElement { DataContext: PVideo row } preview)
        {
            return;
        }

        _pVideoPreview[row] = preview;
        _pVideoClock ??= PVideoClockCreate();
        row.PropertyChanged += PVideoAddressHandle;

        if (preview.Source is not null && row.PVideoPlaying)
        {
            preview.Play();
        }
    }

    private void PVideoAddressHandle(object? sender, PropertyChangedEventArgs arguments)
    {
        if (sender is not PVideo row
            || !string.Equals(arguments.PropertyName, nameof(PVideo.PVideoPreview), StringComparison.Ordinal)
            || !_pVideoPreview.TryGetValue(row, out MediaElement? preview))
        {
            return;
        }

        if (preview.Source is null)
        {
            preview.Close();
            return;
        }

        if (row.PVideoPlaying)
        {
            preview.Play();
        }
    }

    internal void PVideoReadyHandle(object sender, RoutedEventArgs e)
    {
        if (sender is MediaElement { DataContext: PVideo row } preview)
        {
            preview.Position = row.PVideoFrom;
        }
    }

    internal void PVideoDropHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not MediaElement { DataContext: PVideo row } preview)
        {
            return;
        }

        row.PropertyChanged -= PVideoAddressHandle;
        _pVideoPreview.Remove(row);
        preview.Close();
    }

    internal void PVideoPlayHandle(object sender, RoutedEventArgs e)
    {
        if (sender is not FrameworkElement { DataContext: PVideo row }
            || !_pVideoPreview.TryGetValue(row, out MediaElement? preview))
        {
            return;
        }

        if (row.PVideoPlaying)
        {
            preview.Play();
            return;
        }

        preview.Pause();
    }

    internal void PVideoFinishHandle(object sender, RoutedEventArgs e)
    {
        if (sender is MediaElement { DataContext: PVideo row } preview)
        {
            preview.Position = row.PVideoFrom;
            preview.Play();
        }
    }

    private DispatcherTimer PVideoClockCreate()
    {
        DispatcherTimer clock = new(DispatcherPriority.Background)
        {
            Interval = TimeSpan.FromMilliseconds(250),
        };

        clock.Tick += PVideoSpanHandle;
        clock.Start();
        return clock;
    }

    private void PVideoSpanHandle(object? sender, EventArgs e)
    {
        foreach (KeyValuePair<PVideo, MediaElement> pair in _pVideoPreview)
        {
            if (pair.Value.Source is null
                || pair.Key.PVideoUntil is not TimeSpan until
                || pair.Value.Position < until)
            {
                continue;
            }

            pair.Value.Position = pair.Key.PVideoFrom;
        }
    }

    private void PVideoClose()
    {
        _pVideoClock?.Stop();
        _pVideoClock = null;

        foreach (KeyValuePair<PVideo, MediaElement> pair in _pVideoPreview)
        {
            pair.Key.PropertyChanged -= PVideoAddressHandle;
            pair.Value.Close();
        }

        _pVideoPreview.Clear();
    }
}
