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

    private LVista? _cShelfVista;

    private int _cShelfCount;

    private CShelf(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(atelier);
        ArgumentNullException.ThrowIfNull(shownSeam);
        ArgumentNullException.ThrowIfNull(envoy);

        _cShelfAtelier = atelier;
        _cShelfEnvoy = envoy;
        _cShelfEntryPort = atelier.CAtelierEntryPort;
        _cShelfPortraitPort = atelier.CAtelierPortraitPort;
        _cShelfSettingsPort = atelier.CAtelierSettingsPort;
        CEditor editor = CEditor.CEditorCreate(atelier, envoy);
        CShelfEditor = editor;
        CShelfImprint = new CImprint(atelier.CAtelierDraftPort, atelier.CAtelierEntryPort, envoy);
        CShelfPanel = new CPanel(
            envoy, "Source.LoadFailed", "Source",
            CShelfImprint.CImprintDesk.CDeskChangeCheck, CShelfDraftFinish, shownSeam);
        CShelfFootnote = new CFootnote(
            atelier.CAtelierEntryPort, atelier.CAtelierPortraitPort, editor, envoy, CShelfDraftFinish, shownSeam);
        CShelfPanel.CPanelRowsChanged += CShelfFootnote.CFootnotePanel.CPanelRowsResonate;
        CShelfPanel.CPanelCleared += CShelfImprint.LImprintCancel;
        CShelfPanel.CPanelEdited += id => CShelfImprint.LImprintOpen(id);
        CShelfPanel.CPanelDraftChanged +=
            draft => CShelfColophonChanged?.Invoke(COeuvre.LOeuvreColophonRead(_cShelfEntryPort, draft));
        CShelfImprint.CImprintDesk.CDeskFinished += id => LShelfSourceOpen(id, false);
        CShelfImprint.CImprintDesk.CDeskStateChanged += () => CShelfChanged?.Invoke();
        editor.CEditorDesk.CDeskStateChanged += () => CShelfChanged?.Invoke();
    }

    public static CShelf CShelfCreate(CAtelier atelier, Func<bool> shownSeam, CEnvoy envoy)
    {
        return new CShelf(atelier, shownSeam, envoy);
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
        LShelfEntrySide ? CShelfEditor.CEditorDesk.CDeskStorable : CShelfImprint.CImprintDesk.CDeskChangeCheck();

    public bool CShelfPressAllowed => CShelfFootnote.CFootnotePanel.CPanelPressAllowed || LShelfSourcePrintable;

    public bool CShelfPortraitAllowed => CShelfFootnote.CFootnotePanel.CPanelPressAllowed;

    public bool CShelfEmpty => _cShelfCount == 0;

    public bool CShelfFiltered => _cShelfVista?.LVistaFiltered ?? false;

    private bool LShelfEntrySide => CShelfFootnote.CFootnotePanel.CPanelModeEnabled;

    private bool LShelfEditing =>
        LShelfEntrySide ? CShelfFootnote.CFootnotePanel.CPanelEditing : CShelfPanel.CPanelEditing;

    private bool LShelfSourcePrintable => !LShelfEntrySide && CShelfPanel.CPanelPressAllowed;

    private bool LShelfSourceShown => CShelfPanel.CPanelBinEnabled && !CShelfPanel.CPanelEditing;

    private bool LShelfRowChosen =>
        CShelfPanel.CPanelBinEnabled || CShelfFootnote.CFootnotePanel.CPanelBinEnabled;

    public void CShelfVistaRestore()
    {
        LVista vista = _cShelfAtelier.CAtelierVistaStart(
            "reference", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName);
        LVista footnote = _cShelfAtelier.CAtelierVistaStart(
            "footnote", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword);
        _cShelfVista = vista;
        CShelfPanel.CPanelVistaRestore(vista);
        CShelfFootnote.LFootnoteVistaRestore(vista, footnote);
        CShelfImprint.CImprintDesk.CDeskVistaRestore(vista);
        CShelfEditor.LEditorVistaRestore(footnote);
    }

    public IReadOnlyList<CCatalogReference> CShelfRowsRead()
    {
        IReadOnlyList<CCatalogReference> rows = COeuvre.COeuvreReferenceRead(
            _cShelfVista is LVista vista ? _cShelfEntryPort.LEngineReferenceFind(vista) : []);
        _cShelfCount = rows.Count;
        if (LShelfSourceShown && !rows.Any(static row => row.CCatalogReferenceChosen))
        {
            CShelfPanel.CPanelEntryClose();
        }

        return rows;
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

    public string CShelfTallyRead()
    {
        return _cShelfEntryPort.LEngineTallyRead(_cShelfVista?.LVistaChosen);
    }

    public bool CShelfChangeRead()
    {
        return CShelfPanel.CPanelChangeCheck() || CShelfFootnote.CFootnotePanel.CPanelChangeCheck();
    }

    public bool CShelfLeaveConfirm()
    {
        if (!CShelfChangeRead())
        {
            return true;
        }

        if (_cShelfEnvoy.CEnvoyLeaveConfirm() is not bool store)
        {
            return false;
        }

        return !store || CShelfDraftFinish(true);
    }

    public void CShelfReferenceClose()
    {
        CShelfFootnote.CFootnotePanel.CPanelEntryClose();
        CShelfPanel.CPanelEntryClose();
    }

    public void CShelfReferenceSelect(long? id, Action record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (id is null)
        {
            return;
        }

        if (!CShelfLeaveConfirm())
        {
            return;
        }

        record();
        LShelfSourceOpen(id, LShelfEditing);
    }

    public void CShelfReferenceOpen(long id)
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

        if (!CShelfLeaveConfirm())
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

    public void CShelfEntryResonate()
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
        if (!CShelfLeaveConfirm())
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

    public (bool CShelfBackward, bool CShelfForward) CShelfChronicleRead()
    {
        return LShelfEntrySide
            ? CShelfEditor.CEditorDesk.CDeskChronicleRead()
            : CShelfImprint.CImprintDesk.CDeskChronicleRead();
    }

    public bool CShelfDraftFinish(bool store)
    {
        return LShelfEntrySide
            ? CShelfEditor.CEditorFinish(store)
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
