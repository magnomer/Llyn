using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LFavorite
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private LVista? _lFavoriteVista;

    public LFavorite(
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<bool> deleteSeam)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        LFavoriteEditor = editor;

        LFavoritePanel = new LPanel(
            "Favorite.LoadFailed", editor.LEditorDesk.LDeskChangeCheck, shownSeam, leaveSeam, deleteSeam);
        LFavoritePanel.LPanelCleared += LFavoriteEditorClear;
        LFavoritePanel.LPanelEdited += LFavoriteEditorOpen;
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

    public LVista? LFavoriteVista => _lFavoriteVista;

    public bool LFavoriteFiltered => _lFavoriteVista?.LVistaFiltered ?? false;

    public bool LFavoriteGraspOrdered => _lFavoriteVista?.LVistaOrderMatch(LCatalogOrder.LCatalogOrderGrasp) ?? false;

    public void LFavoriteVistaRestore(LVista vista)
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

    public void LFavoriteSeriesSet(LCatalogOrder? order)
    {
        if (_lFavoriteVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(order ?? vista.LVistaOrder);
    }

    public void LFavoriteStrainerSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lFavoriteVista?.LVistaFilterSet(filter);
    }

    public IReadOnlyList<LVistaRow> LFavoriteRowsRead()
    {
        return _lFavoriteVista is LVista vista ? _lEntryPort.LEngineFavoriteFind(vista) : [];
    }

    public IReadOnlyList<string> LFavoriteLanguageRead()
    {
        return _lSettingsPort.LEngineLanguageRead();
    }

    public Task LFavoritePortraitPrint(LPortraitLabel label, LPressTicket ticket)
    {
        return _lPortraitPort.LEnginePortraitPrint(_lFavoriteVista, label, ticket);
    }

    public void LFavoriteVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LFavoriteVistaRestore(
            window.LWindowVistaStart("favorite", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }

    public long LFavoriteVoyageRead()
    {
        return _lFavoriteVista?.LVistaChosen ?? 0;
    }

    public LCatalogOrder LFavoriteOrder => _lFavoriteVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword;

    public LCatalogFilter LFavoriteFilter => _lFavoriteVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty;

    public string LFavoriteFileRead()
    {
        return LVista.LVistaFileRead(_lFavoriteVista);
    }

    public Task LFavoritePortraitExport(string path, LPortraitFormat format, LPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(_lFavoriteVista, path, format, label);
    }
}
