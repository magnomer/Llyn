using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TCustomsLoss
{
    [Fact]
    public void CustomsCardScan_NestedChildren_CountsEveryCard()
    {
        LCardDraft leaf = TInterface.TDraftCardCreate("leaf");
        LCardDraft parent = TInterface.TDraftCardCreate("parent") with { LCardDraftChild = [leaf, leaf] };

        Assert.Equal(4, TInterfaceDeportment.TCustomsCardScan([parent, leaf]));
    }
}
