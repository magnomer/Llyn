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
        CAtelier atelier,
        LDraftPort drafts,
        LEntryPort entries,
        LPortraitPort portraits,
        LEditor editor,
        LLectern lectern,
        Func<bool> shownSeam,
        Func<Func<bool, bool>, bool> leaveSeam,
        CEnvoy envoy)
    {
        ArgumentNullException.ThrowIfNull(drafts);
        ArgumentNullException.ThrowIfNull(entries);
        ArgumentNullException.ThrowIfNull(portraits);
        ArgumentNullException.ThrowIfNull(editor);
        ArgumentNullException.ThrowIfNull(lectern);
        ArgumentNullException.ThrowIfNull(leaveSeam);

        _lRepertoireLeaveSeam = leaveSeam;
        LRepertoireEditor = editor;
        LRepertoireDesk = new CDesk(drafts, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation);
        LRepertoireAtlas = CAtlas.CAtlasCreate(
            atelier,
            LRepertoireDesk,
            shownSeam,
            envoy,
            store => LRepertoireSession!.CSessionFinish(store));
        LRepertoireOccurrence = new LOccurrence(
            entries,
            portraits,
            editor.LEditorStudio.CEditorDesk.CDeskChangeCheck,
            shownSeam,
            envoy,
            store => LRepertoireSession!.CSessionFinish(store));
        LRepertoireAtlas.CAtlasPanel.CPanelRowsChanged += LRepertoireOccurrence.LOccurrencePanel.CPanelRowsUpdate;
        LRepertoireSession = new CSession(
            LRepertoireDesk,
            [LRepertoireOccurrence.LOccurrencePanel.CPanelChangeCheck, LRepertoireAtlas.CAtlasPanel.CPanelChangeCheck],
            editor.LEditorStudio.CEditorDesk,
            () => LRepertoireOccurrence.LOccurrencePanel.CPanelEditing,
            editor.LEditorStudio.CEditorFinish,
            static () => true,
            LRepertoireStoredShow);
        LRepertoireSession.CSessionHeld += () => LRepertoireScenarioChanged?.Invoke(LRepertoireScenarioRead());
        LRepertoireSession.CSessionChanged += () => LRepertoireChanged?.Invoke();
        LRepertoireSession.CSessionFailed += (key, exception) => LRepertoireFailed?.Invoke(key, exception);
        LRepertoireAtlas.CAtlasPanel.CPanelEdited += id => LRepertoireSession.CSessionStart(id);
        LRepertoireAtlas.CAtlasPanel.CPanelCleared += LRepertoireSession.CSessionCancel;
        LRepertoireAtlas.CAtlasPanel.CPanelDraftChanged += LRepertoireSituationUpdate;
        LRepertoireOccurrence.LOccurrencePanel.CPanelEdited += id => editor.LEditorStudio.CEditorEntryOpen(id);
        LRepertoireOccurrence.LOccurrencePanel.CPanelCleared += editor.LEditorStudio.CEditorDesk.CDeskCancel;
        LRepertoireOccurrence.LOccurrencePanel.CPanelCleared += lectern.LLecternClear;
        LRepertoireOccurrence.LOccurrencePanel.CPanelDraftChanged += lectern.LLecternDraftShow;
    }

    public event Action? LRepertoireChanged;

    public event Action<CSituationDraft?>? LRepertoireScenarioChanged;

    public event Action<CSituationDraft>? LRepertoireSituationChanged;

    public event Action<string, Exception>? LRepertoireFailed;

    public event Action? LRepertoireInquestCleared;

    private LEditor LRepertoireEditor { get; }

    public CDesk LRepertoireDesk { get; }

    public CSession LRepertoireSession { get; }

    public CAtlas LRepertoireAtlas { get; }

    public LOccurrence LRepertoireOccurrence { get; }

    private bool LRepertoireOccurrenceSide => LRepertoireOccurrence.LOccurrencePanel.CPanelModeEnabled;

    public bool LRepertoireScenarioShown => !LRepertoireOccurrenceSide && LRepertoireAtlas.CAtlasPanel.CPanelEditing;

    public bool LRepertoireVignetteShown => !LRepertoireOccurrenceSide && !LRepertoireAtlas.CAtlasPanel.CPanelEditing;

    public bool LRepertoireDisplayShown =>
        LRepertoireOccurrenceSide && !LRepertoireOccurrence.LOccurrencePanel.CPanelEditing;

    public bool LRepertoireEditorShown => LRepertoireOccurrence.LOccurrencePanel.CPanelEditing;

    public bool LRepertoireVignetteHeld => LRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool LRepertoireVignetteBlank => !LRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool LRepertoireScribeChecked => LRepertoireScenarioShown || LRepertoireEditorShown;

    public bool LRepertoireViewerChecked => !LRepertoireScribeChecked;

    public bool LRepertoireModeEnabled => LRepertoireOccurrenceSide || LRepertoireAtlas.CAtlasPanel.CPanelModeEnabled;

    public bool LRepertoireBinEnabled => !LRepertoireOccurrenceSide && LRepertoireAtlas.CAtlasPanel.CPanelBinEnabled;

    public bool LRepertoireStoreEnabled =>
        LRepertoireEditorShown ? LRepertoireEditor.LEditorStorable : LRepertoireDesk.CDeskChanged;

    public bool LRepertoirePressAllowed =>
        LRepertoireDisplayShown || (LRepertoireVignetteShown && LRepertoireVignetteHeld);

    public bool LRepertoirePortraitAllowed => LRepertoireDisplayShown;

    public CSituationDraft? LRepertoireScenarioRead()
    {
        try
        {
            return CAtlas.CAtlasSituationRead(LRepertoireDesk.CDeskRead()?.LDraftSituation);
        }
        catch (Exception exception)
        {
            LRepertoireFailed?.Invoke("Situation.HoldFailed", exception);
            return null;
        }
    }

    private void LRepertoireSituationUpdate(LDraft draft)
    {
        if (CAtlas.CAtlasSituationRead(draft.LDraftSituation) is CSituationDraft situation)
        {
            LRepertoireSituationChanged?.Invoke(situation);
        }
    }

    public void LRepertoireSituationShow(long id)
    {
        bool editing = LRepertoireScribeChecked;
        LRepertoireSituationShow(id, editing);
        if (LRepertoireVignetteHeld)
        {
            return;
        }

        if (!LRepertoireAtlas.CAtlasNarrowed)
        {
            return;
        }

        LRepertoireInquestClear();
        LRepertoireSituationShow(id, editing);
    }

    private void LRepertoireSituationShow(long id, bool editing)
    {
        LRepertoireOccurrence.LOccurrencePanel.CPanelEntryClose();
        LRepertoireAtlas.CAtlasPanel.CPanelScribeSet(editing);
        LRepertoireAtlas.CAtlasPanel.CPanelRowOpen(id);
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
        if (!LRepertoireOccurrence.LOccurrencePanel.CPanelRowOpen(id))
        {
            return;
        }

        LRepertoireDesk.CDeskCancel();
        LRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        if (editing)
        {
            LRepertoireOccurrence.LOccurrencePanel.CPanelScribeToggle(true);
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

        LRepertoireOccurrence.LOccurrencePanel.CPanelEntryClose();
        LRepertoireAtlas.CAtlasPanel.CPanelFreshOpen();
        LRepertoireSession.CSessionStart(null);
    }

    public void LRepertoireScribeSet(bool editing)
    {
        if (LRepertoireOccurrenceSide)
        {
            LRepertoireOccurrence.LOccurrencePanel.CPanelScribeToggle(editing);
            if (!LRepertoireOccurrenceSide)
            {
                LRepertoireSituationRestore(editing);
            }

            return;
        }

        LRepertoireAtlas.CAtlasPanel.CPanelScribeToggle(editing);
        if (!LRepertoireAtlas.CAtlasPanel.CPanelEditing)
        {
            LRepertoireDesk.CDeskCancel();
        }
    }

    private void LRepertoireSituationRestore(bool editing)
    {
        if (LRepertoireAtlas.CAtlasChosen is long chosen)
        {
            LRepertoireSituationShow(chosen, editing);
            return;
        }

        LRepertoireClear();
    }

    public void LRepertoireClear()
    {
        LRepertoireOccurrence.LOccurrencePanel.CPanelEntryClose();
        LRepertoireAtlas.CAtlasPanel.CPanelEntryClose();
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
        LRepertoireAtlas.CAtlasPanel.CPanelBinEnabled || LRepertoireOccurrence.LOccurrencePanel.CPanelBinEnabled;

    private void LRepertoireInquestClear()
    {
        LRepertoireAtlas.CAtlasQuerySet(string.Empty);
        LRepertoireAtlas.CAtlasFilterSet(new CCatalogFilter([]));
        LRepertoireInquestCleared?.Invoke();
    }

    private void LRepertoireStoredShow(long id)
    {
        LRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        LRepertoireSituationShow(id);
    }

    public void LRepertoireDelete()
    {
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        LRepertoireAtlas.CAtlasPanel.CPanelEntryDelete();
    }

    private void LRepertoireOccurrenceCreate()
    {
        long? chosen = LRepertoireAtlas.CAtlasChosen;
        LRepertoireDesk.CDeskCancel();
        LRepertoireAtlas.CAtlasPanel.CPanelScribeSet(false);
        LRepertoireOccurrence.LOccurrencePanel.CPanelFreshOpen();
        LRepertoireEditor.LEditorStudio.CEditorEntryOpen(null);
        if (chosen is long id)
        {
            LRepertoireEditor.LEditorStudio.CEditorSituationAdd(id);
        }
    }

    public bool LRepertoireLeaveConfirm()
    {
        return LRepertoireLeaveConfirm(true);
    }

    private bool LRepertoireLeaveConfirm(bool shown)
    {
        if (!LRepertoireSession.CSessionChangeCheck())
        {
            return true;
        }

        return _lRepertoireLeaveSeam(shown ? LRepertoireSession.CSessionFinish : LRepertoireSession.CSessionClose);
    }

    public void LRepertoireEntryUpdate()
    {
        if (!LRepertoireOccurrence.LOccurrencePanel.CPanelBinEnabled)
        {
            return;
        }

        bool editing = LRepertoireScribeChecked;
        LRepertoireOccurrence.LOccurrencePanel.CPanelDraftUpdate();
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        LRepertoireSituationRestore(editing);
    }

    public Task LRepertoirePortraitPrint(CPortraitLabel label, CPortraitLegend legend, CPressTicket ticket)
    {
        if (LRepertoireDisplayShown)
        {
            return LRepertoireOccurrence.LOccurrencePortraitPrint(label, ticket);
        }

        if (LRepertoirePressAllowed)
        {
            return LRepertoireAtlas.CAtlasPortraitPrint(legend, ticket);
        }

        return Task.CompletedTask;
    }

    public Task LRepertoirePortraitExport(string path, CPortraitMedium format, CPortraitLabel label)
    {
        if (!LRepertoirePortraitAllowed)
        {
            return Task.CompletedTask;
        }

        return LRepertoireOccurrence.LOccurrencePortraitExport(path, format, label);
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

        LRepertoireAtlas.CAtlasVistaRestore(vista);
        LRepertoireOccurrence.LOccurrenceVistaRestore(vista, occurrence);
        LRepertoireEditor.LEditorStudio.CEditorVistaRestore(occurrence);
    }

    internal void LRepertoireVistaRestore(CAtelier atelier)
    {
        ArgumentNullException.ThrowIfNull(atelier);

        LRepertoireVistaRestore(
            atelier.CAtelierVistaStart(
                "repertoire", CSubject.CSubjectSituation, CCatalogOrder.CCatalogOrderName),
            atelier.CAtelierVistaStart(
                "occurrence", CSubject.CSubjectEntry, CCatalogOrder.CCatalogOrderHeadword));
    }
}
