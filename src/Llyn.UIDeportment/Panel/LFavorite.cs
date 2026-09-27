using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LFavorite
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lFavoriteVista;

    internal LFavorite(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LFavoriteEditor = editor;

        LFavoritePanel = new LPanel(
            "Favorite.LoadFailed", "Scribe.DeleteFailed",
            editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LFavoritePanel.LPanelCleared += LFavoriteEditorClear;
        LFavoritePanel.LPanelEdited += LFavoriteEditorOpen;
        LFavoritePanel.LPanelDraftChanged += lectern.LLecternDraftShow;
        LFavoritePanel.LPanelCleared += lectern.LLecternClear;
    }

    public LEditor LFavoriteEditor { get; }

    public LPanel LFavoritePanel { get; }

    private void LFavoriteEditorClear()
    {
        LFavoriteEditor.LEditorOpen(null);
    }

    private void LFavoriteEditorOpen(long id)
    {
        LFavoriteEditor.LEditorOpen(id);
    }

    public bool LFavoriteFiltered => _lFavoriteVista?.LVistaFiltered ?? false;

    private bool LFavoriteGraspOrdered => _lFavoriteVista?.LVistaOrderMatch(LCatalogOrder.LCatalogOrderGrasp) ?? false;

    public void LFavoriteGraspApply(Action find)
    {
        ArgumentNullException.ThrowIfNull(find);

        if (LFavoriteGraspOrdered)
        {
            find();
        }
    }

    internal void LFavoriteVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lFavoriteVista = vista;
        LFavoritePanel.LPanelVistaRestore(vista);
        LFavoriteEditor.LEditorVistaRestore(vista);
    }

    public void LFavoriteRecallSet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lFavoriteVista?.LVistaQuerySet(query);
    }

    public void LFavoriteSeriesSet(CCatalogOrder? order)
    {
        if (_lFavoriteVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LFavoriteStrainerSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lFavoriteVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public IReadOnlyList<CVistaRow> LFavoriteRowsRead()
    {
        return _lFavoriteVista is LVista vista
            ? LSplice.LSpliceBuild(_lEntryPort.LEngineFavoriteFind(vista), LPanel.LPanelRowRead)
            : [];
    }

    public IReadOnlyList<string> LFavoriteLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LFavoritePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lFavoriteVista, QPortrait.QPortraitLabelRead(label), QPortrait.QPortraitTicketRead(ticket));
    }

    internal void LFavoriteVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LFavoriteVistaRestore(
            window.LWindowVistaStart("favorite", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public long LFavoriteVoyageRead()
    {
        return _lFavoriteVista?.LVistaChosen ?? 0;
    }

    public CCatalogOrder LFavoriteOrder =>
        LPanel.LPanelOrderRead(_lFavoriteVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);

    public CCatalogFilter LFavoriteFilter =>
        LPanel.LPanelFilterRead(_lFavoriteVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);

    public string LFavoriteFileRead()
    {
        return LVista.LVistaFileRead(_lFavoriteVista);
    }

    public Task LFavoritePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lFavoriteVista, path, QPortrait.QPortraitMediumRead(format), QPortrait.QPortraitLabelRead(label));
    }
}
