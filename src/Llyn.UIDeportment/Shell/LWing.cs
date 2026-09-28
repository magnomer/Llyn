using System;
using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LWing
{
    private readonly CAtelier _lWingAtelier;

    private readonly LDisplay _lWingDisplay;

    private LVista? _lWingVista;

    internal LWing(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        _lWingAtelier = atelier;
        _lWingDisplay = new LDisplay(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryPort,
            atelier.CAtelierPhonologyPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierMediaPort);
        LWingLectern = new LLectern(_lWingDisplay);
    }

    public event Action<string, Exception>? LWingFailed;

    public event Action? LWingLoaded;

    public LLectern LWingLectern { get; }

    public bool LWingFiltered => _lWingVista?.LVistaFiltered ?? false;

    public bool LWingQueried => _lWingVista?.LVistaQueried ?? false;

    private void LWingVistaRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        _lWingVista = vista;
        _lWingDisplay.LDisplayVistaRestore(vista);
    }

    public void LWingQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lWingVista?.LVistaQuerySet(query);
    }

    public void LWingOrderSet(CCatalogOrder? order)
    {
        if (_lWingVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LPanel.LPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void LWingSieveSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lWingVista?.LVistaFilterSet(LPanel.LPanelFilterRead(filter));
    }

    public void LWingSelect(long? id)
    {
        _lWingVista?.LVistaSelect(id);
    }

    internal void LWingVistaRestore(CAtelier atelier, string tab)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LWingVistaRestore(
            atelier.CAtelierVistaStart(
                tab, CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword, true));
    }

    public void LWingObserverAttach(Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectVista, LWingBulletinSend);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectEntry, LWingBulletinSend);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectReflex, LWingBulletinSend);
        _lWingVista?.LVistaObserverAttach(LSubject.LSubjectSettings, LWingBulletinSend);

        void LWingBulletinSend(LBulletin bulletin)
        {
            observer(new CBulletin(bulletin.LBulletinId, bulletin.LBulletinStored));
        }
    }

    public void LWingEntryShow(long? id)
    {
        LWingLectern.LLecternClear();
        if (id is long shown)
        {
            LWingEntryLoad(shown);
        }
    }

    private void LWingEntryLoad(long id)
    {
        try
        {
            _lWingDisplay.LDisplayEntryLoad(id);
        }
        catch (Exception exception)
        {
            LWingFailed?.Invoke("Duplex.LoadFailed", exception);
            return;
        }

        LWingLoaded?.Invoke();
        LWingLectern.LLecternLoadedShow();
    }

    public void LWingEntryOpen(long id)
    {
        LWingEntryLoad(id);
        LWingEntrySave();
    }

    public CCatalogOrder LWingOrder =>
        LPanel.LPanelOrderRead(_lWingVista?.LVistaOrder ?? LCatalogOrder.LCatalogOrderHeadword);

    public CCatalogFilter LWingFilter =>
        LPanel.LPanelFilterRead(_lWingVista?.LVistaFilter ?? LCatalogFilter.LCatalogFilterEmpty);

    public IReadOnlyList<CVistaRow> LWingRowsRead()
    {
        return _lWingVista is LVista vista
            ? LSplice.LSpliceBuild(
                _lWingAtelier.CAtelierEntryPort.LEngineEntryFind(vista), CPanel.CPanelRowRead)
            : [];
    }

    public IReadOnlyList<string> LWingLanguageRead()
    {
        return _lWingAtelier.CAtelierSettingsPort.LEngineLanguageRead();
    }

    private void LWingEntrySave()
    {
        if (_lWingVista is LVista vista)
        {
            if (vista.LVistaLeft)
            {
                _lWingAtelier.CAtelierSettingsPort.LEngineLeftSave(vista.LVistaChosen);
                return;
            }
        }

        _lWingAtelier.CAtelierSettingsPort.LEngineRightSave(_lWingVista?.LVistaChosen);
    }
}
