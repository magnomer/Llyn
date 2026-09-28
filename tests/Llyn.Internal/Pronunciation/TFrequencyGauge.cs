using System.Globalization;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TFrequencyGauge
{
    [Fact]
    public void FrequencyGaugeResolve_NoRows_ReturnsNone()
    {
        Assert.Null(TInterface.TFrequencyGaugeResolve([], "once in {0} words"));
    }

    [Fact]
    public void FrequencyGaugeResolve_UnbandedRows_BandsZero()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", null),
            TInterface.TFrequencyCreate("two", "3", null),
        ];

        Assert.Equal(0, TInterface.TFrequencyGaugeResolve(rows, "{0}")?.LFrequencyGaugeBand);
    }

    [Fact]
    public void FrequencyGaugeResolve_BandedRow_BandsItsRank()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", null),
            TInterface.TFrequencyCreate("two", "3", "Everyday"),
        ];

        Assert.Equal(3, TInterface.TFrequencyGaugeResolve(rows, "{0}")?.LFrequencyGaugeBand);
    }

    [Fact]
    public void FrequencyGaugeResolve_UnknownBandFirst_BandsTheNextRank()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyCreate("one", "12", "Nowhere"),
            TInterface.TFrequencyCreate("two", "3", "Core"),
        ];

        Assert.Equal(4, TInterface.TFrequencyGaugeResolve(rows, "{0}")?.LFrequencyGaugeBand);
    }

    [Fact]
    public void FrequencyGaugeResolve_OnceInterval_FormatsTheInterval()
    {
        LFrequency[] rows =
        [
            TInterface.TFrequencyIntervalCreate("Corpus", "0.5", 12000),
            TInterface.TFrequencyCreate("List", "7", null),
        ];

        string? text = TInterface.TFrequencyGaugeResolve(rows, "once in {0} words")?.LFrequencyGaugeSource;

        Assert.Equal(
            "Corpus: once in " + 12000.ToString("N0", CultureInfo.CurrentCulture) + " words\nList: 7",
            text);
    }
}
