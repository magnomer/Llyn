using System;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineTenure
{
    private const int TTenureHold = 600000;

    [Fact]
    public void TenureDefer_SameKey_AppliesLast()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("one")));
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("two")));

        Assert.Equal(string.Empty, tenure.TTenureRead()?.LDraftExample?.LExampleText.TStateValueShow());

        tenure.TTenurePersist();

        Assert.Equal("two", tenure.TTenureRead()?.LDraftExample?.LExampleText.TStateValueShow());
        Assert.True(tenure.TTenureStateRead().LTenureStateChanged);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureFinish_Unchanged_Cancels()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        long held = tenure.LTenureId;

        Assert.Null(tenure.TTenureFinish(true));
        Assert.Null(engine.TEngineDraftRead(held));
        Assert.Null(tenure.TTenureRead());
        Assert.Empty(engine.TEngineExampleRead());
    }

    [Fact]
    public void TenureFinish_Changed_CommitsExample()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("ember")));

        long? stored = tenure.TTenureFinish(true);

        Assert.NotNull(stored);
        Assert.Equal("ember", engine.TEngineExampleRead(stored.Value)?.LExampleText.TStateValueShow());
        Assert.Null(engine.TEngineDraftRead(tenure.LTenureId));
    }

    [Fact]
    public void TenureDefer_ApplyThrows_MarksHalted()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        engine.TEngineDraftCancel(tenure.LTenureId);

        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("lost")));

        Assert.True(tenure.TTenureStateRead().LTenureStateHalted);
        Assert.Throws<LRefusal>(() => tenure.TTenureFinish(true));
        Assert.Null(tenure.TTenureUndo());
    }

    [Fact]
    public void TenureUndo_AfterDefer_PersistsFirst()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(TTenureHold);
        DateTimeOffset moment = new(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
        engine.TEngineClockSet(() => moment);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestApply(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("one")));
        moment += TimeSpan.FromMilliseconds(2000);
        tenure.TTenureRequestDefer(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterface.TStateValueCreate("two")));

        LDraft? restored = tenure.TTenureUndo();

        Assert.Equal("one", restored?.LDraftExample?.LExampleText.TStateValueShow());
        Assert.True(tenure.TTenureStateRead().LTenureStateForward);
        Assert.Equal("two", tenure.TTenureRedo()?.LDraftExample?.LExampleText.TStateValueShow());
        tenure.TTenureCancel();
    }
}
