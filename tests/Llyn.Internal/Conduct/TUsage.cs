using System.Collections.Generic;
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
            TUsageCreate(false, false, new CStateValue(string.Empty, true, false)).CUsageTitleKey);
        Assert.Null(TUsageCreate(false, false, new CStateValue("sense", false, true)).CUsageTitleKey);
    }

    [Fact]
    public void UsageOpen_QuotedOrNamedPlace_OpensTheExampleOrTheEntry()
    {
        List<long> examples = [];
        List<long> entries = [];

        TUsageCreate(true, false, CStateValue.CStateValueEmpty).CUsageOpen(
            id => { examples.Add(id); return true; }, id => { entries.Add(id); return true; });
        TUsageCreate(false, false, CStateValue.CStateValueEmpty).CUsageOpen(
            id => { examples.Add(id); return true; }, id => { entries.Add(id); return true; });

        Assert.Equal([3], examples);
        Assert.Equal([9], entries);
    }

    private static CUsage TUsageCreate(bool quoted, bool collocated, CStateValue title)
    {
        return new CUsage(3, 9, "water", string.Empty, "English", title, quoted, collocated);
    }
}
