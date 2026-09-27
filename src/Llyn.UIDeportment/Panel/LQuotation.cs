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
        Func<bool> leaveSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        LQuotationPanel = new LPanel(
            "List.LoadFailed", "Scribe.DeleteFailed",
            changeSeam, shownSeam, leaveSeam, static () => false);
    }

    public LPanel LQuotationPanel { get; }

    internal void LQuotationVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lQuotationRoll = roll;
        _lQuotationVista = vista;
        LQuotationPanel.LPanelVistaRestore(vista);
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
            _lEntryPort.LEngineEntryFind(_lQuotationRoll, _lQuotationVista), LPanel.LPanelRowRead);
    }

    public string LQuotationFileRead()
    {
        return LVista.LVistaFileRead(_lQuotationVista);
    }

    public Task LQuotationPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lQuotationVista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    public Task LQuotationPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lQuotationVista, path, QPortrait.QPortraitMediumRead(format), QPortrait.QPortraitLabelRead(label));
    }
}
