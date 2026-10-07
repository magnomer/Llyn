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
    public void GlyphOrderSort_SupplementaryIdeograph_SortsByCodePointNotUtf16()
    {
        string rare = char.ConvertFromUtf32(0x20000);
        string compatibility = char.ConvertFromUtf32(0xF900);

        Assert.Equal(
            ["工", "江", compatibility, rare], TInterface.TGlyphOrderSort([rare, compatibility, "江", "工"]));
        Assert.Equal(["工", "工江"], TInterface.TGlyphOrderSort(["工江", "工"]));
        Assert.Equal([compatibility, rare], TInterface.TGlyphOrderSort([rare, compatibility]));
        Assert.True(string.CompareOrdinal(compatibility, rare) > 0);
    }

    [Fact]
    public void GlyphDivide_GlyphScheme_TakesTranscription()
    {
        LGlyph glyph = TInterface.TGlyphCreate("Traditional", "Classical Chinese");
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "国家", "Mandarin", string.Empty, string.Empty, [], [],
            transcriptions:
            [
                TInterface.TTranscriptionDraftCreate("Pinyin", "guójiā"),
                TInterface.TTranscriptionDraftCreate("Traditional", "國家"),
            ]);

        Assert.Equal(["國", "家"], glyph.TGlyphDivide(draft).Select(cell => cell.LGlyphCellText));
    }

    [Fact]
    public void GlyphDivide_NativeHeadwordAndEmptyGlyphRow_AnswersNoCells()
    {
        LGlyph glyph = TInterface.TGlyphCreate("Hanja", "Classical Chinese");
        LEntryDraft draft = TInterface.TEntryDraftCreate(
            "물", "Korean", string.Empty, string.Empty, [], [],
            transcriptions:
            [
                TInterface.TTranscriptionDraftCreate("Revised Romanization", "mul"),
                TInterface.TTranscriptionDraftCreate("Hanja", string.Empty),
            ]);

        Assert.Empty(glyph.TGlyphDivide(draft));
    }

    [Fact]
    public void GlyphOtherRead_GlyphAndEmptyRows_KeepsOnlyOtherFilledRows()
    {
        LGlyph glyph = TInterface.TGlyphCreate("Traditional", "Classical Chinese");
        IReadOnlyList<LTranscriptionDraft> rows =
        [
            TInterface.TTranscriptionDraftCreate("Pinyin", "guójiā"),
            TInterface.TTranscriptionDraftCreate("Traditional", "國家"),
            TInterface.TTranscriptionDraftCreate("Zhuyin", string.Empty),
        ];

        Assert.Equal(["Pinyin"], TInterface.TGlyphOtherRead(glyph, rows).Select(row => row.LTranscriptionDraftScheme));
        Assert.Equal(
            ["Pinyin", "Traditional"],
            TInterface.TGlyphOtherRead(null, rows).Select(row => row.LTranscriptionDraftScheme));
    }

    [Fact]
    public void GlyphDivide_SingleCharacter_CarriesLanguage()
    {
        LGlyph glyph = TInterface.TGlyphCreate("Traditional", "Classical Chinese");
        LEntryDraft draft = TInterface.TEntryDraftCreate("國a", "Mandarin", string.Empty, string.Empty, [], []);

        IReadOnlyList<LGlyphCell> cells = glyph.TGlyphDivide(draft);

        Assert.Equal("Classical Chinese", cells[0].LGlyphCellLanguage);
        Assert.Empty(cells[1].LGlyphCellLanguage);
        Assert.True(cells[0].LGlyphCellLinked);
        Assert.False(cells[1].LGlyphCellLinked);
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
