using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CQuotation
{
    private readonly CEnvoy _cQuotationEnvoy;

    private readonly LVistaPort _cQuotationVistaPort;

    private readonly LPortraitPort _cQuotationPortraitPort;

    private readonly LSettingsPort _cQuotationSettingsPort;

    private LVista? _cQuotationRoll;

    internal CQuotation(
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

        _cQuotationEnvoy = envoy;
        _cQuotationVistaPort = vistas;
        _cQuotationSettingsPort = settings;
        _cQuotationPortraitPort = portraits;
        CQuotationPanel = new CPanel(
            envoy,
            settings,
            vistas,
            "List.LoadFailed",
            null,
            changeSeam,
            finishSeam,
            shownSeam,
            "Example.Vacant",
            "Example.Unmatched");
    }

    public CPanel CQuotationPanel { get; }

    public bool CQuotationShown => CQuotationPanel.CPanelModeEnabled && !CQuotationPanel.CPanelEditing;

    internal void LQuotationVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _cQuotationRoll = roll;
        CQuotationPanel.CPanelVistaRestore(vista);
    }

    internal void LQuotationObserverAttach(Action<Action> marshal, Action roll, Action chosen)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(chosen);

        CQuotationPanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => CQuotationPanel.CPanelEntrySelect(bulletin)));
        CQuotationPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectEntry, _ => marshal(roll));
        CQuotationPanel.CPanelAperture.CApertureChosenAttach(CSubject.CSubjectEntry, _ => marshal(chosen));
        CQuotationPanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => marshal(CQuotationPanel.CPanelAperture.CApertureRowsResonate));
    }

    public IReadOnlyList<CVistaRow> CQuotationRowsRead()
    {
        try
        {
            return _cQuotationVistaPort.LEngineEntryFind(_cQuotationRoll, CQuotationPanel.CPanelAperture.CApertureVista)
                .Select(CCatalog.LCatalogRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cQuotationEnvoy, _cQuotationSettingsPort, "Example.LoadFailed", exception);
            return [];
        }
    }

    public Task<CEnsignSheet<IReadOnlyList<CVistaRow>>> CQuotationRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cQuotationEnvoy, _cQuotationSettingsPort, "Example.LoadFailed", store, CQuotationRowsRead);

    internal string LQuotationFileRead()
    {
        return _cQuotationVistaPort.LEngineFileRead(CQuotationPanel.CPanelAperture.CApertureVista);
    }

    internal Task LQuotationPortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cQuotationPortraitPort.LEnginePortraitPrint(
                CQuotationPanel.CPanelAperture.CApertureVista, CPortrait.LPortraitLabelRead(settings), chosen));
    }

    public Task CQuotationPortraitExport()
    {
        if (!CQuotationShown)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitFileExport(
            _cQuotationEnvoy,
            _cQuotationSettingsPort,
            LQuotationFileRead,
            (file, medium) => _cQuotationPortraitPort.LEnginePortraitExport(
                CQuotationPanel.CPanelAperture.CApertureVista,
                file,
                medium,
                CPortrait.LPortraitLabelRead(_cQuotationSettingsPort)));
    }
}
