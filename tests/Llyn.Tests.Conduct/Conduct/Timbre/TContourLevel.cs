using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TContourLevel
{
    [Fact]
    public void ContourRead_LevelsOffScale_DropsThemAndTheTone()
    {
        IReadOnlyList<CContour> syllables = TInterfaceConductSound.TContourRead(
            [
                TInterface.TContourCreate("a", [0, 6, -1]),
                TInterface.TContourCreate("b", [7, 3, 3, 9]),
                TInterface.TContourCreate("c", []),
            ],
            [5, 4, 3, 2, 1]);

        Assert.Equal(3, syllables.Count);
        Assert.Empty(syllables[0].CContourLevels);
        Assert.False(syllables[0].CContourToned);
        Assert.Equal([3, 3], syllables[1].CContourLevels);
        Assert.True(syllables[1].CContourToned);
        Assert.Empty(syllables[2].CContourLevels);
        Assert.False(syllables[2].CContourToned);
    }

    [Fact]
    public void ContourRead_EmptyScale_LeavesEachUntoned()
    {
        IReadOnlyList<CContour> syllables = TInterfaceConductSound.TContourRead(
            [TInterface.TContourCreate("ma", [5, 5]), TInterface.TContourCreate("ba", [1])], []);

        Assert.Equal(["ma", "ba"], syllables.Select(static syllable => syllable.CContourText));
        Assert.All(syllables, static syllable => Assert.Empty(syllable.CContourLevels));
        Assert.All(syllables, static syllable => Assert.False(syllable.CContourToned));
    }

    [Fact]
    public void ContourRead_NoSyllables_AnswersNone()
    {
        Assert.Empty(TInterfaceConductSound.TContourRead([], [5, 4, 3, 2, 1]));
    }
}
