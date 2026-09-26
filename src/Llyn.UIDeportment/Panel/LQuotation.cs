using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LQuotation
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lQuotationRoll;

    private LVista? _lQuotationVista;

    public LQuotation(
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

    public void LQuotationVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lQuotationRoll = roll;
        _lQuotationVista = vista;
        LQuotationPanel.LPanelVistaRestore(vista);
    }

    public void LQuotationDredgeSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lQuotationVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<LVistaRow> LQuotationRowsRead()
    {
        return _lEntryPort.LEngineEntryFind(_lQuotationRoll, _lQuotationVista);
    }

    public string LQuotationFileRead()
    {
        return LVista.LVistaFileRead(_lQuotationVista);
    }

    public Task LQuotationPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lQuotationVista, label, ticket);
    }

    public Task LQuotationPortraitExport(string path, LPortraitMedium format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(_lQuotationVista, path, format, label);
    }
}
