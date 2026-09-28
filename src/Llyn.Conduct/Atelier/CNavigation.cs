using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Conduct;

public sealed class CNavigation
{
    private static readonly string[] LNavigationTabs =
    [
        "Input",
        "Library",
        "Phonology",
        "Xiesheng",
        "Yunjing",
        "Taxonomy",
        "Tenor",
        "Repertoire",
        "Corpus",
        "Reference",
        "Guild",
        "Favorite",
        "Duplex",
        "Settings",
    ];

    private readonly CAtelier _cNavigationAtelier;

    private readonly CVoyage _cNavigationVoyage = new();

    private readonly Dictionary<string, CNavigationPanel> _cNavigationPanels = new(StringComparer.Ordinal);

    private IReadOnlyList<string> _cNavigationHidden = [];

    private Action<string, string, string>? _cNavigationDiwei;

    private Action<string, string?>? _cNavigationStem;

    internal CNavigation(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _cNavigationAtelier = atelier;
    }

    public event Action<CNavigationState>? CNavigationChanged;

    public event Action? CNavigationArrived;

    public void CNavigationTabOpen()
    {
        _cNavigationHidden = LNavigationTabs.Where(tab => !LNavigationAllowedCheck(tab)).ToList();
        string? restored = null;
        if (LNavigationFind() is string stored)
        {
            restored = _cNavigationHidden.Contains(stored, StringComparer.Ordinal) ? LNavigationTabs[0] : stored;
            _cNavigationAtelier.CAtelierPosture.LPostureModeSave(restored);
        }

        LNavigationStateRaise(restored);
        if (restored is not null && _cNavigationPanels.TryGetValue(restored, out CNavigationPanel? panel))
        {
            panel.CNavigationPanelScribe(_cNavigationAtelier.LAtelierSplitRead());
        }
    }

    public bool CNavigationTabSelect(string tab)
    {
        ArgumentNullException.ThrowIfNull(tab);

        if (!LNavigationTabSelect(tab, false))
        {
            return false;
        }

        LNavigationStateRaise(tab);
        return true;
    }

    public bool CNavigationEntryOpen(long id)
    {
        return LNavigationRowOpen("Library", id);
    }

    public bool CNavigationUsageOpen(CUsage? usage)
    {
        if (usage is null)
        {
            return false;
        }

        return usage.CUsageQuoted
            ? LNavigationRowOpen("Corpus", usage.CUsageId)
            : CNavigationEntryOpen(usage.CUsageEntry);
    }

    public bool CNavigationDiweiOpen(string language, string kind, string key)
    {
        ArgumentNullException.ThrowIfNull(language);
        ArgumentNullException.ThrowIfNull(kind);
        ArgumentNullException.ThrowIfNull(key);

        if (_cNavigationDiwei is null || !LNavigationTabSelect("Yunjing", true))
        {
            return false;
        }

        LNavigationStateRaise("Yunjing");
        _cNavigationDiwei(language, kind, key);
        return true;
    }

    public bool CNavigationStationUndo()
    {
        (string tab, long id) = LNavigationStationRead();
        return LNavigationVoyageRun(show => _cNavigationVoyage.LVoyageUndo(tab, id, show));
    }

    public bool CNavigationStationRedo()
    {
        (string tab, long id) = LNavigationStationRead();
        return LNavigationVoyageRun(show => _cNavigationVoyage.LVoyageRedo(tab, id, show));
    }

    internal void LNavigationStationAdd()
    {
        (string tab, long id) = LNavigationStationRead();
        _cNavigationVoyage.LVoyageStationAdd(tab, id);
        LNavigationStateRaise(null);
    }

    internal bool LNavigationStemOpen(string language, string? key)
    {
        ArgumentNullException.ThrowIfNull(language);

        if (_cNavigationStem is null || !LNavigationTabSelect("Xiesheng", true))
        {
            return false;
        }

        LNavigationStateRaise("Xiesheng");
        _cNavigationStem(language, key);
        return true;
    }

    internal bool LNavigationRowOpen(string tab, long id)
    {
        ArgumentNullException.ThrowIfNull(tab);

        (string standing, long station) = LNavigationStationRead();
        if (!LNavigationRowSelect(tab))
        {
            return false;
        }

        _cNavigationVoyage.LVoyageStationAdd(standing, station);
        LNavigationRowShow(tab, id);
        return true;
    }

    internal void LNavigationTabAdd(
        string tab,
        Func<bool> leave,
        Func<long> station,
        Action<bool> scribe,
        Action<long> arrival,
        Func<bool>? allowed = null)
    {
        LNavigationTabCheck(tab);
        _cNavigationPanels[tab] = new CNavigationPanel(leave, station, scribe, arrival, allowed);
    }

    internal void LNavigationDiweiAttach(Action<string, string, string> open)
    {
        ArgumentNullException.ThrowIfNull(open);

        _cNavigationDiwei = open;
    }

    internal void LNavigationStemAttach(Action<string, string?> open)
    {
        ArgumentNullException.ThrowIfNull(open);

        _cNavigationStem = open;
    }

    private bool LNavigationVoyageRun(Func<Func<string, long, bool>, bool> step)
    {
        (string LNavigationTab, long LNavigationId)? landed = null;
        bool moved = step((tab, id) =>
        {
            if (!LNavigationRowSelect(tab))
            {
                return false;
            }

            landed = (tab, id);
            return true;
        });
        if (!moved || landed is not (string shown, long chosen))
        {
            return false;
        }

        LNavigationRowShow(shown, chosen);
        return true;
    }

    private void LNavigationRowShow(string tab, long id)
    {
        LNavigationStateRaise(tab);
        _cNavigationPanels[tab].CNavigationPanelArrival(id);
        CNavigationArrived?.Invoke();
    }

    private bool LNavigationRowSelect(string tab)
    {
        return _cNavigationPanels.ContainsKey(tab) && LNavigationTabSelect(tab, true);
    }

    private bool LNavigationTabSelect(string tab, bool arriving)
    {
        LNavigationTabCheck(tab);
        if (arriving && !LNavigationLeaveConfirm(tab))
        {
            return false;
        }

        foreach (string other in LNavigationTabs)
        {
            if (string.Equals(other, tab, StringComparison.Ordinal)
                || !_cNavigationAtelier.CAtelierPosture.LPostureModeMatch(other))
            {
                continue;
            }

            if (!LNavigationLeaveConfirm(other))
            {
                return false;
            }
        }

        _cNavigationAtelier.CAtelierPosture.LPostureModeSave(tab);
        return true;
    }

    private bool LNavigationLeaveConfirm(string tab)
    {
        return !_cNavigationPanels.TryGetValue(tab, out CNavigationPanel? panel) || panel.CNavigationPanelLeave();
    }

    private bool LNavigationAllowedCheck(string tab)
    {
        return !_cNavigationPanels.TryGetValue(tab, out CNavigationPanel? panel)
               || (panel.CNavigationPanelAllowed?.Invoke() ?? true);
    }

    private (string LNavigationTab, long LNavigationId) LNavigationStationRead()
    {
        string tab = LNavigationFind() ?? LNavigationTabs[0];
        return (tab, _cNavigationPanels.TryGetValue(tab, out CNavigationPanel? panel)
            ? panel.CNavigationPanelStation()
            : 0);
    }

    private string? LNavigationFind()
    {
        foreach (string tab in LNavigationTabs)
        {
            if (_cNavigationAtelier.CAtelierPosture.LPostureModeMatch(tab))
            {
                return tab;
            }
        }

        return null;
    }

    private void LNavigationStateRaise(string? tab)
    {
        CNavigationChanged?.Invoke(
            new CNavigationState(tab, _cNavigationHidden, _cNavigationVoyage.LVoyageRead()));
    }

    private static void LNavigationTabCheck(string tab)
    {
        if (!LNavigationTabs.Contains(tab, StringComparer.Ordinal))
        {
            throw new ArgumentOutOfRangeException(nameof(tab), tab, null);
        }
    }
}
