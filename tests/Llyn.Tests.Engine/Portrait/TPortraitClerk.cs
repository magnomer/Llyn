using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitClerk
{
    [Fact]
    public async Task PortraitClerkPrint_EntryPage_HandsTheSheetToThePressToPrint()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LPortraitClerk clerk = TInterface.TPortraitClerkCreate(rig);
        LEntry water = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(
            TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [], []));

        LPortraitPage page = clerk.TPortraitClerkRead(water.LEntryId, TInterface.TPortraitLabelRead());
        LGalleyClerk opened = await TInterface.TPortraitPressCreate(rig).TPortraitClerkOpen(page);
        await using LGalleyClerk galley = opened;

        Assert.Equal("water", page.LPortraitPageTitle);
        Assert.Contains("water", workspace.TWorkspacePress.TPressHtml);
        Assert.True(workspace.TWorkspacePress.TPressOpened);
    }
}
