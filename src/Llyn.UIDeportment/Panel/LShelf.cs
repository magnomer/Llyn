using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LShelf
{
    private readonly LEntryPort _lEntryPort;

    private readonly LPortraitPort _lPortraitPort;

    private readonly LSettingsPort _lSettingsPort;

    private readonly Func<bool> _lShelfLeaveSeam;

    private readonly COeuvre _lShelfOeuvre;

    private LVista? _lShelfVista;

    private int _lShelfCount;

    internal LShelf(
        LDraftPort drafts, LEntryPort entries, LPortraitPort portraits, LSettingsPort settings,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);
        ArgumentNullException.ThrowIfNull(leaveSeam);

        _lEntryPort = entries;
        _lPortraitPort = portraits;
        _lSettingsPort = settings;
        _lShelfLeaveSeam = leaveSeam;
        _lShelfOeuvre = new COeuvre(entries, envoy, shownSeam);
        LShelfEditor = editor;
        LShelfImprint = new LImprint(drafts, entries, settings, envoy);
        LShelfPanel = new CPanel(
            envoy, "Source.LoadFailed", "Source",
            LImprintChangeCheck, LShelfDraftFinish, shownSeam);
        LShelfFootnote = new LFootnote(entries, portraits, editor, shownSeam, envoy, LShelfDraftFinish);
        LShelfPanel.CPanelRowsChanged += LShelfFootnote.LFootnotePanel.CPanelRowsUpdate;
        LShelfPanel.CPanelCleared += LShelfImprint.LImprintCancel;
        LShelfPanel.CPanelEdited += LShelfImprintOpen;
        LShelfPanel.CPanelDraftChanged += LShelfColophonUpdate;
        LShelfFootnote.LFootnotePanel.CPanelCleared += lectern.LLecternClear;
        LShelfFootnote.LFootnotePanel.CPanelDraftChanged += lectern.LLecternDraftShow;
        LShelfImprint.LImprintDesk.CDeskFinished += LShelfStoredShow;
        LShelfImprint.LImprintDesk.CDeskStateChanged += LShelfStateUpdate;
        LShelfEditor.LEditorStudio.CEditorDesk.CDeskStateChanged += LShelfStateUpdate;
    }

    public event Action? LShelfChanged;

    public event Action<CColophon>? LShelfColophonChanged;

    public LEditor LShelfEditor { get; }

    public CPanel LShelfPanel { get; }

    public LFootnote LShelfFootnote { get; }

    public LImprint LShelfImprint { get; }

    public bool LShelfEntrySide => LShelfFootnote.LFootnotePanel.CPanelModeEnabled;

    private bool LShelfEditing =>
        LShelfEntrySide ? LShelfFootnote.LFootnotePanel.CPanelEditing : LShelfPanel.CPanelEditing;

    public bool LShelfEditorShown => LShelfEntrySide && LShelfFootnote.LFootnotePanel.CPanelEditing;

    public bool LShelfDisplayShown => LShelfEntrySide && !LShelfFootnote.LFootnotePanel.CPanelEditing;

    public bool LShelfImprintShown => !LShelfEntrySide && LShelfPanel.CPanelEditing;

    public bool LShelfColophonShown => !LShelfEntrySide && !LShelfPanel.CPanelEditing;

    public bool LShelfViewerChecked => !LShelfEditing;

    public bool LShelfScribeChecked => LShelfEditing;

    public bool LShelfModeEnabled => LShelfPanel.CPanelModeEnabled || LShelfFootnote.LFootnotePanel.CPanelModeEnabled;

    public bool LShelfBinEnabled => !LShelfEntrySide && LShelfPanel.CPanelBinEnabled;

    public bool LShelfStoreEnabled =>
        LShelfEntrySide ? LShelfEditor.LEditorStorable : LShelfImprint.LImprintDesk.CDeskChangeCheck();

    public bool LShelfPressAllowed => LShelfFootnote.LFootnotePanel.CPanelPressAllowed || LShelfSourcePrintable;

    private bool LShelfSourcePrintable => !LShelfEntrySide && LShelfPanel.CPanelPressAllowed;

    private bool LShelfSourceShown => LShelfPanel.CPanelBinEnabled && !LShelfPanel.CPanelEditing;

    private bool LShelfRowChosen => LShelfPanel.CPanelBinEnabled || LShelfFootnote.LFootnotePanel.CPanelBinEnabled;

    public bool LShelfPortraitAllowed => LShelfFootnote.LFootnotePanel.CPanelPressAllowed;

    public bool LShelfEmpty => _lShelfCount == 0;

    public bool LShelfSieveActive => _lShelfVista?.LVistaFiltered ?? false;

    internal void LShelfVistaRestore(LVista vista, LVista footnote)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(footnote);

        _lShelfVista = vista;
        LShelfPanel.CPanelVistaRestore(vista);
        LShelfFootnote.LFootnoteVistaRestore(vista, footnote);
        LShelfImprint.LImprintVistaRestore(vista);
        LShelfEditor.LEditorStudio.CEditorVistaRestore(footnote);
    }

    public IReadOnlyList<CCatalogReference> LShelfRowsRead()
    {
        return COeuvre.COeuvreReferenceRead(
            LShelfRowsApply(_lShelfVista is LVista vista ? _lEntryPort.LEngineReferenceFind(vista) : []));
    }

    private IReadOnlyList<LCatalogReference> LShelfRowsApply(IReadOnlyList<LCatalogReference> rows)
    {
        _lShelfCount = rows.Count;
        if (LShelfSourceShown)
        {
            if (!LShelfChosenCheck(rows))
            {
                LShelfPanel.CPanelEntryClose();
            }
        }

        return rows;
    }

    private static bool LShelfChosenCheck(IReadOnlyList<LCatalogReference> rows)
    {
        foreach (LCatalogReference row in rows)
        {
            if (row.LCatalogReferenceChosen)
            {
                return true;
            }
        }

        return false;
    }

    public void LShelfQuerySet(string query)
    {
        ArgumentNullException.ThrowIfNull(query);

        _lShelfVista?.LVistaQuerySet(query);
    }

    public void LShelfOrderSet(CCatalogOrder? order)
    {
        if (_lShelfVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(CPanel.CPanelOrderRead(order) ?? vista.LVistaOrder);
    }

    public static IReadOnlyList<CCatalogOrder> LShelfOrderRead()
    {
        return
        [
            CCatalogOrder.CCatalogOrderName,
            CCatalogOrder.CCatalogOrderYear,
            CCatalogOrder.CCatalogOrderAuthor,
            CCatalogOrder.CCatalogOrderUsage,
        ];
    }

    public void LShelfSieveSet(CCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lShelfVista?.LVistaFilterSet(filter.CCatalogFilterHidden);
    }

    public string LShelfTallyRead()
    {
        return LShelfTallyRead(_lShelfVista?.LVistaChosen);
    }

    public string LShelfTallyRead(long? id)
    {
        return LShelfTallyFormat(LShelfUsageRead(id));
    }

    private int LShelfUsageRead(long? id)
    {
        return id is long stored ? _lEntryPort.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(stored) : 0;
    }

    private string LShelfTallyFormat(int count)
    {
        return LReference.LReferenceUsageFormat(count, _lSettingsPort.LEngineTextRead);
    }

    private void LShelfColophonUpdate(LDraft draft)
    {
        LShelfColophonChanged?.Invoke(_lShelfOeuvre.COeuvreColophonRead(draft));
    }

    public bool LShelfChangeCheck()
    {
        return LShelfPanel.CPanelChangeCheck() || LShelfFootnote.LFootnotePanel.CPanelChangeCheck();
    }

    private bool LImprintChangeCheck()
    {
        return LShelfImprint.LImprintDesk.CDeskChangeCheck();
    }

    private void LShelfStateUpdate()
    {
        LShelfChanged?.Invoke();
    }

    private void LShelfImprintOpen(long id)
    {
        LShelfImprint.LImprintOpen(id);
    }

    public bool LShelfLeaveConfirm()
    {
        if (!LShelfChangeCheck())
        {
            return true;
        }

        return _lShelfLeaveSeam();
    }

    public void LShelfClear()
    {
        LShelfFootnote.LFootnotePanel.CPanelEntryClose();
        LShelfPanel.CPanelEntryClose();
    }

    public void LShelfRowSelect(long? id)
    {
        if (id is null)
        {
            return;
        }

        if (!LShelfLeaveConfirm())
        {
            return;
        }

        LShelfSourceOpen(id, LShelfEditing);
    }

    public void LShelfRowShow(long id)
    {
        LShelfSourceOpen(id, LShelfEditing);
    }

    private void LShelfSourceOpen(long? id, bool editing)
    {
        LShelfFootnote.LFootnotePanel.CPanelEntryClose();
        LShelfPanel.CPanelScribeSet(editing);
        LShelfPanel.CPanelRowOpen(id);
    }

    private void LShelfStoredShow(long id)
    {
        LShelfSourceOpen(id, false);
    }

    private void LShelfSourceShow()
    {
        LShelfPanel.CPanelRowOpen(_lShelfVista?.LVistaChosen);
    }

    private void LShelfSourceHide()
    {
        LShelfPanel.CPanelScribeSet(false);
        LShelfImprint.LImprintCancel();
    }

    public void LShelfEntrySelect(long? id)
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
        LShelfFootnote.LFootnotePanel.CPanelScribeSet(editing);
        LShelfFootnote.LFootnotePanel.CPanelRowOpen(id);
    }

    public void LShelfEntryUpdate()
    {
        LShelfFootnote.LFootnotePanel.CPanelDraftUpdate();
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void LShelfFreshStart()
    {
        if (!LShelfLeaveConfirm())
        {
            return;
        }

        if (LShelfRowChosen)
        {
            LShelfSourceHide();
            LShelfFootnote.LFootnoteEntryCreate();
            return;
        }

        LShelfFootnote.LFootnotePanel.CPanelEntryClose();
        LShelfPanel.CPanelFreshOpen();
        LShelfImprint.LImprintOpen(null);
    }

    public void LShelfScribeSet(bool editing)
    {
        if (LShelfEntrySide)
        {
            LShelfFootnoteSet(editing);
            return;
        }

        LShelfPanel.CPanelScribeToggle(editing);
        if (!LShelfPanel.CPanelEditing)
        {
            LShelfImprint.LImprintCancel();
        }
    }

    private void LShelfFootnoteSet(bool editing)
    {
        LShelfFootnote.LFootnotePanel.CPanelScribeToggle(editing);
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void LShelfScribeRestore(bool editing)
    {
        LShelfPanel.CPanelScribeRestore(editing);
    }

    public void LShelfDelete()
    {
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfPanel.CPanelEntryDelete();
    }

    public (bool LShelfBackward, bool LShelfForward) LShelfChronicleRead()
    {
        return LShelfEntrySide
            ? LShelfEditor.LEditorStudio.CEditorDesk.CDeskChronicleRead()
            : LShelfImprint.LImprintDesk.CDeskChronicleRead();
    }

    public bool LShelfDraftFinish(bool store)
    {
        return LShelfEntrySide
            ? LShelfEditor.LEditorStudio.CEditorFinish(store)
            : LShelfImprint.LImprintDesk.CDeskFinish(store);
    }

    public void LShelfStoreRun()
    {
        if (LShelfEntrySide)
        {
            LShelfEditor.LEditorStudio.CEditorEntrySave();
            return;
        }

        LShelfImprint.LImprintSave();
    }

    public void LShelfUndo()
    {
        if (LShelfEntrySide)
        {
            LShelfEditor.LEditorStudio.CEditorDesk.CDeskUndo();
            return;
        }

        LShelfImprint.LImprintDesk.CDeskUndo();
    }

    public void LShelfRedo()
    {
        if (LShelfEntrySide)
        {
            LShelfEditor.LEditorStudio.CEditorDesk.CDeskRedo();
            return;
        }

        LShelfImprint.LImprintDesk.CDeskRedo();
    }

    public Task LShelfPortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)
    {
        if (LShelfFootnote.LFootnotePanel.CPanelPressAllowed)
        {
            return LShelfFootnote.LFootnotePortraitPrint(label, ticket);
        }

        if (LShelfSourcePrintable)
        {
            return _lPortraitPort.LEnginePortraitPrint(
                _lShelfVista, LAtlas.LAtlasLegendRead(legend), QPortrait.QPortraitTicketRead(ticket));
        }

        return Task.CompletedTask;
    }

    internal void LShelfVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LShelfVistaRestore(
            atelier.CAtelierVistaStart(
                "reference", CSubject.CSubjectReference, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "footnote", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }

    public string LShelfFileRead()
    {
        return LVista.LVistaFileRead(LShelfFootnote.LFootnotePanel.CPanelVista);
    }

    public Task LShelfPortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        return _lPortraitPort.LEnginePortraitExport(
            LShelfFootnote.LFootnotePanel.CPanelVista,
            path,
            QPortrait.QPortraitMediumRead(format),
            QPortrait.QPortraitLabelRead(label));
    }
}
