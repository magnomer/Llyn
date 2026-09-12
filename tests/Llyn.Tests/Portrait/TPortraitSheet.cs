using Llyn.Core;
using Llyn.Infrastructure;
using Xunit;

namespace Llyn.Tests;

public sealed class TPortraitSheet
{
    [Fact]
    public void SheetFormat_FullPortrait_CarriesEveryFieldTheDisplayShows()
    {
        string page = TInterface.TSheetFormat(
            TPortraitSample.TPortraitSampleCreate(), TInterface.TThemeLoad());

        Assert.Contains("<h1 class=\"headword\">kindle</h1>", page);
        Assert.Contains("/ˈkɪnd(ə)l/", page);
        Assert.Contains("<span>verb</span>", page);
        Assert.Contains("<h2>Meanings</h2>", page);
        Assert.Contains("<h2>Collocations</h2>", page);
        Assert.Contains("set alight", page);
        Assert.Contains("kindle interest", page);
        Assert.Contains("(+with)", page);
        Assert.Contains("불붙이다", page);
        Assert.Contains("<h2>Links here</h2>", page);
        Assert.Contains("<p>Chiefly <em>literary</em>.</p>", page);
        Assert.Contains("<ul>\n<li>poetic</li>\n<li>archaic</li>\n</ul>", page);
        Assert.Contains("★", page);
    }

    [Fact]
    public void SheetFormat_ThemedPortrait_CarriesTheDisplayPalette()
    {
        LTheme theme = TInterface.TThemeLoad();
        string page = TInterface.TSheetFormat(TPortraitSample.TPortraitSampleCreate(), theme);

        Assert.Contains("--accent:" + theme.TThemeRead("accent"), page);
        Assert.Contains("--canvas:" + theme.TThemeRead("canvas"), page);
        Assert.Contains("print-color-adjust:exact", page);
    }

    [Fact]
    public void SheetFormat_MarkupInFieldText_LeavesNoUnescapedTag()
    {
        string page = TInterface.TSheetFormat(
            TPortraitSample.TPortraitSampleCreate(), TInterface.TThemeLoad());

        Assert.Contains("around a hearth &lt;cold&gt;", page);
        Assert.Contains("fire &amp; light", page);
    }

    [Fact]
    public void SheetFormat_YouTubeVideo_ShowsThePosterFrameAndTheLink()
    {
        string page = TInterface.TSheetFormat(
            TPortraitSample.TPortraitSampleCreate(), TInterface.TThemeLoad());

        Assert.Contains("https://img.youtube.com/vi/dQw4w9WgXcQ/hqdefault.jpg", page);
        Assert.Contains("0:12-0:30", page);
    }
}
