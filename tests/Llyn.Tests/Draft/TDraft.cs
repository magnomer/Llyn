using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraft
{
    [Fact]
    public void DraftArchiveSave_WholeDraft_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TDraftCreate("editor", "kindle");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft);
        LDraft? loaded = TInterface.TDraftArchiveRead(workspace.TWorkspaceFolder, draft.LDraftId);

        Assert.NotNull(loaded);
        Assert.Equal(draft.LDraftId, loaded.LDraftId);
        Assert.Equal(draft.LDraftOrigin, loaded.LDraftOrigin);
        Assert.Equal(draft.LDraftEntry, loaded.LDraftEntry);
        Assert.Equal(draft.LDraftMoment, loaded.LDraftMoment);
        Assert.Equal(draft.LDraftContent.LEntryDraftHeadword, loaded.LDraftContent.LEntryDraftHeadword);
        Assert.Equal(draft.LDraftContent.LEntryDraftLanguage, loaded.LDraftContent.LEntryDraftLanguage);
        Assert.Equal(draft.LDraftContent.LEntryDraftPronunciation, loaded.LDraftContent.LEntryDraftPronunciation);
        Assert.Equal(draft.LDraftContent.LEntryDraftNote, loaded.LDraftContent.LEntryDraftNote);
        Assert.Equal(
            draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftTitle.TStateValueShow(),
            loaded.LDraftContent.LEntryDraftMeanings[0].LCardDraftTitle.TStateValueShow());
        Assert.Equal(
            draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftMeaning.TStateValueShow(),
            loaded.LDraftContent.LEntryDraftMeanings[0].LCardDraftMeaning.TStateValueShow());
        Assert.Empty(loaded.LDraftContent.LEntryDraftCollocations);
    }

    [Fact]
    public void DraftArchiveScan_HeldDrafts_ListsAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft first = TDraftCreate("editor", "kindle");
        LDraft second = TDraftCreate("library", "ember");
        LDraft third = TDraftCreate("editor", "hearth");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, first);
        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, second);
        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, third);

        IReadOnlyList<LDraft> drafts = TInterface.TDraftArchiveScan(workspace.TWorkspaceFolder);

        Assert.Equal(3, drafts.Count);
        Assert.Contains(drafts, draft => draft.LDraftId == first.LDraftId);
        Assert.Contains(drafts, draft => draft.LDraftId == second.LDraftId);
        Assert.Contains(drafts, draft => draft.LDraftId == third.LDraftId);
    }

    [Fact]
    public void DraftArchiveDelete_OneOfSeveral_LeavesOthers()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft first = TDraftCreate("editor", "kindle");
        LDraft second = TDraftCreate("library", "ember");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, first);
        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, second);
        TInterface.TDraftArchiveDelete(workspace.TWorkspaceFolder, first.LDraftId);

        IReadOnlyList<LDraft> drafts = TInterface.TDraftArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(drafts);
        Assert.Equal(second.LDraftId, drafts[0].LDraftId);
        Assert.Null(TInterface.TDraftArchiveRead(workspace.TWorkspaceFolder, first.LDraftId));
    }

    [Fact]
    public void DraftArchiveScan_BrokenFile_SkipsIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TDraftCreate("editor", "kindle");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft);

        string folder = TInterface.TWorkspaceDraftRead(workspace.TWorkspaceFolder);
        File.WriteAllText(Path.Combine(folder, "broken.json"), "{ not json");

        IReadOnlyList<LDraft> drafts = TInterface.TDraftArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(drafts);
        Assert.Equal(draft.LDraftId, drafts[0].LDraftId);
    }

    [Fact]
    public void CourtArchiveSettle_WaitingLinks_DropsAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = TInterface.TIdentityCreate();
        LCourtLink first = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourtLink second = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, first);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, second);

        IReadOnlyList<LCourtLink> settled = TInterface.TCourtArchiveSettle(
            workspace.TWorkspaceFolder,
            target);

        Assert.Equal(2, settled.Count);
        Assert.Contains(settled, link => link.LCourtLinkId == first.LCourtLinkId);
        Assert.Contains(settled, link => link.LCourtLinkId == second.LCourtLinkId);
        Assert.Contains(settled, link => link.LCourtLinkOwner == first.LCourtLinkOwner);
        Assert.Contains(settled, link => link.LCourtLinkOwner == second.LCourtLinkOwner);
        Assert.Null(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, first.LCourtLinkId));
        Assert.Null(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, second.LCourtLinkId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void CourtArchiveSettle_OtherTargetLink_LeavesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = TInterface.TIdentityCreate();
        string other = TInterface.TIdentityCreate();
        LCourtLink settled = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourtLink kept = TDraftLinkCreate(TInterface.TIdentityCreate(), other, "ember");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, settled);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, kept);
        TInterface.TCourtArchiveSettle(workspace.TWorkspaceFolder, target);

        IReadOnlyList<LCourtLink> remaining = TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(remaining);
        Assert.Equal(kept.LCourtLinkId, remaining[0].LCourtLinkId);
        Assert.Equal(other, remaining[0].LCourtLinkTarget);

        Assert.Empty(TInterface.TCourtArchiveSettle(workspace.TWorkspaceFolder, target));
        Assert.NotNull(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, kept.LCourtLinkId));
    }

    [Fact]
    public void CourtArchiveSave_TentativeLink_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LCourtLink link = TDraftLinkCreate(TInterface.TIdentityCreate(), TInterface.TIdentityCreate(), "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, link);
        LCourtLink? loaded = TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, link.LCourtLinkId);

        Assert.NotNull(loaded);
        Assert.Equal(link.LCourtLinkOwner, loaded.LCourtLinkOwner);
        Assert.Equal(link.LCourtLinkTarget, loaded.LCourtLinkTarget);
        Assert.Equal(link.LCourtLinkHeadword, loaded.LCourtLinkHeadword);
        Assert.Equal(link.LCourtLinkLanguage, loaded.LCourtLinkLanguage);
    }

    [Fact]
    public void DraftCommit_HeldDraft_StoresEntryAndDeletesFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(started with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("set alight")],
            },
        });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.False(string.IsNullOrWhiteSpace(stored.LEntryId));
        Assert.Equal("kindle", engine.TEngineEntryRead(stored.LEntryId)?.LEntryHeadword);
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Null(engine.TEngineDraftRead(started.LDraftId));
    }

    [Fact]
    public void DraftCommit_WriteRefused_KeepsDraftFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);

        Assert.Throws<LRefusal>(() => engine.TEngineDraftCommit(started.LDraftId));

        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(started.LDraftId, held.LDraftId);
        Assert.Single(engine.TEngineDraftScan());
        Assert.Empty(engine.TEngineEntryFind(string.Empty));
    }

    [Fact]
    public void DraftMove_CardMoved_RenumbersAllCards()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(started with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings =
                [
                    TDraftCardCreate("first"),
                    TDraftCardCreate("second"),
                    TDraftCardCreate("third"),
                ],
            },
        });

        IReadOnlyList<LCardDraft> moved = engine.TEngineDraftMove(started.LDraftId, false, 0, 2);

        Assert.Equal(
            ["second", "third", "first"],
            moved.Select(card => card.LCardDraftTitle.TStateValueShow()));
        Assert.Equal([1, 2, 3], moved.Select(card => card.LCardDraftPosition));

        LDraft? held = engine.TEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(
            ["second", "third", "first"],
            held.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftTitle.TStateValueShow()));
        Assert.Equal([1, 2, 3], held.LDraftContent.LEntryDraftMeanings.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void DraftCheck_OnlyLanguageChanged_ReportsChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(first with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("set alight")],
            },
        });

        LEntry stored = engine.TEngineDraftCommit(first.LDraftId);
        LDraft opened = engine.TEngineDraftStart("editor", stored.LEntryId);

        Assert.False(engine.TEngineDraftCheck(opened.LDraftId));

        engine.TEngineDraftSave(opened with
        {
            LDraftContent = opened.LDraftContent with { LEntryDraftLanguage = "Korean" },
        });

        Assert.True(engine.TEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void DraftCommit_DraftsPointingAtEachOther_NoRecursion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        LDraft second = engine.TEngineDraftStart("editor", null);

        engine.TEngineDraftSave(first with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("set alight")],
            },
        });
        engine.TEngineDraftSave(second with
        {
            LDraftContent = TDraftCreate("editor", "ember").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("a glowing coal")],
            },
        });

        engine.TEngineCourtSave(first.LDraftId, second.LDraftId, "ember", "English");
        engine.TEngineCourtSave(second.LDraftId, first.LDraftId, "kindle", "English");

        LEntry stored = engine.TEngineDraftCommit(first.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.Single(engine.TEngineEntryFind("ember"));
        Assert.Empty(engine.TEngineDraftScan());
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void DraftCancel_TargetAnotherDraftNames_KeepsTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        LDraft second = engine.TEngineDraftStart("editor", null);
        LDraft target = engine.TEngineDraftStart("editor", null);

        engine.TEngineCourtSave(first.LDraftId, target.LDraftId, "ember", "English");
        engine.TEngineCourtSave(second.LDraftId, target.LDraftId, "ember", "English");

        engine.TEngineDraftCancel(first.LDraftId);

        Assert.Null(engine.TEngineDraftRead(first.LDraftId));
        Assert.NotNull(engine.TEngineDraftRead(target.LDraftId));
        Assert.NotNull(engine.TEngineCourtFind(second.LDraftId, target.LDraftId));

        engine.TEngineDraftCancel(second.LDraftId);

        Assert.Null(engine.TEngineDraftRead(target.LDraftId));
    }

    [Fact]
    public void LeftoverRead_DraftStillHeldOpen_PassesOverIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string first;
        string second;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft input = engine.TEngineDraftStart("Input", null);
            LDraft library = engine.TEngineDraftStart("Library", null);
            engine.TEngineDraftStart("Phonology", null);

            first = input.LDraftId;
            second = library.LDraftId;

            engine.TEngineDraftSave(input with
            {
                LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
                {
                    LEntryDraftMeanings = [TDraftCardCreate("set alight")],
                },
            });
            engine.TEngineDraftSave(library with
            {
                LDraftContent = TDraftCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftMeanings = [TDraftCardCreate("a glowing coal")],
                },
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

        string swept;
        string kept;

        using (LEngine engine = workspace.TWorkspaceEngineStart())
        {
            LDraft committed = engine.TEngineDraftStart("Input", null);
            LDraft edited = engine.TEngineDraftStart("Library", null);

            swept = committed.LDraftId;
            kept = edited.LDraftId;

            LDraft written = committed with
            {
                LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
                {
                    LEntryDraftMeanings = [TDraftCardCreate("set alight")],
                },
            };

            engine.TEngineDraftSave(written);

            LEntry stored = engine.TEngineEntrySave(written.LDraftContent);

            LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

            Assert.NotNull(loaded);

            engine.TEngineDraftSave(written with
            {
                LDraftEntry = stored.LEntryId,
                LDraftContent = loaded,
            });
            engine.TEngineDraftSave(edited with
            {
                LDraftContent = TDraftCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftMeanings = [TDraftCardCreate("a glowing coal")],
                },
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

    [Fact]
    public void DraftCommit_NamesStoredEntry_UpdatesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft written = started with
        {
            LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("set alight")],
            },
        };

        engine.TEngineDraftSave(written);

        LEntry stored = engine.TEngineEntrySave(written.LDraftContent);

        engine.TEngineDraftSave(written with { LDraftEntry = stored.LEntryId });

        LEntry recommitted = engine.TEngineDraftCommit(started.LDraftId);

        Assert.Equal(stored.LEntryId, recommitted.LEntryId);
        Assert.Single(engine.TEngineEntryFind("kindle"));
        Assert.Empty(engine.TEngineDraftScan());
    }

    [Fact]
    public void DraftCommit_NamesDeletedEntry_StoresNewEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        engine.TEngineDraftSave(started with
        {
            LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("set alight")],
            },
        });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LDraft opened = engine.TEngineDraftStart("Input", stored.LEntryId);

        engine.TEngineDraftSave(opened with
        {
            LDraftContent = opened.LDraftContent with { LEntryDraftHeadword = "ember" },
        });

        engine.TEngineEntryDelete(stored.LEntryId);

        Assert.True(engine.TEngineDraftCheck(opened.LDraftId));

        LEntry recommitted = engine.TEngineDraftCommit(opened.LDraftId);

        Assert.Equal("ember", recommitted.LEntryHeadword);
        Assert.NotEqual(stored.LEntryId, recommitted.LEntryId);
        Assert.Equal("ember", engine.TEngineEntryRead(recommitted.LEntryId)?.LEntryHeadword);
        Assert.Empty(engine.TEngineDraftScan());
    }

    [Fact]
    public void DraftCommit_TargetAnotherEditorHolds_KeepsTargetFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using LEngine other = workspace.TWorkspaceEngineStart();

        LDraft owner = engine.TEngineDraftStart("Input", null);
        LDraft target = other.TEngineDraftStart("Library", null);

        engine.TEngineDraftSave(owner with
        {
            LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("set alight")],
            },
        });
        other.TEngineDraftSave(target with
        {
            LDraftContent = TDraftCreate("Library", "ember").LDraftContent with
            {
                LEntryDraftMeanings = [TDraftCardCreate("a glowing coal")],
            },
        });

        engine.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        LEntry stored = engine.TEngineDraftCommit(owner.LDraftId);
        LDraft? kept = engine.TEngineDraftRead(target.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.Null(engine.TEngineDraftRead(owner.LDraftId));
        Assert.NotNull(kept);
        Assert.Single(engine.TEngineEntryFind("ember"));
        Assert.Equal(engine.TEngineEntryFind("ember")[0].LEntryId, kept.LDraftEntry);
        Assert.True(TInterface.TClaimArchiveCheck(workspace.TWorkspaceFolder, target.LDraftId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));

        LEntry recommitted = other.TEngineDraftCommit(target.LDraftId);

        Assert.Equal(kept.LDraftEntry, recommitted.LEntryId);
        Assert.Single(engine.TEngineEntryFind("ember"));
    }

    [Fact]
    public void DraftDelete_DraftOwningLinks_DropsThem()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft owner = engine.TEngineDraftStart("Input", null);
        LDraft target = engine.TEngineDraftStart("Input", null);

        engine.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");
        engine.TEngineDraftDelete(owner.LDraftId);

        Assert.Null(engine.TEngineDraftRead(owner.LDraftId));
        Assert.Null(engine.TEngineDraftRead(target.LDraftId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void DraftDelete_OwnerDraftGone_CollectsRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft held = engine.TEngineDraftStart("Input", null);
        LCourtLink stranded = TDraftLinkCreate(
            TInterface.TIdentityCreate(), TInterface.TIdentityCreate(), "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, stranded);
        engine.TEngineDraftDelete(held.LDraftId);

        Assert.Null(engine.TEngineDraftRead(held.LDraftId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void DraftSave_EveryCardNamed_ReturnsTheContentSent()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        LEntryDraft sent = TDraftCreate("editor", "kindle").LDraftContent with
        {
            LEntryDraftMeanings = [TDraftCardCreate("set alight", TInterface.TIdentityCreate())],
        };

        LEntryDraft stored = engine.TEngineDraftSave(started with { LDraftContent = sent });

        Assert.Same(sent, stored);
    }

    [Fact]
    public void DraftSave_UnnamedCard_ReturnsFreshContentWithEveryCardNamed()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        LEntryDraft sent = TDraftCreate("editor", "kindle").LDraftContent with
        {
            LEntryDraftMeanings = [TDraftCardCreate("set alight", TInterface.TIdentityCreate())],
            LEntryDraftCollocations = [TDraftCardCreate("kindle a hope")],
        };

        LEntryDraft stored = engine.TEngineDraftSave(started with { LDraftContent = sent });

        Assert.NotSame(sent, stored);
        Assert.Same(sent.LEntryDraftMeanings, stored.LEntryDraftMeanings);
        Assert.All(
            stored.LEntryDraftMeanings,
            card => Assert.NotEqual(string.Empty, card.LCardDraftId));
        Assert.All(
            stored.LEntryDraftCollocations,
            card => Assert.NotEqual(string.Empty, card.LCardDraftId));
    }

    private static LCardDraft TDraftCardCreate(string title, string id)
    {
        return TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            0,
            id);
    }

    private static LCardDraft TDraftCardCreate(string title)
    {
        return TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate(title),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            0);
    }

    [Fact]
    public void DraftCommit_TargetHeldByAnotherEngine_LeavesItNamingTheStoredEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        using LEngine holder = workspace.TWorkspaceEngineStart();
        using LEngine writer = workspace.TWorkspaceEngineStart();

        LDraft target = holder.TEngineDraftStart("Library", null);
        holder.TEngineDraftSave(target with { LDraftContent = TDraftPlainCreate("ember") });

        LDraft owner = writer.TEngineDraftStart("Input", null);
        writer.TEngineDraftSave(owner with { LDraftContent = TDraftPlainCreate("kindle") });
        writer.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        writer.TEngineDraftCommit(owner.LDraftId);

        LDraft? kept = writer.TEngineDraftRead(target.LDraftId);

        Assert.NotNull(kept);
        Assert.NotEqual(string.Empty, kept.LDraftEntry);
        Assert.NotNull(writer.TEngineEntryLoad(kept.LDraftEntry));
        Assert.NotEqual(
            string.Empty,
            kept.LDraftContent.LEntryDraftMeanings[0].LCardDraftId);
        Assert.Empty(writer.TEngineLeftoverRead());
    }

    [Fact]
    public void DraftCancel_TargetOfAnotherDraft_StrikesItsIdFromThatDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft owner = engine.TEngineDraftStart("Input", null);
        LDraft target = engine.TEngineDraftStart("Library", null);

        LEntryDraft content = TDraftPlainCreate("kindle");
        engine.TEngineDraftSave(owner with
        {
            LDraftContent = content with
            {
                LEntryDraftMeanings =
                [
                    content.LEntryDraftMeanings[0] with
                    {
                        LCardDraftTranslation = [target.LDraftId],
                    },
                ],
            },
        });
        engine.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        engine.TEngineDraftCancel(target.LDraftId);

        LDraft? stranded = engine.TEngineDraftRead(owner.LDraftId);

        Assert.NotNull(stranded);
        Assert.Empty(stranded.LDraftContent.LEntryDraftMeanings[0].LCardDraftTranslation);
        Assert.Null(engine.TEngineCourtFind(owner.LDraftId, target.LDraftId));
    }

    [Fact]
    public void DraftCancel_TargetHeldByAnotherEngine_LeavesItAlone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        using LEngine holder = workspace.TWorkspaceEngineStart();
        using LEngine writer = workspace.TWorkspaceEngineStart();

        LDraft target = holder.TEngineDraftStart("Library", null);
        LDraft owner = writer.TEngineDraftStart("Input", null);

        writer.TEngineCourtSave(owner.LDraftId, target.LDraftId, "ember", "English");

        writer.TEngineDraftCancel(owner.LDraftId);

        Assert.NotNull(holder.TEngineDraftRead(target.LDraftId));
        Assert.Null(writer.TEngineDraftRead(owner.LDraftId));
    }

    [Fact]
    public void DraftCommit_TranslationNamingNoEntry_DropsItInsteadOfFailing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        LEntryDraft content = TDraftPlainCreate("kindle");

        engine.TEngineDraftSave(started with
        {
            LDraftContent = content with
            {
                LEntryDraftMeanings =
                [
                    content.LEntryDraftMeanings[0] with
                    {
                        LCardDraftTranslation = ["nosuchentry00"],
                    },
                ],
            },
        });

        LEntry stored = engine.TEngineDraftCommit(started.LDraftId);
        LEntryDraft? loaded = engine.TEngineEntryLoad(stored.LEntryId);

        Assert.NotNull(loaded);
        Assert.Empty(loaded.LEntryDraftMeanings[0].LCardDraftTranslation);
    }

    private static LEntryDraft TDraftPlainCreate(string headword)
    {
        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("to set something burning"),
            [],
            [],
            [],
            string.Empty,
            [],
            [],
            1);

        return TInterface.TEntryDraftCreate(headword, "English", string.Empty, string.Empty, [meaning], []);
    }

    private static LCourtLink TDraftLinkCreate(string owner, string target, string headword)
    {
        return TInterface.TCourtLinkCreate(TInterface.TIdentityCreate(), owner, target, headword, "English");
    }

    private static LDraft TDraftCreate(string origin, string headword)
    {
        LCardDraft meaning = TInterface.TCardDraftCreate(
            TInterface.TStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            TInterface.TStateValueCreate("to set something burning"),
            [TInterface.TExampleDraftCreate("she knelt to kindle the damp logs")],
            [TInterface.TSituationDraftCreate("around a hearth")],
            ["불을 붙이다"],
            "ignite",
            ["literal"],
            [],
            0);

        LEntryDraft content = TInterface.TEntryDraftCreate(
            headword,
            "English",
            "/ˈkɪnd(ə)l/",
            "Chiefly literary.",
            [meaning],
            []);

        return TInterface.TDraftCreate(
            TInterface.TIdentityCreate(),
            origin,
            string.Empty,
            content,
            DateTimeOffset.UtcNow);
    }
}
