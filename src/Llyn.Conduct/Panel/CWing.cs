using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CWing
{
    private readonly CAtelier _cWingAtelier;

    private readonly CEnvoy _cWingEnvoy;

    private readonly bool _cWingLeft;

    private LVista? _cWingVista;

    private IReadOnlyList<CVistaRow> _cWingRows = [];

    private CWing(CAtelier atelier, CEnvoy envoy, bool left)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cWingAtelier = atelier;
        _cWingEnvoy = envoy;
        _cWingLeft = left;
        CWingDisplay = new CDisplay(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryPort,
            atelier.CAtelierPhonologyPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierMediaPort,
            envoy,
            atelier.CAtelierLedger.LLedgerRepaint);
        CWingDisplay.LDisplayNavigationAttach(atelier.CAtelierNavigation, atelier.CAtelierMention);
        LWingVistaRestore();
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LWingVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LWingClose);
        atelier.CAtelierWorkspace.LWorkspaceStateOpened += LWingEntryRestore;
    }

    public static CWing CWingCreate(CAtelier atelier, CEnvoy envoy, bool left)
    {
        return new CWing(atelier, envoy, left);
    }

    public event Action<CBulletin>? CWingChanged;

    public event Action? CWingLoaded;

    public event Action? CWingRowsChanged;

    public CDisplay CWingDisplay { get; }

    public bool CWingFiltered => _cWingVista?.LVistaFiltered ?? false;

    public bool CWingQueried => _cWingVista?.LVistaQueried ?? false;

    public bool CWingEmpty => CWingQueried && _cWingRows.Count == 0;

    public CCatalogOrder CWingOrder => CCatalog.LCatalogOrderRead(LVista.LVistaOrderRead(_cWingVista));

    public CCatalogFilter CWingFilter => CCatalog.LCatalogFilterRead(LVista.LVistaFilterRead(_cWingVista));

    private void LWingVistaRestore()
    {
        _cWingVista = _cWingAtelier.CAtelierVistaStart(
            _cWingLeft ? "left" : "right", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword, true);
        _cWingRows = [];
        CWingDisplay.LDisplayVistaRestore(_cWingVista);
        CWingDisplay.LDisplayRule.LDisplayObserverAttach(CSubject.CSubjectVista, LWingBulletinSend);
        CWingDisplay.LDisplayRule.LDisplayObserverAttach(CSubject.CSubjectEntry, LWingBulletinSend);
        CWingDisplay.LDisplayRule.LDisplayObserverAttach(CSubject.CSubjectReflex, LWingBulletinSend);
        CWingDisplay.LDisplayRule.LDisplayObserverAttach(CSubject.CSubjectSettings, LWingBulletinSend);
    }

    public void CWingQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cWingVista?.LVistaQuerySet(query);
    }

    public void CWingOrderSet(CCatalogOrder? order)
    {
        if (_cWingVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(CCatalog.LCatalogOrderRead(order) ?? vista.LVistaOrder);
    }

    public void CWingFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cWingVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public long? CWingRowMove(bool down)
    {
        if (_cWingVista is not LVista vista || _cWingRows.Count == 0)
        {
            return null;
        }

        int place = -1;
        for (int index = 0; index < _cWingRows.Count; index++)
        {
            if (_cWingRows[index].CVistaRowId == vista.LVistaChosen)
            {
                place = index;
                break;
            }
        }

        place = down ? Math.Min(place + 1, _cWingRows.Count - 1) : Math.Max(place - 1, 0);
        long id = _cWingRows[place].CVistaRowId;
        vista.LVistaSelect(id);
        CWingRowsChanged?.Invoke();
        return id;
    }

    private void LWingEntryRestore(CWorkspaceState state)
    {
        CWingDisplay.CDisplayEntryClose();
        if ((_cWingLeft ? state.CWorkspaceStateLeft : state.CWorkspaceStateRight) is long shown)
        {
            LWingEntryLoad(shown);
        }
    }

    public bool CWingEntryOpen(long? id)
    {
        if (id is not long chosen)
        {
            return false;
        }

        LWingEntryLoad(chosen);
        _cWingVista?.LVistaSideSave();
        return true;
    }

    public IReadOnlyList<CVistaRow> CWingRowsRead()
    {
        try
        {
            _cWingRows = _cWingVista is LVista vista
                ? _cWingAtelier.CAtelierEntryPort.LEngineEntryFind(vista).Select(CCatalog.LCatalogRowRead).ToList()
                : [];
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cWingEnvoy, _cWingAtelier.CAtelierSettingsPort, "Duplex.LoadFailed", exception);
            _cWingRows = [];
        }

        return _cWingRows;
    }

    private void LWingEntryLoad(long id)
    {
        try
        {
            CWingDisplay.LDisplayEntryOpen(CWingDisplay.LDisplayRule.LDisplayEntryLoad(id));
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cWingEnvoy, _cWingAtelier.CAtelierSettingsPort, "Duplex.LoadFailed", exception);
            return;
        }

        CWingLoaded?.Invoke();
    }

    private void LWingClose()
    {
        CWingDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    private void LWingBulletinSend(CBulletin bulletin)
    {
        CWingChanged?.Invoke(bulletin);
    }
}
