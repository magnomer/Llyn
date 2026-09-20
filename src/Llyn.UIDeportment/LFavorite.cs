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

    public LFavorite(LEntryPort entries, LPortraitPort portraits, LSettingsPort settings)
    {
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
    }

    public LVista? LFavoriteVista => _lFavoriteVista;

    public long? LFavoriteChosen => _lFavoriteVista?.LVistaChosen;

    public bool LFavoriteFiltered => _lFavoriteVista?.LVistaFiltered ?? false;

    public bool LFavoriteGraspOrdered => _lFavoriteVista?.LVistaOrderMatch(LCatalogOrder.LCatalogOrderGrasp) ?? false;

    public void LFavoriteVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lFavoriteVista = vista;
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

    public void LFavoriteSelect(long? id)
    {
        _lFavoriteVista?.LVistaSelect(id);
    }

    public void LFavoriteScribeSet(bool editing)
    {
        _lFavoriteVista?.LVistaEditingSet(editing);
    }

    public IReadOnlyList<LVistaRow> LFavoriteRowsRead()
    {
        return _lFavoriteVista is LVista vista ? _lEntryPort.LEngineFavoriteFind(vista) : [];
    }

    public LEntryDraft? LFavoriteLoad()
    {
        return _lFavoriteVista?.LVistaLoad()?.LDraftContent;
    }

    public void LFavoriteDelete()
    {
        _lFavoriteVista?.LVistaDelete();
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

    public void LFavoriteObserverAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lFavoriteVista?.LVistaObserverAttach(subject, observer);
    }

    public void LFavoriteChosenAttach(LSubject subject, Action<LBulletin> observer)
    {
        _lFavoriteVista?.LVistaChosenAttach(subject, observer);
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
