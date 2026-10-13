using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeskDraft
{
    [Fact]
    public void DraftUpdate_HeldDraft_AnnouncesItWhileFilling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, []));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        List<bool> filling = [];
        desk.CDeskDraft.CDeskDraftChanged += _ => filling.Add(desk.CDeskDraft.CDeskDraftFilling);

        desk.CDeskDraft.CDeskDraftResonate();

        Assert.Equal([true], filling);
        Assert.False(desk.CDeskDraft.CDeskDraftFilling);
    }
}
