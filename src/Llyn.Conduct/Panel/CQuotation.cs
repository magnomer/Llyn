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

    public string CQuotationEmptyKey =>
        _cQuotationVista?.LVistaQueried ?? false ? "Example.Unmatched" : "Example.Vacant";

    internal void LQuotationVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _cQuotationRoll = roll;
        _cQuotationVista = vista;
        CQuotationPanel.CPanelVistaRestore(vista);
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
                .Select(CPanel.CPanelRowRead)
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cQuotationEnvoy, _cQuotationSettingsPort, "Example.LoadFailed", exception);
            return [];
        }
    }

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

    internal Task LQuotationPortraitExport(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitFileExport(
            envoy,
            settings,
            LQuotationFileRead(),
            (file, medium) => _cQuotationPortraitPort.LEnginePortraitExport(
                _cQuotationVista, file, medium, CPortrait.LPortraitLabelRead(settings)));
    }
}
