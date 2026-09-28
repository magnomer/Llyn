using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CWing
{
    private readonly CAtelier _cWingAtelier;

    private readonly CEnvoy _cWingEnvoy;

    private LVista? _cWingVista;

    internal CWing(CAtelier atelier, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(envoy);

        _cWingAtelier = atelier;
        _cWingEnvoy = envoy;
        CWingDisplay = new LDisplay(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryPort,
            atelier.CAtelierPhonologyPort,
            atelier.CAtelierSettingsPort,
            atelier.CAtelierMediaPort,
            envoy);
    }

    public static CWing CWingCreate(CAtelier atelier, CEnvoy envoy)
    {
        return new CWing(atelier, envoy);
    }

    public event Action<CBulletin>? CWingChanged;

    public event Action? CWingLoaded;

    public LDisplay CWingDisplay { get; }

    public bool CWingFiltered => _cWingVista?.LVistaFiltered ?? false;

    public bool CWingQueried => _cWingVista?.LVistaQueried ?? false;

    public CCatalogOrder CWingOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cWingVista));

    public CCatalogFilter CWingFilter => CPanel.CPanelFilterRead(LVista.LVistaFilterRead(_cWingVista));

    public void CWingVistaRestore(string tab)
    {
        ArgumentNullException.ThrowIfNull(tab);

        _cWingVista = _cWingAtelier.CAtelierVistaStart(
            tab, CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword, true);
        CWingDisplay.LDisplayVistaRestore(_cWingVista);
        CWingDisplay.LDisplayObserverAttach(CSubject.CSubjectVista, LWingBulletinSend);
        CWingDisplay.LDisplayObserverAttach(CSubject.CSubjectEntry, LWingBulletinSend);
        CWingDisplay.LDisplayObserverAttach(CSubject.CSubjectReflex, LWingBulletinSend);
        CWingDisplay.LDisplayObserverAttach(CSubject.CSubjectSettings, LWingBulletinSend);
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

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public void CWingFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cWingVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public void CWingEntrySelect(long? id)
    {
        _cWingVista?.LVistaSelect(id);
    }

    public void CWingEntryRestore(long? id)
    {
        if (id is long shown)
        {
            LWingEntryLoad(shown);
        }
    }

    public void CWingEntryOpen(long id)
    {
        LWingEntryLoad(id);
        _cWingVista?.LVistaSideSave();
    }

    public IReadOnlyList<CVistaRow> CWingRowsRead()
    {
        return _cWingVista is LVista vista
            ? _cWingAtelier.CAtelierEntryPort.LEngineEntryFind(vista).Select(CPanel.CPanelRowRead).ToList()
            : [];
    }

    public IReadOnlyList<string> CWingLanguageRead()
    {
        return _cWingAtelier.CAtelierSettingsPort.LEngineLanguageRead();
    }

    private void LWingEntryLoad(long id)
    {
        try
        {
            CWingDisplay.LDisplayEntryLoad(id);
        }
        catch (Exception exception)
        {
            _cWingEnvoy.CEnvoyFailureShow("Duplex.LoadFailed", exception);
            return;
        }

        CWingLoaded?.Invoke();
    }

    private void LWingBulletinSend(CBulletin bulletin)
    {
        CWingChanged?.Invoke(bulletin);
    }
}
