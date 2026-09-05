using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Database.Tests;

public sealed class TDraft
{
    [Fact]
    public void TDraftRoundTrip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TDraftCreate("editor", "kindle");

        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, draft);
        LDraft? loaded = LDraftArchive.LDraftArchiveRead(workspace.TWorkspaceFolder, draft.LDraftId);

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
            draft.LDraftContent.LEntryDraftSenses[0].LCardDraftTitle.LStateValueShow(),
            loaded.LDraftContent.LEntryDraftSenses[0].LCardDraftTitle.LStateValueShow());
        Assert.Equal(
            draft.LDraftContent.LEntryDraftSenses[0].LCardDraftMeaning.LStateValueShow(),
            loaded.LDraftContent.LEntryDraftSenses[0].LCardDraftMeaning.LStateValueShow());
        Assert.Empty(loaded.LDraftContent.LEntryDraftCollocations);
    }

    [Fact]
    public void TDraftListEveryFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft first = TDraftCreate("editor", "kindle");
        LDraft second = TDraftCreate("library", "ember");
        LDraft third = TDraftCreate("editor", "hearth");

        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, first);
        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, second);
        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, third);

        IReadOnlyList<LDraft> drafts = LDraftArchive.LDraftArchiveScan(workspace.TWorkspaceFolder);

        Assert.Equal(3, drafts.Count);
        Assert.Contains(drafts, draft => draft.LDraftId == first.LDraftId);
        Assert.Contains(drafts, draft => draft.LDraftId == second.LDraftId);
        Assert.Contains(drafts, draft => draft.LDraftId == third.LDraftId);
    }

    [Fact]
    public void TDraftDeleteOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft first = TDraftCreate("editor", "kindle");
        LDraft second = TDraftCreate("library", "ember");

        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, first);
        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, second);
        LDraftArchive.LDraftArchiveDelete(workspace.TWorkspaceFolder, first.LDraftId);

        IReadOnlyList<LDraft> drafts = LDraftArchive.LDraftArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(drafts);
        Assert.Equal(second.LDraftId, drafts[0].LDraftId);
        Assert.Null(LDraftArchive.LDraftArchiveRead(workspace.TWorkspaceFolder, first.LDraftId));
    }

    [Fact]
    public void TDraftSkipBrokenFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TDraftCreate("editor", "kindle");

        LDraftArchive.LDraftArchiveSave(workspace.TWorkspaceFolder, draft);

        string folder = LWorkspaceRoot.LWorkspaceDraftRead(workspace.TWorkspaceFolder);
        File.WriteAllText(Path.Combine(folder, "broken.json"), "{ not json");

        IReadOnlyList<LDraft> drafts = LDraftArchive.LDraftArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(drafts);
        Assert.Equal(draft.LDraftId, drafts[0].LDraftId);
    }

    [Fact]
    public void TDraftCourtResolveEveryLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = LIdentity.LIdentityCreate();
        LCourtLink first = TDraftLinkCreate(LIdentity.LIdentityCreate(), target, "hearth");
        LCourtLink second = TDraftLinkCreate(LIdentity.LIdentityCreate(), target, "hearth");

        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, first);
        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, second);

        IReadOnlyList<LCourtLink> settled = LCourtArchive.LCourtArchiveResolve(
            workspace.TWorkspaceFolder,
            target,
            "entry-real");

        Assert.Equal(2, settled.Count);
        Assert.Contains(settled, link => link.LCourtLinkId == first.LCourtLinkId);
        Assert.Contains(settled, link => link.LCourtLinkId == second.LCourtLinkId);
        Assert.Contains(settled, link => link.LCourtLinkOwner == first.LCourtLinkOwner);
        Assert.Contains(settled, link => link.LCourtLinkOwner == second.LCourtLinkOwner);
        Assert.Null(LCourtArchive.LCourtArchiveRead(workspace.TWorkspaceFolder, first.LCourtLinkId));
        Assert.Null(LCourtArchive.LCourtArchiveRead(workspace.TWorkspaceFolder, second.LCourtLinkId));
        Assert.Empty(LCourtArchive.LCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void TDraftCourtCancelEveryLink()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = LIdentity.LIdentityCreate();
        LCourtLink first = TDraftLinkCreate(LIdentity.LIdentityCreate(), target, "hearth");
        LCourtLink second = TDraftLinkCreate(LIdentity.LIdentityCreate(), target, "hearth");

        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, first);
        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, second);

        IReadOnlyList<LCourtLink> dropped = LCourtArchive.LCourtArchiveCancel(
            workspace.TWorkspaceFolder,
            target);

        Assert.Equal(2, dropped.Count);
        Assert.Contains(dropped, link => link.LCourtLinkId == first.LCourtLinkId);
        Assert.Contains(dropped, link => link.LCourtLinkId == second.LCourtLinkId);
        Assert.Empty(LCourtArchive.LCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void TDraftCourtKeepOtherTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        string target = LIdentity.LIdentityCreate();
        string other = LIdentity.LIdentityCreate();
        LCourtLink settled = TDraftLinkCreate(LIdentity.LIdentityCreate(), target, "hearth");
        LCourtLink kept = TDraftLinkCreate(LIdentity.LIdentityCreate(), other, "ember");

        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, settled);
        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, kept);
        LCourtArchive.LCourtArchiveResolve(workspace.TWorkspaceFolder, target, "entry-real");

        IReadOnlyList<LCourtLink> remaining = LCourtArchive.LCourtArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(remaining);
        Assert.Equal(kept.LCourtLinkId, remaining[0].LCourtLinkId);
        Assert.Equal(other, remaining[0].LCourtLinkTarget);

        Assert.Empty(LCourtArchive.LCourtArchiveCancel(workspace.TWorkspaceFolder, target));
        Assert.NotNull(LCourtArchive.LCourtArchiveRead(workspace.TWorkspaceFolder, kept.LCourtLinkId));
    }

    [Fact]
    public void TDraftCourtRoundTrip()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LCourtLink link = TDraftLinkCreate(LIdentity.LIdentityCreate(), LIdentity.LIdentityCreate(), "hearth");

        LCourtArchive.LCourtArchiveSave(workspace.TWorkspaceFolder, link);
        LCourtLink? loaded = LCourtArchive.LCourtArchiveRead(workspace.TWorkspaceFolder, link.LCourtLinkId);

        Assert.NotNull(loaded);
        Assert.Equal(link.LCourtLinkOwner, loaded.LCourtLinkOwner);
        Assert.Equal(link.LCourtLinkTarget, loaded.LCourtLinkTarget);
        Assert.Equal(link.LCourtLinkHeadword, loaded.LCourtLinkHeadword);
        Assert.Equal(link.LCourtLinkLanguage, loaded.LCourtLinkLanguage);
    }

    [Fact]
    public void TDraftCommitEmptyFolder()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft started = engine.LEngineDraftStart("editor", null);
        engine.LEngineDraftSave(started with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
            },
        });

        LEntry stored = engine.LEngineDraftCommit(started.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.False(string.IsNullOrWhiteSpace(stored.LEntryId));
        Assert.Equal("kindle", engine.LEngineEntryRead(stored.LEntryId)?.LEntryHeadword);
        Assert.Empty(engine.LEngineDraftScan());
        Assert.Null(engine.LEngineDraftRead(started.LDraftId));
    }

    [Fact]
    public void TDraftCommitRefusalKeepFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft started = engine.LEngineDraftStart("editor", null);

        Assert.Throws<LRefusal>(() => engine.LEngineDraftCommit(started.LDraftId));

        LDraft? held = engine.LEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(started.LDraftId, held.LDraftId);
        Assert.Single(engine.LEngineDraftScan());
        Assert.Empty(engine.LEngineEntryFind(string.Empty));
    }

    [Fact]
    public void TDraftMoveRenumberEveryCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft started = engine.LEngineDraftStart("editor", null);
        engine.LEngineDraftSave(started with
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

        IReadOnlyList<LCardDraft> moved = engine.LEngineDraftMove(started.LDraftId, false, 0, 2);

        Assert.Equal(
            ["second", "third", "first"],
            moved.Select(card => card.LCardDraftTitle.LStateValueShow()));
        Assert.Equal([1, 2, 3], moved.Select(card => card.LCardDraftPosition));

        LDraft? held = engine.LEngineDraftRead(started.LDraftId);

        Assert.NotNull(held);
        Assert.Equal(
            ["second", "third", "first"],
            held.LDraftContent.LEntryDraftSenses.Select(card => card.LCardDraftTitle.LStateValueShow()));
        Assert.Equal([1, 2, 3], held.LDraftContent.LEntryDraftSenses.Select(card => card.LCardDraftPosition));
    }

    [Fact]
    public void TDraftLanguageOnlyEditCountChanged()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft first = engine.LEngineDraftStart("editor", null);
        engine.LEngineDraftSave(first with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
            },
        });

        LEntry stored = engine.LEngineDraftCommit(first.LDraftId);
        LDraft opened = engine.LEngineDraftStart("editor", stored.LEntryId);

        Assert.False(engine.LEngineDraftCheck(opened.LDraftId));

        engine.LEngineDraftSave(opened with
        {
            LDraftContent = opened.LDraftContent with { LEntryDraftLanguage = "Korean" },
        });

        Assert.True(engine.LEngineDraftCheck(opened.LDraftId));
    }

    [Fact]
    public void TDraftCommitCycleWithoutRecursion()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft first = engine.LEngineDraftStart("editor", null);
        LDraft second = engine.LEngineDraftStart("editor", null);

        engine.LEngineDraftSave(first with
        {
            LDraftContent = TDraftCreate("editor", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
            },
        });
        engine.LEngineDraftSave(second with
        {
            LDraftContent = TDraftCreate("editor", "ember").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("a glowing coal")],
            },
        });

        engine.LEngineCourtSave(first.LDraftId, second.LDraftId, "ember", "English");
        engine.LEngineCourtSave(second.LDraftId, first.LDraftId, "kindle", "English");

        LEntry stored = engine.LEngineDraftCommit(first.LDraftId);

        Assert.Equal("kindle", stored.LEntryHeadword);
        Assert.Single(engine.LEngineEntryFind("ember"));
        Assert.Empty(engine.LEngineDraftScan());
        Assert.Empty(LCourtArchive.LCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void TDraftCancelKeepSharedTarget()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft first = engine.LEngineDraftStart("editor", null);
        LDraft second = engine.LEngineDraftStart("editor", null);
        LDraft target = engine.LEngineDraftStart("editor", null);

        engine.LEngineCourtSave(first.LDraftId, target.LDraftId, "ember", "English");
        engine.LEngineCourtSave(second.LDraftId, target.LDraftId, "ember", "English");

        engine.LEngineDraftCancel(first.LDraftId);

        Assert.Null(engine.LEngineDraftRead(first.LDraftId));
        Assert.NotNull(engine.LEngineDraftRead(target.LDraftId));
        Assert.NotNull(engine.LEngineCourtFind(second.LDraftId, target.LDraftId));

        engine.LEngineDraftCancel(second.LDraftId);

        Assert.Null(engine.LEngineDraftRead(target.LDraftId));
    }

    [Fact]
    public void TDraftLeftoverSkipHeldDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();

        string first;
        string second;

        using (LEngine engine = new(workspace.TWorkspaceFolder))
        {
            LDraft input = engine.LEngineDraftStart("Input", null);
            LDraft library = engine.LEngineDraftStart("Library", null);
            engine.LEngineDraftStart("Phonology", null);

            first = input.LDraftId;
            second = library.LDraftId;

            engine.LEngineDraftSave(input with
            {
                LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
                {
                    LEntryDraftSenses = [TDraftCardCreate("set alight")],
                },
            });
            engine.LEngineDraftSave(library with
            {
                LDraftContent = TDraftCreate("Library", "ember").LDraftContent with
                {
                    LEntryDraftSenses = [TDraftCardCreate("a glowing coal")],
                },
            });

            Assert.Empty(engine.LEngineLeftoverRead());
        }

        using LEngine launched = new(workspace.TWorkspaceFolder);

        IReadOnlyList<LDraft> leftovers = launched.LEngineLeftoverRead();

        Assert.Equal(2, leftovers.Count);
        Assert.Contains(leftovers, draft => draft.LDraftId == first);
        Assert.Contains(leftovers, draft => draft.LDraftId == second);
    }

    [Fact]
    public void TDraftLeftoverNameStoredEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = new(workspace.TWorkspaceFolder);

        LDraft started = engine.LEngineDraftStart("Input", null);
        LDraft written = started with
        {
            LDraftContent = TDraftCreate("Input", "kindle").LDraftContent with
            {
                LEntryDraftSenses = [TDraftCardCreate("set alight")],
            },
        };

        engine.LEngineDraftSave(written);

        LEntry stored = engine.LEngineEntrySave(written.LDraftContent);

        engine.LEngineDraftSave(written with { LDraftEntry = stored.LEntryId });

        LEntry recommitted = engine.LEngineDraftCommit(started.LDraftId);

        Assert.Equal(stored.LEntryId, recommitted.LEntryId);
        Assert.Single(engine.LEngineEntryFind("kindle"));
        Assert.Empty(engine.LEngineDraftScan());
    }

    private static LCardDraft TDraftCardCreate(string title)
    {
        return new LCardDraft(
            LStateValue.LStateValueCreate(title),
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
        return new LCourtLink(LIdentity.LIdentityCreate(), owner, target, headword, "English");
    }

    private static LDraft TDraftCreate(string origin, string headword)
    {
        LCardDraft sense = new(
            LStateValue.LStateValueCreate("set alight"),
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueCreate("to set something burning"),
            [LExampleDraft.LExampleDraftCreate("she knelt to kindle the damp logs")],
            [LSituationDraft.LSituationDraftCreate("around a hearth")],
            ["불을 붙이다"],
            "ignite",
            ["literal"],
            [],
            0);

        LEntryDraft content = new(
            headword,
            "English",
            "/ˈkɪnd(ə)l/",
            "Chiefly literary.",
            [sense],
            []);

        return new LDraft(
            LIdentity.LIdentityCreate(),
            origin,
            string.Empty,
            content,
            DateTimeOffset.UtcNow);
    }
}
