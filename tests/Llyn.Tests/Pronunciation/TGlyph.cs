using System.Linq;
using Llyn.Core;
using Llyn.ShellEngine;
using Xunit;

namespace Llyn.Tests;

public sealed class TGlyph
{
    [Fact]
    public void GlyphScan_MixedScript_KeepsHanOnlyDistinctInOrder()
    {
        Assert.Equal(["食", "物"], TInterface.TGlyphScan("食べ物 tabemono"));
        Assert.Equal(["人"], TInterface.TGlyphScan("人人"));
        Assert.Equal(["國", "家"], TInterface.TGlyphScan(" 國家。"));
        Assert.Empty(TInterface.TGlyphScan("hello"));
        Assert.Empty(TInterface.TGlyphScan(string.Empty));
    }

    [Fact]
    public void GlyphScan_SupplementaryIdeograph_KeepsWholeRune()
    {
        string rare = char.ConvertFromUtf32(0x20000);

        Assert.Equal([rare, "水"], TInterface.TGlyphScan(rare + "水"));
    }

    [Fact]
    public void GlyphResolve_MissingEntry_CreatesOnceThenFinds()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry made = engine.TEngineGlyphResolve("國", "Classical Chinese");
        LEntry found = engine.TEngineGlyphResolve("國", "Classical Chinese");

        Assert.True(made.LEntryId > 0);
        Assert.Equal(made.LEntryId, found.LEntryId);
        Assert.Equal("國", found.LEntryHeadword);
        Assert.Equal("Classical Chinese", found.LEntryLanguage);
        Assert.Single(engine.TEngineEntryFind("國"));
    }

    [Fact]
    public void GlyphResolve_SameHeadwordOtherLanguage_MakesOwnEntry()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        LEntry mandarin = engine.TEngineTranslationCreate("國", "Mandarin");
        LEntry classical = engine.TEngineGlyphResolve("國", "Classical Chinese");

        Assert.NotEqual(mandarin.LEntryId, classical.LEntryId);
        Assert.Equal(
            ["Classical Chinese", "Mandarin"],
            engine.TEngineEntryFind("國").Select(entry => entry.LEntryLanguage).OrderBy(name => name));
    }

    [Fact]
    public void GlyphRead_PackWithSection_ReadsItAndPackWithoutReadsNull()
    {
        using TWorkspace workspace = TWorkspace.TWorkspacePrepare();
        using LEngine engine = workspace.TWorkspaceEngineStart();

        Assert.Equal("Traditional", engine.TEngineGlyphRead("Cantonese")?.LGlyphName);
        Assert.Null(engine.TEngineGlyphRead("English"));
    }
}
