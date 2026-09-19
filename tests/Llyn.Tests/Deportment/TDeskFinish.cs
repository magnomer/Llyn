using Llyn.Core;
using Llyn.ShellEngine;
using Llyn.UIDeportment;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeskFinish
{
    [Fact]
    public void Finish_StoreAfterName_CommitsAndClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TDeskPrepare(engine);
        long? finished = null;
        desk.LDeskFinished += id => finished = id;

        desk.TDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.LDeskId, "Ada"));

        Assert.True(desk.LDeskHeld);
        Assert.True(desk.TDeskChangeCheck());
        Assert.True(desk.TDeskFinish(true));
        Assert.False(desk.LDeskHeld);
        Assert.NotNull(finished);
        Assert.Equal("Ada", engine.TEngineAuthorRead(finished!.Value)?.LAuthorName);
    }

    [Fact]
    public void Cancel_AfterName_DiscardsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LDesk desk = TDeskPrepare(engine);
        int finished = 0;
        desk.LDeskFinished += _ => finished++;

        desk.TDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.LDeskId, "Ada"));
        desk.TDeskCancel();

        Assert.False(desk.LDeskHeld);
        Assert.Equal(0, finished);
        Assert.Empty(engine.TEngineAuthorRead());
        Assert.False(desk.TDeskChangeCheck());
    }

    [Fact]
    public void Start_StoredAuthor_ShowsItsName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        LDesk desk = TDeskPrepare(engine);
        string? shown = null;
        desk.LDeskDraftChanged += draft => shown = draft.LDraftAuthorName;

        desk.TDeskStart(ada.LAuthorId);

        Assert.Equal("Ada", shown);
        Assert.True(desk.LDeskStored);
        Assert.False(desk.TDeskChangeCheck());
    }

    private static LDesk TDeskPrepare(LEngine engine)
    {
        LDesk desk = TInterfaceDeportment.TDeskCreate(engine, "Guild", () => false);
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        return desk;
    }
}
