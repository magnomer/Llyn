using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class COccurrence
{
    private readonly LEntryPort _cOccurrenceEntryPort;

    private readonly LPortraitPort _cOccurrencePortraitPort;

    private LVista? _cOccurrenceRoll;

    private LVista? _cOccurrenceVista;

    internal COccurrence(
        LEntryPort entries,
        LPortraitPort portraits,
        CEnvoy envoy,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);

        _cOccurrenceEntryPort = entries;
        _cOccurrencePortraitPort = portraits;
        COccurrencePanel = new CPanel(envoy, "List.LoadFailed", null, changeSeam, finishSeam, shownSeam);
    }

    public CPanel COccurrencePanel { get; }

    public string COccurrenceEmptyKey =>
        _cOccurrenceVista?.LVistaQueried ?? false ? "Situation.Unmatched" : "Situation.Vacant";

    internal void LOccurrenceVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        _cOccurrenceRoll = roll;
        _cOccurrenceVista = vista;
        COccurrencePanel.CPanelVistaRestore(vista);
    }

    public void COccurrenceQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cOccurrenceVista?.LVistaQuerySet(query);
    }

    public IReadOnlyList<CVistaRow> COccurrenceRowsRead()
    {
        return _cOccurrenceEntryPort.LEngineEntryFind(_cOccurrenceRoll, _cOccurrenceVista)
            .Select(CPanel.CPanelRowRead)
            .ToList();
    }

    public string COccurrenceFileRead()
    {
        return LVista.LVistaFileRead(_cOccurrenceVista);
    }

    internal Task LOccurrencePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _cOccurrencePortraitPort.LEnginePortraitPrint(
            _cOccurrenceVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }

    internal Task LOccurrencePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _cOccurrencePortraitPort.LEnginePortraitExport(
            _cOccurrenceVista, path, CPortrait.CPortraitMediumRead(format), CPortrait.CPortraitLabelRead(label));
    }
}
