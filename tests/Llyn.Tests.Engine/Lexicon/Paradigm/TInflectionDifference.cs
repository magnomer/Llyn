using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TInflectionDifference
{
    [Theory]
    [InlineData("bose", "bosque", 3, 2)]
    [InlineData("cantamos", "cantábamos", 5, 2)]
    public void InflectionDifferenceScan_FoldedOrInserted_MarksRange(
        string predicted, string actual, int start, int length)
    {
        IReadOnlyList<LInflectionRule> folds = TInflectionBook.TInflectionBookCreate().LInflectionBookFolds;

        IReadOnlyList<LInflectionMark> marks = TInterfaceInflection.TInflectionDifferenceScan(
            folds, predicted, actual);

        Assert.Equal([TInterfaceInflection.TInflectionMarkCreate(start, length)], marks);
    }

    [Fact]
    public void InflectionDifferenceScan_AccentOnly_MarksNothing()
    {
        IReadOnlyList<LInflectionRule> folds = TInflectionBook.TInflectionBookCreate().LInflectionBookFolds;

        Assert.Empty(TInterfaceInflection.TInflectionDifferenceScan(folds, "hablo", "Habló"));
        Assert.Empty(TInterfaceInflection.TInflectionDifferenceScan(folds, "averigue", "averigüé"));
    }

    [Theory]
    [InlineData("teno", "tengo", 4)]
    [InlineData("tenes", "tienes", 4)]
    [InlineData("tenemos", "tenemos", 3)]
    public void InflectionDifferenceDivide_Tener_AnswersEndingStart(string predicted, string actual, int split)
    {
        IReadOnlyList<LInflectionRule> folds = TInflectionBook.TInflectionBookCreate().LInflectionBookFolds;

        Assert.Equal(split, TInterfaceInflection.TInflectionDifferenceDivide(folds, predicted, 3, actual));
    }

    [Fact]
    public void InflectionMarkFormat_TwoMarks_JoinsPairs()
    {
        Assert.Equal(
            "1:2,7:1",
            TInterfaceInflection.TInflectionMarkFormat(
                [TInterfaceInflection.TInflectionMarkCreate(1, 2), TInterfaceInflection.TInflectionMarkCreate(7, 1)]));
        Assert.Equal("", TInterfaceInflection.TInflectionMarkFormat([]));
    }

    [Fact]
    public void InflectionMarkParse_Formatted_RoundTrips()
    {
        IReadOnlyList<LInflectionMark> marks =
            [TInterfaceInflection.TInflectionMarkCreate(0, 3), TInterfaceInflection.TInflectionMarkCreate(12, 1)];

        Assert.Equal(
            marks, TInterfaceInflection.TInflectionMarkParse(TInterfaceInflection.TInflectionMarkFormat(marks)));
        Assert.Empty(TInterfaceInflection.TInflectionMarkParse(""));
    }

    [Fact]
    public void InflectionMarkParse_Malformed_SkipsPairs()
    {
        IReadOnlyList<LInflectionMark> marks = TInterfaceInflection.TInflectionMarkParse(
            "1:2,x:3,-1:2,4:-1,5,6:7:8,,9:1");

        Assert.Equal(
            [TInterfaceInflection.TInflectionMarkCreate(1, 2), TInterfaceInflection.TInflectionMarkCreate(9, 1)],
            marks);
    }
}
