using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class LNavigation
{
    private readonly CAtelier _lNavigationAtelier;

    private readonly IReadOnlyList<string> _lNavigationTabs;

    public LNavigation(CAtelier atelier, IReadOnlyList<string> tabs)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(tabs);

        _lNavigationAtelier = atelier;
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
            if (_lNavigationAtelier.CAtelierModeMatch(tab))
            {
                return LNavigationAllowRead(tab, allowed);
            }
        }

        return null;
    }

    private string LNavigationAllowRead(string tab, Func<string, bool> allowed)
    {
        string restored = allowed(tab) ? tab : _lNavigationTabs[0];
        _lNavigationAtelier.CAtelierModeSave(restored);
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

            if (!_lNavigationAtelier.CAtelierModeMatch(other))
            {
                continue;
            }

            if (!leave(other))
            {
                return false;
            }
        }

        _lNavigationAtelier.CAtelierModeSave(tab);
        return true;
    }

    private string? LNavigationFind()
    {
        foreach (string tab in _lNavigationTabs)
        {
            if (_lNavigationAtelier.CAtelierModeMatch(tab))
            {
                return tab;
            }
        }

        return null;
    }
}
