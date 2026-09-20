using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TTrail
{
    private static readonly string TTrailRoot = Path.Combine(Path.GetTempPath(), "llyn-trail");

    [Fact]
    public void TrailRelativeResolve_PathOutsideRoot_ReturnsNull()
    {
        LTrail trail = TInterface.TTrailCreate();

        Assert.Null(trail.TTrailRelativeResolve(TTrailRoot, Path.Combine(Path.GetTempPath(), "elsewhere", "a.mp3")));
        Assert.Null(trail.TTrailRelativeResolve(TTrailRoot, Path.Combine(TTrailRoot, "..", "a.mp3")));
    }

    [Fact]
    public void TrailRelativeResolve_PathInsideRoot_ReturnsRelative()
    {
        LTrail trail = TInterface.TTrailCreate();

        Assert.Equal(
            Path.Combine("audio", "a.mp3"),
            trail.TTrailRelativeResolve(TTrailRoot, Path.Combine(TTrailRoot, "audio", "a.mp3")));
    }

    [Fact]
    public void TrailResolve_QualifiedPath_KeepsIt()
    {
        LTrail trail = TInterface.TTrailCreate();
        string qualified = Path.Combine(Path.GetTempPath(), "elsewhere", "a.mp3");

        Assert.Equal(Path.GetFullPath(qualified), trail.TTrailResolve(TTrailRoot, qualified));
    }

    [Fact]
    public void TrailResolve_RelativePath_AnchorsUnderRoot()
    {
        LTrail trail = TInterface.TTrailCreate();

        Assert.Equal(
            Path.GetFullPath(Path.Combine(TTrailRoot, "audio", "a.mp3")),
            trail.TTrailResolve(TTrailRoot, Path.Combine("audio", "a.mp3")));
        Assert.Null(trail.TTrailResolve(TTrailRoot, Path.Combine("..", "a.mp3")));
    }

    [Fact]
    public void TrailNameNormalize_BarredCharacters_ReplacesEach()
    {
        LTrail trail = TInterface.TTrailCreate();

        Assert.Equal("a_b_c", trail.TTrailNameNormalize("a<b>c"));
        Assert.Equal("ember", trail.TTrailNameNormalize("ember"));
    }

    [Fact]
    public void TrailNameRead_TrailingSeparator_ReturnsLastSegment()
    {
        LTrail trail = TInterface.TTrailCreate();

        Assert.Equal("llyn-trail", trail.TTrailNameRead(TTrailRoot + Path.DirectorySeparatorChar));
        Assert.True(trail.TTrailRootCheck(TTrailRoot));
        Assert.False(trail.TTrailRootCheck("audio"));
    }
}
