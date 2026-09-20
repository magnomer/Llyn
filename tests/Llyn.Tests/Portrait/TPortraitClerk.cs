using Llyn.Application;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitClerk
{
    [Fact]
    public async Task PortraitClerkPrint_EntryPage_HandsTheSheetAndTheTicketToThePress()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        LRig rig = workspace.TWorkspaceRigCreate();
        LPortraitClerk clerk = TInterface.TPortraitClerkCreate(rig);
        LEntry water = TInterface.TEntryClerkCreate(rig).TEntryClerkSave(
            TInterface.TEntryDraftCreate("water", "English", string.Empty, string.Empty, [], []));
        LPressTicket ticket = TInterface.TPressTicketCreate("Paper Printer", false, 1);

        LPortraitPage page = clerk.TPortraitClerkRead(water.LEntryId, TInterface.TPortraitLabelRead());
        await clerk.TPortraitClerkPrint(page, ticket);

        Assert.Equal("water", page.LPortraitPageTitle);
        Assert.Contains("water", workspace.TWorkspacePress.TPressHtml);
        Assert.Same(ticket, workspace.TWorkspacePress.TPressTicket);
    }
}
