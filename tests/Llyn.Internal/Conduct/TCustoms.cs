using System.Linq;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TCustoms
{
    [Fact]
    public void CustomsRowRead_OneCandidate_MergesIntoIt()
    {
        CSCustomsRow row = TCustomsPrepare([7]).TCustomsRowRead(0);

        Assert.Equal(new CSCustomsRow(CSCustomsMode.CSCustomsModeMerge, 7, true, null, 0, 0), row);
    }

    [Fact]
    public void CustomsRowRead_SeveralCandidates_StartsFreshWithoutTarget()
    {
        CSCustoms customs = TCustomsPrepare([7, 8], []);

        Assert.Equal(
            new CSCustomsRow(CSCustomsMode.CSCustomsModeFresh, 0, false, null, 0, 0), customs.TCustomsRowRead(0));
        Assert.Equal(
            new CSCustomsRow(CSCustomsMode.CSCustomsModeFresh, 0, false, null, 0, 0), customs.TCustomsRowRead(1));
    }

    [Fact]
    public void CustomsRowRead_FreshAfterPick_KeepsThePick()
    {
        CSCustoms customs = TCustomsPrepare([7, 8]);
        customs.TCustomsTargetSet(0, 8);
        customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeFresh);

        Assert.Equal(
            new CSCustomsRow(CSCustomsMode.CSCustomsModeFresh, 8, false, null, 0, 0), customs.TCustomsRowRead(0));
    }

    [Fact]
    public void CustomsModeSet_MergeWithoutTarget_AnswersNotReady()
    {
        CSCustoms customs = TCustomsPrepare([7, 8], []);
        Assert.True(customs.TCustomsReadyCheck());

        Assert.False(customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeMerge));
        Assert.True(customs.TCustomsTargetSet(0, 8));
    }

    [Fact]
    public void CustomsRowRead_ReplaceWithTarget_WordsTheLostCards()
    {
        CSCustoms customs = TCustomsPrepare([7], [7, 8]);
        Assert.Null(customs.TCustomsRowRead(0).CSCustomsRowLoss);

        customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeReplace);
        customs.TCustomsModeSet(1, CSCustomsMode.CSCustomsModeReplace);

        Assert.Equal(
            new CSCustomsRow(CSCustomsMode.CSCustomsModeReplace, 7, true, "Customs.Loss", 17, 27),
            customs.TCustomsRowRead(0));
        Assert.Null(customs.TCustomsRowRead(1).CSCustomsRowLoss);
    }

    [Fact]
    public void CustomsRowsRead_PickedRows_ReadsEveryRowInFileOrder()
    {
        CSCustoms customs = TCustomsPrepare([7, 8], [9]);
        customs.TCustomsModeSet(0, CSCustomsMode.CSCustomsModeReplace);
        customs.TCustomsTargetSet(0, 8);

        Assert.Equal(
            [
                new CSCustomsRow(CSCustomsMode.CSCustomsModeReplace, 8, true, "Customs.Loss", 18, 28),
                new CSCustomsRow(CSCustomsMode.CSCustomsModeMerge, 9, true, null, 0, 0),
            ],
            customs.TCustomsRowsRead());
    }

    private static CSCustoms TCustomsPrepare(params long[][] rows) =>
        TInterfaceConduct.TCustomsCreate(
            rows.Select(static ids => new CMarkupEntry("English", "ember", ids)).ToList(),
            rows.SelectMany(static ids => ids)
                .Distinct()
                .Select(static id => new CMarkupTarget(id, (int)id + 10, (int)id + 20))
                .ToList());
}
