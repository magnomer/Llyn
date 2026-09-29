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
        LSettingsPort settings,
        CEnvoy envoy,
        Func<bool> changeSeam,
        Func<bool, bool> finishSeam,
        Func<bool> shownSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);

        _cOccurrenceEntryPort = entries;
        _cOccurrencePortraitPort = portraits;
        COccurrencePanel = new CPanel(envoy, settings, "List.LoadFailed", null, changeSeam, finishSeam, shownSeam);
    }

    public CPanel COccurrencePanel { get; }

    public string COccurrenceEmptyKey =>
        _cOccurrenceVista?.LVistaQueried ?? false ? "Situation.Unmatched" : "Situation.Vacant";

    internal void LOccurrenceVistaRestore(LVista roll, LVista vista)
    {
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cOccurrenceVista?.LVistaQuery ?? string.Empty);
        _cOccurrenceRoll = roll;
        _cOccurrenceVista = vista;
        COccurrencePanel.CPanelVistaRestore(vista);
    }

    internal void LOccurrenceObserverAttach(Action<Action> marshal, Action roll, Action chosen)
    {
        ArgumentNullException.ThrowIfNull(marshal);
        ArgumentNullException.ThrowIfNull(roll);
        ArgumentNullException.ThrowIfNull(chosen);

        COccurrencePanel.CPanelObserverAttach(
            CSubject.CSubjectEntry, bulletin => marshal(() => COccurrencePanel.CPanelEntrySelect(bulletin)));
        COccurrencePanel.CPanelObserverAttach(CSubject.CSubjectEntry, _ => marshal(roll));
        COccurrencePanel.CPanelChosenAttach(CSubject.CSubjectEntry, _ => marshal(chosen));
        COccurrencePanel.CPanelObserverAttach(
            CSubject.CSubjectVista, _ => marshal(COccurrencePanel.CPanelRowsResonate));
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

    internal string LOccurrenceFileRead()
    {
        return LVista.LVistaFileRead(_cOccurrenceVista);
    }

    internal Task LOccurrencePortraitPrint(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitTicketPrint(
            envoy,
            settings,
            chosen => _cOccurrencePortraitPort.LEnginePortraitPrint(
                _cOccurrenceVista, CPortrait.LPortraitLabelRead(settings), chosen));
    }

    internal Task LOccurrencePortraitExport(CEnvoy envoy, LSettingsPort settings)
    {
        return CPortrait.LPortraitFileExport(
            envoy,
            settings,
            LOccurrenceFileRead(),
            (file, medium) => _cOccurrencePortraitPort.LEnginePortraitExport(
                _cOccurrenceVista, file, medium, CPortrait.LPortraitLabelRead(settings)));
    }
}
