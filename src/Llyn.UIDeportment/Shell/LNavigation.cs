using System;
using System.Collections.Generic;

namespace Llyn.UIDeportment;

public sealed class LNavigation
{
    private readonly LWindow _lWindow;

    private readonly IReadOnlyList<string> _lNavigationTabs;

    public LNavigation(LWindow window, IReadOnlyList<string> tabs)
    {
        ArgumentNullException.ThrowIfNull(window);
        ArgumentNullException.ThrowIfNull(tabs);

        _lWindow = window;
        _lNavigationTabs = tabs;
    }

    public LVoyage LNavigationVoyage { get; } = new();

    public string LNavigationShownRead()
    {
        return LNavigationFind() ?? _lNavigationTabs[0];
    }

    public string? LNavigationRestore(Func<string, bool> allowed)
    {
        ArgumentNullException.ThrowIfNull(allowed);

        foreach (string tab in _lNavigationTabs)
        {
            if (_lWindow.LWindowAtelier.CAtelierModeMatch(tab))
            {
                return LNavigationAllowRead(tab, allowed);
            }
        }

        return null;
    }

    private string LNavigationAllowRead(string tab, Func<string, bool> allowed)
    {
        string restored = allowed(tab) ? tab : _lNavigationTabs[0];
        _lWindow.LWindowAtelier.CAtelierModeSave(restored);
        return restored;
    }

    public bool LNavigationShow(string tab, Func<string, bool> leave)
    {
        ArgumentNullException.ThrowIfNull(leave);

        if (!leave(tab))
        {
            return false;
        }

        return LNavigationSelect(tab, leave);
    }

    public bool LNavigationSelect(string tab, Func<string, bool> leave)
    {
        ArgumentNullException.ThrowIfNull(tab);
        ArgumentNullException.ThrowIfNull(leave);

        foreach (string other in _lNavigationTabs)
        {
            if (string.Equals(other, tab, StringComparison.Ordinal))
            {
                continue;
            }

            if (!_lWindow.LWindowAtelier.CAtelierModeMatch(other))
            {
                continue;
            }

            if (!leave(other))
            {
                return false;
            }
        }

        _lWindow.LWindowAtelier.CAtelierModeSave(tab);
        return true;
    }

    private string? LNavigationFind()
    {
        foreach (string tab in _lNavigationTabs)
        {
            if (_lWindow.LWindowAtelier.CAtelierModeMatch(tab))
            {
                return tab;
            }
        }

        return null;
    }
}
