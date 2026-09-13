using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TRequestSituation
{
    [Fact]
    public void RequestApply_ImageRemovalOnSituationDraft_DropsTheRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineSituationStart("Repertoire", null);

        engine.TEngineRequestApply(
            TInterface.TImageAdditionCreate(started.LDraftId, 0, "court-a.png", 0));
        LDraft added = engine.TEngineRequestApply(
            TInterface.TImageAdditionCreate(started.LDraftId, 0, "court-b.png", 1));
        long dropped = added.LDraftSituation!.LSituationImage[0].LImageDraftId;

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TImageRemovalCreate(started.LDraftId, 0, dropped));

        LImageDraft kept = Assert.Single(answered.LDraftSituation!.LSituationImage);
        Assert.Equal("court-b.png", kept.LImageDraftLocation.TStateValueShow());
        Assert.Empty(answered.LDraftContent.LEntryDraftMeanings);
    }

    [Fact]
    public void RequestApply_ImageLocationOnSituationDraft_RewritesTheLocation()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineSituationStart("Repertoire", null);

        LDraft added = engine.TEngineRequestApply(
            TInterface.TImageAdditionCreate(started.LDraftId, 0, "court-a.png", 0));
        long image = added.LDraftSituation!.LSituationImage[0].LImageDraftId;

        LDraft answered = engine.TEngineRequestApply(TInterface.TImageLocationCreate(
            started.LDraftId, image, TInterface.TStateValueCreate("court-b.png")));

        LImageDraft held = Assert.Single(answered.LDraftSituation!.LSituationImage);
        Assert.Equal(image, held.LImageDraftId);
        Assert.Equal("court-b.png", held.LImageDraftLocation.TStateValueShow());
    }

    [Fact]
    public void RequestApply_VideoRemovalOnSituationDraft_RefusesUnknownRow()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineSituationStart("Repertoire", null);

        engine.TEngineRequestApply(
            TInterface.TVideoAdditionCreate(started.LDraftId, 0, "court.mp4", 0));

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TVideoRemovalCreate(started.LDraftId, 0, TInterface.TIdentityCreate())));

        Assert.Equal(LRefusal.LRefusalItem, refusal.LRefusalReason);
    }

    [Fact]
    public void RequestApply_ImageAdditionOnEntryDraft_StillRoutesToTheCard()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDraft started = engine.TEngineDraftStart("Input", null);
        LDraft opened = engine.TEngineRequestApply(
            TInterface.TRequestAdditionCreate(started.LDraftId, LCardKind.LCardKindMeaning, 0, 0));
        long card = opened.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;

        LDraft answered = engine.TEngineRequestApply(
            TInterface.TImageAdditionCreate(started.LDraftId, card, "media/fire.jpg", 0));

        Assert.Null(answered.LDraftSituation);
        LImageDraft held = Assert.Single(answered.LDraftContent.LEntryDraftMeanings[0].LCardDraftImage);
        Assert.Equal("media/fire.jpg", held.LImageDraftLocation.TStateValueShow());

        LRefusal refusal = Assert.Throws<LRefusal>(() => engine.TEngineRequestApply(
            TInterface.TImageAdditionCreate(started.LDraftId, 0, "media/ash.jpg", 0)));

        Assert.Equal(LRefusal.LRefusalCard, refusal.LRefusalReason);
    }
}
