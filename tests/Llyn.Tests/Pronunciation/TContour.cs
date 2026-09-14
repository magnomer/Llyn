using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TContour
{
    [Fact]
    public void ContourParse_SuperscriptDigits_ReadsOneSyllablePerRun()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("/nei̯¹³ hou̯³⁵/");

        Assert.Equal(["nei̯¹³", "hou̯³⁵"], syllables.Select(syllable => syllable.LContourText));
        Assert.Equal([1, 3], syllables[0].LContourLevels);
        Assert.Equal([3, 5], syllables[1].LContourLevels);
    }

    [Fact]
    public void ContourParse_ChaoLetters_ReadsLevels()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("[sa˨˩.wat̚˨˩.diː˧]");

        Assert.Equal(3, syllables.Count);
        Assert.Equal([2, 1], syllables[0].LContourLevels);
        Assert.Equal([2, 1], syllables[1].LContourLevels);
        Assert.Equal([3], syllables[2].LContourLevels);
    }

    [Fact]
    public void ContourParse_PlainDigits_ReadsLevels()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("ma43 ma3");

        Assert.Equal([4, 3], syllables[0].LContourLevels);
        Assert.Equal([3], syllables[1].LContourLevels);
    }

    [Fact]
    public void ContourParse_SandhiJoiner_KeepsSurfaceTone()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("/ni²¹⁴⁻²¹ xɑʊ̯²¹⁴/");

        Assert.Equal("ni²¹⁴⁻²¹", syllables[0].LContourText);
        Assert.Equal([2, 1], syllables[0].LContourLevels);
        Assert.Equal([2, 1, 4], syllables[1].LContourLevels);
    }

    [Fact]
    public void ContourParse_TonelessSyllable_ReadsNoLevel()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("ma⁵⁵ də");

        Assert.Equal(2, syllables.Count);
        Assert.Equal("də", syllables[1].LContourText);
        Assert.Empty(syllables[1].LContourLevels);
    }

    [Fact]
    public void ContourParse_MarkOutsideLevels_ReadsNoLevel()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("ma⁶");

        Assert.Empty(Assert.Single(syllables).LContourLevels);
    }

    [Fact]
    public void ContourParse_BlankReading_ReadsNothing()
    {
        Assert.Empty(TInterface.TContourParse("   "));
    }

    [Fact]
    public void ContourToneCheck_UntonedReading_ReportsFalse()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("/həˈləʊ/");

        Assert.False(TInterface.TContourToneCheck(syllables));
    }

    [Fact]
    public void ContourToneCheck_OneTonedSyllable_ReportsTrue()
    {
        IReadOnlyList<LContour> syllables = TInterface.TContourParse("ma⁵⁵ də");

        Assert.True(TInterface.TContourToneCheck(syllables));
    }

    [Fact]
    public void EngineTonalCheck_TonalPack_ReportsTrue()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.True(engine.TEngineTonalCheck("Mandarin"));
        Assert.False(engine.TEngineTonalCheck("English"));
    }
}
