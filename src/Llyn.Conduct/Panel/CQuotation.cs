using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CQuotation
{
    private readonly LEntryPort _cQuotationEntryPort;

    private readonly LPortraitPort _cQuotationPortraitPort;

    private LVista? _cQuotationRoll;

    private LVista? _cQuotationVista;

    internal CQuotation(
        LEntryPort entries,
        LPortraitPort portraits,
        CEnvoy envoy,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);

        _cQuotationEntryPort = entries;
        _cQuotationPortraitPort = portraits;
        CQuotationPanel = new CPanel(envoy, "List.LoadFailed", null, changeSeam, finishSeam, shownSeam);
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
        return _cQuotationEntryPort.LEngineEntryFind(_cQuotationRoll, _cQuotationVista)
            .Select(CPanel.CPanelRowRead)
            .ToList();
    }

    internal string LQuotationFileRead()
    {
        return LVista.LVistaFileRead(_cQuotationVista);
    }

    internal Task LQuotationPortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            chosen => _cQuotationPortraitPort.LEnginePortraitPrint(
                _cQuotationVista, CPortrait.LPortraitLabelRead(settings), chosen));
    }

    internal Task LQuotationPortraitExport(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitFileExport(
            envoy,
            LQuotationFileRead(),
            (file, medium) => _cQuotationPortraitPort.LEnginePortraitExport(
                _cQuotationVista, file, medium, CPortrait.LPortraitLabelRead(settings)));
    }
}
