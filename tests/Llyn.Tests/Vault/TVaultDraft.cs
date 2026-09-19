using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TVaultDraft
{
    [Fact]
    public void DraftRead_AfterSave_ReturnsStoredDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraftVault drafts = TInterface.TDraftVaultCreate(workspace.TWorkspaceFolder);
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");

        drafts.TDraftSave(draft);
        LDraft? read = drafts.TDraftRead(draft.LDraftId);

        Assert.NotNull(read);
        Assert.Equal(draft.LDraftId, read.LDraftId);
        Assert.Equal("kindle", read.LDraftContent.LEntryDraftHeadword);
    }

    [Fact]
    public void DraftScan_TwoSaved_ListsBoth()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraftVault drafts = TInterface.TDraftVaultCreate(workspace.TWorkspaceFolder);
        LDraft first = TInterface.TDraftNestedCreate("editor", "kindle");
        LDraft second = TInterface.TDraftNestedCreate("editor", "ember");

        drafts.TDraftSave(first);
        drafts.TDraftSave(second);

        Assert.Equal(2, drafts.TDraftScan().Count);
    }

    [Fact]
    public void DraftDelete_SavedDraft_ReadReturnsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspaceCreate();
        LDraftVault drafts = TInterface.TDraftVaultCreate(workspace.TWorkspaceFolder);
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");

        drafts.TDraftSave(draft);
        drafts.TDraftDelete(draft.LDraftId);

        Assert.Null(drafts.TDraftRead(draft.LDraftId));
        Assert.Empty(drafts.TDraftScan());
    }
}
