using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftArchive
{
    [Fact]
    public void DraftArchiveSave_WholeDraft_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft);
        LDraft? loaded = TInterface.TDraftArchiveRead(workspace.TWorkspaceFolder, draft.LDraftId);

        Assert.NotNull(loaded);
        Assert.Equal(draft.LDraftId, loaded.LDraftId);
        Assert.Equal(draft.LDraftOrigin, loaded.LDraftOrigin);
        Assert.Equal(draft.LDraftEntryId, loaded.LDraftEntryId);
        Assert.Equal(draft.LDraftMoment, loaded.LDraftMoment);
        Assert.Equal(draft.LDraftContent.LEntryDraftHeadword, loaded.LDraftContent.LEntryDraftHeadword);
        Assert.Equal(draft.LDraftContent.LEntryDraftLanguage, loaded.LDraftContent.LEntryDraftLanguage);
        Assert.Equal(draft.LDraftContent.LEntryDraftIpa, loaded.LDraftContent.LEntryDraftIpa);
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
    public void DraftArchiveSave_DerivedProperties_LeavesThemOutOfTheFile()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft);
        string text = File.ReadAllText(
            Path.Combine(workspace.TWorkspaceFolder, "drafts", draft.LDraftId + ".json"));

        Assert.DoesNotContain("\"LEntryDraftIpa\"", text);
        Assert.DoesNotContain("\"LEntryDraftTargets\"", text);
        Assert.Contains("\"LEntryDraftHeadword\"", text);
    }

    [Fact]
    public void DraftArchiveSave_SituationWithMedia_ReadsBackBothLists()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineSituationStart("Repertoire", null);
        LDraft draft = started with
        {
            LDraftSituation = started.LDraftSituation! with
            {
                LSituationImage =
                [
                    TInterface.TImageDraftCreate("court-a.png", TInterface.TIdentityCreate()),
                    TInterface.TImageDraftCreate("court-b.png", TInterface.TIdentityCreate()),
                ],
                LSituationVideo =
                [
                    TInterface.TVideoDraftCreate("court.mp4", "00:10-00:40", TInterface.TIdentityCreate()),
                ],
            },
        };

        TInterface.TDraftArchiveSave(workspace.TWorkspaceFolder, draft);
        LDraft? loaded = TInterface.TDraftArchiveRead(workspace.TWorkspaceFolder, draft.LDraftId);

        Assert.NotNull(loaded);
        Assert.NotNull(loaded.LDraftSituation);
        Assert.Equal(draft.LDraftSituation.LSituationImage, loaded.LDraftSituation.LSituationImage);
        Assert.Equal(draft.LDraftSituation.LSituationVideo, loaded.LDraftSituation.LSituationVideo);
        Assert.Equal("00:10-00:40", loaded.LDraftSituation.LSituationVideo[0].LVideoDraftSpan.TStateValueShow());
    }

    [Fact]
    public void DraftArchiveScan_HeldDrafts_ListsAll()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraft first = TInterface.TDraftNestedCreate("editor", "kindle");
        LDraft second = TInterface.TDraftNestedCreate("library", "ember");
        LDraft third = TInterface.TDraftNestedCreate("editor", "hearth");

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
        LDraft first = TInterface.TDraftNestedCreate("editor", "kindle");
        LDraft second = TInterface.TDraftNestedCreate("library", "ember");

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
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");

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
        long target = TInterface.TIdentityCreate();
        LCourt first = TInterface.TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourt second = TInterface.TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, first);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, second);

        IReadOnlyList<LCourt> settled = TInterface.TCourtArchiveSettle(
            workspace.TWorkspaceFolder,
            target);

        Assert.Equal(2, settled.Count);
        Assert.Contains(settled, link => link.LCourtId == first.LCourtId);
        Assert.Contains(settled, link => link.LCourtId == second.LCourtId);
        Assert.Contains(settled, link => link.LCourtOwnerId == first.LCourtOwnerId);
        Assert.Contains(settled, link => link.LCourtOwnerId == second.LCourtOwnerId);
        Assert.Null(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, first.LCourtId));
        Assert.Null(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, second.LCourtId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }

    [Fact]
    public void CourtArchiveSettle_OtherTargetLink_LeavesIt()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        long target = TInterface.TIdentityCreate();
        long other = TInterface.TIdentityCreate();
        LCourt settled = TInterface.TDraftLinkCreate(TInterface.TIdentityCreate(), target, "hearth");
        LCourt kept = TInterface.TDraftLinkCreate(TInterface.TIdentityCreate(), other, "ember");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, settled);
        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, kept);
        TInterface.TCourtArchiveSettle(workspace.TWorkspaceFolder, target);

        IReadOnlyList<LCourt> remaining = TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder);

        Assert.Single(remaining);
        Assert.Equal(kept.LCourtId, remaining[0].LCourtId);
        Assert.Equal(other, remaining[0].LCourtTargetId);

        Assert.Empty(TInterface.TCourtArchiveSettle(workspace.TWorkspaceFolder, target));
        Assert.NotNull(TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, kept.LCourtId));
    }

    [Fact]
    public void CourtArchiveSave_TentativeLink_ReadsBackAsWritten()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LCourt link = TInterface.TDraftLinkCreate(TInterface.TIdentityCreate(), TInterface.TIdentityCreate(), "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, link);
        LCourt? loaded = TInterface.TCourtArchiveRead(workspace.TWorkspaceFolder, link.LCourtId);

        Assert.NotNull(loaded);
        Assert.Equal(link.LCourtOwnerId, loaded.LCourtOwnerId);
        Assert.Equal(link.LCourtTargetId, loaded.LCourtTargetId);
        Assert.Equal(link.LCourtHeadword, loaded.LCourtHeadword);
        Assert.Equal(link.LCourtLanguage, loaded.LCourtLanguage);
    }
}
