using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CTenor
{
    private readonly CAtelier _cTenorAtelier;

    private readonly LEntryPort _cTenorEntryPort;

    private readonly LPortraitPort _cTenorPortraitPort;

    private readonly CEnvoy _cTenorEnvoy;

    private readonly LSettingsPort _cTenorSettingsPort;

    private LVista? _cTenorVista;

    private LVista? _cTenorCohort;

    private CTenor(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cTenorAtelier = atelier;
        _cTenorEntryPort = atelier.CAtelierEntryPort;
        _cTenorPortraitPort = atelier.CAtelierPortraitPort;
        _cTenorEnvoy = envoy;
        _cTenorSettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CTenorEditor = editor;
        CTenorPanel = new CPanel(
            envoy,
            _cTenorSettingsPort,
            "Register.LoadFailed",
            "Scribe",
            editor.CEditorDesk.LDeskChangeCheck,
            editor.LEditorFinish,
            shownSeam);
        CTenorPanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        CTenorPanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Tenor",
            CTenorPanel.CPanelLeaveConfirm,
            () => LTenorChosen ?? 0,
            CTenorPanel.LPanelScribeRestore,
            LTenorRegisterOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CTenorPanel.LPanelChangeCheck, editor.LEditorFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(CTenorVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LTenorClose);
    }

    public static CTenor CTenorCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CTenor(atelier, shownSeam, envoy);
    }

    public CEditor CTenorEditor { get; }

    public CPanel CTenorPanel { get; }

    public event Action? CTenorRegisterOpened;

    internal long? LTenorChosen => _cTenorVista?.LVistaChosen;

    public bool CTenorFiltered => _cTenorVista?.LVistaFiltered ?? false;

    public bool CTenorCoinageAllowed => LTenorChosen is null && !CTenorPanel.CPanelBinEnabled;

    public string CTenorEmptyKey =>
        _cTenorCohort?.LVistaQueried ?? false ? "Register.Unmatched" : "Register.Vacant";

    public CCatalogOrder CTenorOrder => CPanel.CPanelOrderRead(LVista.LVistaOrderRead(_cTenorVista));

    public CCatalogFilter CTenorFilter => CPanel.CPanelFilterRead(LVista.LVistaFilterRead(_cTenorVista));

    public void CTenorVistaRestore()
    {
        LVista vista = _cTenorAtelier.CAtelierVistaStart(
            "tenor", CSubject.CSubjectRegister, CCatalogOrder.CCatalogOrderName);
        LVista cohort = _cTenorAtelier.CAtelierVistaStart(
            "cohort", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cTenorVista = vista;
        _cTenorCohort = cohort;
        CTenorPanel.CPanelVistaRestore(cohort);
        CTenorEditor.LEditorVistaRestore(cohort);
    }

    public void CTenorObserverAttach(CSubject subject, Action<CBulletin> observer)
    {
        ArgumentNullException.ThrowIfNull(observer);

        _cTenorVista?.LVistaObserverAttach(
            CPanel.CPanelSubjectRead(subject), bulletin => observer(CAtelier.CAtelierBulletinRead(bulletin)));
    }

    public void CTenorQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cTenorVista?.LVistaQuerySet(query);
    }

    public void CTenorOrderSet(CCatalogOrder? order)
    {
        _cTenorVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

    public void CTenorFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cTenorVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public void CTenorCohortFind(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cTenorCohort?.LVistaQuerySet(query);
    }

    public void CTenorRegisterSelect(long? id)
    {
        _cTenorVista?.LVistaSelect(id);
    }

    public void CTenorRegisterToggle(long id)
    {
        _cTenorAtelier.CAtelierNavigation.LNavigationStationAdd();
        _cTenorVista?.LVistaToggle(id);
    }

    internal void LTenorRegisterOpen(long id)
    {
        CTenorRegisterSelect(id);
        _cTenorVista?.LVistaQuerySet(string.Empty);
        _cTenorCohort?.LVistaQuerySet(string.Empty);
        CTenorRegisterOpened?.Invoke();
    }

    public IReadOnlyList<CCatalogRegister> CTenorRowsRead()
    {
        return _cTenorVista is LVista vista
            ? _cTenorEntryPort.LEngineRegisterFind(vista)
                .Select(static row => new CCatalogRegister(
                    CCard.LCardRegisterRead(row.LCatalogRegisterStored),
                    row.LCatalogRegisterUsage,
                    row.LCatalogRegisterIcon,
                    row.LCatalogRegisterChosen))
                .ToList()
            : [];
    }

    public IReadOnlyList<CVistaRow> CTenorCohortRead()
    {
        return _cTenorEntryPort.LEngineEntryFind(_cTenorVista, _cTenorCohort)
            .Select(CPanel.CPanelRowRead)
            .ToList();
    }

    public IReadOnlyList<string> CTenorLanguageRead()
    {
        return _cTenorSettingsPort.LEngineLanguageRead();
    }

    public void CTenorRegisterCreate(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

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

        CTenorPanel.CPanelEntryClose();
        LTenorRegisterOpen(id);
    }

    public void CTenorEntryCreate()
    {
        long? chosen = LTenorChosen;
        CTenorPanel.CPanelFreshOpen();
        CTenorEditor.CEditorDesk.LDeskCohortStart(chosen);
    }

    internal string LTenorFileRead()
    {
        return LVista.LVistaFileRead(_cTenorCohort);
    }

    public Task CTenorPortraitPrint()
    {
        if (!CTenorPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitTicketPrint(
            _cTenorEnvoy,
            _cTenorSettingsPort,
            chosen => _cTenorPortraitPort.LEnginePortraitPrint(
                _cTenorCohort, CPortrait.LPortraitLabelRead(_cTenorSettingsPort), chosen));
    }

    public Task CTenorPortraitExport()
    {
        if (!CTenorPanel.CPanelPressAllowed)
        {
            return Task.CompletedTask;
        }

        return CPortrait.LPortraitFileExport(
            _cTenorEnvoy,
            _cTenorSettingsPort,
            LTenorFileRead(),
            (file, medium) => _cTenorPortraitPort.LEnginePortraitExport(
                _cTenorCohort, file, medium, CPortrait.LPortraitLabelRead(_cTenorSettingsPort)));
    }

    private void LTenorClose()
    {
        CTenorEditor.CEditorClose();
        CTenorEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }
}
