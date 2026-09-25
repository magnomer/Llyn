using System.Collections.Generic;
using System.Text.Json;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftLeftover
{
    [Fact]
    public void LeftoverRead_DraftStillHeldOpen_PassesOverIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long first;
        long second;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft input = engine.TEngineDraftStart("Input", null);
            LDraft library = engine.TEngineDraftStart("Library", null);
            engine.TEngineDraftStart("Phonology", null);

            first = input.LDraftId;
            second = library.LDraftId;

            engine.TRequestContentApply(
                input.LDraftId,
                TInterface.TDraftNestedCreate("Input", "kindle").LDraftContent with
                {
                    LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
                });
            engine.TRequestContentApply(
                library.LDraftId,
                TInterface.TDraftNestedCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftMeanings = [TInterface.TDraftCardCreate("a glowing coal")],
                });

            Assert.Empty(engine.TEngineLeftoverRead());
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        IReadOnlyList<LDraft> leftovers = launched.TEngineLeftoverRead();

        Assert.Equal(2, leftovers.Count);
        Assert.Contains(leftovers, draft => draft.LDraftId == first);
        Assert.Contains(leftovers, draft => draft.LDraftId == second);
    }

    [Fact]
    public void LeftoverSweep_DraftMatchingStoredEntry_SweepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long swept;
        long kept;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft committed = engine.TEngineDraftStart("Input", null);
            LDraft edited = engine.TEngineDraftStart("Library", null);

            swept = committed.LDraftId;
            kept = edited.LDraftId;

            LDraft written = engine.TRequestContentApply(
                committed.LDraftId,
                TInterface.TDraftNestedCreate("Input", "kindle").LDraftContent with
                {
                    LEntryDraftMeanings = [TInterface.TDraftCardCreate("set alight")],
                });

            LEntry stored = engine.TEngineEntrySave(written.LDraftContent);

            LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

            Assert.NotNull(loaded);

            TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, written with
            {
                LDraftEntryId = stored.LEntryId,
                LDraftContent = loaded,
            });
            engine.TRequestContentApply(
                edited.LDraftId,
                TInterface.TDraftNestedCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftMeanings = [TInterface.TDraftCardCreate("a glowing coal")],
                });
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        launched.TEngineLeftoverSweep();

        Assert.Null(launched.TEngineDraftRead(swept));
        Assert.NotNull(launched.TEngineDraftRead(kept));

        IReadOnlyList<LDraft> leftovers = launched.TEngineLeftoverRead();

        Assert.Single(leftovers);
        Assert.Equal(kept, leftovers[0].LDraftId);
    }

    [Fact]
    public void LeftoverSweep_BlankDraftNamingNoEntry_SweepsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        long blank;
        long typed;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            blank = engine.TEngineDraftStart("Input", null).LDraftId;
            LDraft edited = engine.TEngineDraftStart("Library", null);
            typed = edited.LDraftId;

            engine.TRequestContentApply(
                edited.LDraftId,
                TInterface.TDraftNestedCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftMeanings = [TInterface.TDraftCardCreate("a glowing coal")],
                });
        }

        using LEngine launched = workspace.TWorkspaceEngineStart();

        launched.TEngineLeftoverSweep();

        Assert.Null(launched.TEngineDraftRead(blank));
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, blank));
        Assert.NotNull(launched.TEngineDraftRead(typed));

        IReadOnlyList<LDraft> leftovers = launched.TEngineLeftoverRead();

        Assert.Single(leftovers);
        Assert.Equal(typed, leftovers[0].LDraftId);
    }

    [Fact]
    public void LeftoverSweep_ClaimWithoutDraftFile_DropsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder, TInterface.TClaimCreate(-9, 1, DateTimeOffset.UtcNow));

        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLeftoverSweep();

        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, -9));
        Assert.Empty(TInterface.TClaimArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void LeftoverSweep_DraftFileOfAnotherVersion_DropsItWithClaimAndLinks()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TInterface.TDraftNestedCreate("Input", "kindle");
        LDraft owner = TInterface.TDraftNestedCreate("Library", "ember");
        LCourt link = TInterface.TDraftLinkCreate(owner.LDraftId, draft.LDraftId, "kindle");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, owner);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, link);
        TInterface.TClaimArchiveSave(
            workspace.TWorkspaceFolder, TInterface.TClaimCreate(draft.LDraftId, 1, DateTimeOffset.UtcNow));

        string folder = TInterface.TWorkspaceDraftRead(workspace.TWorkspaceFolder);
        string path = Path.Combine(folder, draft.LDraftId + ".json");
        File.WriteAllText(path, JsonSerializer.Serialize(draft with { LDraftVersion = 0 }));

        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Null(engine.TEngineDraftRead(draft.LDraftId));

        engine.TEngineLeftoverSweep();

        Assert.False(File.Exists(path));
        Assert.True(File.Exists(Path.Combine(folder, "broken", draft.LDraftId + ".json")));
        Assert.DoesNotContain(engine.TEngineDraftScan(), held => held.LDraftId == draft.LDraftId);
        Assert.Null(TInterface.TClaimArchiveRead(workspace.TWorkspaceFolder, draft.LDraftId));
        Assert.Null(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, link.LCourtId));
        Assert.NotNull(engine.TEngineDraftRead(owner.LDraftId));
    }

    [Fact]
    public void LeftoverSweep_UnreadableDraftFile_SetsItAsideUnchanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string folder = TInterface.TWorkspaceDraftRead(workspace.TWorkspaceFolder);
        string path = Path.Combine(folder, "-7.json");
        File.WriteAllText(path, "{ half a draft");

        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLeftoverSweep();

        Assert.False(File.Exists(path));
        Assert.Equal("{ half a draft", File.ReadAllText(Path.Combine(folder, "broken", "-7.json")));
        Assert.Empty(engine.TEngineLeftoverRead());
    }

    [Fact]
    public void LeftoverSweep_StalePendingFile_RemovesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string drafts = TInterface.TWorkspaceDraftRead(workspace.TWorkspaceFolder);
        string court = TInterface.TWorkspaceCourtRead(workspace.TWorkspaceFolder);

        string stale = Path.Combine(drafts, "stale.json.tmp");
        string fresh = Path.Combine(drafts, "fresh.json.tmp");
        string staleLink = Path.Combine(court, "stale.json.tmp");

        File.WriteAllText(stale, "{ half written");
        File.WriteAllText(fresh, "{ half written");
        File.WriteAllText(staleLink, "{ half written");
        File.SetLastWriteTimeUtc(stale, DateTime.UtcNow.AddHours(-2));
        File.SetLastWriteTimeUtc(staleLink, DateTime.UtcNow.AddHours(-2));

        using LEngine engine = workspace.TWorkspaceEngineStart();

        engine.TEngineLeftoverSweep();

        Assert.False(File.Exists(stale));
        Assert.False(File.Exists(staleLink));
        Assert.True(File.Exists(fresh));
    }
}
