using Llyn.Conduct;
using Xunit;

namespace Llyn.Tests;

public sealed class TUsage
{
    [Theory]
    [InlineData(false, false, "Display.MeaningSingle")]
    [InlineData(true, false, "Display.CollocationSingle")]
    [InlineData(false, true, "Vita.Example")]
    public void UsageOwnerKey_Kind_NamesTheCitingSide(bool collocated, bool quoted, string key)
    {
        Assert.Equal(key, TUsageCreate(quoted, collocated, CStateValue.CStateValueEmpty).CUsageOwnerKey);
    }

    [Fact]
    public void UsageTitleKey_UncertainOrKnownTitle_MarksOnlyTheUnknown()
    {
        Assert.Equal(
            "Display.Unknown",
            TUsageCreate(false, false, new CStateValue(string.Empty, true)).CUsageTitleKey);
        Assert.Null(TUsageCreate(false, false, new CStateValue("sense", false)).CUsageTitleKey);
    }

    private static CUsage TUsageCreate(bool quoted, bool collocated, CStateValue title)
    {
        return new CUsage(3, 9, "water", string.Empty, "English", title, quoted, collocated);
    }
}
