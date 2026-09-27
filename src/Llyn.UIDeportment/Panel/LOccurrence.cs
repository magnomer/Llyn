using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LOccurrence
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lOccurrenceRoll;

    private LVista? _lOccurrenceVista;

    internal LOccurrence(
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
        LOccurrencePanel = new LPanel(
            "List.LoadFailed", "Scribe.DeleteFailed",
            changeSeam, shownSeam, leaveSeam, static () => false);
    }

    public LPanel LOccurrencePanel { get; }

    internal void LOccurrenceVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lOccurrenceRoll = roll;
        _lOccurrenceVista = vista;
        LOccurrencePanel.LPanelVistaRestore(vista);
    }

    public void LOccurrenceSortieSet(string inquest)
    {
        ArgumentNullException.ThrowIfNull(inquest);

        _lOccurrenceVista?.LVistaQuerySet(inquest);
    }

    public IReadOnlyList<CVistaRow> LOccurrenceRowsRead()
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineEntryFind(_lOccurrenceRoll, _lOccurrenceVista), LPanel.LPanelRowRead);
    }

    public string LOccurrenceEmptyRead(string? sortie)
    {
        return string.IsNullOrWhiteSpace(sortie) ? "Situation.Vacant" : "Situation.Unmatched";
    }

    public string LOccurrenceFileRead()
    {
        return LVista.LVistaFileRead(_lOccurrenceVista);
    }

    public Task LOccurrencePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lOccurrenceVista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    public Task LOccurrencePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lOccurrenceVista, path, QPortrait.QPortraitMediumRead(format), QPortrait.QPortraitLabelRead(label));
    }
}
