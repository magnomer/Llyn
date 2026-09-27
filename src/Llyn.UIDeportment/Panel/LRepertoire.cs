using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIDeportment;

public sealed class LRepertoire
{
    private readonly Func<Func<bool, bool>, bool> _lRepertoireLeaveSeam;

    internal LRepertoire(
        LDraftPort drafts,
        LEntryPort entries,
        LPortraitPort portraits,
        LSettingsPort settings,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<Func<bool, bool>, bool> leaveSeam,
        Func<int, bool> removalSeam,
        Func<bool> unreadableSeam)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(settings);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);
        ArgumentNullException.ThrowIfNull(leaveSeam);

        _lRepertoireLeaveSeam = leaveSeam;
        LRepertoireEditor = editor;
        LRepertoireDesk = new LDesk(drafts, "Situation", unreadableSeam);
        LRepertoireAtlas = new LAtlas(
            entries, portraits, settings, LRepertoireDesk.LDeskChangeCheck, shownSeam, LRepertoireLeaveConfirm,
            removalSeam);
        LRepertoireOccurrence = new LOccurrence(
            entries, portraits, editor.LEditorDesk.LDeskChangeCheck, shownSeam, LRepertoireLeaveConfirm);
        LRepertoireAtlas.LAtlasPanel.LPanelRowsChanged += LRepertoireOccurrence.LOccurrencePanel.LPanelRowsUpdate;
        LRepertoireAtlas.LAtlasPanel.LPanelEdited += id => LRepertoireDraftStart(id);
        LRepertoireAtlas.LAtlasPanel.LPanelCleared += LRepertoireDraftCancel;
        LRepertoireAtlas.LAtlasPanel.LPanelDraftChanged += LRepertoireSituationUpdate;
        LRepertoireOccurrence.LOccurrencePanel.LPanelEdited += id => editor.LEditorOpen(id);
        LRepertoireOccurrence.LOccurrencePanel.LPanelCleared += editor.LEditorClose;
        LRepertoireOccurrence.LOccurrencePanel.LPanelCleared += lectern.LLecternClear;
        LRepertoireOccurrence.LOccurrencePanel.LPanelDraftChanged += lectern.LLecternDraftShow;
        LRepertoireDesk.LDeskStateChanged += LRepertoireStateUpdate;
        editor.LEditorStateChanged += LRepertoireStateUpdate;
    }

    public event Action? LRepertoireChanged;

    public event Action<CSituationDraft?>? LRepertoireScenarioChanged;

    public event Action<CSituationDraft>? LRepertoireSituationChanged;

    public event Action<string, Exception>? LRepertoireFailed;

    public event Action? LRepertoireInquestCleared;

    private LEditor LRepertoireEditor { get; }

    public LDesk LRepertoireDesk { get; }

    public LAtlas LRepertoireAtlas { get; }

    public LOccurrence LRepertoireOccurrence { get; }

    private bool LRepertoireOccurrenceSide => LRepertoireOccurrence.LOccurrencePanel.LPanelModeEnabled;

    public bool LRepertoireScenarioShown => !LRepertoireOccurrenceSide && LRepertoireAtlas.LAtlasPanel.LPanelEditing;

    public bool LRepertoireVignetteShown => !LRepertoireOccurrenceSide && !LRepertoireAtlas.LAtlasPanel.LPanelEditing;

    public bool LRepertoireDisplayShown =>
        LRepertoireOccurrenceSide && !LRepertoireOccurrence.LOccurrencePanel.LPanelEditing;

    public bool LRepertoireEditorShown => LRepertoireOccurrence.LOccurrencePanel.LPanelEditing;

    public bool LRepertoireVignetteHeld => LRepertoireAtlas.LAtlasPanel.LPanelBinEnabled;

    public bool LRepertoireVignetteBlank => !LRepertoireAtlas.LAtlasPanel.LPanelBinEnabled;

    public bool LRepertoireScribeChecked => LRepertoireScenarioShown || LRepertoireEditorShown;

    public bool LRepertoireViewerChecked => !LRepertoireScribeChecked;

    public bool LRepertoireModeEnabled => LRepertoireOccurrenceSide || LRepertoireAtlas.LAtlasPanel.LPanelModeEnabled;

    public bool LRepertoireBinEnabled => !LRepertoireOccurrenceSide && LRepertoireAtlas.LAtlasPanel.LPanelBinEnabled;

    public bool LRepertoireStoreEnabled =>
        LRepertoireEditorShown ? LRepertoireEditor.LEditorStorable : LRepertoireDesk.LDeskChanged;

    public bool LRepertoirePressAllowed =>
        LRepertoireDisplayShown || (LRepertoireVignetteShown && LRepertoireVignetteHeld);

    public bool LRepertoirePortraitAllowed => LRepertoireDisplayShown;

    private void LRepertoireDraftStart(long? id)
    {
        LRepertoireDesk.LDeskStart("Repertoire", CSubject.CSubjectSituation, id);
        LRepertoireScenarioChanged?.Invoke(LRepertoireScenarioRead());
    }

    public CSituationDraft? LRepertoireScenarioRead()
    {
        try
        {
            return LAtlas.LAtlasSituationRead(LRepertoireDesk.LDeskRead()?.LDraftSituation);
        }
        catch (Exception exception)
        {
            LRepertoireFailed?.Invoke("Situation.HoldFailed", exception);
            return null;
        }
    }

    private void LRepertoireDraftCancel()
    {
        LRepertoireDesk.LDeskCancel();
        LRepertoireScenarioChanged?.Invoke(null);
    }

    private void LRepertoireSituationUpdate(LDraft draft)
    {
        if (LAtlas.LAtlasSituationRead(draft.LDraftSituation) is CSituationDraft situation)
        {
            LRepertoireSituationChanged?.Invoke(situation);
        }
    }

    private void LRepertoireStateUpdate()
    {
        LRepertoireChanged?.Invoke();
    }

    public void LRepertoireSituationShow(long id)
    {
        bool editing = LRepertoireScribeChecked;
        LRepertoireSituationShow(id, editing);
        if (LRepertoireVignetteHeld)
        {
            return;
        }

        if (!LRepertoireAtlas.LAtlasNarrowed)
        {
            return;
        }

        LRepertoireInquestClear();
        LRepertoireSituationShow(id, editing);
    }

    private void LRepertoireSituationShow(long id, bool editing)
    {
        LRepertoireOccurrence.LOccurrencePanel.LPanelClear();
        LRepertoireAtlas.LAtlasPanel.LPanelScribeShow(editing);
        LRepertoireAtlas.LAtlasPanel.LPanelRowShow(id);
    }

    public void LRepertoireSelect(long? id, Action record)
    {
        ArgumentNullException.ThrowIfNull(record);

        if (id is not long chosen)
        {
            return;
        }

        bool editing = LRepertoireScribeChecked;
        if (!LRepertoireLeaveConfirm(false))
        {
            return;
        }

        record();
        LRepertoireSituationShow(chosen, editing);
    }

    private void LRepertoireOccurrenceOpen(long id, bool editing)
    {
        if (!LRepertoireOccurrence.LOccurrencePanel.LPanelRowShow(id))
        {
            return;
        }

        LRepertoireDesk.LDeskCancel();
        LRepertoireAtlas.LAtlasPanel.LPanelScribeShow(false);
        if (editing)
        {
            LRepertoireOccurrence.LOccurrencePanel.LPanelScribeSet(true);
        }
    }

    public void LRepertoireOccurrenceSelect(long? id)
    {
        if (id is not long chosen)
        {
            return;
        }

        bool editing = LRepertoireScribeChecked;
        if (!LRepertoireLeaveConfirm())
        {
            return;
        }

        LRepertoireOccurrenceOpen(chosen, editing);
    }

    public void LRepertoireFreshStart()
    {
        if (!LRepertoireLeaveConfirm())
        {
            return;
        }

        if (LRepertoireRowHeld)
        {
            LRepertoireOccurrenceCreate();
            return;
        }

        LRepertoireOccurrence.LOccurrencePanel.LPanelClear();
        LRepertoireAtlas.LAtlasPanel.LPanelFreshOpen();
        LRepertoireDraftStart(null);
    }

    public void LRepertoireScribeSet(bool editing)
    {
        if (LRepertoireOccurrenceSide)
        {
            LRepertoireOccurrence.LOccurrencePanel.LPanelScribeSet(editing);
            if (!LRepertoireOccurrenceSide)
            {
                LRepertoireSituationRestore(editing);
            }

            return;
        }

        LRepertoireAtlas.LAtlasPanel.LPanelScribeSet(editing);
        if (!LRepertoireAtlas.LAtlasPanel.LPanelEditing)
        {
            LRepertoireDesk.LDeskCancel();
        }
    }

    private void LRepertoireSituationRestore(bool editing)
    {
        if (LRepertoireAtlas.LAtlasChosen is long chosen)
        {
            LRepertoireSituationShow(chosen, editing);
            return;
        }

        LRepertoireClear();
    }

    public void LRepertoireClear()
    {
        LRepertoireOccurrence.LOccurrencePanel.LPanelClear();
        LRepertoireAtlas.LAtlasPanel.LPanelClear();
    }

    public void LRepertoireRowsApply(IReadOnlyList<CCatalogSituation> rows)
    {
        ArgumentNullException.ThrowIfNull(rows);

        if (!LRepertoireRowShown)
        {
            return;
        }

        foreach (CCatalogSituation row in rows)
        {
            if (row.CCatalogSituationChosen)
            {
                return;
            }
        }

        LRepertoireClear();
    }

    private bool LRepertoireRowShown => LRepertoireVignetteShown && LRepertoireVignetteHeld;

    private bool LRepertoireRowHeld =>
        LRepertoireAtlas.LAtlasPanel.LPanelBinEnabled || LRepertoireOccurrence.LOccurrencePanel.LPanelBinEnabled;

    private void LRepertoireInquestClear()
    {
        LRepertoireAtlas.LAtlasInquestSet(string.Empty);
        LRepertoireAtlas.LAtlasMeshSet(new CCatalogFilter([]));
        LRepertoireInquestCleared?.Invoke();
    }

    public void LRepertoireSave()
    {
        if (LRepertoireEditorShown)
        {
            LRepertoireEditor.LEditorSave();
            return;
        }

        if (LRepertoireDesk.LDeskChangeCheck())
        {
            LRepertoireDesk.LDeskFinish(true, LRepertoireStoredShow);
        }
    }

    private void LRepertoireStoredShow(long id)
    {
        LRepertoireAtlas.LAtlasPanel.LPanelScribeShow(false);
        LRepertoireSituationShow(id);
    }

    public void LRepertoireDelete()
    {
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        LRepertoireAtlas.LAtlasPanel.LPanelDelete();
    }

    private void LRepertoireOccurrenceCreate()
    {
        long? chosen = LRepertoireAtlas.LAtlasChosen;
        LRepertoireDesk.LDeskCancel();
        LRepertoireAtlas.LAtlasPanel.LPanelScribeShow(false);
        LRepertoireOccurrence.LOccurrencePanel.LPanelFreshOpen();
        LRepertoireEditor.LEditorOpen(null);
        if (chosen is long id)
        {
            LRepertoireEditor.LEditorSituationAdd(id);
        }
    }

    public bool LRepertoireChangeCheck()
    {
        LPanel occurrence = LRepertoireOccurrence.LOccurrencePanel;
        LPanel atlas = LRepertoireAtlas.LAtlasPanel;
        return occurrence.LPanelChangeCheck() || atlas.LPanelChangeCheck();
    }

    public bool LRepertoireLeaveConfirm()
    {
        return LRepertoireLeaveConfirm(true);
    }

    private bool LRepertoireLeaveConfirm(bool shown)
    {
        if (!LRepertoireChangeCheck())
        {
            return true;
        }

        return _lRepertoireLeaveSeam(shown ? LRepertoireDraftFinish : LRepertoireDraftClose);
    }

    public void LRepertoireEntryUpdate()
    {
        if (!LRepertoireOccurrence.LOccurrencePanel.LPanelBinEnabled)
        {
            return;
        }

        bool editing = LRepertoireScribeChecked;
        LRepertoireOccurrence.LOccurrencePanel.LPanelDraftUpdate();
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        LRepertoireSituationRestore(editing);
    }

    public bool LRepertoireDraftFinish(bool store)
    {
        return LRepertoireEditorShown
            ? LRepertoireEditor.LEditorFinish(store)
            : LRepertoireDesk.LDeskFinish(store, LRepertoireStoredShow);
    }

    private bool LRepertoireDraftClose(bool store)
    {
        return LRepertoireEditorShown ? LRepertoireEditor.LEditorFinish(store) : LRepertoireDesk.LDeskFinish(store);
    }

    public (bool LDeskBackward, bool LDeskForward) LRepertoireChronicleRead()
    {
        return LRepertoireEditorShown
            ? LRepertoireEditor.LEditorDesk.LDeskChronicleRead()
            : LRepertoireDesk.LDeskChronicleRead();
    }

    public void LRepertoireUndo()
    {
        if (LRepertoireEditorShown)
        {
            LRepertoireEditor.LEditorDesk.LDeskUndo();
            return;
        }

        LRepertoireChronicleRun(LRepertoireDesk.LDeskUndo);
    }

    public void LRepertoireRedo()
    {
        if (LRepertoireEditorShown)
        {
            LRepertoireEditor.LEditorDesk.LDeskRedo();
            return;
        }

        LRepertoireChronicleRun(LRepertoireDesk.LDeskRedo);
    }

    private void LRepertoireChronicleRun(Action step)
    {
        try
        {
            step();
        }
        catch (Exception exception)
        {
            LRepertoireFailed?.Invoke("Situation.HoldFailed", exception);
        }
    }

    public Task LRepertoirePortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)
    {
        if (LRepertoireDisplayShown)
        {
            return LRepertoireOccurrence.LOccurrencePortraitPrint(
                LPanel.LPanelLabelRead(label), LPanel.LPanelTicketRead(ticket));
        }

        if (LRepertoirePressAllowed)
        {
            return LRepertoireAtlas.LAtlasPortraitPrint(legend, ticket);
        }

        return Task.CompletedTask;
    }

    public Task LRepertoirePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        if (!LRepertoirePortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return LRepertoireOccurrence.LOccurrencePortraitExport(
            path, LPanel.LPanelMediumRead(format), LPanel.LPanelLabelRead(label));
    }

    public bool LRepertoireVacantCheck(string? text)
    {
        return string.IsNullOrEmpty(text);
    }

    public string LRepertoireMeasureRead(string? text, string hint)
    {
        return string.IsNullOrEmpty(text) ? hint : text;
    }

    internal void LRepertoireVistaRestore(LVista vista, LVista occurrence)
    {
        ArgumentNullException.ThrowIfNull(vista);
        ArgumentNullException.ThrowIfNull(occurrence);

        LRepertoireAtlas.LAtlasVistaRestore(vista);
        LRepertoireOccurrence.LOccurrenceVistaRestore(vista, occurrence);
        LRepertoireEditor.LEditorVistaRestore(occurrence);
    }

    internal void LRepertoireVistaRestore(LWindow window)
    {
        ArgumentNullException.ThrowIfNull(window);

        LRepertoireVistaRestore(
            window.LWindowVistaStart("repertoire", LSubject.LSubjectSituation, LCatalogOrder.LCatalogOrderName),
            window.LWindowVistaStart("occurrence", LSubject.LSubjectEntry, LCatalogOrder.LCatalogOrderHeadword));
    }
}
