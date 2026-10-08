using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CMembership
{
    private readonly CEnvoy _cMembershipEnvoy;

    private readonly LVistaPort _cMembershipVistaPort;

    private readonly LPortraitPort _cMembershipPortraitPort;

    private readonly LSettingsPort _cMembershipSettingsPort;

    private LVista? _cMembershipRoll;

    internal CMembership(
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

        _cMembershipEnvoy = envoy;
        _cMembershipVistaPort = vistas;
        _cMembershipPortraitPort = portraits;
        _cMembershipSettingsPort = settings;
        CMembershipPanel = new CPanel(
            envoy,
            settings,
            vistas,
            "Tag.LoadFailed",
            "Scribe",
            changeSeam,
            finishSeam,
            shownSeam,
            "Tag.Vacant",
            "Tag.Unmatched");
    }

    public CPanel CMembershipPanel { get; }

    internal void LMembershipVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _cMembershipRoll = roll;
        CMembershipPanel.CPanelVistaRestore(vista);
    }

    internal void LMembershipObserverAttach(Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(marshal);

        CPanel panel = CMembershipPanel;
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => panel.CPanelEntryResonate(bulletin)));
        panel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => marshal(panel.CPanelAperture.CApertureRowsResonate));
        panel.CPanelAperture.CApertureChosenAttach(CSubject.CSubjectEntry, _ => marshal(panel.CPanelDraftResonate));
    }

    public IReadOnlyList<CVistaRow> CMembershipRowsRead()
    {
        try
        {
            return _cMembershipVistaPort.LEngineEntryFind(
                    _cMembershipRoll, CMembershipPanel.CPanelAperture.CApertureVista)
                .Select(CCatalog.LCatalogRowRead)
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
        return _cMembershipVistaPort.LEngineFileRead(CMembershipPanel.CPanelAperture.CApertureVista);
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
                CMembershipPanel.CPanelAperture.CApertureVista,
                CPortrait.LPortraitLabelRead(_cMembershipSettingsPort),
                chosen));
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
            LMembershipFileRead,
            (file, medium) => _cMembershipPortraitPort.LEnginePortraitExport(
                CMembershipPanel.CPanelAperture.CApertureVista,
                file,
                medium,
                CPortrait.LPortraitLabelRead(_cMembershipSettingsPort)));
    }
}
