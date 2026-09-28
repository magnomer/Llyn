using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LQuotation
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lQuotationRoll;

    private LVista? _lQuotationVista;

    internal LQuotation(
        LEntryPort entries,
        LPortraitPort portraits,
        Func<bool> changeSeam,
        Func<bool> shownSeam,
        CEnvoy envoy,
        Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        LQuotationPanel = new CPanel(
            envoy, "List.LoadFailed", null,
            changeSeam, finishSeam, shownSeam);
    }

    public CPanel LQuotationPanel { get; }

    internal void LQuotationVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lQuotationRoll = roll;
        _lQuotationVista = vista;
        LQuotationPanel.CPanelVistaRestore(vista);
    }

    public string LQuotationEmptyRead(string? dredge)
    {
        return string.IsNullOrWhiteSpace(dredge) ? "Example.Vacant" : "Example.Unmatched";
    }

    public void LQuotationDredgeSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lQuotationVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> LQuotationRowsRead()
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineEntryFind(_lQuotationRoll, _lQuotationVista), CPanel.CPanelRowRead);
    }

    public string LQuotationFileRead()
    {
        return LVista.LVistaFileRead(_lQuotationVista);
    }

    public Task LQuotationPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lQuotationVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }

    public Task LQuotationPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lQuotationVista, path, CPortrait.CPortraitMediumRead(format), CPortrait.CPortraitLabelRead(label));
    }
}
