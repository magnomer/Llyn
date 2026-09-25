using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TEngineVault
{
    [Fact]
    public void EntryRead_FakeVault_AnswersWithoutDatabase()
    {
        TVaultFake fake = new();
        LEntry seeded = fake.TVaultFakeAdd(TInterface.TEntryCreate(0, "kindle", "en", 0, null, null));
        using LEngine engine = new(TRigFake.TRigFakeBuild(fake));

        LEntry? read = engine.TEngineEntryRead(seeded.LEntryId);

        Assert.Equal("kindle", read?.LEntryHeadword);
        Assert.Equal(1, fake.TVaultFakeReads);
    }

    [Fact]
    public void EntryRead_FakeVaultUnknownId_ReturnsNull()
    {
        TVaultFake fake = new();
        using LEngine engine = new(TRigFake.TRigFakeBuild(fake));

        Assert.Null(engine.TEngineEntryRead(42));
        Assert.Equal(1, fake.TVaultFakeReads);
    }
}
