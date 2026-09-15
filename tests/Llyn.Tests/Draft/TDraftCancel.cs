using Llyn.Core;
using Llyn.Infrastructure;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDraftCancel
{
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
    public void DraftCancel_TargetOfAnotherDraft_StrikesItsIdFromThatDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LDraft owner = engine.TEngineDraftStart("Input", null);
        LDraft target = engine.TEngineDraftStart("Library", null);

        LEntryDraft content = TInterface.TDraftPlainCreate("kindle");
        engine.TRequestContentApply(owner.LDraftId, content with
            {
                LEntryDraftMeanings =
                [
                    content.LEntryDraftMeanings[0] with
                    {
                        LCardDraftTranslation = [target.LDraftId],
                    },
                ],
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
        LCourt stranded = TInterface.TDraftLinkCreate(
            TInterface.TIdentityCreate(), TInterface.TIdentityCreate(), "hearth");

        TInterface.TCourtArchiveSave(workspace.TWorkspaceFolder, stranded);
        engine.TEngineDraftDelete(held.LDraftId);

        Assert.Null(engine.TEngineDraftRead(held.LDraftId));
        Assert.Empty(TInterface.TCourtArchiveScan(workspace.TWorkspaceFolder));
    }
}
