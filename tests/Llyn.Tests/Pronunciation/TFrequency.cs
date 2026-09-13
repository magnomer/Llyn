using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TFrequency
{
    [Fact]
    public void FrequencyParse_FormattedValue_RoundTripsOnFirstBar()
    {
        LFrequency written = TInterface.TFrequencyCreate("Datamuse", "12.5|per million", "Common");

        LFrequency parsed = TInterface.TFrequencyParse(written.TFrequencyFormat());

        Assert.Equal("Datamuse|12.5|per million", written.TFrequencyFormat());
        Assert.Equal(("Datamuse", "12.5|per million"), (parsed.LFrequencySource, parsed.LFrequencyRaw));
        Assert.Null(parsed.LFrequencyBand);
    }

    [Fact]
    public void FrequencyParse_NoBar_ReadsWholeTextAsRaw()
    {
        LFrequency parsed = TInterface.TFrequencyParse("S1");

        Assert.Equal(string.Empty, parsed.LFrequencySource);
        Assert.Equal("S1", parsed.LFrequencyRaw);
        Assert.Null(parsed.LFrequencyBand);
    }
}
