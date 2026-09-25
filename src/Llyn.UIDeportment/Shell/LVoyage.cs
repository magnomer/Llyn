using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace Llyn.UIDeportment;

public sealed class LVoyage
{
    private const int LVoyageCap = 50;

    private readonly LinkedList<(LTab LVoyageTab, long LVoyageId)> _lVoyagePast = new();

    private readonly LinkedList<(LTab LVoyageTab, long LVoyageId)> _lVoyageFuture = new();

    private readonly LNavigation _lVoyageNavigation;

    public LVoyage(UIElement window, LNavigation navigation)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(navigation);
        _lVoyageNavigation = navigation;
        window.PreviewKeyDown += LVoyageKeyHandle;
        window.PreviewMouseDown += LVoyageMouseHandle;
    }

    public (LTab LVoyageTab, long LVoyageId) LVoyageStationRead()
    {
        LTab tab = _lVoyageNavigation.LNavigationShownRead();
        return (tab, tab.LTabStation?.Invoke() ?? 0);
    }

    public void LVoyageRecord()
    {
        LVoyageRecord(LVoyageStationRead());
    }

    public void LVoyageRecord((LTab LVoyageTab, long LVoyageId) station)
    {
        if (station.LVoyageId == 0 || _lVoyagePast.Last?.Value == station)
        {
            return;
        }

        LVoyageStationAdd(_lVoyagePast, station);
        _lVoyageFuture.Clear();
        LVoyageUpdate();
    }

    private static void LVoyageStationAdd(
        LinkedList<(LTab LVoyageTab, long LVoyageId)> trail, (LTab LVoyageTab, long LVoyageId) station)
    {
        if (station.LVoyageId == 0)
        {
            return;
        }

        if (trail.Count >= LVoyageCap)
        {
            trail.RemoveFirst();
        }

        trail.AddLast(station);
    }

    public void LVoyageRetreat()
    {
        LVoyageRun(_lVoyagePast, _lVoyageFuture);
    }

    public void LVoyageAdvance()
    {
        LVoyageRun(_lVoyageFuture, _lVoyagePast);
    }

    private void LVoyageRun(
        LinkedList<(LTab LVoyageTab, long LVoyageId)> source, LinkedList<(LTab LVoyageTab, long LVoyageId)> target)
    {
        if (source.Last is null)
        {
            return;
        }

        (LTab LVoyageTab, long LVoyageId) current = LVoyageStationRead();
        (LTab LVoyageTab, long LVoyageId) next = source.Last.Value;
        if (!_lVoyageNavigation.LNavigationTabShow(next.LVoyageTab, next.LVoyageId))
        {
            return;
        }

        source.RemoveLast();
        LVoyageStationAdd(target, current);
        LVoyageUpdate();
    }

    private void LVoyageKeyHandle(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.System || Keyboard.Modifiers != ModifierKeys.Alt)
        {
            return;
        }

        if (e.SystemKey == Key.Left)
        {
            LVoyageRetreat();
            e.Handled = true;
        }
        else if (e.SystemKey == Key.Right)
        {
            LVoyageAdvance();
            e.Handled = true;
        }
    }

    private void LVoyageMouseHandle(object sender, MouseButtonEventArgs e)
    {
        if (e.ChangedButton == MouseButton.XButton1)
        {
            LVoyageRetreat();
            e.Handled = true;
        }
        else if (e.ChangedButton == MouseButton.XButton2)
        {
            LVoyageAdvance();
            e.Handled = true;
        }
    }

    private void LVoyageUpdate()
    {
        bool past = _lVoyagePast.Count > 0;
        bool future = _lVoyageFuture.Count > 0;
        foreach (LTab tab in _lVoyageNavigation.LNavigationTabs)
        {
            tab.LTabVoyage?.Invoke(past, future);
        }
    }
}
