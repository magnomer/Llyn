using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCohort
{
    private readonly CEnvoy _cCohortEnvoy;

    private readonly LEntryPort _cCohortEntryPort;

    private readonly LPortraitPort _cCohortPortraitPort;

    private readonly LSettingsPort _cCohortSettingsPort;

    private LVista? _cCohortRoll;

    private LVista? _cCohortVista;

    internal CCohort(
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

        _cCohortEnvoy = envoy;
        _cCohortEntryPort = entries;
        _cCohortPortraitPort = portraits;
        _cCohortSettingsPort = settings;
        CCohortPanel = new CPanel(
            envoy, settings, "Register.LoadFailed", "Scribe", changeSeam, finishSeam, shownSeam);
    }

    public CPanel CCohortPanel { get; }

    public string CCohortEmptyKey =>
        _cCohortVista?.LVistaQueried ?? false ? "Register.Unmatched" : "Register.Vacant";

    internal void LCohortVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cCohortVista?.LVistaQuery ?? string.Empty);
        _cCohortRoll = roll;
        _cCohortVista = vista;
        CCohortPanel.CPanelVistaRestore(vista);
    }

    internal void LCohortObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        CPanel panel = CCohortPanel;
        panel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelObserverAttach(CSubject.CSubjectVista, _ => marshal(panel.CPanelRowsResonate));
        panel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => marshal(panel.CPanelDraftResonate));
    }

    public void CCohortQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cCohortVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> CCohortRowsRead()
    {
        try
        {
            return _cCohortEntryPort.LEngineEntryFind(_cCohortRoll, _cCohortVista)
                .Select(CCatalog.LCatalogRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cCohortEnvoy, _cCohortSettingsPort, "Register.LoadFailed", exception);
            return [];
        }
    }

    internal string LCohortFileRead()
    {
        return LVista.LVistaFileRead(_cCohortVista);
    }

    public Task CCohortPortraitPrint()
    {
        if (!CCohortPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cCohortEnvoy,
            _cCohortSettingsPort,
            chosen => _cCohortPortraitPort.LEnginePortraitPrint(
                _cCohortVista, CPortrait.LPortraitLabelRead(_cCohortSettingsPort), chosen));
    }

    public Task CCohortPortraitExport()
    {
        if (!CCohortPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitFileExport(
            _cCohortEnvoy,
            _cCohortSettingsPort,
            LCohortFileRead,
            (file, medium) => _cCohortPortraitPort.LEnginePortraitExport(
                _cCohortVista, file, medium, CPortrait.LPortraitLabelRead(_cCohortSettingsPort)));
    }
}
