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
        CEnvoy envoy,
        Func<bool, bool> finishSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        LOccurrencePanel = new CPanel(
            envoy, "List.LoadFailed", null,
            changeSeam, finishSeam, shownSeam);
    }

    public CPanel LOccurrencePanel { get; }

    internal void LOccurrenceVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _lOccurrenceRoll = roll;
        _lOccurrenceVista = vista;
        LOccurrencePanel.CPanelVistaRestore(vista);
    }

    public void LOccurrenceSortieSet(string inquest)
    {
        ArgumentNullException.ThrowIfNull(inquest);

        _lOccurrenceVista?.LVistaQuerySet(inquest);
    }

    public IReadOnlyList<CVistaRow> LOccurrenceRowsRead()
    {
        return LSplice.LSpliceBuild(
            _lEntryPort.LEngineEntryFind(_lOccurrenceRoll, _lOccurrenceVista), CPanel.CPanelRowRead);
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
            _lOccurrenceVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }

    public Task LOccurrencePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lOccurrenceVista, path, CPortrait.CPortraitMediumRead(format), CPortrait.CPortraitLabelRead(label));
    }
}
