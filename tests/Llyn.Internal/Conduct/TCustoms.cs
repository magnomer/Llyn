using Llyn.Conduct;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TCustoms
{
    [Fact]
    public void CustomsCardScan_NestedChildren_CountsEveryCard()
    {
        LCardDraft leaf = TInterface.TDraftCardCreate("leaf");
        LCardDraft parent = TInterface.TDraftCardCreate("parent") with { LCardDraftChild = [leaf, leaf] };

        Assert.Equal(4, TInterfaceConduct.TCustomsCardScan([parent, leaf]));
    }

    [Fact]
    public void CustomsRowRead_OneCandidate_MergesIntoIt()
    {
        CSCustomsRow row = TInterfaceConduct.TCustomsCreate([[7]]).TCustomsRowRead(0);

        Assert.Equal(new CSCustomsRow(CSCustomsMode.CSCustomsModeMerge, 7, true, 0), row);
    }

    [Fact]
    public void CustomsRowRead_SeveralCandidates_StartsFreshWithoutTarget()
    {
        CSCustoms customs = TInterfaceConduct.TCustomsCreate([[7, 8], []]);

        Assert.Equal(new CSCustomsRow(CSCustomsMode.CSCustomsModeFresh, 0, false, 0), customs.TCustomsRowRead(0));
        Assert.Equal(new CSCustomsRow(CSCustomsMode.CSCustomsModeFresh, 0, false, 0), customs.TCustomsRowRead(1));
    }

    [Fact]
    public void CustomsRowRead_FreshAfterPick_KeepsThePick()
    {
        CSCustoms customs = TInterfaceConduct.TCustomsCreate([[7, 8]]);
        customs.TCustomsTargetSet(0, 8);
        customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeFresh);

        Assert.Equal(new CSCustomsRow(CSCustomsMode.CSCustomsModeFresh, 8, false, 0), customs.TCustomsRowRead(0));
    }

    [Fact]
    public void CustomsModeSet_MergeWithoutTarget_AnswersNotReady()
    {
        CSCustoms customs = TInterfaceConduct.TCustomsCreate([[7, 8], []]);
        Assert.True(customs.TCustomsReadyCheck());

        Assert.False(customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeMerge));
        Assert.True(customs.TCustomsTargetSet(0, 8));
    }

    [Fact]
    public void CustomsRowRead_ReplaceWithTarget_NamesTheLostEntry()
    {
        CSCustoms customs = TInterfaceConduct.TCustomsCreate([[7], [7, 8]]);
        Assert.Equal(0, customs.TCustomsRowRead(0).CSCustomsRowLoss);

        customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeReplace);
        customs.TCustomsModeSet(1, CSCustomsMode.CSCustomsModeReplace);

        Assert.Equal(7, customs.TCustomsRowRead(0).CSCustomsRowLoss);
        Assert.Equal(0, customs.TCustomsRowRead(1).CSCustomsRowLoss);
    }
}
