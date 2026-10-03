using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineObserver
{
    [Fact]
    public void ObserverAttach_SettingsChanged_CountsOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        int count = 0;
        engine.TEngineObserverAttach(bulletin =>
        {
            if (bulletin.LBulletinSubject == LSubject.LSubjectSettings)
            {
                count++;
            }
        });

        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        Assert.Equal(1, count);
    }

    [Fact]
    public void ObserverDetach_SameDelegate_CountStays()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);
        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        engine.TEngineObserverDetach(bulletins.Add);
        engine.TEngineEpithetSave(settings.LSettingsEpithet);

        LBulletin bulletin = Assert.Single(bulletins);
        Assert.Equal(LSubject.LSubjectSettings, bulletin.LBulletinSubject);
    }

    [Fact]
    public void ObserverAttach_SameDelegateTwice_CountsOnce()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        LSettings settings = engine.TEngineSettingsRead();
        List<LBulletin> bulletins = [];
        engine.TEngineObserverAttach(bulletins.Add);
        engine.TEngineObserverAttach(bulletins.Add);

        engine.TEngineEpithetSave(!settings.LSettingsEpithet);

        Assert.Single(bulletins);
    }

    [Fact]
    public void TenureStateRead_NothingChanged_AnswersTheKeptStateAgain()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        LTenureState first = tenure.TTenureStateRead();

        Assert.Same(first, tenure.TTenureStateRead());
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureStateRead_RequestApplied_ComputesAFreshStateThatSeesTheChange()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        LTenureState before = tenure.TTenureStateRead();
        tenure.TTenureRequestApply(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("ember")));
        LTenureState after = tenure.TTenureStateRead();

        Assert.NotSame(before, after);
        Assert.False(before.LTenureStateChanged);
        Assert.True(after.LTenureStateChanged);
        Assert.True(after.LTenureStateBackward);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureStateRead_Undone_ComputesAFreshStateThatSeesTheStepBack()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestApply(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("ember")));
        LTenureState before = tenure.TTenureStateRead();
        tenure.TTenureUndo();
        LTenureState after = tenure.TTenureStateRead();

        Assert.NotSame(before, after);
        Assert.False(after.LTenureStateChanged);
        Assert.False(after.LTenureStateBackward);
        Assert.True(after.LTenureStateForward);
        tenure.TTenureCancel();
    }

    [Fact]
    public void TenureStateRead_SameRecordStoredByAnotherTenure_ComputesAFreshStateThatSeesTheStoredRecord()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure first = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        first.TTenureRequestApply(
            TInterface.TExampleTextCreate(first.LTenureId, TInterfaceState.TStateValueCreate("ash")));
        long stored = first.TTenureFinish(true)!.Value;

        LTenure watching = engine.TEngineTenureStart("test", LSubject.LSubjectExample, stored);
        LTenureState before = watching.TTenureStateRead();
        LTenure storing = engine.TEngineTenureStart("test", LSubject.LSubjectExample, stored);
        storing.TTenureRequestApply(
            TInterface.TExampleTextCreate(storing.LTenureId, TInterfaceState.TStateValueCreate("ember")));
        storing.TTenureFinish(true);
        LTenureState after = watching.TTenureStateRead();

        Assert.NotSame(before, after);
        Assert.False(before.LTenureStateChanged);
        Assert.True(after.LTenureStateChanged);
        watching.TTenureCancel();
    }

    [Fact]
    public void TenureStateRead_Cancelled_AnswersTheEndedStateInsteadOfTheKeptOne()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        engine.TEngineDelaySet(0);

        LTenure tenure = engine.TEngineTenureStart("test", LSubject.LSubjectExample, null);
        tenure.TTenureRequestApply(
            TInterface.TExampleTextCreate(tenure.LTenureId, TInterfaceState.TStateValueCreate("ember")));
        LTenureState before = tenure.TTenureStateRead();
        tenure.TTenureCancel();
        LTenureState after = tenure.TTenureStateRead();

        Assert.NotSame(before, after);
        Assert.False(after.LTenureStateChanged);
        Assert.False(after.LTenureStateBackward);
    }
}
