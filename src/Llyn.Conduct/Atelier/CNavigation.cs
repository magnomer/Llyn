using System;
using System.Collections.Generic;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CNavigation
{
    private readonly LPosture _cNavigationPosture;

    private readonly IReadOnlyList<string> _cNavigationTabs;

    internal CNavigation(LPosture posture, IReadOnlyList<string> tabs)
    {
        ArgumentNullException.ThrowIfNull(posture);
        ArgumentNullException.ThrowIfNull(tabs);

        _cNavigationPosture = posture;
        _cNavigationTabs = tabs;
    }

    public static CNavigation CNavigationCreate(CAtelier atelier, IReadOnlyList<string> tabs)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        return new CNavigation(atelier.CAtelierPosture, tabs);
    }

    public string CNavigationTabRead()
    {
        return LNavigationFind() ?? _cNavigationTabs[0];
    }

    public string? CNavigationTabOpen(Func<string, bool> allowed)
    {
        ArgumentNullException.ThrowIfNull(allowed);

        if (LNavigationFind() is not string tab)
        {
            return null;
        }

        return LNavigationAllowRead(tab, allowed);
    }

    public bool CNavigationTabSelect(string tab, bool arriving, Func<string, bool> leave)
    {
        ArgumentNullException.ThrowIfNull(tab);
        ArgumentNullException.ThrowIfNull(leave);

        if (arriving && !leave(tab))
        {
            return false;
        }

        foreach (string other in _cNavigationTabs)
        {
            if (string.Equals(other, tab, StringComparison.Ordinal))
            {
                continue;
            }

            if (!_cNavigationPosture.LPostureModeMatch(other))
            {
                continue;
            }

            if (!leave(other))
            {
                return false;
            }
        }

        _cNavigationPosture.LPostureModeSave(tab);
        return true;
    }

    private string LNavigationAllowRead(string tab, Func<string, bool> allowed)
    {
        string restored = allowed(tab) ? tab : _cNavigationTabs[0];
        _cNavigationPosture.LPostureModeSave(restored);
        return restored;
    }

    private string? LNavigationFind()
    {
        foreach (string tab in _cNavigationTabs)
        {
            if (_cNavigationPosture.LPostureModeMatch(tab))
            {
                return tab;
            }
        }

        return null;
    }
}
