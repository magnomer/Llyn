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
        CEnvoy envoy)
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

        LFavoritePanel = new CPanel(
            envoy, "Favorite.LoadFailed", "Scribe",
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck, editor.LEditorStudio.CEditorFinish,
            shownSeam);
        LFavoritePanel.CPanelCleared += LFavoriteEditorClear;
        LFavoritePanel.CPanelEdited += LFavoriteEditorOpen;
        LFavoritePanel.CPanelDraftChanged += lectern.LLecternDraftShow;
        LFavoritePanel.CPanelCleared += lectern.LLecternClear;
    }

    public LEditor LFavoriteEditor { get; }

    public CPanel LFavoritePanel { get; }

    private void LFavoriteEditorClear()
    {
        LFavoriteEditor.LEditorStudio.CEditorEntryOpen(null);
    }

    private void LFavoriteEditorOpen(long id)
    {
        LFavoriteEditor.LEditorStudio.CEditorEntryOpen(id);
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
        LFavoritePanel.CPanelVistaRestore(vista);
        LFavoriteEditor.LEditorStudio.CEditorVistaRestore(vista);
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

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LFavoriteStrainerSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lFavoriteVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public IReadOnlyList<CVistaRow> LFavoriteRowsRead()
    {
        return _lFavoriteVista is LVista vista
            ? LSplice.LSpliceBuild(_lEntryPort.LEngineFavoriteFind(vista), CPanel.CPanelRowRead)
            : [];
    }

    public IReadOnlyList<string> LFavoriteLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LFavoritePortraitPrint(CPortraitLabel label, CPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(
            _lFavoriteVista, CPortrait.CPortraitLabelRead(label), CPortrait.CPortraitTicketRead(ticket));
    }

    internal void LFavoriteVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LFavoriteVistaRestore(
            atelier.CAtelierVistaStart(
                "favorite", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public long LFavoriteVoyageRead()
    {
        return _lFavoriteVista?.LVistaChosen ?? 0;
    }

    public CCatalogOrder LFavoriteOrder =>
        CPanel.CPanelOrderRead(_lFavoriteVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);

    public CCatalogFilter LFavoriteFilter =>
        CPanel.CPanelFilterRead(_lFavoriteVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);

    public string LFavoriteFileRead()
    {
        return LVista.LVistaFileRead(_lFavoriteVista);
    }

    public Task LFavoritePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            _lFavoriteVista, path, CPortrait.CPortraitMediumRead(format), CPortrait.CPortraitLabelRead(label));
    }
}
