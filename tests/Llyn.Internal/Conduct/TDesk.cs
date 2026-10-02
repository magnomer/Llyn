using System;
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
        Assert.True(desk.TDeskChangeCheck());
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
        Assert.False(desk.TDeskChangeCheck());
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
        Assert.False(desk.TDeskChangeCheck());
    }

    [Fact]
    public void Start_NoVista_HoldsNothing()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, []));

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
            engine, "Guild", TEnvoyFake.TEnvoyCreate(false, []), "Guild", CSubject.CSubjectAuthor);
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

        desk.CDeskDraftResonate();

        Assert.Equal([true], filling);
        Assert.False(desk.CDeskFilling);
    }

    [Fact]
    public void StateUpdate_NotHalted_RefusesNothingAndAnnouncesState()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        List<string> refused = [];
        CDesk desk = TDeskPrepare(engine, refused);
        desk.CDeskStart(null);
        int changed = 0;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskStateResonate();

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
    public void Start_RefusedTenure_ShowsTheLoadFailureOnce()
    {
        List<string> asked = [];
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTenureStart"] = _ => throw new InvalidOperationException("refused"),
        });
        CDesk desk = new(
            drafts,
            TInterfaceConduct.TSettingsCreate(),
            "Example",
            TEnvoyFake.TEnvoyCreate(false, asked),
            "Corpus",
            CSubject.CSubjectExample);

        desk.CDeskStart(null);

        Assert.Equal(["Example.LoadFailed"], asked);
        Assert.False(desk.CDeskHeld);
    }

    [Fact]
    public void Start_OverAHeldTenure_AnnouncesTheChangeOfStateOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        desk.CDeskStart(null);
        int changed = 0;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskStart(null);

        Assert.True(desk.CDeskHeld);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void Start_RefusedTenure_AnnouncesTheChangeOfStateOnce()
    {
        LDraftPort drafts = TEngineFake.TEngineCreate<LDraftPort>(new Dictionary<string, Func<object?[]?, object?>>
        {
            ["LEngineTenureStart"] = _ => throw new InvalidOperationException("refused"),
        });
        CDesk desk = new(
            drafts,
            TInterfaceConduct.TSettingsCreate(),
            "Example",
            TEnvoyFake.TEnvoyCreate(false, []),
            "Corpus",
            CSubject.CSubjectExample);
        int changed = 0;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskStart(null);

        Assert.False(desk.CDeskHeld);
        Assert.Equal(1, changed);
    }

    [Fact]
    public void Cancel_HeldTenure_AnnouncesTheChangeOfStateOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        CDesk desk = TDeskPrepare(engine);
        desk.CDeskStart(null);
        int changed = 0;
        desk.CDeskStateChanged += () => changed++;

        desk.CDeskCancel();

        Assert.False(desk.CDeskHeld);
        Assert.Equal(1, changed);
    }

    private static CDesk TDeskPrepare(LEngine engine) => TDeskPrepare(engine, []);

    private static CDesk TDeskPrepare(LEngine engine, List<string> asked)
    {
        CDesk desk = TInterfaceConduct.TDeskCreate(engine, "Guild", TEnvoyFake.TEnvoyCreate(false, asked));
        desk.TDeskVistaRestore(engine.TEngineVistaStart("guild", LCatalogOrder.LCatalogOrderName));
        return desk;
    }
}
