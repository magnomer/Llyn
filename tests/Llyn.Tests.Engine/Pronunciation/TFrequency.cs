using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TFrequency
{
    [Theory]
    [InlineData(17631875, 40)]
    [InlineData(523605, 1300)]
    [InlineData(54127, 13000)]
    [InlineData(2245, 310000)]
    public void OnceResolve_TotalOverCount_RoundsToTwoFigures(double raw, long once)
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Corpus", [TFrequencyAttemptCreate()], total: 698100000);

        Assert.Equal(once, TInterface.TFrequencyOnceResolve(spec, raw));
    }

    [Fact]
    public void OnceResolve_PerMillionFigure_DividesTheMillion()
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Datamuse", [TFrequencyAttemptCreate()], total: 1000000);

        Assert.Equal(400000, TInterface.TFrequencyOnceResolve(spec, 2.5));
        Assert.Equal(770, TInterface.TFrequencyOnceResolve(spec, 1300));
        Assert.Equal(1, TInterface.TFrequencyOnceResolve(spec, 3000000));
    }

    [Fact]
    public void OnceResolve_ClassWithBase_RaisesThenScales()
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Leipzig", [TFrequencyAttemptCreate()], factor: 22, power: 2);

        Assert.Equal(22, TInterface.TFrequencyOnceResolve(spec, 0));
        Assert.Equal(180, TInterface.TFrequencyOnceResolve(spec, 3));
        Assert.Equal(23000, TInterface.TFrequencyOnceResolve(spec, 10));
    }

    [Fact]
    public void OnceResolve_RankWithFactor_Multiplies()
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Logeion", [TFrequencyAttemptCreate()], factor: 12);

        Assert.Equal(12, TInterface.TFrequencyOnceResolve(spec, 1));
        Assert.Equal(5900, TInterface.TFrequencyOnceResolve(spec, 493));
    }

    [Theory]
    [InlineData(100, "Core")]
    [InlineData(10, "Everyday")]
    [InlineData(9.99, "Advanced")]
    [InlineData(1, "Advanced")]
    [InlineData(0.5, "Rare")]
    public void BandResolve_PerMillionFigure_GradesByDecade(double raw, string band)
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Datamuse", [TFrequencyAttemptCreate()], total: 1000000);

        Assert.Equal(band, TInterface.TFrequencyBandResolve(spec, raw));
    }

    [Theory]
    [InlineData(833, "Core")]
    [InlineData(834, "Everyday")]
    [InlineData(8333, "Everyday")]
    [InlineData(8334, "Advanced")]
    [InlineData(83334, "Rare")]
    public void BandResolve_RankWithFactor_GradesByInterval(double raw, string band)
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Logeion", [TFrequencyAttemptCreate()], factor: 12);

        Assert.Equal(band, TInterface.TFrequencyBandResolve(spec, raw));
    }

    [Fact]
    public void BandResolve_NoFigures_YieldsNull()
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Longman", [TFrequencyAttemptCreate()]);

        Assert.Null(TInterface.TFrequencyBandResolve(spec, 5));
    }

    [Fact]
    public void OnceResolve_NoFigures_YieldsNull()
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Longman", [TFrequencyAttemptCreate()]);

        Assert.Null(TInterface.TFrequencyOnceResolve(spec, 5));
    }

    [Fact]
    public void OnceResolve_ZeroCount_YieldsNull()
    {
        LSourceSpec spec = TInterface.TSourceSpecCreate("Corpus", [TFrequencyAttemptCreate()], total: 1000);

        Assert.Null(TInterface.TFrequencyOnceResolve(spec, 0));
    }

    private static LSourceAttempt TFrequencyAttemptCreate() =>
        TInterface.TSourceAttemptCreate(
            ["https://example.test/{word}"],
            [TInterface.TSourceReadingCreate("1", "regex", "(\\d+)", 1, null, false, 0)],
            null,
            null,
            null);
}
