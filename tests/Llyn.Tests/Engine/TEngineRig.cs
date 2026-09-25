using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineRig
{
    [Fact]
    public void Engine_FakeRig_StartsWithoutDisk()
    {
        using LEngine engine = new(TRigFake.TRigFakeBuild(new TVaultFake(), "fake-start"));

        Assert.Equal("fake-start", engine.TEngineWorkspaceRead());
        Assert.Equal("en", engine.TEngineSettingsRead().LSettingsLocalization);
        Assert.False(engine.TEngineRescueRead().LDoctorRescueDone);
    }

    [Fact]
    public void EntryCreate_FakeRig_LandsInFakeVault()
    {
        TVaultFake fake = new();
        using LEngine engine = new(TInterface.TRigClerkCreate(fake));
        engine.TEngineFrequencySave(false);

        LEntry created = engine.TEngineTranslationCreate("kindle", "en");

        Assert.Equal(1, created.LEntryId);
        Assert.Equal("kindle", engine.TEngineEntryRead(created.LEntryId)?.LEntryHeadword);
        Assert.Equal(1, fake.TVaultFakeReads);
    }

    [Fact]
    public void RigApply_SecondFakeRig_ReadsFromSecondFake()
    {
        TVaultFake first = new();
        TVaultFake second = new();
        LEntry seeded = second.TVaultFakeAdd(TInterface.TEntryCreate(0, "ember", "en", 0, null, null));
        using LEngine engine = new(TRigFake.TRigFakeBuild(first, "fake-first"));
        engine.TEngineRespellingSave(true);

        engine.TEngineRigApply(TRigFake.TRigFakeBuild(second, "fake-second"));

        Assert.Equal("fake-second", engine.TEngineWorkspaceRead());
        Assert.Equal("ember", engine.TEngineEntryRead(seeded.LEntryId)?.LEntryHeadword);
        Assert.Equal(0, first.TVaultFakeReads);
        Assert.True(engine.TEngineSettingsRead().LSettingsRespelled);
    }
}
