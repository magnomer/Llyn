using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTenor
{
    private readonly CAtelier _cTenorAtelier;

    private readonly LEntryPort _cTenorEntryPort;

    private readonly CEnvoy _cTenorEnvoy;

    private readonly LSettingsPort _cTenorSettingsPort;

    private readonly Action<Action> _cTenorMarshal;

    private LVista? _cTenorVista;

    private CTenor(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cTenorAtelier = atelier;
        _cTenorEntryPort = atelier.CAtelierEntryPort;
        _cTenorEnvoy = envoy;
        _cTenorSettingsPort = atelier.CAtelierSettingsPort;
        _cTenorMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CTenorEditor = editor;
        CTenorCohort = new CCohort(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            _cTenorSettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CPanel panel = CTenorCohort.CCohortPanel;
        panel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        panel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Tenor",
            panel.CPanelLeaveConfirm,
            () => LTenorChosen ?? 0,
            panel.LPanelScribeRestore,
            LTenorRegisterOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(panel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LTenorVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LTenorClose);
        LTenorVistaRestore();
    }

    public static CTenor CTenorCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CTenor(atelier, shownSeam, envoy, marshal);
    }

    public CEditor CTenorEditor { get; }

    public CCohort CTenorCohort { get; }

    public event Action? CTenorRegisterOpened;

    public event Action? CTenorRowsChanged;

    public event Action? CTenorWorkspaceChanged;

    internal long? LTenorChosen => _cTenorVista?.LVistaChosen;

    public bool CTenorFiltered => _cTenorVista?.LVistaFiltered ?? false;

    public CCatalogOrder CTenorOrder => CCatalog.LCatalogOrderRead(LVista.LVistaOrderRead(_cTenorVista));

    public CCatalogFilter CTenorFilter => CCatalog.LCatalogFilterRead(LVista.LVistaFilterRead(_cTenorVista));

    internal void LTenorVistaRestore()
    {
        LVista vista = _cTenorAtelier.CAtelierVistaStart(
            "tenor", CSubject.CSubjectRegister, CCatalogOrder.CCatalogOrderName);
        LVista cohort = _cTenorAtelier.CAtelierVistaStart(
            "cohort", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        vista.LVistaQuerySet(_cTenorVista?.LVistaQuery ?? string.Empty);
        _cTenorVista = vista;
        CTenorCohort.LCohortVistaRestore(vista, cohort);
        CTenorEditor.LEditorVistaRestore(cohort);
        LTenorObserverAttach();
    }

    private void LTenorObserverAttach()
    {
        LTenorSubjectAttach(CSubject.CSubjectVista, LTenorRowsResonate);
        LTenorSubjectAttach(CSubject.CSubjectWorkspace, LTenorWorkspaceResonate);
        LTenorSubjectAttach(CSubject.CSubjectRegister, LTenorRegisterResonate);
        LTenorSubjectAttach(CSubject.CSubjectReflex, LTenorRowsResonate);
        LTenorSubjectAttach(CSubject.CSubjectSettings, LTenorRowsResonate);
        CTenorCohort.LCohortObserverAttach(_cTenorMarshal);
        LTenorSubjectAttach(CSubject.CSubjectEntry, LTenorRowsResonate);
    }

    private void LTenorSubjectAttach(CSubject subject, Action resonate)
    {
        _cTenorVista?.LVistaObserverAttach(CCatalog.LCatalogSubjectRead(subject), _ => _cTenorMarshal(resonate));
    }

    private void LTenorRowsResonate()
    {
        CTenorRowsChanged?.Invoke();
    }

    private void LTenorWorkspaceResonate()
    {
        CTenorCohort.CCohortPanel.CPanelEntryClose();
        _cTenorVista?.LVistaSelect(null);
        LTenorRowsResonate();
        CTenorWorkspaceChanged?.Invoke();
    }

    private void LTenorRegisterResonate()
    {
        LTenorRowsResonate();
        CTenorCohort.CCohortPanel.CPanelDraftResonate();
    }

    public void CTenorQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cTenorVista?.LVistaQuerySet(query);
    }

    public void CTenorOrderSet(CCatalogOrder? order)
    {
        _cTenorVista?.LVistaOrderSet(CCatalog.LCatalogOrderRead(order));
    }

    public void CTenorFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cTenorVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public void CTenorRegisterToggle(long id)
    {
        _cTenorAtelier.CAtelierNavigation.LNavigationStationAdd();
        _cTenorVista?.LVistaToggle(id);
        LTenorRowsResonate();
    }

    internal void LTenorRegisterOpen(long id)
    {
        _cTenorVista?.LVistaSelect(id);
        _cTenorVista?.LVistaQuerySet(string.Empty);
        CTenorCohort.CCohortQuerySet(string.Empty);
        CTenorRegisterOpened?.Invoke();
        LTenorRowsResonate();
    }

    public IReadOnlyList<CCatalogRegister> CTenorRowsRead()
    {
        if (_cTenorVista is not LVista vista)
        {
            return [];
        }

        IReadOnlyList<CCatalogRegister> rows;
        try
        {
            rows = _cTenorEntryPort.LEngineRegisterFind(vista)
                .Select(static row => new CCatalogRegister(
                    CCard.LCardRegisterRead(row.LCatalogRegisterStored),
                    row.LCatalogRegisterUsage,
                    row.LCatalogRegisterIcon,
                    row.LCatalogRegisterChosen))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTenorEnvoy, _cTenorSettingsPort, "Register.LoadFailed", exception);
            return [];
        }

        CTenorCohort.CCohortPanel.CPanelRowsResonate();
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogRegister>>> CTenorRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cTenorEnvoy, _cTenorSettingsPort, "Register.LoadFailed", store, CTenorRowsRead);

    private bool LTenorCoinageAllowed =>
        LTenorChosen is null && !CTenorCohort.CCohortPanel.CPanelBinEnabled;

    private void LTenorRegisterCreate(string name)
    {
        long id;
        try
        {
            id = _cTenorEntryPort.LEngineRegisterCreate(name).LRegisterId;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTenorEnvoy, _cTenorSettingsPort, "Register.CreateFailed", exception);
            return;
        }

        CTenorCohort.CCohortPanel.CPanelEntryClose();
        LTenorRegisterOpen(id);
    }

    public void CTenorEntryCreate()
    {
        if (!CTenorCohort.CCohortPanel.CPanelLeaveConfirm())
        {
            return;
        }

        if (LTenorCoinageAllowed)
        {
            if (_cTenorEnvoy.CEnvoyCoinageRead("Coinage.Register") is string name)
            {
                LTenorRegisterCreate(name);
            }

            return;
        }

        long? chosen = LTenorChosen;
        CTenorCohort.CCohortPanel.CPanelFreshOpen();
        CTenorEditor.CEditorDesk.LDeskCohortStart(chosen);
    }

    private void LTenorClose()
    {
        CTenorEditor.CEditorClose();
        CTenorEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }
}
