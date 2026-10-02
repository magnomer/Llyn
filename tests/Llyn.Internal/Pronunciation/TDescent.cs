using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TDescent
{
    private const string TDescentLanguage = "Classical Chinese";

    private static IReadOnlyList<LDescent> TDescentRead() =>
        TInterface.TLanguageLoad(TDescentLanguage).LLanguageDescents;

    [Fact]
    public void LanguageLoaderLoad_ClassicalChinesePack_ReadsToneRows()
    {
        IReadOnlyList<LDescent> rows = TDescentRead();

        Assert.NotEmpty(rows);
        Assert.Contains(rows, row => row.TDescentMatch("Mandarin"));
        Assert.Contains(rows, row => row.TDescentMatch("Cantonese"));
        Assert.Contains(rows, row => row.TDescentMatch("Gan"));
        Assert.Contains(rows, row => row.TDescentMatch("Jin"));
        Assert.Contains(rows, row => row.TDescentMatch("Xiang"));
        Assert.Empty(TInterface.TLanguageLoad("Korean").LLanguageDescents);
    }

    [Fact]
    public void DescentScan_MandarinLevelTone_NamesPlainAndChecked()
    {
        Assert.Equal(["1", "7"], TInterface.TDescentScan(TDescentRead(), "Mandarin", "55"));
    }

    [Fact]
    public void DescentScan_CantoneseVariants_ReadOneClass()
    {
        IReadOnlyList<LDescent> rows = TDescentRead();

        Assert.Equal(["3"], TInterface.TDescentScan(rows, "Cantonese", "35"));
        Assert.Equal(["3"], TInterface.TDescentScan(rows, "Cantonese", "34"));
        Assert.Equal(["4S", "4"], TInterface.TDescentScan(rows, "Cantonese", "13"));
    }

    [Theory]
    [InlineData("Gan", "42", "1")]
    [InlineData("Gan", "35", "2,5")]
    [InlineData("Gan", "45", "2,5")]
    [InlineData("Gan", "55", "2,5")]
    [InlineData("Gan", "24", "2")]
    [InlineData("Gan", "213", "3,4S,5")]
    [InlineData("Gan", "2", "4,6,8S,8")]
    [InlineData("Gan", "21", "4,6,8S,8")]
    [InlineData("Gan", "5", "7")]
    [InlineData("Jin", "11", "1,2")]
    [InlineData("Jin", "53", "3,4S")]
    [InlineData("Jin", "45", "4,5,6")]
    [InlineData("Jin", "2", "7,8S")]
    [InlineData("Jin", "54", "8")]
    [InlineData("Xiang", "33", "1")]
    [InlineData("Xiang", "13", "2")]
    [InlineData("Xiang", "41", "3,4S")]
    [InlineData("Xiang", "21", "4,6")]
    [InlineData("Xiang", "45", "5")]
    [InlineData("Xiang", "55", "5")]
    [InlineData("Xiang", "24", "7,8S,8")]
    public void DescentScan_GanJinXiangContours_ReadClasses(
        string language,
        string contour,
        string expected)
    {
        Assert.Equal(
            expected.Split(','),
            TInterface.TDescentScan(TDescentRead(), language, contour));
    }

    [Fact]
    public void DescentScan_SandhiForm_ReadsCitationTone()
    {
        Assert.Equal(["3", "4S"], TInterface.TDescentScan(TDescentRead(), "Mandarin", "214-21"));
    }

    [Fact]
    public void DescentScan_UnlistedContour_ReadsNothing()
    {
        IReadOnlyList<LDescent> rows = TDescentRead();

        Assert.Empty(TInterface.TDescentScan(rows, "Mandarin", "44"));
        Assert.Empty(TInterface.TDescentScan(rows, "Gan", "34"));
        Assert.Empty(TInterface.TDescentScan(rows, "Mandarin", ""));
        Assert.Empty(TInterface.TDescentScan(rows, "Korean", "55"));
    }

    [Fact]
    public void ReflexAnchorScan_MandarinLevelTone_EstimatesItsClass()
    {
        LFanqieRow level = TInterface.TFanqieRowCreate("疑", "模", "模", "一", "平", false)
            with { LFanqieRowId = 5, LFanqieRowClass = "1" };
        LFanqieRow departing = TInterface.TFanqieRowCreate("匣", "寒", "寒", "一", "去", false)
            with { LFanqieRowId = 6, LFanqieRowClass = "5" };

        IReadOnlyList<LAnchorRow> rows = TInterface.TReflexAnchorScan(
            [level, departing], [6], TDescentRead(), "Mandarin", "55");

        Assert.Equal(2, rows.Count);
        Assert.True(rows[0].LAnchorRowEstimated);
        Assert.False(rows[0].LAnchorRowHeld);
        Assert.False(rows[1].LAnchorRowEstimated);
        Assert.True(rows[1].LAnchorRowHeld);
    }
}
