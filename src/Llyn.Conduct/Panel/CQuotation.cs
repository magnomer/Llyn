using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CQuotation
{
    private readonly CEnvoy _cQuotationEnvoy;

    private readonly LEntryPort _cQuotationEntryPort;

    private readonly LPortraitPort _cQuotationPortraitPort;

    private readonly LSettingsPort _cQuotationSettingsPort;

    private LVista? _cQuotationRoll;

    private LVista? _cQuotationVista;

    internal CQuotation(
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

        _cQuotationEnvoy = envoy;
        _cQuotationEntryPort = entries;
        _cQuotationSettingsPort = settings;
        _cQuotationPortraitPort = portraits;
        CQuotationPanel = new CPanel(envoy, settings, "List.LoadFailed", null, changeSeam, finishSeam, shownSeam);
    }

    public CPanel CQuotationPanel { get; }

    public bool CQuotationShown => CQuotationPanel.CPanelModeEnabled && !CQuotationPanel.CPanelEditing;

    public string CQuotationEmptyKey =>
        _cQuotationVista?.LVistaQueried ?? false ? "Example.Unmatched" : "Example.Vacant";

    internal void LQuotationVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cQuotationVista?.LVistaQuery ?? string.Empty);
        _cQuotationRoll = roll;
        _cQuotationVista = vista;
        CQuotationPanel.CPanelVistaRestore(vista);
    }

    internal void LQuotationObserverAttach(Action<Action> marshal, Action roll, Action chosen)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(chosen);

        CQuotationPanel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => CQuotationPanel.CPanelEntrySelect(bulletin)));
        CQuotationPanel.CPanelObserverAttach(CSubject.CSubjectEntry, _ => marshal(roll));
        CQuotationPanel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => marshal(chosen));
        CQuotationPanel.CPanelObserverAttach(
            CSubject.CSubjectVista, _ => marshal(CQuotationPanel.CPanelRowsResonate));
    }

    public void CQuotationQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cQuotationVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> CQuotationRowsRead()
    {
        try
        {
            return _cQuotationEntryPort.LEngineEntryFind(_cQuotationRoll, _cQuotationVista)
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
        return LVista.LVistaFileRead(_cQuotationVista);
    }

    internal Task LQuotationPortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cQuotationPortraitPort.LEnginePortraitPrint(
                _cQuotationVista, CPortrait.LPortraitLabelRead(settings), chosen));
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
                _cQuotationVista, file, medium, CPortrait.LPortraitLabelRead(_cQuotationSettingsPort)));
    }
}
