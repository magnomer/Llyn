using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TPanelRow
{
    [Fact]
    public void PanelRowSelect_UnsavedLeaveKept_KeepsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, null, [], [], "Scribe", () => true);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelScribeToggle(true);

        CPanelOutcome opened = panel.CPanelRowSelect(chosen + 100);

        Assert.Equal(CPanelOutcome.CPanelOutcomeStayed, opened);
        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelRowSelect_CancelledOrFailed_ReturnsStayedOrFailed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel cancelled = TPanel.TPanelPrepare(engine, null, asked, [], "Scribe", () => true);
        long chosen = cancelled.TPanelChosenRead();
        cancelled.CPanelScribeToggle(true);
        CPanel viewing = TPanel.TPanelPrepare(engine, null, asked, [], "Scribe", () => false);

        CPanelOutcome stayed = cancelled.CPanelRowSelect(chosen);
        workspace.TWorkspaceScriptRun("ALTER TABLE entry RENAME TO entry_gone;");
        CPanelOutcome failed = viewing.CPanelRowSelect(chosen);

        Assert.Equal(CPanelOutcome.CPanelOutcomeStayed, stayed);
        Assert.Equal(CPanelOutcome.CPanelOutcomeFailed, failed);
        Assert.Equal(CPanelOutcome.CPanelOutcomeFailed, viewing.CPanelRowSelect(null));
        Assert.Equal(["Leave", "List.LoadFailed"], asked);
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(null, false)]
    public void PanelRowSelect_UnsavedDraft_AsksBeforeRecordingStation(bool? answer, bool left)
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanel.TPanelPrepare(engine, answer, asked, [], "Scribe", () => true);
        panel.TPanelStationAttach(() => asked.Add("Station"));
        LEntry fire = engine.TEngineEntrySave(TInterface.TEntryDraftCreate(
            "fire", "English", "f", string.Empty, [TInterface.TCardCreate("a flame", 1)], []));
        long chosen = panel.TPanelChosenRead();
        panel.CPanelScribeToggle(true);

        CPanelOutcome opened = panel.CPanelRowSelect(fire.LEntryId);

        Assert.Equal(left ? CPanelOutcome.CPanelOutcomeOpened : CPanelOutcome.CPanelOutcomeStayed, opened);
        Assert.Equal(left ? ["Leave", "Station"] : ["Leave"], asked);
        Assert.Equal(left ? fire.LEntryId : chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelRowOpen_StoredRow_ChoosesRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryClose();
        int changed = 0;
        panel.CPanelChanged += () => changed++;

        bool opened = panel.CPanelRowOpen(chosen);

        Assert.True(opened);
        Assert.Equal(chosen, panel.TPanelChosenRead());
        Assert.Equal(1, changed);
    }

    [Fact]
    public void PanelRowOpen_MissingRow_ClosesPanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanel.TPanelPrepare(engine, null, asked, [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        int cleared = 0;
        panel.CPanelCleared += () => cleared++;

        bool opened = panel.CPanelRowOpen(chosen + 100);

        Assert.False(opened);
        Assert.Equal(0, panel.TPanelChosenRead());
        Assert.Equal(1, cleared);
        Assert.Empty(asked);
    }

    [Fact]
    public void PanelRowOpen_EditingPanel_HandsRowToEditor()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryCreate();
        List<long> edited = [];
        panel.CPanelEdited += edited.Add;

        panel.CPanelRowOpen(chosen);

        Assert.Equal([chosen], edited);
        Assert.True(panel.CPanelEditing);
    }

    [Fact]
    public void PanelDraftResonate_StoredEntry_RepaintsPanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        int changed = 0;
        panel.CPanelChanged += () => changed++;

        panel.CPanelDraftResonate();

        Assert.Equal(1, changed);
        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelDraftResonate_DeletedEntry_ClosesPanel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        engine.TEngineEntryDelete(panel.TPanelChosenRead());
        int cleared = 0;
        panel.CPanelCleared += () => cleared++;

        panel.CPanelDraftResonate();

        Assert.Equal(1, cleared);
        Assert.Equal(0, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelDraftResonate_EngineFails_ShowsLoadFailureOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CPanel panel = TPanel.TPanelPrepare(engine, null, asked, [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        workspace.TWorkspaceScriptRun("ALTER TABLE entry RENAME TO entry_gone;");
        int changed = 0;
        panel.CPanelChanged += () => changed++;

        panel.CPanelDraftResonate();

        Assert.Equal(["List.LoadFailed"], asked);
        Assert.Equal(0, changed);
        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelEntryResonate_StoredBulletin_RelistsRows()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, null, [], [], "Scribe", () => false);
        int rows = 0;
        int changed = 0;
        panel.CPanelAperture.CApertureRowsChanged += () => rows++;
        panel.CPanelChanged += () => changed++;

        panel.CPanelEntryResonate(new CBulletin(panel.TPanelChosenRead(), true));

        Assert.Equal(1, rows);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void PanelEntrySelect_StoredWhileFreshEditing_AdoptsEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryCreate();

        panel.CPanelEntrySelect(new CBulletin(chosen, true));

        Assert.Equal(chosen, panel.TPanelChosenRead());
    }

    [Fact]
    public void PanelEntrySelect_Viewing_KeepsNothingChosen()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CPanel panel = TPanel.TPanelPrepare(engine, true, [], [], "Scribe", () => false);
        long chosen = panel.TPanelChosenRead();
        panel.CPanelEntryClose();

        panel.CPanelEntrySelect(new CBulletin(chosen, true));

        Assert.Equal(0, panel.TPanelChosenRead());
    }
}
