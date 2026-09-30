using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTaxonomy
{
    private readonly CAtelier _cTaxonomyAtelier;

    private readonly LEntryPort _cTaxonomyEntryPort;

    private readonly CEnvoy _cTaxonomyEnvoy;

    private readonly LSettingsPort _cTaxonomySettingsPort;

    private readonly Action<Action> _cTaxonomyMarshal;

    private LVista? _cTaxonomyVista;

    private CTaxonomy(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cTaxonomyAtelier = atelier;
        _cTaxonomyEntryPort = atelier.CAtelierEntryPort;
        _cTaxonomyEnvoy = envoy;
        _cTaxonomySettingsPort = atelier.CAtelierSettingsPort;
        _cTaxonomyMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CTaxonomyEditor = editor;
        CTaxonomyMembership = new CMembership(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            _cTaxonomySettingsPort,
            envoy,
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CPanel panel = CTaxonomyMembership.CMembershipPanel;
        panel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        panel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Taxonomy",
            panel.CPanelLeaveConfirm,
            () => LTaxonomyChosen ?? 0,
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

    public CEditor CTaxonomyEditor { get; }

    public CMembership CTaxonomyMembership { get; }

    public event Action? CTaxonomyTagOpened;

    public event Action? CTaxonomyRowsChanged;

    public event Action? CTaxonomyWorkspaceChanged;

    internal long? LTaxonomyChosen => _cTaxonomyVista?.LVistaChosen;

    public bool CTaxonomyFiltered => _cTaxonomyVista?.LVistaFiltered ?? false;

    public CCatalogOrder CTaxonomyOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cTaxonomyVista));

    public CCatalogFilter CTaxonomyFilter => CPanel.CPanelFilterRead(LVista.LVistaFilterRead(_cTaxonomyVista));

    internal void LTaxonomyVistaRestore()
    {
        LVista vista = _cTaxonomyAtelier.CAtelierVistaStart(
            "taxonomy", CSubject.CSubjectTag, CCatalogOrder.CCatalogOrderName);
        LVista membership = _cTaxonomyAtelier.CAtelierVistaStart(
            "membership", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        vista.LVistaQuerySet(_cTaxonomyVista?.LVistaQuery ?? string.Empty);
        _cTaxonomyVista = vista;
        CTaxonomyMembership.LMembershipVistaRestore(vista, membership);
        CTaxonomyEditor.LEditorVistaRestore(membership);
        LTaxonomyObserverAttach();
    }

    private void LTaxonomyObserverAttach()
    {
        LTaxonomySubjectAttach(CSubject.CSubjectVista, LTaxonomyRowsResonate);
        LTaxonomySubjectAttach(CSubject.CSubjectWorkspace, LTaxonomyWorkspaceResonate);
        LTaxonomySubjectAttach(CSubject.CSubjectTag, LTaxonomyTagResonate);
        LTaxonomySubjectAttach(CSubject.CSubjectReflex, LTaxonomyRowsResonate);
        LTaxonomySubjectAttach(CSubject.CSubjectSettings, LTaxonomyRowsResonate);
        CTaxonomyMembership.LMembershipObserverAttach(_cTaxonomyMarshal);
        LTaxonomySubjectAttach(CSubject.CSubjectEntry, LTaxonomyRowsResonate);
    }

    private void LTaxonomySubjectAttach(CSubject subject, Action resonate)
    {
        _cTaxonomyVista?.LVistaObserverAttach(CPanel.CPanelSubjectRead(subject), _ => _cTaxonomyMarshal(resonate));
    }

    private void LTaxonomyRowsResonate()
    {
        CTaxonomyRowsChanged?.Invoke();
    }

    private void LTaxonomyWorkspaceResonate()
    {
        CTaxonomyMembership.CMembershipPanel.CPanelEntryClose();
        _cTaxonomyVista?.LVistaSelect(null);
        LTaxonomyRowsResonate();
        CTaxonomyWorkspaceChanged?.Invoke();
    }

    private void LTaxonomyTagResonate()
    {
        LTaxonomyRowsResonate();
        CTaxonomyMembership.CMembershipPanel.CPanelDraftResonate();
    }

    public void CTaxonomyQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cTaxonomyVista?.LVistaQuerySet(query);
    }

    public void CTaxonomyOrderSet(CCatalogOrder? order)
    {
        _cTaxonomyVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CTaxonomyFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cTaxonomyVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public void CTaxonomyTagToggle(long id)
    {
        _cTaxonomyAtelier.CAtelierNavigation.LNavigationStationAdd();
        _cTaxonomyVista?.LVistaToggle(id);
        LTaxonomyRowsResonate();
    }

    internal void LTaxonomyTagOpen(long id)
    {
        _cTaxonomyVista?.LVistaSelect(id);
        _cTaxonomyVista?.LVistaQuerySet(string.Empty);
        CTaxonomyMembership.CMembershipQuerySet(string.Empty);
        CTaxonomyTagOpened?.Invoke();
        LTaxonomyRowsResonate();
    }

    public IReadOnlyList<CCatalogTag> CTaxonomyRowsRead()
    {
        if (_cTaxonomyVista is not LVista vista)
        {
            return [];
        }

        IReadOnlyList<CCatalogTag> rows;
        try
        {
            rows = _cTaxonomyEntryPort.LEngineTagFind(vista)
                .Select(static row => new CCatalogTag(
                    CCard.LCardTagRead(row.LCatalogTagStored), row.LCatalogTagChosen))
                .ToList();
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cTaxonomyEnvoy, _cTaxonomySettingsPort, "Tag.LoadFailed", exception);
            return [];
        }

        CTaxonomyMembership.CMembershipPanel.CPanelRowsResonate();
        return rows;
    }

    public Task<CEnsignSheet<IReadOnlyList<CCatalogTag>>> CTaxonomyRowsLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(_cTaxonomySettingsPort, store, CTaxonomyRowsRead);

    private bool LTaxonomyCoinageAllowed =>
        LTaxonomyChosen is null && !CTaxonomyMembership.CMembershipPanel.CPanelBinEnabled;

    private void LTaxonomyTagCreate(string name)
    {
        long id;
        try
        {
            id = _cTaxonomyEntryPort.LEngineTagCreate(name).LTagId;
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

        long? chosen = LTaxonomyChosen;
        CTaxonomyMembership.CMembershipPanel.CPanelFreshOpen();
        CTaxonomyEditor.CEditorDesk.LDeskMembershipStart(chosen);
    }

    private void LTaxonomyClose()
    {
        CTaxonomyEditor.CEditorClose();
        CTaxonomyEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }
}
