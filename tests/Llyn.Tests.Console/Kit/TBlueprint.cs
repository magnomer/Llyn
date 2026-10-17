using System.Text;
using Llyn.Conduct;
using Llyn.ShellEngine;
using Llyn.UIDemeanor;
using Llyn.UITerminal;
using Xunit;

namespace Llyn.Tests;

public sealed class TBlueprint
{
    [Fact]
    public void BlueprintLoad_Terminal_LoadsEveryBlueprint()
    {
        ABootstrap bootstrap = new();

        Assert.Equal(["AWindow", "ANotice", "ASParley", "ASNavigation"], bootstrap.BBlueprintParts.Keys);
        Assert.Equal(["double", "single", "ascii"], bootstrap.BBlueprintGlyphs.Keys);
    }

    [Fact]
    public void BlueprintLoad_UnlistedBlueprint_SkipsIt()
    {
        TBlueprintFake fake = new();

        Assert.Equal(["TBlueprintListed"], fake.BBlueprintParts.Keys);
    }

    [Fact]
    public void ContractFind_WindowContracts_FindsEveryPart()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();
        TConsoleFake console = new();
        BLocalizationCatalog catalog = new();
        CAtelier atelier = TInterfaceConduct.TAtelierCreate(engine, false);
        ABootstrap bootstrap = new();
        BEnvoy envoy = new(console, catalog, bootstrap);
        BTheme theme = new(BTheme.BThemeSpareRead);
        BLoop loop = new(static _ => { });

        Exception? fault = Record.Exception(
            () => new BWindow(console, loop, catalog, atelier, envoy, bootstrap, theme));

        Assert.Null(fault);
    }

    [Fact]
    public void BlueprintDraw_AsciiGlyphs_DrawsTheAsciiFrame()
    {
        const string xml = "<Blueprint x:Name=\"AWindow\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
            + "<Frame Glyphs=\"ascii\" /></Blueprint>";
        using MemoryStream markup = new(Encoding.UTF8.GetBytes(xml));
        BBlueprintPart blueprint = BBlueprint.BBlueprintRead(markup);

        BBlueprint.BBlueprintPlace(blueprint, 0, 0, 3, 3);
        IEnumerable<BCanvasText> texts = BBlueprint.BBlueprintDraw(blueprint, new ABootstrap().BBlueprintGlyphs);
        string[] drawn = [.. texts.Select(text => text.BCanvasTextValue)];

        Assert.Equal(["+-+", "| |", "+-+"], drawn);
    }

    [Fact]
    public void BlueprintRead_GlyphsMissingMark_Throws()
    {
        const string xml = "<Blueprint x:Name=\"AWindow\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
            + "<Glyphs x:Key=\"short\" Frame=\"+ - + | + +\" Rule=\"+ - +\" /></Blueprint>";
        using MemoryStream markup = new(Encoding.UTF8.GetBytes(xml));

        Assert.Throws<InvalidDataException>(() => BBlueprint.BBlueprintRead(markup));
    }

    [Fact]
    public void BlueprintRead_FrameWithoutGlyphs_ThrowsNamingAttribute()
    {
        const string xml = "<Blueprint x:Name=\"AWindow\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
            + "<Frame /></Blueprint>";
        using MemoryStream markup = new(Encoding.UTF8.GetBytes(xml));

        InvalidDataException fault = Assert.Throws<InvalidDataException>(() => BBlueprint.BBlueprintRead(markup));

        Assert.Contains("a Frame takes Glyphs", fault.Message);
    }

    [Fact]
    public void BlueprintRead_UnknownElement_Throws()
    {
        const string xml = "<Blueprint x:Name=\"AWindow\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
            + "<Reel /></Blueprint>";
        using MemoryStream markup = new(Encoding.UTF8.GetBytes(xml));

        Assert.Throws<InvalidDataException>(() => BBlueprint.BBlueprintRead(markup));
    }

    [Fact]
    public void BlueprintRead_UnknownAttribute_Throws()
    {
        const string xml = "<Blueprint x:Name=\"AWindow\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">"
            + "<Text Key=\"Window.Opening\" /></Blueprint>";
        using MemoryStream markup = new(Encoding.UTF8.GetBytes(xml));

        Assert.Throws<InvalidDataException>(() => BBlueprint.BBlueprintRead(markup));
    }
}
