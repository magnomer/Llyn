using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CMembership
{
    private readonly CEnvoy _cMembershipEnvoy;

    private readonly LEntryPort _cMembershipEntryPort;

    private readonly LPortraitPort _cMembershipPortraitPort;

    private readonly LSettingsPort _cMembershipSettingsPort;

    private LVista? _cMembershipRoll;

    private LVista? _cMembershipVista;

    internal CMembership(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        CEnvoy envoy,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);

        _cMembershipEnvoy = envoy;
        _cMembershipEntryPort = entries;
        _cMembershipPortraitPort = portraits;
        _cMembershipSettingsPort = settings;
        CMembershipPanel = new CPanel(envoy, settings, "Tag.LoadFailed", "Scribe", changeSeam, finishSeam, shownSeam);
    }

    public CPanel CMembershipPanel { get; }

    public string CMembershipEmptyKey =>
        _cMembershipVista?.LVistaQueried ?? false ? "Tag.Unmatched" : "Tag.Vacant";

    internal void LMembershipVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cMembershipVista?.LVistaQuery ?? string.Empty);
        _cMembershipRoll = roll;
        _cMembershipVista = vista;
        CMembershipPanel.CPanelVistaRestore(vista);
    }

    internal void LMembershipObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        CPanel panel = CMembershipPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelObserverAttach(CSubject.CSubjectVista, _ => marshal(panel.CPanelRowsResonate));
        panel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => marshal(panel.CPanelDraftResonate));
    }

    public void CMembershipQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cMembershipVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> CMembershipRowsRead()
    {
        try
        {
            return _cMembershipEntryPort.LEngineEntryFind(_cMembershipRoll, _cMembershipVista)
                .Select(CPanel.CPanelRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cMembershipEnvoy, _cMembershipSettingsPort, "Tag.LoadFailed", exception);
            return [];
        }
    }

    internal string LMembershipFileRead()
    {
        return LVista.LVistaFileRead(_cMembershipVista);
    }

    public Task CMembershipPortraitPrint()
    {
        if (!CMembershipPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cMembershipEnvoy,
            _cMembershipSettingsPort,
            chosen => _cMembershipPortraitPort.LEnginePortraitPrint(
                _cMembershipVista, CPortrait.LPortraitLabelRead(_cMembershipSettingsPort), chosen));
    }

    public Task CMembershipPortraitExport()
    {
        if (!CMembershipPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitFileExport(
            _cMembershipEnvoy,
            _cMembershipSettingsPort,
            LMembershipFileRead(),
            (file, medium) => _cMembershipPortraitPort.LEnginePortraitExport(
                _cMembershipVista, file, medium, CPortrait.LPortraitLabelRead(_cMembershipSettingsPort)));
    }
}
