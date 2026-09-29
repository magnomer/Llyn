using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTaxonomy
{
    private readonly CAtelier _cTaxonomyAtelier;

    private readonly LEntryPort _cTaxonomyEntryPort;

    private readonly LPortraitPort _cTaxonomyPortraitPort;

    private readonly CEnvoy _cTaxonomyEnvoy;

    private readonly LSettingsPort _cTaxonomySettingsPort;

    private LVista? _cTaxonomyVista;

    private LVista? _cTaxonomyMembership;

    private CTaxonomy(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cTaxonomyAtelier = atelier;
        _cTaxonomyEntryPort = atelier.CAtelierEntryPort;
        _cTaxonomyPortraitPort = atelier.CAtelierPortraitPort;
        _cTaxonomyEnvoy = envoy;
        _cTaxonomySettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CTaxonomyEditor = editor;
        CTaxonomyPanel = new CPanel(
            envoy,
            _cTaxonomySettingsPort,
            "Tag.LoadFailed", "Scribe", editor.CEditorDesk.LDeskChangeCheck, editor.LEditorFinish, shownSeam);
        CTaxonomyPanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        CTaxonomyPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Taxonomy",
            CTaxonomyPanel.CPanelLeaveConfirm,
            () => LTaxonomyChosen ?? 0,
            CTaxonomyPanel.LPanelScribeRestore,
            LTaxonomyTagOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CTaxonomyPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(CTaxonomyVistaRestore);
    }

    public static CTaxonomy CTaxonomyCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CTaxonomy(atelier, shownSeam, envoy);
    }

    public CEditor CTaxonomyEditor { get; }

    public CPanel CTaxonomyPanel { get; }

    public event Action? CTaxonomyTagOpened;

    internal long? LTaxonomyChosen => _cTaxonomyVista?.LVistaChosen;

    public bool CTaxonomyFiltered => _cTaxonomyVista?.LVistaFiltered ?? false;

    public bool CTaxonomyCoinageAllowed => LTaxonomyChosen is null && !CTaxonomyPanel.CPanelBinEnabled;

    public string CTaxonomyEmptyKey =>
        _cTaxonomyMembership?.LVistaQueried ?? false ? "Tag.Unmatched" : "Tag.Vacant";

    public CCatalogOrder CTaxonomyOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cTaxonomyVista));

    public CCatalogFilter CTaxonomyFilter => CPanel.CPanelFilterRead(LVista.LVistaFilterRead(_cTaxonomyVista));

    public void CTaxonomyVistaRestore()
    {
        LVista vista = _cTaxonomyAtelier.CAtelierVistaStart(
            "taxonomy", CSubject.CSubjectTag, CCatalogOrder.CCatalogOrderName);
        LVista membership = _cTaxonomyAtelier.CAtelierVistaStart(
            "membership", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cTaxonomyVista = vista;
        _cTaxonomyMembership = membership;
        CTaxonomyPanel.CPanelVistaRestore(membership);
        CTaxonomyEditor.LEditorVistaRestore(membership);
    }

    public void CTaxonomyObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cTaxonomyVista?.LVistaObserverAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
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

    public void CTaxonomyMembershipFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cTaxonomyMembership?.LVistaQuerySet(query);
    }

    public void CTaxonomyTagSelect(long? id)
    {
        _cTaxonomyVista?.LVistaSelect(id);
    }

    public void CTaxonomyTagToggle(long id)
    {
        _cTaxonomyAtelier.CAtelierNavigation.LNavigationStationAdd();
        _cTaxonomyVista?.LVistaToggle(id);
    }

    internal void LTaxonomyTagOpen(long id)
    {
        CTaxonomyTagSelect(id);
        _cTaxonomyVista?.LVistaQuerySet(string.Empty);
        _cTaxonomyMembership?.LVistaQuerySet(string.Empty);
        CTaxonomyTagOpened?.Invoke();
    }

    public IReadOnlyList<CCatalogTag> CTaxonomyRowsRead()
    {
        return _cTaxonomyVista is LVista vista
            ? _cTaxonomyEntryPort.LEngineTagFind(vista)
                .Select(static row => new CCatalogTag(
                    CCard.LCardTagRead(row.LCatalogTagStored), row.LCatalogTagChosen))
                .ToList()
            : [];
    }

    public IReadOnlyList<CVistaRow> CTaxonomyMembershipRead()
    {
        return _cTaxonomyEntryPort.LEngineEntryFind(_cTaxonomyVista, _cTaxonomyMembership)
            .Select(CPanel.CPanelRowRead)
            .ToList();
    }

    public IReadOnlyList<string> CTaxonomyLanguageRead()
    {
        return _cTaxonomySettingsPort.LEngineLanguageRead();
    }

    public void CTaxonomyTagCreate(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

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

        CTaxonomyPanel.CPanelEntryClose();
        LTaxonomyTagOpen(id);
    }

    public void CTaxonomyEntryCreate()
    {
        long? chosen = LTaxonomyChosen;
        CTaxonomyPanel.CPanelFreshOpen();
        CTaxonomyEditor.CEditorDesk.LDeskMembershipStart(chosen);
    }

    internal string LTaxonomyFileRead()
    {
        return LVista.LVistaFileRead(_cTaxonomyMembership);
    }

    public Task CTaxonomyPortraitPrint()
    {
        if (!CTaxonomyPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cTaxonomyEnvoy,
            _cTaxonomySettingsPort,
            chosen => _cTaxonomyPortraitPort.LEnginePortraitPrint(
                _cTaxonomyMembership, CPortrait.LPortraitLabelRead(_cTaxonomySettingsPort), chosen));
    }

    public Task CTaxonomyPortraitExport()
    {
        if (!CTaxonomyPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitFileExport(
            _cTaxonomyEnvoy,
            _cTaxonomySettingsPort,
            LTaxonomyFileRead(),
            (file, medium) => _cTaxonomyPortraitPort.LEnginePortraitExport(
                _cTaxonomyMembership, file, medium, CPortrait.LPortraitLabelRead(_cTaxonomySettingsPort)));
    }
}
