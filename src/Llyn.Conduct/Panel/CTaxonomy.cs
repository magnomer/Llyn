using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTaxonomy
{
    private readonly CAtelier _cTaxonomyAtelier;

    private readonly LTagPort _cTaxonomyTagPort;

    private readonly CEnvoy _cTaxonomyEnvoy;

    private readonly LSettingsPort _cTaxonomySettingsPort;

    private readonly Action<Action> _cTaxonomyMarshal;

    private CTaxonomy(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cTaxonomyAtelier = atelier;
        _cTaxonomyTagPort = atelier.CAtelierEntryBundle.CEntryBundleTag;
        _cTaxonomyEnvoy = envoy;
        _cTaxonomySettingsPort = atelier.CAtelierSettingsPort;
        _cTaxonomyMarshal = marshal;
        CTaxonomyAperture = new CAperture(
            envoy, _cTaxonomySettingsPort, atelier.CAtelierEntryBundle.CEntryBundleVista, "Tag.LoadFailed");
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CTaxonomyEditor = editor;
        CTaxonomyMembership = new CMembership(
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            atelier.CAtelierPortraitPort,
            _cTaxonomySettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CPanel panel = CTaxonomyMembership.CMembershipPanel;
        panel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        panel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        editor.CEditorDisplay.CDisplayPanelAttach(panel);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Taxonomy",
            panel.CPanelLeaveConfirm,
            () => CTaxonomyAperture.CApertureChosen ?? 0,
            panel.LPanelScribeRestore,
            LTaxonomyTagOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(panel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LTaxonomyVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LTaxonomyClose);
        LTaxonomyVistaRestore();
    }

    public static CTaxonomy CTaxonomyCreate(
        CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CTaxonomy(atelier, shownSeam, envoy, marshal);
    }

    public CAperture CTaxonomyAperture { get; }

    public CEditor CTaxonomyEditor { get; }

    public CMembership CTaxonomyMembership { get; }

    public event Action? CTaxonomyTagOpened;

    public event Action? CTaxonomyWorkspaceChanged;

    internal void LTaxonomyVistaRestore()
    {
        LVista vista = _cTaxonomyAtelier.CAtelierVistaStart(
            "taxonomy", CSubject.CSubjectTag, CCatalogOrder.CCatalogOrderName);
        LVista membership = _cTaxonomyAtelier.CAtelierVistaStart(
            "membership", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CTaxonomyAperture.CApertureRestore(vista);
        CTaxonomyMembership.LMembershipVistaRestore(vista, membership);
        CTaxonomyEditor.LEditorVistaRestore(membership);
        LTaxonomyObserverAttach();
    }

    private void LTaxonomyObserverAttach()
    {
        CTaxonomyAperture.CApertureObserverAttach(
            CSubject.CSubjectVista, _ => _cTaxonomyMarshal(CTaxonomyAperture.CApertureRowsResonate));
        CTaxonomyAperture.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cTaxonomyMarshal(LTaxonomyWorkspaceResonate));
        CTaxonomyAperture.CApertureObserverAttach(
            CSubject.CSubjectTag, _ => _cTaxonomyMarshal(LTaxonomyTagResonate));
        CTaxonomyAperture.CApertureObserverAttach(
            CSubject.CSubjectReflex, _ => _cTaxonomyMarshal(CTaxonomyAperture.CApertureRowsResonate));
        CTaxonomyAperture.CApertureObserverAttach(
            CSubject.CSubjectSettings, _ => _cTaxonomyMarshal(CTaxonomyAperture.CApertureRowsResonate));
        CTaxonomyMembership.LMembershipObserverAttach(_cTaxonomyMarshal);
        CTaxonomyAperture.CApertureObserverAttach(
            CSubject.CSubjectEntry, _ => _cTaxonomyMarshal(CTaxonomyAperture.CApertureRowsResonate));
    }

    private void LTaxonomyWorkspaceResonate()
    {
        CTaxonomyMembership.CMembershipPanel.CPanelEntryClose();
        CTaxonomyAperture.CApertureVista?.LVistaSelect(null);
        CTaxonomyAperture.CApertureRowsResonate();
        CTaxonomyWorkspaceChanged?.Invoke();
    }

    private void LTaxonomyTagResonate()
    {
        CTaxonomyAperture.CApertureRowsResonate();
        CTaxonomyMembership.CMembershipPanel.CPanelDraftResonate();
    }

    public void CTaxonomyTagToggle(long id)
    {
        _cTaxonomyAtelier.CAtelierNavigation.LNavigationStationAdd();
        CTaxonomyAperture.CApertureVista?.LVistaToggle(id);
        CTaxonomyAperture.CApertureRowsResonate();
    }

    internal void LTaxonomyTagOpen(long id)
    {
        CTaxonomyAperture.CApertureVista?.LVistaSelect(id);
        CTaxonomyAperture.CApertureQuerySet(string.Empty);
        CTaxonomyMembership.CMembershipPanel.CPanelAperture.CApertureQuerySet(string.Empty);
        CTaxonomyTagOpened?.Invoke();
        CTaxonomyAperture.CApertureRowsResonate();
    }

    public IReadOnlyList<CCatalogTag> CTaxonomyRowsRead()
    {
        if (CTaxonomyAperture.CApertureVista is not LVista vista)
        {
            return [];
        }

        IReadOnlyList<CCatalogTag> rows;
        try
        {
            rows = _cTaxonomyTagPort.LEngineTagFind(vista)
                .Select(static row => new CCatalogTag(
                    CCard.LCardTagRead(row.LCatalogTagStored), row.LCatalogTagChosen))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTaxonomyEnvoy, _cTaxonomySettingsPort, "Tag.LoadFailed", exception);
            return [];
        }

        CTaxonomyMembership.CMembershipPanel.CPanelAperture.CApertureRowsResonate();
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogTag>>> CTaxonomyRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cTaxonomyEnvoy, _cTaxonomySettingsPort, "Tag.LoadFailed", store, CTaxonomyRowsRead);

    private bool LTaxonomyCoinageAllowed =>
        CTaxonomyAperture.CApertureChosen is null && !CTaxonomyMembership.CMembershipPanel.CPanelBinEnabled;

    private void LTaxonomyTagCreate(string name)
    {
        long id;
        try
        {
            id = _cTaxonomyTagPort.LEngineTagCreate(name).LTagId;
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTaxonomyEnvoy, _cTaxonomySettingsPort, "Tag.CreateFailed", exception);
            return;
        }

        CTaxonomyMembership.CMembershipPanel.CPanelEntryClose();
        LTaxonomyTagOpen(id);
    }

    public void CTaxonomyEntryCreate()
    {
        if (!CTaxonomyMembership.CMembershipPanel.CPanelLeaveConfirm())
        {
            return;
        }

        if (LTaxonomyCoinageAllowed)
        {
            if (_cTaxonomyEnvoy.CEnvoyCoinageRead("Coinage.Tag") is string name)
            {
                LTaxonomyTagCreate(name);
            }

            return;
        }

        long? chosen = CTaxonomyAperture.CApertureChosen;
        CTaxonomyMembership.CMembershipPanel.CPanelFreshOpen();
        CTaxonomyEditor.CEditorDesk.LDeskRun((drafts, vista) => drafts.LEngineMembershipStart(vista, chosen));
    }

    private void LTaxonomyClose()
    {
        CTaxonomyEditor.CEditorClose();
        CTaxonomyEditor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
    }
}
