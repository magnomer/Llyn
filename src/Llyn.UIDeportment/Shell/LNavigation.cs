using System;
using System.Collections.Generic;
using System.Windows;

namespace Llyn.UIDeportment;

public sealed class LNavigation
{
    private readonly LWindow _lWindow;

    private readonly DependencyProperty _lNavigationChosen;

    public LNavigation(Window window, LWindow deportment, DependencyProperty chosen, IReadOnlyList<LTab> tabs)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(deportment);
        ArgumentNullException.ThrowIfNull(chosen);
        ArgumentNullException.ThrowIfNull(tabs);
        _lWindow = deportment;
        _lNavigationChosen = chosen;
        LNavigationTabs = tabs;
        LNavigationVoyage = new LVoyage(window, this);
    }

    public IReadOnlyList<LTab> LNavigationTabs { get; }

    public LVoyage LNavigationVoyage { get; }

    public LTab LNavigationShownRead()
    {
        foreach (LTab tab in LNavigationTabs)
        {
            if (_lWindow.LWindowModeMatch(tab.LTabMode))
            {
                return tab;
            }
        }

        return LNavigationTabs[0];
    }

    public bool LNavigationSelect(object? button)
    {
        LTab? chosen = LNavigationFind(button);
        if (chosen is null)
        {
            return false;
        }

        return LNavigationTabSelect(chosen);
    }

    public void LNavigationRestore()
    {
        foreach (LTab tab in LNavigationTabs)
        {
            bool allowed = tab.LTabAllowed is null || tab.LTabAllowed();
            tab.LTabButton.Visibility = allowed ? Visibility.Visible : Visibility.Collapsed;
        }

        LTab? restored = null;
        foreach (LTab tab in LNavigationTabs)
        {
            if (_lWindow.LWindowModeMatch(tab.LTabMode))
            {
                restored = tab;
                break;
            }
        }

        if (restored is null)
        {
            return;
        }

        if (restored.LTabAllowed is not null)
        {
            if (!restored.LTabAllowed())
            {
                restored = LNavigationTabs[0];
            }
        }

        LNavigationApply(restored);
        restored.LTabScribe?.Invoke(_lWindow.LWindowPostureRead().LPostureStateSplit);
    }

    public bool LNavigationShow(object button, long id)
    {
        ArgumentNullException.ThrowIfNull(button);
        (LTab, long) station = LNavigationVoyage.LVoyageStationRead();
        LTab? target = LNavigationFind(button);
        if (target is null)
        {
            return false;
        }

        if (!LNavigationTabShow(target, id))
        {
            return false;
        }

        LNavigationVoyage.LVoyageRecord(station);
        return true;
    }

    public void LNavigationShow(object button, Action arrival)
    {
        ArgumentNullException.ThrowIfNull(button);
        ArgumentNullException.ThrowIfNull(arrival);
        LTab? target = LNavigationFind(button);
        if (target is null)
        {
            return;
        }

        if (!LNavigationTargetSelect(target))
        {
            return;
        }

        arrival();
    }

    public bool LNavigationTabShow(LTab target, long id)
    {
        ArgumentNullException.ThrowIfNull(target);
        if (target.LTabArrival is null)
        {
            return false;
        }

        if (!LNavigationTargetSelect(target))
        {
            return false;
        }

        target.LTabArrival(id);
        return true;
    }

    private bool LNavigationTargetSelect(LTab target)
    {
        if (target.LTabLeave is not null)
        {
            if (!target.LTabLeave())
            {
                return false;
            }
        }

        return LNavigationTabSelect(target);
    }

    private bool LNavigationTabSelect(LTab chosen)
    {
        foreach (LTab tab in LNavigationTabs)
        {
            if (ReferenceEquals(tab, chosen))
            {
                continue;
            }

            if (!_lWindow.LWindowModeMatch(tab.LTabMode))
            {
                continue;
            }

            if (tab.LTabLeave is null)
            {
                continue;
            }

            if (!tab.LTabLeave())
            {
                return false;
            }
        }

        LNavigationApply(chosen);
        return true;
    }

    private void LNavigationApply(LTab chosen)
    {
        foreach (LTab tab in LNavigationTabs)
        {
            bool shown = ReferenceEquals(tab, chosen);
            tab.LTabButton.SetValue(_lNavigationChosen, shown);
            tab.LTabPanel.Visibility = shown ? Visibility.Visible : Visibility.Collapsed;
        }

        _lWindow.LWindowModeSave(chosen.LTabMode);
    }

    private LTab? LNavigationFind(object? button)
    {
        foreach (LTab tab in LNavigationTabs)
        {
            if (ReferenceEquals(tab.LTabButton, button))
            {
                return tab;
            }
        }

        return null;
    }
}
