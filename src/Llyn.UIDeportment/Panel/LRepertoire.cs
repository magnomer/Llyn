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
        CEnvoy envoy)
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
        LRepertoireDesk = new CDesk(drafts, "Situation", envoy, "Repertoire", CSubject.CSubjectSituation);
        LRepertoireAtlas = new LAtlas(
            entries,
            portraits,
            settings,
            LRepertoireDesk.CDeskChangeCheck,
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
        LRepertoireAtlas.LAtlasPanel.CPanelRowsChanged += LRepertoireOccurrence.LOccurrencePanel.CPanelRowsUpdate;
        LRepertoireSession = new CSession(
            LRepertoireDesk,
            [LRepertoireOccurrence.LOccurrencePanel.CPanelChangeCheck, LRepertoireAtlas.LAtlasPanel.CPanelChangeCheck],
            editor.LEditorStudio.CEditorDesk,
            () => LRepertoireOccurrence.LOccurrencePanel.CPanelEditing,
            editor.LEditorStudio.CEditorFinish,
            static () => true,
            LRepertoireStoredShow);
        LRepertoireSession.CSessionHeld += () => LRepertoireScenarioChanged?.Invoke(LRepertoireScenarioRead());
        LRepertoireSession.CSessionChanged += () => LRepertoireChanged?.Invoke();
        LRepertoireSession.CSessionFailed += (key, exception) => LRepertoireFailed?.Invoke(key, exception);
        LRepertoireAtlas.LAtlasPanel.CPanelEdited += id => LRepertoireSession.CSessionStart(id);
        LRepertoireAtlas.LAtlasPanel.CPanelCleared += LRepertoireSession.CSessionCancel;
        LRepertoireAtlas.LAtlasPanel.CPanelDraftChanged += LRepertoireSituationUpdate;
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

    public LAtlas LRepertoireAtlas { get; }

    public LOccurrence LRepertoireOccurrence { get; }

    private bool LRepertoireOccurrenceSide => LRepertoireOccurrence.LOccurrencePanel.CPanelModeEnabled;

    public bool LRepertoireScenarioShown => !LRepertoireOccurrenceSide && LRepertoireAtlas.LAtlasPanel.CPanelEditing;

    public bool LRepertoireVignetteShown => !LRepertoireOccurrenceSide && !LRepertoireAtlas.LAtlasPanel.CPanelEditing;

    public bool LRepertoireDisplayShown =>
        LRepertoireOccurrenceSide && !LRepertoireOccurrence.LOccurrencePanel.CPanelEditing;

    public bool LRepertoireEditorShown => LRepertoireOccurrence.LOccurrencePanel.CPanelEditing;

    public bool LRepertoireVignetteHeld => LRepertoireAtlas.LAtlasPanel.CPanelBinEnabled;

    public bool LRepertoireVignetteBlank => !LRepertoireAtlas.LAtlasPanel.CPanelBinEnabled;

    public bool LRepertoireScribeChecked => LRepertoireScenarioShown || LRepertoireEditorShown;

    public bool LRepertoireViewerChecked => !LRepertoireScribeChecked;

    public bool LRepertoireModeEnabled => LRepertoireOccurrenceSide || LRepertoireAtlas.LAtlasPanel.CPanelModeEnabled;

    public bool LRepertoireBinEnabled => !LRepertoireOccurrenceSide && LRepertoireAtlas.LAtlasPanel.CPanelBinEnabled;

    public bool LRepertoireStoreEnabled =>
        LRepertoireEditorShown ? LRepertoireEditor.LEditorStorable : LRepertoireDesk.CDeskChanged;

    public bool LRepertoirePressAllowed =>
        LRepertoireDisplayShown || (LRepertoireVignetteShown && LRepertoireVignetteHeld);

    public bool LRepertoirePortraitAllowed => LRepertoireDisplayShown;

    public CSituationDraft? LRepertoireScenarioRead()
    {
        try
        {
            return LAtlas.LAtlasSituationRead(LRepertoireDesk.CDeskRead()?.LDraftSituation);
        }
        catch (Exception exception)
        {
            LRepertoireFailed?.Invoke("Situation.HoldFailed", exception);
            return null;
        }
    }

    private void LRepertoireSituationUpdate(LDraft draft)
    {
        if (LAtlas.LAtlasSituationRead(draft.LDraftSituation) is CSituationDraft situation)
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

        if (!LRepertoireAtlas.LAtlasNarrowed)
        {
            return;
        }

        LRepertoireInquestClear();
        LRepertoireSituationShow(id, editing);
    }

    private void LRepertoireSituationShow(long id, bool editing)
    {
        LRepertoireOccurrence.LOccurrencePanel.CPanelEntryClose();
        LRepertoireAtlas.LAtlasPanel.CPanelScribeSet(editing);
        LRepertoireAtlas.LAtlasPanel.CPanelRowOpen(id);
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
        LRepertoireAtlas.LAtlasPanel.CPanelScribeSet(false);
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
        LRepertoireAtlas.LAtlasPanel.CPanelFreshOpen();
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

        LRepertoireAtlas.LAtlasPanel.CPanelScribeToggle(editing);
        if (!LRepertoireAtlas.LAtlasPanel.CPanelEditing)
        {
            LRepertoireDesk.CDeskCancel();
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
        LRepertoireOccurrence.LOccurrencePanel.CPanelEntryClose();
        LRepertoireAtlas.LAtlasPanel.CPanelEntryClose();
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
        LRepertoireAtlas.LAtlasPanel.CPanelBinEnabled || LRepertoireOccurrence.LOccurrencePanel.CPanelBinEnabled;

    private void LRepertoireInquestClear()
    {
        LRepertoireAtlas.LAtlasInquestSet(string.Empty);
        LRepertoireAtlas.LAtlasMeshSet(new CCatalogFilter([]));
        LRepertoireInquestCleared?.Invoke();
    }

    private void LRepertoireStoredShow(long id)
    {
        LRepertoireAtlas.LAtlasPanel.CPanelScribeSet(false);
        LRepertoireSituationShow(id);
    }

    public void LRepertoireDelete()
    {
        if (LRepertoireOccurrenceSide)
        {
            return;
        }

        LRepertoireAtlas.LAtlasPanel.CPanelEntryDelete();
    }

    private void LRepertoireOccurrenceCreate()
    {
        long? chosen = LRepertoireAtlas.LAtlasChosen;
        LRepertoireDesk.CDeskCancel();
        LRepertoireAtlas.LAtlasPanel.CPanelScribeSet(false);
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

        LRepertoireAtlas.LAtlasVistaRestore(vista);
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
