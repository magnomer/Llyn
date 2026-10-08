using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class COccurrence
{
    private readonly CEnvoy _cOccurrenceEnvoy;

    private readonly LVistaPort _cOccurrenceVistaPort;

    private readonly LPortraitPort _cOccurrencePortraitPort;

    private readonly LSettingsPort _cOccurrenceSettingsPort;

    private LVista? _cOccurrenceRoll;

    internal COccurrence(
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

        _cOccurrenceEnvoy = envoy;
        _cOccurrenceVistaPort = vistas;
        _cOccurrenceSettingsPort = settings;
        _cOccurrencePortraitPort = portraits;
        COccurrencePanel = new CPanel(
            envoy,
            settings,
            vistas,
            "List.LoadFailed",
            null,
            changeSeam,
            finishSeam,
            shownSeam,
            "Situation.Vacant",
            "Situation.Unmatched");
    }

    public CPanel COccurrencePanel { get; }

    internal void LOccurrenceVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _cOccurrenceRoll = roll;
        COccurrencePanel.CPanelVistaRestore(vista);
    }

    internal void LOccurrenceObserverAttach(Action<Action> marshal, Action roll, Action chosen)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(chosen);

        COccurrencePanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => COccurrencePanel.CPanelEntrySelect(bulletin)));
        COccurrencePanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectEntry, _ => marshal(roll));
        COccurrencePanel.CPanelAperture.CApertureChosenAttach(CSubject.CSubjectEntry, _ => marshal(chosen));
        COccurrencePanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => marshal(COccurrencePanel.CPanelAperture.CApertureRowsResonate));
    }

    public IReadOnlyList<CVistaRow> COccurrenceRowsRead()
    {
        try
        {
            return _cOccurrenceVistaPort.LEngineEntryFind(
                    _cOccurrenceRoll, COccurrencePanel.CPanelAperture.CApertureVista)
                .Select(CCatalog.LCatalogRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cOccurrenceEnvoy, _cOccurrenceSettingsPort, "Situation.LoadFailed", exception);
            return [];
        }
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> COccurrenceRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cOccurrenceEnvoy, _cOccurrenceSettingsPort, "Situation.LoadFailed", store, COccurrenceRowsRead);

    internal string LOccurrenceFileRead()
    {
        return _cOccurrenceVistaPort.LEngineFileRead(COccurrencePanel.CPanelAperture.CApertureVista);
    }

    internal Task LOccurrencePortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cOccurrencePortraitPort.LEnginePortraitPrint(
                COccurrencePanel.CPanelAperture.CApertureVista, CPortrait.LPortraitLabelRead(settings), chosen));
    }

    internal Task LOccurrencePortraitExport(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitFileExport(
            envoy,
            settings,
            LOccurrenceFileRead,
            (file, medium) => _cOccurrencePortraitPort.LEnginePortraitExport(
                COccurrencePanel.CPanelAperture.CApertureVista, file, medium, CPortrait.LPortraitLabelRead(settings)));
    }
}
