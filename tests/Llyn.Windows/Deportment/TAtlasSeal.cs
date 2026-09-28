using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TAtlasSeal
{
    [Fact]
    public void AtlasSituationRead_NoSituation_ReturnsNone()
    {
        Assert.Null(TInterfaceGate.TAtlasSituationRead(null));
    }

    [Fact]
    public void AtlasSituationRead_StoredSituation_CarriesEveryField()
    {
        LSituation situation = TInterface.TSituationCreate(7, "Hearth", "By the fire", LStateValue.LStateValueUnknown);

        CSituationDraft? held = TInterfaceGate.TAtlasSituationRead(situation);

        Assert.NotNull(held);
        Assert.Equal(7, held!.CSituationDraftId);
        Assert.Equal("Hearth", held.CSituationDraftTitle.CStateValueText);
        Assert.Equal("By the fire", held.CSituationDraftDescription.CStateValueText);
        Assert.True(held.CSituationDraftKind.CStateValueUncertain);
        Assert.Empty(held.CSituationDraftImage);
        Assert.Empty(held.CSituationDraftVideo);
    }
}
