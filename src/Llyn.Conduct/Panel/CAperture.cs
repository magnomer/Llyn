using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CAperture
{
    private readonly CEnvoy _cApertureEnvoy;

    private readonly LSettingsPort _cApertureSettingsPort;

    private readonly LVistaPort _cApertureVistaPort;

    private readonly string _cApertureLoadKey;

    private readonly string _cApertureVacantKey;

    private readonly string _cApertureUnmatchedKey;

    private LVista? _cApertureVista;

    private int _cApertureCount;

    internal CAperture(
        CEnvoy envoy,
        LSettingsPort settings,
        LVistaPort vistas,
        string loadKey,
        string? vacantKey = null,
        string? unmatchedKey = null)
    {
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(vistas);
        ArgumentException.ThrowIfNullOrWhiteSpace(loadKey);

        _cApertureEnvoy = envoy;
        _cApertureSettingsPort = settings;
        _cApertureVistaPort = vistas;
        _cApertureLoadKey = loadKey;
        _cApertureVacantKey = vacantKey ?? string.Empty;
        _cApertureUnmatchedKey = unmatchedKey ?? string.Empty;
    }

    public event Action? CApertureRowsChanged;

    internal LVista? CApertureVista => _cApertureVista;

    public CCatalogOrder CApertureOrder => CCatalog.LCatalogOrderRead(LVista.LVistaOrderRead(_cApertureVista));

    public CCatalogFilter CApertureFilter => CCatalog.LCatalogFilterRead(LVista.LVistaFilterRead(_cApertureVista));

    public long? CApertureChosen => _cApertureVista?.LVistaChosen;

    public bool CApertureFiltered => _cApertureVista?.LVistaFiltered ?? false;

    internal bool CApertureNarrowed => _cApertureVista?.LVistaNarrowed ?? false;

    public bool CApertureEmpty => _cApertureCount == 0;

    public bool CApertureQueried => _cApertureVista?.LVistaQueried ?? false;

    public string CApertureKey => CApertureQueried ? _cApertureUnmatchedKey : _cApertureVacantKey;

    public void CApertureQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cApertureVista?.LVistaQuerySet(query);
    }

    public void CApertureOrderSet(CCatalogOrder? order)
    {
        _cApertureVista?.LVistaOrderSet(CCatalog.LCatalogOrderRead(order));
    }

    public void CApertureFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cApertureVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal void CApertureCountSet(int count)
    {
        _cApertureCount = count;
    }

    public string CApertureTallyRead()
    {
        try
        {
            return _cApertureVista is null ? string.Empty : _cApertureVistaPort.LEngineTallyRead(_cApertureVista);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cApertureEnvoy, _cApertureSettingsPort, _cApertureLoadKey, exception);
            return string.Empty;
        }
    }

    public void CApertureRowsResonate()
    {
        CApertureRowsChanged?.Invoke();
    }

    internal void CApertureRestore(LVista vista)
    {
        ArgumentNullException.ThrowIfNull(vista);

        vista.LVistaQuerySet(_cApertureVista?.LVistaQuery ?? string.Empty);
        _cApertureVista = vista;
    }

    public void CApertureObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cApertureVista?.LVistaObserverAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    public void CApertureChosenAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cApertureVista?.LVistaChosenAttach(
            CCatalog.LCatalogSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }
}
