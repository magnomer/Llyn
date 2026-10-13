using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTenor
{
    private readonly CAtelier _cTenorAtelier;

    private readonly LRegisterPort _cTenorRegisterPort;

    private readonly CEnvoy _cTenorEnvoy;

    private readonly LSettingsPort _cTenorSettingsPort;

    private readonly Action<Action> _cTenorMarshal;

    private CTenor(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cTenorAtelier = atelier;
        _cTenorRegisterPort = atelier.CAtelierEntryBundle.CEntryBundleRegister;
        _cTenorEnvoy = envoy;
        _cTenorSettingsPort = atelier.CAtelierSettingsPort;
        _cTenorMarshal = marshal;
        CTenorAperture = new CAperture(
            envoy, _cTenorSettingsPort, atelier.CAtelierEntryBundle.CEntryBundleVista, "Register.LoadFailed");
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CTenorEditor = editor;
        CTenorCohort = new CCohort(
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            atelier.CAtelierPortraitPort,
            _cTenorSettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CPanel panel = CTenorCohort.CCohortPanel;
        panel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        panel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        editor.CEditorDisplay.CDisplayPanelAttach(panel);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Tenor",
            panel.CPanelLeaveConfirm,
            () => CTenorAperture.CApertureChosen ?? 0,
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

    public CAperture CTenorAperture { get; }

    public CEditor CTenorEditor { get; }

    public CCohort CTenorCohort { get; }

    public event Action? CTenorRegisterOpened;

    public event Action? CTenorWorkspaceChanged;

    internal void LTenorVistaRestore()
    {
        LVista vista = _cTenorAtelier.CAtelierVistaStart(
            "tenor", CSubject.CSubjectRegister, CCatalogOrder.CCatalogOrderName);
        LVista cohort = _cTenorAtelier.CAtelierVistaStart(
            "cohort", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CTenorAperture.CApertureRestore(vista);
        CTenorCohort.LCohortVistaRestore(vista, cohort);
        CTenorEditor.LEditorVistaRestore(cohort);
        LTenorObserverAttach();
    }

    private void LTenorObserverAttach()
    {
        CTenorAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => _cTenorMarshal(CTenorAperture.CApertureRowsResonate));
        CTenorAperture.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cTenorMarshal(LTenorWorkspaceResonate));
        CTenorAperture.CApertureObserverAttach(
            CSubject.CSubjectRegister, _ => _cTenorMarshal(LTenorRegisterResonate));
        CTenorAperture.CApertureObserverAttach(
            CSubject.CSubjectReflex, _ => _cTenorMarshal(CTenorAperture.CApertureRowsResonate));
        CTenorAperture.CApertureObserverAttach(
            CSubject.CSubjectSettings, _ => _cTenorMarshal(CTenorAperture.CApertureRowsResonate));
        CTenorCohort.LCohortObserverAttach(_cTenorMarshal);
        CTenorAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, _ => _cTenorMarshal(CTenorAperture.CApertureRowsResonate));
    }

    private void LTenorWorkspaceResonate()
    {
        CTenorCohort.CCohortPanel.CPanelEntryClose();
        CTenorAperture.CApertureVista?.LVistaSelect(null);
        CTenorAperture.CApertureRowsResonate();
        CTenorWorkspaceChanged?.Invoke();
    }

    private void LTenorRegisterResonate()
    {
        CTenorAperture.CApertureRowsResonate();
        CTenorCohort.CCohortPanel.CPanelDraftResonate();
    }

    public void CTenorRegisterToggle(long id)
    {
        _cTenorAtelier.CAtelierNavigation.LNavigationStationAdd();
        CTenorAperture.CApertureVista?.LVistaToggle(id);
        CTenorAperture.CApertureRowsResonate();
    }

    internal void LTenorRegisterOpen(long id)
    {
        CTenorAperture.CApertureVista?.LVistaSelect(id);
        CTenorAperture.CApertureQuerySet(string.Empty);
        CTenorCohort.CCohortPanel.CPanelAperture.CApertureQuerySet(string.Empty);
        CTenorRegisterOpened?.Invoke();
        CTenorAperture.CApertureRowsResonate();
    }

    public IReadOnlyList<CCatalogRegister> CTenorRowsRead()
    {
        if (CTenorAperture.CApertureVista is not LVista vista)
        {
            return [];
        }

        IReadOnlyList<CCatalogRegister> rows;
        try
        {
            rows = _cTenorRegisterPort.LEngineRegisterFind(vista)
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

        CTenorCohort.CCohortPanel.CPanelAperture.CApertureRowsResonate();
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogRegister>>> CTenorRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cTenorEnvoy, _cTenorSettingsPort, "Register.LoadFailed", store, CTenorRowsRead);

    private bool LTenorCoinageAllowed =>
        CTenorAperture.CApertureChosen is null && !CTenorCohort.CCohortPanel.CPanelBinEnabled;

    private void LTenorRegisterCreate(string name)
    {
        long id;
        try
        {
            id = _cTenorRegisterPort.LEngineRegisterCreate(name).LRegisterId;
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

        long? chosen = CTenorAperture.CApertureChosen;
        CTenorCohort.CCohortPanel.CPanelFreshOpen();
        CTenorEditor.CEditorDesk.LDeskRun((drafts, vista) => drafts.LEngineCohortStart(vista, chosen));
    }

    private void LTenorClose()
    {
        CTenorEditor.CEditorClose();
        CTenorEditor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
    }
}
