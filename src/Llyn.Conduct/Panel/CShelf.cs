using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CShelf
{
    private readonly CAtelier _cShelfAtelier;

    private readonly CEnvoy _cShelfEnvoy;

    private readonly LReferencePort _cShelfReferencePort;

    private readonly LPortraitPort _cShelfPortraitPort;

    private readonly LSettingsPort _cShelfSettingsPort;

    private readonly Action<Action> _cShelfMarshal;

    private CShelf(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cShelfAtelier = atelier;
        _cShelfEnvoy = envoy;
        _cShelfReferencePort = atelier.CAtelierEntryBundle.CEntryBundleReference;
        _cShelfPortraitPort = atelier.CAtelierPortraitPort;
        _cShelfSettingsPort = atelier.CAtelierSettingsPort;
        _cShelfMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CShelfEditor = editor;
        CShelfImprint = new CImprint(
            atelier.CAtelierDraftPort,
            atelier.CAtelierEntryBundle.CEntryBundleEntry,
            atelier.CAtelierEntryBundle.CEntryBundleReference,
            atelier.CAtelierEntryBundle.CEntryBundleAuthor,
            atelier.CAtelierSettingsPort,
            envoy,
            atelier.CAtelierLedger.LLedgerRepaint,
            marshal);
        CShelfPanel = new CPanel(
            envoy,
            _cShelfSettingsPort,
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            "Source.LoadFailed", "Source",
            CShelfImprint.CImprintDesk.LDeskChangeCheck,
            store => CShelfSession!.LSessionFinish(store),
            shownSeam);
        CShelfFootnote = new CFootnote(
            atelier.CAtelierEntryBundle.CEntryBundleVista,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            editor.CEditorDesk,
            envoy,
            store => CShelfSession!.LSessionFinish(store),
            shownSeam);
        CShelfFootnote.CFootnotePanel.CPanelCleared += editor.CEditorDesk.CDeskCancel;
        CShelfFootnote.CFootnotePanel.CPanelEdited += id => editor.CEditorEntryOpen(id);
        editor.CEditorDisplay.CDisplayPanelAttach(CShelfFootnote.CFootnotePanel);
        CShelfSession = new CSession(
            CShelfImprint.CImprintDesk,
            [CShelfPanel.LPanelChangeCheck, CShelfFootnote.CFootnotePanel.LPanelChangeCheck],
            editor.CEditorDesk,
            () => CShelfDiptych!.CDiptychChildSide,
            editor.LEditorFinish,
            static () => true,
            id => CShelfDiptych!.LDiptychParentShow(id, false),
            envoy);
        CShelfDiptych = new CDiptych(
            CShelfPanel,
            CShelfFootnote.CFootnotePanel,
            CShelfSession,
            atelier.CAtelierNavigation,
            CShelfImprint.LImprintOpen,
            CShelfImprint.LImprintCancel,
            () =>
            {
                LShelfSourceHide();
                CShelfFootnote.LFootnoteEntryCreate();
            });
        CShelfPanel.CPanelAperture.CApertureRowsChanged +=
            CShelfFootnote.CFootnotePanel.CPanelAperture.CApertureRowsResonate;
        CShelfPanel.CPanelCleared += CShelfImprint.LImprintCancel;
        CShelfPanel.CPanelEdited += id => CShelfImprint.LImprintOpen(id);
        CShelfPanel.CPanelDraftChanged +=
            draft => CShelfColophonChanged?.Invoke(COeuvre.LOeuvreColophonRead(_cShelfReferencePort, draft));
        CShelfImprint.CImprintDesk.CDeskFinished += id => CShelfDiptych.LDiptychParentShow(id, false);
        CShelfSession.CSessionChanged += () => CShelfChanged?.Invoke();
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Reference",
            () => CShelfSession.LSessionLeaveConfirm(true),
            CShelfPanel.LPanelChosenRead,
            CShelfPanel.LPanelScribeRestore,
            LShelfReferenceOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(CShelfSession.LSessionChangeCheck, CShelfSession.LSessionFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LShelfVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(() =>
        {
            CShelfImprint.CImprintByline.CBylineClose();
            editor.CEditorClose();
            editor.CEditorDisplay.CDisplayPlayback.CDisplayPlaybackCancel();
        });
        LShelfVistaRestore();
    }

    public static CShelf CShelfCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        return new CShelf(atelier, shownSeam, envoy, marshal);
    }

    public event Action? CShelfChanged;

    public event Action<CColophon>? CShelfColophonChanged;

    public CEditor CShelfEditor { get; }

    public CPanel CShelfPanel { get; }

    public CFootnote CShelfFootnote { get; }

    public CImprint CShelfImprint { get; }

    public CSession CShelfSession { get; }

    public CDiptych CShelfDiptych { get; }

    public bool CShelfStoreEnabled =>
        (CShelfDiptych.CDiptychChildSide ? CShelfEditor.CEditorDesk : CShelfImprint.CImprintDesk)
            .CDeskDraft.CDeskDraftStorable;

    public bool CShelfPressAllowed => CShelfFootnote.CFootnotePanel.CPanelPressAllowed || LShelfSourcePrintable;

    public bool CShelfPortraitAllowed => CShelfFootnote.CFootnotePanel.CPanelPressAllowed;

    private bool LShelfSourcePrintable => !CShelfDiptych.CDiptychChildSide && CShelfPanel.CPanelPressAllowed;

    private bool LShelfSourceShown => CShelfPanel.CPanelBinEnabled && !CShelfPanel.CPanelEditing;

    internal void LShelfVistaRestore()
    {
        LVista vista = _cShelfAtelier.CAtelierVistaStart(
            "reference", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName);
        LVista footnote = _cShelfAtelier.CAtelierVistaStart(
            "footnote", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        CShelfPanel.CPanelVistaRestore(vista);
        CShelfFootnote.LFootnoteVistaRestore(vista, footnote);
        CShelfImprint.CImprintDesk.CDeskVistaRestore(vista);
        CShelfEditor.LEditorVistaRestore(footnote);
        LShelfObserverAttach();
        CShelfFootnote.LFootnoteObserverAttach(
            _cShelfMarshal, CShelfPanel.CPanelAperture.CApertureRowsResonate, LShelfEntryResonate);
    }

    private void LShelfObserverAttach()
    {
        Action<CBulletin> rows = _ => _cShelfMarshal(CShelfPanel.CPanelAperture.CApertureRowsResonate);
        CShelfPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectVista, rows);
        CShelfPanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectWorkspace, _ => _cShelfMarshal(CShelfDiptych.LDiptychEntryClose));
        CShelfPanel.CPanelAperture.CApertureObserverAttach(
            CSubject.CSubjectAuthor, _ => _cShelfMarshal(CShelfImprint.CImprintDesk.CDeskDraft.CDeskDraftResonate));
        CShelfPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectAuthor, rows);
        CShelfPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectReference, rows);
        CShelfPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectExample, rows);
        CShelfPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectReflex, rows);
        CShelfPanel.CPanelAperture.CApertureObserverAttach(CSubject.CSubjectSettings, rows);
    }

    public CShelfRoll CShelfRollRead()
    {
        IReadOnlyList<CCatalogReference> rows;
        try
        {
            rows = COeuvre.COeuvreReferenceRead(
                CShelfPanel.CPanelAperture.CApertureVista is LVista vista
                    ? _cShelfReferencePort.LEngineReferenceFind(vista)
                    : []);
        }
        catch (Exception exception)
        {
            CLedger.LLedgerFailureShow(_cShelfEnvoy, _cShelfSettingsPort, "Source.LoadFailed", exception);
            rows = [];
        }

        if (LShelfSourceShown && !rows.Any(static row => row.CCatalogReferenceChosen))
        {
            CShelfPanel.CPanelEntryClose();
        }

        return new CShelfRoll(rows, rows.Count == 0, CShelfPanel.CPanelAperture.CApertureTallyRead());
    }

    public Task<CEnsignSheet<CShelfRoll>> CShelfRollLoad(
        Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store) =>
        CCatalog.LCatalogEnsignLoad(
            _cShelfEnvoy, _cShelfSettingsPort, "Source.LoadFailed", store, CShelfRollRead);

    public static IReadOnlyList<CCatalogOrder> CShelfOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderYear,
            CCatalogOrder.CCatalogOrderAuthor,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public void CShelfReferenceSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        if (!CShelfSession.LSessionLeaveConfirm(true))
        {
            return;
        }

        _cShelfAtelier.CAtelierNavigation.LNavigationStationAdd();
        CShelfDiptych.LDiptychParentShow(chosen, CShelfDiptych.CDiptychScribeChecked);
    }

    internal void LShelfReferenceOpen(long id)
    {
        CShelfDiptych.LDiptychParentShow(id, CShelfDiptych.CDiptychScribeChecked);
    }

    private void LShelfSourceShow()
    {
        CShelfPanel.CPanelRowOpen(CShelfPanel.CPanelAperture.CApertureChosen);
    }

    private void LShelfSourceHide()
    {
        CShelfPanel.CPanelScribeSet(false);
        CShelfImprint.LImprintCancel();
    }

    public void CShelfEntrySelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!CShelfSession.LSessionLeaveConfirm(true))
        {
            return;
        }

        LShelfEntryOpen(id, CShelfDiptych.CDiptychScribeChecked);
    }

    private void LShelfEntryOpen(long? id, bool editing)
    {
        LShelfSourceHide();
        CShelfFootnote.CFootnotePanel.CPanelScribeSet(editing);
        CShelfFootnote.CFootnotePanel.CPanelRowOpen(id);
    }

    private void LShelfEntryResonate()
    {
        CShelfFootnote.CFootnotePanel.CPanelDraftResonate();
        if (CShelfDiptych.CDiptychChildSide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void CShelfScribeToggle(bool editing)
    {
        if (CShelfDiptych.CDiptychChildSide)
        {
            LShelfFootnoteSet(editing);
            return;
        }

        CShelfPanel.CPanelScribeToggle(editing);
        if (!CShelfPanel.CPanelEditing)
        {
            CShelfImprint.LImprintCancel();
        }
    }

    private void LShelfFootnoteSet(bool editing)
    {
        CShelfFootnote.CFootnotePanel.CPanelScribeToggle(editing);
        if (CShelfDiptych.CDiptychChildSide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public Task CShelfPortraitPrint()
    {
        if (CShelfFootnote.CFootnotePanel.CPanelPressAllowed)
        {
            return CShelfFootnote.LFootnotePortraitPrint(_cShelfEnvoy, _cShelfSettingsPort);
        }

        if (LShelfSourcePrintable)
        {
            return CPortrait.LPortraitTicketPrint(
                _cShelfEnvoy,
                _cShelfSettingsPort,
                chosen => _cShelfPortraitPort.LEnginePortraitPrint(
                    CShelfPanel.CPanelAperture.CApertureVista,
                    CPortrait.LPortraitLegendRead(_cShelfSettingsPort, "Source"),
                    chosen));
        }

        return Task.CompletedTask;
    }

    public Task CShelfPortraitExport()
    {
        if (!CShelfPortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return CShelfFootnote.LFootnotePortraitExport(_cShelfEnvoy, _cShelfSettingsPort);
    }
}
