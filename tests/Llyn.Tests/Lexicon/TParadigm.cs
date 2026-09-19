using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TParadigm
{
    [Fact]
    public void SpeechPackLoad_PartNamingParent_ReadsParentCode()
    {
        LSpeechPack pack = TParadigmPackLoad("""
            {
              "parts": [
                { "id": 1, "name": "Noun" },
                { "id": 2, "name": "Noun, countable", "parent": 1 },
                { "id": 3, "name": "Noun, proper", "parent": "one" }
              ]
            }
            """);

        Assert.Equal([0L, 1L, 0L], pack.LSpeechPackValues.Select(row => row.LSpeechValueParent));
    }

    [Fact]
    public void SpeechPackLoad_ParadigmDeclared_ReadsValuesInOrder()
    {
        LSpeechPack pack = TParadigmPackLoad("""
            {
              "paradigms": [
                { "part": 6, "values": [7, 5, 6] },
                { "part": 1, "values": [2], "except": [3, 4],
                  "regular": [["$", "s"], ["y$", "ies"], ["(", "x"], ["bad"], 7] }
              ]
            }
            """);

        Assert.Equal(2, pack.LSpeechPackParadigms.Count);
        LParadigm verb = pack.LSpeechPackParadigms[0];
        Assert.Equal(6, verb.LParadigmSpeechCode);
        Assert.Equal([7L, 5L, 6L], verb.LParadigmMorphology);
        Assert.Empty(verb.LParadigmRegular);
        Assert.Empty(verb.LParadigmExcept);
        LParadigm noun = pack.LSpeechPackParadigms[1];
        Assert.Equal(1, noun.LParadigmSpeechCode);
        Assert.Equal([2L], noun.LParadigmMorphology);
        Assert.Equal([3L, 4L], noun.LParadigmExcept);
        Assert.Equal(["$", "y$"], noun.LParadigmRegular.Select(rule => rule.LParadigmRulePattern));
        Assert.Equal("cats", TInterface.TParadigmRuleResolve(noun.LParadigmRegular[0], "cat"));
        Assert.Null(TInterface.TParadigmRuleResolve(noun.LParadigmRegular[1], "cat"));
    }

    [Fact]
    public void SpeechPackLoad_ParadigmRowMalformed_SkipsRow()
    {
        LSpeechPack pack = TParadigmPackLoad("""
            {
              "paradigms": [
                { "part": "noun", "values": [2] },
                { "part": 0, "values": [2] },
                { "part": 1, "values": [2, "plural"] },
                { "part": 1, "values": [2, -1] },
                { "part": 1 },
                { "part": 12, "values": [11, 12], "regular": "", "except": [1, "x"] }
              ]
            }
            """);

        LParadigm kept = Assert.Single(pack.LSpeechPackParadigms);
        Assert.Equal(12, kept.LParadigmSpeechCode);
        Assert.Equal([11L, 12L], kept.LParadigmMorphology);
        Assert.Empty(kept.LParadigmRegular);
        Assert.Empty(kept.LParadigmExcept);
    }

    [Fact]
    public void SpeechPackLoad_EnglishPack_DeclaresParadigms()
    {
        LSpeechPack pack = TInterface.TSpeechPackLoad("English");

        LSpeechValue countable = Assert.Single(pack.LSpeechPackValues, row => row.LSpeechValueCode == 2);
        Assert.Equal(1, countable.LSpeechValueParent);
        LParadigm noun = Assert.Single(pack.LSpeechPackParadigms, row => row.LParadigmSpeechCode == 1);
        Assert.Equal([2L], noun.LParadigmMorphology);
        Assert.Equal([3L, 4L], noun.LParadigmExcept);
        Assert.NotEmpty(noun.LParadigmRegular);
    }

    [Theory]
    [InlineData("Classical Latin", 3, 11, 3)]
    [InlineData("Classical Greek", 3, 9, 5)]
    public void SpeechPackLoad_ClassicalPack_DeclaresPrincipalParts(
        string language, int paradigms, long genitive, int principalParts)
    {
        LSpeechPack pack = TInterface.TSpeechPackLoad(language);

        Assert.Equal(paradigms, pack.LSpeechPackParadigms.Count);
        LParadigm noun = Assert.Single(pack.LSpeechPackParadigms, row => row.LParadigmSpeechCode == 1);
        Assert.Equal([genitive], noun.LParadigmMorphology);
        Assert.Equal([2L], noun.LParadigmExcept);
        LParadigm verb = Assert.Single(pack.LSpeechPackParadigms, row => row.LParadigmSpeechCode == 3);
        Assert.Equal(principalParts, verb.LParadigmMorphology.Count);
        Assert.All(
            pack.LSpeechPackParadigms.SelectMany(row => row.LParadigmMorphology),
            code => Assert.Contains(pack.LSpeechPackMorphology, row => row.LMorphologyCode == code));
    }

    private static LSpeechPack TParadigmPackLoad(string json)
    {
        string name = "Fixture" + Guid.NewGuid().ToString("N");
        string folder = Path.Combine(AppContext.BaseDirectory, "languages", name);
        Directory.CreateDirectory(folder);
        try
        {
            File.WriteAllText(Path.Combine(folder, "vocabulary.json"), json);
            return TInterface.TSpeechPackLoad(name);
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    [Fact]
    public void ParadigmRowScan_TwoPartsOfSpeech_LeadsEachPartOnce()
    {
        LSpeechValue noun = TInterface.TSpeechValueCreate("English", 1, "noun", 0) with { LSpeechValueId = 1 };
        LSpeechValue verb = TInterface.TSpeechValueCreate("English", 2, "verb", 1) with { LSpeechValueId = 2 };
        LMorphology plural = TInterface.TMorphologyCreate(1, 1, "plural", 0);
        LMorphology past = TInterface.TMorphologyCreate(2, 1, "past", 0);
        LInflection wolves = TInterface.TInflectionCreate(1, 0, "wolves", null, 1, [1]);
        IReadOnlyList<LParadigmSlot> slots =
        [
            TInterface.TParadigmSlotCreate(noun, plural, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(noun, past, wolves, LState.LStateSpecified),
            TInterface.TParadigmSlotCreate(verb, past, null, LState.LStateUnknown),
        ];

        IReadOnlyList<LParadigmRow> rows = TInterface.TParadigmRowScan(slots);

        Assert.Equal(2, rows.Count);
        Assert.Equal("plural, past", rows[0].LParadigmRowName);
        Assert.Equal("noun", rows[0].LParadigmRowPart);
        Assert.True(rows[0].LParadigmRowLead);
        Assert.True(rows[1].LParadigmRowLead);
        Assert.True(rows[1].LParadigmRowFirst.LParadigmSlotUncertain);
        Assert.Empty(TInterface.TParadigmRowScan([]));
    }
}
