using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCohort
{
    private readonly CEnvoy _cCohortEnvoy;

    private readonly LVistaPort _cCohortVistaPort;

    private readonly LPortraitPort _cCohortPortraitPort;

    private readonly LSettingsPort _cCohortSettingsPort;

    private LVista? _cCohortRoll;

    internal CCohort(
        LVistaPort vistas,
        LPortraitPort portraits,
        LSettingsPort settings,
        CEnvoy envoy,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);

        _cCohortEnvoy = envoy;
        _cCohortVistaPort = vistas;
        _cCohortPortraitPort = portraits;
        _cCohortSettingsPort = settings;
        CCohortPanel = new CPanel(
            envoy,
            settings,
            vistas,
            "Register.LoadFailed",
            "Scribe",
            changeSeam,
            finishSeam,
            shownSeam,
            "Register.Vacant",
            "Register.Unmatched");
    }

    public CPanel CCohortPanel { get; }

    internal void LCohortVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _cCohortRoll = roll;
        CCohortPanel.CPanelVistaRestore(vista);
    }

    internal void LCohortObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        CPanel panel = CCohortPanel;
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => marshal(panel.CPanelAperture.CApertureRowsResonate));
        panel.CPanelAperture.CApertureChosenAttach(CSubject.CSubjectEntry, _ => marshal(panel.CPanelDraftResonate));
    }

    public IReadOnlyList<CVistaRow> CCohortRowsRead()
    {
        try
        {
            return _cCohortVistaPort.LEngineEntryFind(_cCohortRoll, CCohortPanel.CPanelAperture.CApertureVista)
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
        return _cCohortVistaPort.LEngineFileRead(CCohortPanel.CPanelAperture.CApertureVista);
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
                CCohortPanel.CPanelAperture.CApertureVista,
                CPortrait.LPortraitLabelRead(_cCohortSettingsPort),
                chosen));
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
                CCohortPanel.CPanelAperture.CApertureVista,
                file,
                medium,
                CPortrait.LPortraitLabelRead(_cCohortSettingsPort)));
    }
}
