using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TScoreboard
{
    [Fact]
    public void ScoreboardEmptyKey_Placeholder_ReturnsScoreboardEmpty()
    {
        CScoreboard scoreboard = new();

        Assert.Equal("Scoreboard.Empty", scoreboard.CScoreboardEmptyKey);
    }
}
