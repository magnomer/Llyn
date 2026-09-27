using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LPhonology
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lPhonologyVista;

    private int _lPhonologyCount;

    internal LPhonology(
        LPhonologyPort phonology,
        LPortraitPort portraits,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        LPhonologyEditor = editor;
        LPhonologyPanel = new LPanel(
            "Sound.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.CDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LPhonologyPanel.LPanelCleared += LPhonologyEditorClear;
        LPhonologyPanel.LPanelEdited += LPhonologyEditorOpen;
        LPhonologyPanel.LPanelDraftChanged += lectern.LLecternDraftShow;
        LPhonologyPanel.LPanelCleared += lectern.LLecternClear;
    }

    public LEditor LPhonologyEditor { get; }

    public LPanel LPhonologyPanel { get; }

    private void LPhonologyEditorClear()
    {
        LPhonologyEditor.LEditorOpen(null);
    }

    private void LPhonologyEditorOpen(long id)
    {
        LPhonologyEditor.LEditorOpen(id);
    }

    public bool LPhonologyInventoryEmpty => _lPhonologyCount == 0;

    public bool LPhonologyFilterActive => _lPhonologyVista?.LVistaFiltered ?? false;

    internal void LPhonologyVistaRestore(LVista vista)
    {
        _lPhonologyVista = vista;
        LPhonologyPanel.LPanelVistaRestore(vista);
        LPhonologyEditor.LEditorVistaRestore(vista);
    }

    public IReadOnlyList<CCatalogPronunciation> LPhonologyRowsRead()
    {
        IReadOnlyList<CCatalogPronunciation> rows = _lPhonologyVista is LVista vista
            ? LPhonologyPronunciationRead(_lPhonologyPort.LEnginePronunciationFind(vista))
            : [];
        _lPhonologyCount = rows.Count;
        return rows;
    }

    internal static IReadOnlyList<CCatalogPronunciation> LPhonologyPronunciationRead(
        IReadOnlyList<LCatalogPronunciation> rows)
    {
        return LSplice.LSpliceBuild(
            rows,
            static row => new CCatalogPronunciation(
                new CVistaRow(
                    row.LCatalogPronunciationEntry.LEntryId,
                    row.LCatalogPronunciationEntry.LEntryHeadword,
                    row.LCatalogPronunciationEntry.LEntryLanguage,
                    row.LCatalogPronunciationEpithet ?? string.Empty,
                    row.LCatalogPronunciationName,
                    row.LCatalogPronunciationChosen),
                row.LCatalogPronunciationSound));
    }

    public void LPhonologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lPhonologyVista?.LVistaQuerySet(query);
    }

    public void LPhonologyOrderSet(CCatalogOrder? order)
    {
        if (_lPhonologyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LPhonologyFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lPhonologyVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public Task LPhonologyPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        if (_lPhonologyVista is not LVista vista)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            vista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    internal void LPhonologyVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LPhonologyVistaRestore(
            atelier.CAtelierVistaStart(
                "phonology", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public string LPhonologyFileRead()
    {
        return LVista.LVistaFileRead(LPhonologyPanel.LPanelVista);
    }

    public Task LPhonologyPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            LPhonologyPanel.LPanelVista,
            path,
            QPortrait.QPortraitMediumRead(format),
            QPortrait.QPortraitLabelRead(label));
    }
}
