using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraft
{
    [Fact]
    public void ADraftIsSavedAndReadBackExactlyAsItWasWritten()
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
            draft.LDraftContent.LEntryDraftSenses[0].LCardDraftTitle.TStateValueShow(),
            loaded.LDraftContent.LEntryDraftSenses[0].LCardDraftTitle.TStateValueShow());
        Assert.Equal(
            draft.LDraftContent.LEntryDraftSenses[0].LCardDraftMeaning.TStateValueShow(),
            loaded.LDraftContent.LEntryDraftSenses[0].LCardDraftMeaning.TStateValueShow());
        Assert.Empty(loaded.LDraftContent.LEntryDraftCollocations);
    }

    [Fact]
    public void EveryHeldDraftIsListedFromTheWorkspaceFolder()
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
    public void DeletingOneDraftLeavesTheOthersStanding()
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
    public void ABrokenDraftFileIsSkippedRatherThanFailingTheScan()
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
    public void ResolvingADraftIdRewritesEveryLinkWaitingOnIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = TInterface.TIdentityCreate();
        LCourtLink first = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourtLink second = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, first);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, second);

        IReadOnlyList<LCourtLink> settled = TInterface.TCourtArchiveResolve(
            workspace.TWorkspaceFolder,
            target,
            "entry-real");

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
    public void CancellingADraftDropsEveryLinkWaitingOnIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = TInterface.TIdentityCreate();
        LCourtLink first = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourtLink second = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, first);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, second);

        IReadOnlyList<LCourtLink> dropped = TInterface.TCourtArchiveCancel(
            workspace.TWorkspaceFolder,
            target);

        Assert.Equal(2, dropped.Count);
        Assert.Contains(dropped, link => link.LCourtLinkId == first.LCourtLinkId);
        Assert.Contains(dropped, link => link.LCourtLinkId == second.LCourtLinkId);
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void ALinkOnAnotherTargetSurvivesACancel()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = TInterface.TIdentityCreate();
        string other = TInterface.TIdentityCreate();
        LCourtLink settled = TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourtLink kept = TDraftLinkCreate(TInterface.TIdentityCreate(), other, "ember");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, settled);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, kept);
        TInterface.TCourtArchiveResolve(workspace.TWorkspaceFolder, target, "entry-real");

        IReadOnlyList<LCourtLink> remaining = TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(remaining);
        Assert.Equal(kept.LCourtLinkId, remaining[0].LCourtLinkId);
        Assert.Equal(other, remaining[0].LCourtLinkTarget);

        Assert.Empty(TInterface.TCourtArchiveCancel(workspace.TWorkspaceFolder, target));
        Assert.NotNull(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, kept.LCourtLinkId));
    }

    [Fact]
    public void ATentativeLinkIsSavedAndReadBackAsItWasWritten()
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
    public void CommittingADraftLeavesTheDraftFolderEmpty()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(started with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
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
    public void ARefusedCommitLeavesTheDraftFileWhereItWas()
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
    public void MovingACardRenumbersEveryCardInTheHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(started with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses =
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
            held.LDraftContent.LEntryDraftSenses.Select(card => card.LCardDraftTitle.TStateValueShow()));
        Assert.Equal([1, 2, 3], held.LDraftContent.LEntryDraftSenses.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void ChangingOnlyTheLanguageIsRecordedAsOneChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        engine.TEngineDraftSave(first with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
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
    public void TwoDraftsPointingAtEachOtherCommitWithoutRecursion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft first = engine.TEngineDraftStart("editor", null);
        LDraft second = engine.TEngineDraftStart("editor", null);

        engine.TEngineDraftSave(first with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
            },
        });
        engine.TEngineDraftSave(second with
        {
            LDraftContent = TDraftCreate("editor", "ember").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("a glowing coal")],
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
    public void CancellingOneDraftKeepsATargetAnotherDraftStillNames()
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
    public void ADraftStillHeldOpenIsNotReportedAsLeftover()
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
                    LEntryDraftSenses = [TDraftCardCreate("set alight")],
                },
            });
            engine.TEngineDraftSave(library with
            {
                LDraftContent = TDraftCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftSenses = [TDraftCardCreate("a glowing coal")],
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
    public void ALeftoverDraftNamesTheEntryItWasStartedFrom()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft written = started with
        {
            LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
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

    private static LCourtLink TDraftLinkCreate(string owner, string target, string headword)
    {
        return TInterface.TCourtLinkCreate(TInterface.TIdentityCreate(), owner, target, headword, "English");
    }

    private static LDraft TDraftCreate(string origin, string headword)
    {
        LCardDraft sense = TInterface.TCardDraftCreate(
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
            [sense],
            []);

        return TInterface.TDraftCreate(
            TInterface.TIdentityCreate(),
            origin,
            string.Empty,
            content,
            DateTimeOffset.UtcNow);
    }
}
