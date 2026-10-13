using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDeskChronicle
{
    [Fact]
    public void Undo_AfterName_StepsBackAndRedoStepsForward()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, []));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskDraft.CDeskDraftPersist();

        Assert.True(desk.CDeskChronicle.CDeskChronicleRead().CDeskBackward);
        Assert.True(desk.CDeskDraft.CDeskDraftAltered);
        Assert.True(desk.CDeskDraft.CDeskDraftStorable);

        desk.CDeskChronicle.CDeskChronicleUndo();

        Assert.True(desk.CDeskChronicle.CDeskChronicleRead().CDeskForward);
        Assert.Equal(string.Empty, desk.TDeskRead()?.LDraftAuthorName ?? string.Empty);

        desk.CDeskChronicle.CDeskChronicleRedo();

        Assert.Equal("Ada", desk.TDeskRead()?.LDraftAuthorName);
    }

    [Fact]
    public void Undo_WorkspaceChanged_ShowsTheHoldFailureOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, asked));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskDraft.CDeskDraftPersist();
        engine.TEngineWorkspaceOpen(workspace.TWorkspaceFolder);
        int changed = 0;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskChronicle.CDeskChronicleUndo();

        Assert.Equal(["Guild.HoldFailed"], asked);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void Redo_WorkspaceChanged_ShowsTheHoldFailureOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> asked = [];
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, asked));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskDraft.CDeskDraftPersist();
        desk.CDeskChronicle.CDeskChronicleUndo();
        engine.TEngineWorkspaceOpen(workspace.TWorkspaceFolder);

        desk.CDeskChronicle.CDeskChronicleRedo();

        Assert.Equal(["Guild.HoldFailed"], asked);
    }

    [Fact]
    public void StateUpdate_NotHalted_RefusesNothingAndAnnouncesState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> refused = [];
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, refused));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        desk.CDeskStart(null);
        int changed = 0;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskChronicle.CDeskChronicleResonate();

        Assert.False(desk.CDeskChronicle.CDeskChronicleHalted);
        Assert.Empty(refused);
        Assert.Equal(1, changed);
    }
}
