using System.Collections.Generic;
using Llyn.Conduct;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TDesk
{
    [Fact]
    public void Finish_StoreAfterName_CommitsAndClears()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        long? finished = null;
        desk.CDeskFinished += id => finished = id;

        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));

        Assert.True(desk.CDeskHeld);
        Assert.True(desk.CDeskChangeCheck());
        Assert.True(desk.CDeskFinish(true));
        Assert.False(desk.CDeskHeld);
        Assert.NotNull(finished);
        Assert.Equal("Ada", engine.TEngineAuthorRead(finished!.Value)?.LAuthorName);
    }

    [Fact]
    public void Cancel_AfterName_DiscardsDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        int finished = 0;
        desk.CDeskFinished += _ => finished++;

        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskCancel();

        Assert.False(desk.CDeskHeld);
        Assert.Equal(0, finished);
        Assert.Empty(engine.TEngineAuthorFind(string.Empty));
        Assert.False(desk.CDeskChangeCheck());
    }

    [Fact]
    public void Start_StoredAuthor_ShowsItsName()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LAuthor ada = engine.TEngineAuthorCreate(TInterface.TAuthorCreate(0, "Ada"));
        CDesk desk = TDeskPrepare(engine);
        string? shown = null;
        desk.CDeskDraftChanged += draft => shown = draft.CDraftAuthorName;

        desk.CDeskStart(ada.LAuthorId);

        Assert.Equal("Ada", shown);
        Assert.True(desk.CDeskStored);
        Assert.Equal(ada.LAuthorId, desk.CDeskStoredRead());
        Assert.False(desk.CDeskChangeCheck());
    }

    [Fact]
    public void Start_NoVista_HoldsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Guild", TInterfaceConduct.TEnvoyCreate(false, []));

        desk.CDeskStart(null);

        Assert.False(desk.CDeskHeld);
        Assert.Null(desk.CDeskStoredRead());
        Assert.Equal((false, false), desk.CDeskChronicleRead());
    }

    [Fact]
    public void Start_OriginAndSubject_HoldsAFreshDraft()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConduct.TDeskCreate(
            engine, "Guild", TInterfaceConduct.TEnvoyCreate(false, []), "Guild", CSubject.CSubjectAuthor);
        int started = 0;
        desk.CDeskStarted += () => started++;

        desk.CDeskStart(null);

        Assert.True(desk.CDeskHeld);
        Assert.True(desk.CDeskRunning);
        Assert.False(desk.CDeskStored);
        Assert.Equal(1, started);
        Assert.NotNull(desk.TDeskRead());
    }

    [Fact]
    public void Undo_AfterName_StepsBackAndRedoStepsForward()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        desk.CDeskPersist();

        Assert.True(desk.CDeskChronicleRead().CDeskBackward);
        Assert.True(desk.CDeskChanged);
        Assert.True(desk.CDeskStorable);

        desk.CDeskUndo();

        Assert.True(desk.CDeskChronicleRead().CDeskForward);
        Assert.Equal(string.Empty, desk.TDeskRead()?.LDraftAuthorName ?? string.Empty);

        desk.CDeskRedo();

        Assert.Equal("Ada", desk.TDeskRead()?.LDraftAuthorName);
    }

    [Fact]
    public void DraftUpdate_HeldDraft_AnnouncesItWhileFilling()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        desk.CDeskStart(null);
        desk.TDeskDefer(TInterface.TRequestAuthorCreate(desk.CDeskId, "Ada"));
        List<bool> filling = [];
        desk.CDeskDraftChanged += _ => filling.Add(desk.CDeskFilling);

        desk.CDeskDraftUpdate();

        Assert.Equal([true], filling);
        Assert.False(desk.CDeskFilling);
    }

    [Fact]
    public void StateUpdate_NotHalted_RefusesNothingAndAnnouncesState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        desk.CDeskStart(null);
        List<string> refused = [];
        int changed = 0;
        desk.CDeskRefused += refused.Add;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskStateUpdate();

        Assert.False(desk.CDeskHalted);
        Assert.Empty(refused);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void Finish_NothingHeld_ReadsDone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        List<long> stored = [];

        Assert.True(desk.CDeskFinish(true, stored.Add));
        Assert.Empty(stored);
    }

    [Fact]
    public void MentionFind_NothingHeld_FindsNone()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        using CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, TEngineFake.TEngineStubCreate<LMediaPort>());
        CDesk desk = TDeskPrepare(engine);

        Assert.Null(atelier.CAtelierMention.CMentionFind(desk, 0, 0, "happy", 0, 5, true));
    }

    private static CDesk TDeskPrepare(LEngine engine)
    {
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Guild", TInterfaceConduct.TEnvoyCreate(false, []));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        return desk;
    }
}
