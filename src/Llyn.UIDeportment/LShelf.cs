using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LShelf
{
    private readonly LEngine _lEngine;

    private readonly Func<bool> _lShelfLeaveSeam;

    private readonly Func<int, bool> _lShelfRemovalSeam;

    private LVista? _lShelfVista;

    private int _lShelfCount;

    private bool _lShelfImprintStorable;

    private bool _lShelfEditorStorable;

    public LShelf(
        LEngine engine,
        Func<bool> sourceChangeSeam,
        Func<bool> entryChangeSeam,
        Func<bool> shownSeam,
        Func<bool> leaveSeam,
        Func<int, bool> removalSeam)
    {
        ArgumentNullException.ThrowIfNull(engine);
        ArgumentNullException.ThrowIfNull(leaveSeam);
        ArgumentNullException.ThrowIfNull(removalSeam);

        _lEngine = engine;
        _lShelfLeaveSeam = leaveSeam;
        _lShelfRemovalSeam = removalSeam;
        LShelfPanel = new LPanel("Source.LoadFailed", sourceChangeSeam, shownSeam, leaveSeam, LShelfDeleteConfirm);
        LShelfFootnote = new LFootnote(engine, entryChangeSeam, shownSeam, leaveSeam);
        LShelfPanel.LPanelRowsChanged += LShelfFootnote.LFootnotePanel.LPanelRowsUpdate;
    }

    public event Action? LShelfChanged;

    public event Action? LShelfOpened;

    public event Action? LShelfClosed;

    public LPanel LShelfPanel { get; }

    public LFootnote LShelfFootnote { get; }

    public bool LShelfEntrySide => LShelfFootnote.LFootnotePanel.LPanelModeEnabled;

    public bool LShelfEditing =>
        LShelfEntrySide ? LShelfFootnote.LFootnotePanel.LPanelEditing : LShelfPanel.LPanelEditing;

    public bool LShelfEditorShown => LShelfEntrySide && LShelfFootnote.LFootnotePanel.LPanelEditing;

    public bool LShelfDisplayShown => LShelfEntrySide && !LShelfFootnote.LFootnotePanel.LPanelEditing;

    public bool LShelfImprintShown => !LShelfEntrySide && LShelfPanel.LPanelEditing;

    public bool LShelfColophonShown => !LShelfEntrySide && !LShelfPanel.LPanelEditing;

    public bool LShelfViewerChecked => !LShelfEditing;

    public bool LShelfScribeChecked => LShelfEditing;

    public bool LShelfModeEnabled => LShelfPanel.LPanelModeEnabled || LShelfFootnote.LFootnotePanel.LPanelModeEnabled;

    public bool LShelfBinEnabled => !LShelfEntrySide && LShelfPanel.LPanelBinEnabled;

    public bool LShelfStoreEnabled => LShelfEntrySide ? _lShelfEditorStorable : _lShelfImprintStorable;

    public bool LShelfPressAllowed => LShelfFootnote.LFootnotePanel.LPanelPressAllowed || LShelfSourcePrintable;

    private bool LShelfSourcePrintable => !LShelfEntrySide && LShelfPanel.LPanelPressAllowed;

    private bool LShelfSourceShown => LShelfPanel.LPanelBinEnabled && !LShelfPanel.LPanelEditing;

    private bool LShelfRowChosen => LShelfPanel.LPanelBinEnabled || LShelfFootnote.LFootnotePanel.LPanelBinEnabled;

    public bool LShelfPortraitAllowed => LShelfFootnote.LFootnotePanel.LPanelPressAllowed;

    public bool LShelfEmpty => _lShelfCount == 0;

    public bool LShelfSieveActive => _lShelfVista?.LVistaFiltered ?? false;

    public void LShelfVistaRestore(LVista vista, LVista footnote)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(footnote);

        _lShelfVista = vista;
        LShelfPanel.LPanelVistaRestore(vista);
        LShelfFootnote.LFootnoteVistaRestore(vista, footnote);
    }

    public IReadOnlyList<LCatalogReference> LShelfRowsRead()
    {
        return LShelfRowsApply(_lShelfVista is LVista vista ? _lEngine.LEngineReferenceFind(vista) : []);
    }

    private IReadOnlyList<LCatalogReference> LShelfRowsApply(IReadOnlyList<LCatalogReference> rows)
    {
        _lShelfCount = rows.Count;
        if (LShelfSourceShown)
        {
            if (!LShelfChosenCheck(rows))
            {
                LShelfPanel.LPanelClear();
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

    public void LShelfOrderSet(string? choice)
    {
        if (choice is null)
        {
            return;
        }

        if (_lShelfVista is not LVista vista)
        {
            return;
        }

        vista.LVistaOrderSet(LCatalog.LCatalogOrderParse(choice, vista.LVistaOrder));
    }

    public void LShelfSieveSet(LCatalogFilter filter)
    {
        ArgumentNullException.ThrowIfNull(filter);

        _lShelfVista?.LVistaFilterSet(filter);
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
        return id is long stored ? _lEngine.LEngineUsageRead(LOwner.LOwnerReference).GetValueOrDefault(stored) : 0;
    }

    private static string LShelfTallyFormat(int count)
    {
        return LReference.LReferenceUsageFormat(count, LLocalization.LLocalizationTextRead);
    }

    public LColophon LShelfColophonRead(LDraft draft)
    {
        ArgumentNullException.ThrowIfNull(draft);

        LReference reference = draft.LDraftReference
            ?? throw new InvalidOperationException("The shelf draft holds no reference.");
        return reference.LReferenceColophonRead(
            draft.LDraftAuthor, LShelfTallyRead(), LLocalization.LLocalizationTextRead);
    }

    public bool LShelfChangeCheck()
    {
        return LShelfPanel.LPanelChangeCheck() || LShelfFootnote.LFootnotePanel.LPanelChangeCheck();
    }

    public bool LShelfLeaveConfirm()
    {
        if (!LShelfChangeCheck())
        {
            return true;
        }

        return _lShelfLeaveSeam();
    }

    private bool LShelfDeleteConfirm()
    {
        return _lShelfRemovalSeam(LShelfUsageRead(_lShelfVista?.LVistaChosen));
    }

    public void LShelfClear()
    {
        LShelfFootnote.LFootnotePanel.LPanelClear();
        LShelfPanel.LPanelClear();
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

    private void LShelfSourceOpen(long? id, bool editing)
    {
        LShelfFootnote.LFootnotePanel.LPanelClear();
        LShelfPanel.LPanelScribeShow(editing);
        LShelfPanel.LPanelRowShow(id);
    }

    public void LShelfStoredShow(long id)
    {
        LShelfSourceOpen(id, false);
    }

    private void LShelfSourceShow()
    {
        LShelfPanel.LPanelRowShow(_lShelfVista?.LVistaChosen);
    }

    private void LShelfSourceHide()
    {
        LShelfPanel.LPanelScribeShow(false);
        LShelfClosed?.Invoke();
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
        LShelfFootnote.LFootnotePanel.LPanelScribeShow(editing);
        LShelfFootnote.LFootnotePanel.LPanelRowShow(id);
    }

    public void LShelfEntryUpdate()
    {
        LShelfFootnote.LFootnotePanel.LPanelDraftUpdate();
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

        LShelfFootnote.LFootnotePanel.LPanelClear();
        LShelfPanel.LPanelFreshOpen();
        LShelfOpened?.Invoke();
    }

    public void LShelfScribeSet(bool editing)
    {
        if (LShelfEntrySide)
        {
            LShelfFootnoteSet(editing);
            return;
        }

        LShelfPanel.LPanelScribeSet(editing);
        if (!LShelfPanel.LPanelEditing)
        {
            LShelfClosed?.Invoke();
        }
    }

    private void LShelfFootnoteSet(bool editing)
    {
        LShelfFootnote.LFootnotePanel.LPanelScribeSet(editing);
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfSourceShow();
    }

    public void LShelfScribeRestore(bool editing)
    {
        LShelfPanel.LPanelScribeRestore(editing);
    }

    public void LShelfDelete()
    {
        if (LShelfEntrySide)
        {
            return;
        }

        LShelfPanel.LPanelDelete();
    }

    public void LShelfEditorSet(bool storable)
    {
        _lShelfEditorStorable = storable;
        LShelfChanged?.Invoke();
    }

    public void LShelfImprintSet(bool storable)
    {
        _lShelfImprintStorable = storable;
        LShelfChanged?.Invoke();
    }

    public Task LShelfPortraitPrint(LPortraitLabel label, LPortraitLegend legend, LPressTicket ticket)
    {
        if (LShelfFootnote.LFootnotePanel.LPanelPressAllowed)
        {
            return LShelfFootnote.LFootnotePortraitPrint(label, ticket);
        }

        if (LShelfSourcePrintable)
        {
            return _lEngine.LEnginePortraitPrint(_lShelfVista, legend, ticket);
        }

        return Task.CompletedTask;
    }
}
