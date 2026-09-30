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

    private readonly LEntryPort _cShelfEntryPort;

    private readonly LPortraitPort _cShelfPortraitPort;

    private readonly LSettingsPort _cShelfSettingsPort;

    private readonly Action<Action> _cShelfMarshal;

    private LVista? _cShelfVista;

    private CShelf(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy, Action<Action> marshal)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);
        ArgumentNullException.ThrowIfNull(marshal);

        _cShelfAtelier = atelier;
        _cShelfEnvoy = envoy;
        _cShelfEntryPort = atelier.CAtelierEntryPort;
        _cShelfPortraitPort = atelier.CAtelierPortraitPort;
        _cShelfSettingsPort = atelier.CAtelierSettingsPort;
        _cShelfMarshal = marshal;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CShelfEditor = editor;
        CShelfImprint = new CImprint(atelier.CAtelierDraftPort, atelier.CAtelierEntryPort, envoy, marshal);
        CShelfPanel = new CPanel(
            envoy,
            _cShelfSettingsPort,
            "Source.LoadFailed", "Source",
            CShelfImprint.CImprintDesk.LDeskChangeCheck, LShelfDraftFinish, shownSeam);
        CShelfFootnote = new CFootnote(
            atelier.CAtelierEntryPort,
            atelier.CAtelierPortraitPort,
            atelier.CAtelierSettingsPort,
            editor,
            envoy,
            LShelfDraftFinish,
            shownSeam);
        CShelfPanel.CPanelRowsChanged += CShelfFootnote.CFootnotePanel.CPanelRowsResonate;
        CShelfPanel.CPanelCleared += CShelfImprint.LImprintCancel;
        CShelfPanel.CPanelEdited += id => CShelfImprint.LImprintOpen(id);
        CShelfPanel.CPanelDraftChanged +=
            draft => CShelfColophonChanged?.Invoke(COeuvre.LOeuvreColophonRead(_cShelfEntryPort, draft));
        CShelfImprint.CImprintDesk.CDeskFinished += id => LShelfSourceOpen(id, false);
        CShelfImprint.CImprintDesk.CDeskStateChanged += () => CShelfChanged?.Invoke();
        editor.CEditorDesk.CDeskStateChanged += () => CShelfChanged?.Invoke();
        atelier.CAtelierNavigation.LNavigationTabAdd(
            "Reference",
            LShelfLeaveConfirm,
            CShelfPanel.LPanelChosenRead,
            CShelfPanel.LPanelScribeRestore,
            LShelfReferenceOpen);
        atelier.CAtelierWorkspace.LWorkspaceDraftAdd(LShelfChangeRead, LShelfDraftFinish);
        atelier.CAtelierWorkspace.LWorkspaceVistaAdd(LShelfVistaRestore);
        atelier.CAtelierWorkspace.LWorkspaceClosureAdd(LShelfClose);
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

    public bool CShelfEditorShown => LShelfEntrySide && CShelfFootnote.CFootnotePanel.CPanelEditing;

    public bool CShelfDisplayShown => LShelfEntrySide && !CShelfFootnote.CFootnotePanel.CPanelEditing;

    public bool CShelfImprintShown => !LShelfEntrySide && CShelfPanel.CPanelEditing;

    public bool CShelfColophonShown => !LShelfEntrySide && !CShelfPanel.CPanelEditing;

    public bool CShelfViewerChecked => !LShelfEditing;

    public bool CShelfScribeChecked => LShelfEditing;

    public bool CShelfModeEnabled =>
        CShelfPanel.CPanelModeEnabled || CShelfFootnote.CFootnotePanel.CPanelModeEnabled;

    public bool CShelfBinEnabled => !LShelfEntrySide && CShelfPanel.CPanelBinEnabled;

    public bool CShelfStoreEnabled =>
        LShelfEntrySide ? CShelfEditor.CEditorDesk.CDeskStorable : CShelfImprint.CImprintDesk.LDeskChangeCheck();

    public bool CShelfPressAllowed => CShelfFootnote.CFootnotePanel.CPanelPressAllowed || LShelfSourcePrintable;

    public bool CShelfPortraitAllowed => CShelfFootnote.CFootnotePanel.CPanelPressAllowed;

    public bool CShelfFiltered => _cShelfVista?.LVistaFiltered ?? false;

    private bool LShelfEntrySide => CShelfFootnote.CFootnotePanel.CPanelModeEnabled;

    private bool LShelfEditing =>
        LShelfEntrySide ? CShelfFootnote.CFootnotePanel.CPanelEditing : CShelfPanel.CPanelEditing;

    private bool LShelfSourcePrintable => !LShelfEntrySide && CShelfPanel.CPanelPressAllowed;

    private bool LShelfSourceShown => CShelfPanel.CPanelBinEnabled && !CShelfPanel.CPanelEditing;

    private bool LShelfRowChosen =>
        CShelfPanel.CPanelBinEnabled || CShelfFootnote.CFootnotePanel.CPanelBinEnabled;

    internal void LShelfVistaRestore()
    {
        LVista vista = _cShelfAtelier.CAtelierVistaStart(
            "reference", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName);
        LVista footnote = _cShelfAtelier.CAtelierVistaStart(
            "footnote", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        vista.LVistaQuerySet(_cShelfVista?.LVistaQuery ?? string.Empty);
        _cShelfVista = vista;
        CShelfPanel.CPanelVistaRestore(vista);
        CShelfFootnote.LFootnoteVistaRestore(vista, footnote);
        CShelfImprint.CImprintDesk.CDeskVistaRestore(vista);
        CShelfEditor.LEditorVistaRestore(footnote);
        LShelfObserverAttach();
        CShelfFootnote.LFootnoteObserverAttach(_cShelfMarshal, CShelfPanel.CPanelRowsResonate, LShelfEntryResonate);
    }

    private void LShelfObserverAttach()
    {
        Action<CBulletin> rows = _ => _cShelfMarshal(CShelfPanel.CPanelRowsResonate);
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectVista, rows);
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectWorkspace, _ => _cShelfMarshal(LShelfWorkspaceResonate));
        CShelfPanel.CPanelObserverAttach(
            CSubject.CSubjectAuthor, _ => _cShelfMarshal(CShelfImprint.CImprintDesk.CDeskDraftResonate));
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectAuthor, rows);
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectReference, rows);
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectExample, rows);
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectReflex, rows);
        CShelfPanel.CPanelObserverAttach(CSubject.CSubjectSettings, rows);
    }

    private void LShelfWorkspaceResonate()
    {
        CShelfFootnote.CFootnotePanel.CPanelEntryClose();
        CShelfPanel.CPanelEntryClose();
    }

    public CShelfRoll CShelfRollRead()
    {
        IReadOnlyList<CCatalogReference> rows = COeuvre.COeuvreReferenceRead(
            _cShelfVista is LVista vista ? _cShelfEntryPort.LEngineReferenceFind(vista) : []);
        if (LShelfSourceShown && !rows.Any(static row => row.CCatalogReferenceChosen))
        {
            CShelfPanel.CPanelEntryClose();
        }

        return new CShelfRoll(rows, rows.Count == 0, CShelfPanel.CPanelTallyRead());
    }

    public void CShelfQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _cShelfVista?.LVistaQuerySet(query);
    }

    public void CShelfOrderSet(CCatalogOrder? order)
    {
        _cShelfVista?.LVistaOrderSet(CPanel.CPanelOrderRead(order));
    }

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

    public void CShelfFilterSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _cShelfVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    internal bool LShelfChangeRead()
    {
        return CShelfPanel.LPanelChangeCheck() || CShelfFootnote.CFootnotePanel.LPanelChangeCheck();
    }

    internal bool LShelfLeaveConfirm()
    {
        if (!LShelfChangeRead())
        {
            return true;
        }

        if (_cShelfEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        return !store || LShelfDraftFinish(true);
    }

    public void CShelfReferenceSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LShelfLeaveConfirm())
        {
            return;
        }

        _cShelfAtelier.CAtelierNavigation.LNavigationStationAdd();
        LShelfSourceOpen(id, LShelfEditing);
    }

    internal void LShelfReferenceOpen(long id)
    {
        LShelfSourceOpen(id, LShelfEditing);
    }

    private void LShelfSourceOpen(long? id, bool editing)
    {
        CShelfFootnote.CFootnotePanel.CPanelEntryClose();
        CShelfPanel.CPanelScribeSet(editing);
        CShelfPanel.CPanelRowOpen(id);
    }

    private void LShelfSourceShow()
    {
        CShelfPanel.CPanelRowOpen(_cShelfVista?.LVistaChosen);
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

        if (!LShelfLeaveConfirm())
        {
            return;
        }

        LShelfEntryOpen(id, LShelfEditing);
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
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void CShelfReferenceCreate()
    {
        if (!LShelfLeaveConfirm())
        {
            return;
        }

        if (LShelfRowChosen)
        {
            LShelfSourceHide();
            CShelfFootnote.LFootnoteEntryCreate();
            return;
        }

        CShelfFootnote.CFootnotePanel.CPanelEntryClose();
        CShelfPanel.CPanelFreshOpen();
        CShelfImprint.LImprintOpen(null);
    }

    public void CShelfScribeToggle(bool editing)
    {
        if (LShelfEntrySide)
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
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void CShelfReferenceDelete()
    {
        if (LShelfEntrySide)
        {
            return;
        }

        CShelfPanel.CPanelEntryDelete();
    }

    private void LShelfClose()
    {
        CShelfImprint.CImprintByline.CBylineClose();
        CShelfEditor.CEditorClose();
        CShelfEditor.CEditorDisplay.CDisplaySound.CDisplayPlaybackCancel();
    }

    public (bool CShelfBackward, bool CShelfForward) CShelfChronicleRead()
    {
        return LShelfEntrySide
            ? CShelfEditor.CEditorDesk.CDeskChronicleRead()
            : CShelfImprint.CImprintDesk.CDeskChronicleRead();
    }

    internal bool LShelfDraftFinish(bool store)
    {
        return LShelfEntrySide
            ? CShelfEditor.LEditorFinish(store)
            : CShelfImprint.CImprintDesk.CDeskFinish(store);
    }

    public void CShelfDraftSave()
    {
        if (LShelfEntrySide)
        {
            CShelfEditor.CEditorEntrySave();
            return;
        }

        CShelfImprint.LImprintSave();
    }

    public void CShelfDraftUndo()
    {
        if (LShelfEntrySide)
        {
            CShelfEditor.CEditorDesk.CDeskUndo();
            return;
        }

        CShelfImprint.CImprintDesk.CDeskUndo();
    }

    public void CShelfDraftRedo()
    {
        if (LShelfEntrySide)
        {
            CShelfEditor.CEditorDesk.CDeskRedo();
            return;
        }

        CShelfImprint.CImprintDesk.CDeskRedo();
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
                    _cShelfVista, CPortrait.LPortraitLegendRead(_cShelfSettingsPort, "Source"), chosen));
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
