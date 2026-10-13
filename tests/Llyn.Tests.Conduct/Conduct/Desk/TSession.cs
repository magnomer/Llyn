using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TSession
{
    [Fact]
    public void Save_NamedDraft_StoresAndShowsTheStoredId()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        List<long> shown = [];
        CSession session =
            TInterfaceConductDesk.TSessionCreate(desk, [desk.LDeskChangeCheck], static () => true, shown.Add);
        int held = 0;
        session.CSessionHeld += () => held++;

        session.CSessionStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));

        Assert.True(session.TSessionChangeCheck());
        Assert.True(session.CSessionSave());
        Assert.Equal(1, held);
        Assert.Single(shown);
        Assert.False(desk.CDeskHeld);
    }

    [Fact]
    public void Save_Unchanged_KeepsTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        List<long> shown = [];
        CSession session =
            TInterfaceConductDesk.TSessionCreate(desk, [desk.LDeskChangeCheck], static () => true, shown.Add);

        session.CSessionStart(null);

        Assert.False(session.TSessionChangeCheck());
        Assert.True(session.CSessionSave());
        Assert.True(desk.CDeskHeld);
        Assert.Empty(shown);
    }

    [Fact]
    public void Finish_NotReady_RefusesAndKeepsTheDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, [], static () => false, static _ => { });
        session.CSessionStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));

        Assert.False(session.TSessionFinish(true));
        Assert.False(session.CSessionSave());
        Assert.True(desk.CDeskHeld);
        Assert.True(session.TSessionFinish(false));
        Assert.False(desk.CDeskHeld);
    }

    [Fact]
    public void Close_Store_FinishesWithoutTheReadyCheck()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        long? finished = null;
        desk.CDeskFinished += id => finished = id;
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, [], static () => false, static _ => { });
        session.CSessionStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));

        Assert.True(session.CSessionClose(true));
        Assert.NotNull(finished);
    }

    [Fact]
    public void Cancel_HeldDraft_DropsItAndAnnouncesHeld()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, [], static () => true, static _ => { });
        int held = 0;
        int changed = 0;
        session.CSessionHeld += () => held++;
        session.CSessionChanged += () => changed++;
        session.CSessionStart(null);

        session.CSessionCancel();

        Assert.False(desk.CDeskHeld);
        Assert.Equal(2, held);
        Assert.True(changed > 0);
    }

    [Fact]
    public void Undo_OwnDraft_StepsTheDeskBackAndForward()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, [], static () => true, static _ => { });
        session.CSessionStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskDraft.CDeskDraftPersist();

        Assert.True(session.CSessionChronicleRead().CDeskBackward);

        session.CSessionUndo();

        Assert.True(session.CSessionChronicleRead().CDeskForward);

        session.CSessionRedo();

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
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, [], static () => true, static _ => { });
        session.CSessionStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskDraft.CDeskDraftPersist();
        engine.TEngineWorkspaceOpen(workspace.TWorkspaceFolder);

        session.CSessionUndo();

        Assert.Equal(["Guild.HoldFailed"], asked);
    }

    [Fact]
    public void Finish_EditorShown_HandsTheEditorTheDecision()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        List<string> seen = [];
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, editor, static () => true, seen);

        Assert.True(session.TSessionFinish(true));
        Assert.True(session.CSessionClose(false));
        Assert.True(session.CSessionSave());
        Assert.Equal(["Finish", "Drop"], seen);
        Assert.Equal((false, false), session.CSessionChronicleRead());
    }

    [Fact]
    public void Finish_EditorHidden_FinishesTheOwnDesk()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TSessionDeskPrepare(engine);
        TEditorFixture editor = new(TInterfaceEditor.TEditorCreate(engine));
        List<string> seen = [];
        CSession session = TInterfaceConductDesk.TSessionCreate(desk, editor, static () => false, seen);
        session.CSessionStart(null);

        Assert.True(session.TSessionFinish(false));
        Assert.Empty(seen);
        Assert.False(desk.CDeskHeld);
    }

    [Fact]
    public void QuitConfirm_NothingUnsaved_ClosesEveryEditorWithoutAsking()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        List<string> asked = [];
        List<bool> closed = [];

        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => false, store => { closed.Add(store); return true; });
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => false, store => { closed.Add(store); return true; });

        bool quit = atelier.CAtelierQuitConfirm(TEnvoyFake.TEnvoyCreate(true, asked));

        Assert.True(quit);
        Assert.Empty(asked);
        Assert.Equal([false, false], closed);
    }

    [Fact]
    public void QuitConfirm_UnsavedAndStore_StoresEveryEditor()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        List<string> asked = [];
        List<bool> closed = [];

        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => false, store => { closed.Add(store); return true; });
        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => true, store => { closed.Add(store); return false; });

        bool quit = atelier.CAtelierQuitConfirm(TEnvoyFake.TEnvoyCreate(true, asked));

        Assert.False(quit);
        Assert.Equal(["Leave"], asked);
        Assert.Equal([true, true], closed);
    }

    [Fact]
    public void QuitConfirm_UnsavedAndStay_ClosesNothing()
    {
        using LEngine engine = TRigFake.TRigFakeStart(TRigFake.TRigFakeBuild());
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        List<bool> closed = [];

        atelier.CAtelierWorkspace.TWorkspaceDraftAdd(static () => true, store => { closed.Add(store); return true; });

        bool quit = atelier.CAtelierQuitConfirm(TEnvoyFake.TEnvoyCreate(null, []));

        Assert.False(quit);
        Assert.Empty(closed);
    }

    private static CDesk TSessionDeskPrepare(LEngine engine)
    {
        CDesk desk = TInterfaceConductDesk.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, []));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        return desk;
    }
}
