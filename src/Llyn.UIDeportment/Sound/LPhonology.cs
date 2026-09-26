using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LPhonology
{
    private readonly LPhonologyPort _lPhonologyPort;

    private readonly LPortraitPort _lPortraitPort;

    private LVista? _lPhonologyVista;

    private int _lPhonologyCount;

    public LPhonology(
        LPhonologyPort phonology,
        LPortraitPort portraits,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(phonology);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);

        _lPhonologyPort = phonology;
        _lPortraitPort = portraits;
        LPhonologyEditor = editor;
        LPhonologyPanel = new LPanel(
            "Sound.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LPhonologyPanel.LPanelCleared += LPhonologyEditorClear;
        LPhonologyPanel.LPanelEdited += LPhonologyEditorOpen;
        LPhonologyPanel.LPanelDraftChanged += editor.LEditorLectern.LLecternDraftShow;
        LPhonologyPanel.LPanelCleared += editor.LEditorLectern.LLecternClear;
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

    public void LPhonologyVistaRestore(LVista vista)
    {
        _lPhonologyVista = vista;
        LPhonologyPanel.LPanelVistaRestore(vista);
        LPhonologyEditor.LEditorVistaRestore(vista);
    }

    public IReadOnlyList<LCatalogPronunciation> LPhonologyRowsRead()
    {
        IReadOnlyList<LCatalogPronunciation> rows =
            _lPhonologyVista is LVista vista ? _lPhonologyPort.LEnginePronunciationFind(vista) : [];
        _lPhonologyCount = rows.Count;
        return rows;
    }

    public void LPhonologyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lPhonologyVista?.LVistaQuerySet(query);
    }

    public void LPhonologyOrderSet(LCatalogOrder? order)
    {
        if (_lPhonologyVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LPhonologyFilterSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lPhonologyVista?.LVistaFilterSet(filter);
    }

    public Task LPhonologyPortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        if (_lPhonologyVista is not LVista vista)
        {
            return Task.CompletedTask;
        }

        return _lPortraitPort.LEnginePortraitPrint(vista, label, ticket);
    }

    public void LPhonologyVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LPhonologyVistaRestore(
            window.LWindowVistaStart("phonology", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public string LPhonologyFileRead()
    {
        return LVista.LVistaFileRead(LPhonologyPanel.LPanelVista);
    }

    public Task LPhonologyPortraitExport(string path, LPortraitMedium format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(LPhonologyPanel.LPanelVista, path, format, label);
    }
}
