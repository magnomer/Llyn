using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TReading
{
    [Fact]
    public void ReadingScan_UndeclaredTagAndUntagged_KeepsTagFirst()
    {
        IReadOnlyList<LReading> expanded = TInterface.TReadingScan(
            [
                TInterface.TReadingCreate(string.Empty, "a"),
                TInterface.TReadingCreate("Scottish", "b"),
            ],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);

        Assert.Equal(
            [("Scottish", "b"), ("British", "a"), ("American", "a")],
            expanded.Select(reading => (reading.LReadingVariety, reading.LReadingPhonetic)));
    }

    [Fact]
    public void ReadingScan_NoVarieties_ReturnsReadingsUnchanged()
    {
        IReadOnlyList<LReading> readings = [TInterface.TReadingCreate(string.Empty, "a")];

        IReadOnlyList<LReading> expanded = TInterface.TReadingScan(readings, []);

        Assert.Same(readings, expanded);
    }

    [Fact]
    public void ReadingScan_TaggedCoversVariety_SkipsCoveredFanOut()
    {
        IReadOnlyList<LReading> expanded = TInterface.TReadingScan(
            [
                TInterface.TReadingCreate("British", "a"),
                TInterface.TReadingCreate(string.Empty, "b"),
            ],
            [TInterface.TVarietyCreate("British"), TInterface.TVarietyCreate("American")]);

        Assert.Equal(
            [("British", "a"), ("American", "b")],
            expanded.Select(reading => (reading.LReadingVariety, reading.LReadingPhonetic)));
    }
}
