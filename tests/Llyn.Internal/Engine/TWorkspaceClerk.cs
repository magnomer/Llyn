using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TWorkspaceClerk
{
    [Fact]
    public void WorkspaceRescueCreate_DoctorRefuses_ThrowsBeforeAnythingIsRead()
    {
        LRig rig = TRigFake.TRigFakeBuild() with { LRigDoctor = new TDoctorFake() };

        Assert.Throws<InvalidOperationException>(() => TInterface.TWorkspaceRescueCreate(rig));
    }

    [Fact]
    public void WorkspaceSettingsRead_NothingStored_AnswersTheFallbackUnsettled()
    {
        LRig rig = TRigFake.TRigFakeBuild();
        LSettings fallback = TInterface.TSettingsCreate("ko", respelled: true);

        LSettings settings = TInterface.TWorkspaceSettingsRead(rig, fallback, out bool settled);

        Assert.False(settled);
        Assert.Same(fallback, settings);
    }

    [Fact]
    public void WorkspaceSettingsRead_SettingsStored_AnswersTheStoredOnesSettled()
    {
        LRig rig = TRigFake.TRigFakeBuild();
        rig.LRigSettings.TSettingsSave(TInterface.TSettingsCreate("fr"));

        LSettings settings = TInterface.TWorkspaceSettingsRead(
            rig, TInterface.TSettingsCreate("ko"), out bool settled);

        Assert.True(settled);
        Assert.Equal("fr", settings.LSettingsLocalization);
    }

    [Fact]
    public void WorkspaceNoticeRead_RefusalWrappedTwice_AnswersTheReason()
    {
        Exception wrapped = new InvalidOperationException(
            "outer", new AggregateException(TInterface.TRefusalCreate(LRefusal.LRefusalStale)));

        Assert.Equal(LRefusal.LRefusalStale, TInterface.TWorkspaceNoticeRead(wrapped));
        Assert.Null(TInterface.TWorkspaceNoticeRead(new InvalidOperationException("bare")));
    }

    private sealed class TDoctorFake : LDoctorVault
    {
        public LDoctorRescue LDoctorDatabaseCreate() =>
            throw new InvalidOperationException("The database folder is a file.");
    }
}
