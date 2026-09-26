using Llyn.Core;
using System.Text.RegularExpressions;
using Xunit;

namespace Llyn.Tests;

public sealed class TThemePalette
{
    private static readonly Regex TThemePaletteHex = new("^#[0-9A-Fa-f]{6}([0-9A-Fa-f]{2})?$", RegexOptions.Compiled);

    [Fact]
    public void ThemeColorRead_EmbeddedPalette_ReturnsNamedColors()
    {
        IReadOnlyDictionary<string, string> colors = TInterface.TThemeColorRead();

        Assert.True(colors.ContainsKey("ink"));
        Assert.True(colors.ContainsKey("accent"));
        Assert.True(colors.ContainsKey("pending"));
        Assert.True(colors.Count >= 36);
    }

    [Fact]
    public void ThemeColorRead_MissingName_Throws()
    {
        Assert.Equal(TInterface.TThemeColorRead()["ink"], TInterface.TThemeColorRead("ink"));
        Assert.Throws<KeyNotFoundException>(() => TInterface.TThemeColorRead("nowhere"));
    }

    [Fact]
    public void ThemeRead_MissingName_YieldsTheSpare()
    {
        LTheme theme = TInterface.TThemeLoad();

        Assert.Equal(TInterface.TThemeColorRead("ink"), theme.TThemeRead("ink"));
        Assert.Equal("#000000", theme.TThemeRead("nowhere"));
    }

    [Fact]
    public void ThemeColorRead_EmbeddedPalette_ReturnsHexValues()
    {
        IReadOnlyDictionary<string, string> colors = TInterface.TThemeColorRead();

        Assert.All(colors.Values, value => Assert.Matches(TThemePaletteHex, value));
    }
}
