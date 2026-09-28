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
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        LPhonologyEditor = editor;
        LPhonologyPanel = new CPanel(
            envoy, "Sound.LoadFailed", "Scribe",
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck, editor.LEditorStudio.CEditorFinish,
            shownSeam);
        LPhonologyPanel.CPanelCleared += LPhonologyEditorClear;
        LPhonologyPanel.CPanelEdited += LPhonologyEditorOpen;
        LPhonologyPanel.CPanelDraftChanged += lectern.LLecternDraftShow;
        LPhonologyPanel.CPanelCleared += lectern.LLecternClear;
    }

    public LEditor LPhonologyEditor { get; }

    public CPanel LPhonologyPanel { get; }

    private void LPhonologyEditorClear()
    {
        LPhonologyEditor.LEditorStudio.CEditorEntryOpen(null);
    }

    private void LPhonologyEditorOpen(long id)
    {
        LPhonologyEditor.LEditorStudio.CEditorEntryOpen(id);
    }

    public bool LPhonologyInventoryEmpty => _lPhonologyCount == 0;

    public bool LPhonologyFilterActive => _lPhonologyVista?.LVistaFiltered ?? false;

    internal void LPhonologyVistaRestore(LVista vista)
    {
        _lPhonologyVista = vista;
        LPhonologyPanel.CPanelVistaRestore(vista);
        LPhonologyEditor.LEditorStudio.CEditorVistaRestore(vista);
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

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LPhonologyFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lPhonologyVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public Task LPhonologyPortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        if (_lPhonologyVista is not LVista vista)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(
            vista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
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
        return LVista.LVistaFileRead(LPhonologyPanel.CPanelVista);
    }

    public Task LPhonologyPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            LPhonologyPanel.CPanelVista,
            path,
            CPortrait.CPortraitMediumRead(format),
            CPortrait.CPortraitLabelRead(label));
    }
}
