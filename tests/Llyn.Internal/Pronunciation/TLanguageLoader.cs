using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TLanguageLoader
{
    [Fact]
    public void LanguageLoad_EnglishPack_ReadsTwoFlaggedVarieties()
    {
        LLanguage language = TInterface.TLanguageLoad("English");

        Assert.Equal(
            [("British", "gb"), ("American", "us")],
            language.LLanguageVarieties.Select(variety => (variety.LVarietyName, variety.LVarietyFlag)));
        Assert.True(language.LLanguageVarietyFlagged);
    }

    [Fact]
    public void LanguageLoad_TonalPack_ReadsTonal()
    {
        Assert.True(TInterface.TLanguageLoad("Mandarin").LLanguageTonal);
        Assert.True(TInterface.TLanguageLoad("Cantonese").LLanguageTonal);
        Assert.False(TInterface.TLanguageLoad("English").LLanguageTonal);
    }

    [Fact]
    public void LanguageLoad_SilentPack_ReadsSilent()
    {
        Assert.True(TInterface.TLanguageLoad("Classical Chinese").LLanguageSilent);
        Assert.False(TInterface.TLanguageLoad("Mandarin").LLanguageSilent);
        Assert.False(TInterface.TLanguageLoad("English").LLanguageSilent);
    }

    [Fact]
    public void LanguageLoad_UnlistedPack_ReadsUnlistedAndStaysOffTheList()
    {
        Assert.True(TInterface.TLanguageLoad("Wu").LLanguagePhonemic);
        Assert.DoesNotContain("Wu", TInterface.TLanguageScan());
        Assert.Contains("Mandarin", TInterface.TLanguageScan());
    }

    [Fact]
    public void LanguageLoad_PackWithoutVarieties_ReadsNoneUnflagged()
    {
        LLanguage language = TInterface.TLanguageLoad("Spanish");

        Assert.Empty(language.LLanguageVarieties);
        Assert.False(language.LLanguageVarietyFlagged);
    }

    [Fact]
    public void LanguageLoad_EnglishPack_ReadsTwoScopedRespellingGroups()
    {
        LLanguage language = TInterface.TLanguageLoad("English");

        Assert.Equal(
            ["British", "American"],
            language.LLanguageRespellings.Select(group => string.Join(",", group.LRespellingVarieties)));
        Assert.All(language.LLanguageRespellings, group => Assert.NotEmpty(group.LRespellingRules));
        Assert.All(language.LLanguageRespellings, group => Assert.NotEmpty(group.LRespellingVarieties));
        Assert.Empty(language.LLanguageCleanups);
    }

    [Fact]
    public void LanguageLoad_VarietiesDeclared_DropsUnscopedGroup()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "varieties": { "list": [ { "name": "British" }, { "name": "American" } ] },
              "cleanup": [
                { "name": "Loose", "rules": [["g", "ɡ"]] },
                { "name": "Tight", "varieties": ["British"], "rules": [["g", "ɡ"]] } ],
              "respelling": [
                { "name": "Shared", "rules": [["ɛ", "e"]] },
                { "name": "Blank", "varieties": [], "rules": [["ɛ", "e"]] },
                { "name": "British", "varieties": ["British"], "rules": [["æ", "a"]] },
                { "name": "American", "varieties": ["American"], "rules": [["ɝ", "ər"]] } ] }
            """);

        Assert.Equal(
            ["British"],
            language.LLanguageCleanups.Select(group => string.Join(",", group.LRespellingVarieties)));
        Assert.Equal(
            ["British", "American"],
            language.LLanguageRespellings.Select(group => string.Join(",", group.LRespellingVarieties)));
    }

    [Fact]
    public void LanguageLoad_NoVarieties_KeepsUnscopedGroup()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "cleanup": [ { "name": "Loose", "rules": [["g", "ɡ"]] } ],
              "respelling": [ { "name": "Shared", "rules": [["ɛ", "e"]] } ] }
            """);

        Assert.Equal(
            ["g"],
            language.LLanguageCleanups.Select(group => group.LRespellingRules[0].LRespellingRulePattern));
        Assert.Equal(
            ["ɛ"],
            language.LLanguageRespellings.Select(group => group.LRespellingRules[0].LRespellingRulePattern));
    }

    [Fact]
    public void LanguageLoad_SchemeObject_ReadsNameAndSources()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "transcription": [
                { "name": "Pinyin", "sources": [ { "name": "Wiktionary", "attempts": [
                    { "urls": ["https://example.test/{word}"], "strategy": "regex",
                      "match": "m=(\\S+)", "group": 1 } ] } ] },
                "Bopomofo",
                " ",
                "Pinyin" ] }
            """);

        Assert.Equal(["Pinyin", "Bopomofo"], language.LLanguageSchemes.Select(scheme => scheme.LSchemeName));
        Assert.Equal("Wiktionary", Assert.Single(language.LLanguageSchemes[0].LSchemeSources).LSourceSpecName);
        Assert.Empty(language.LLanguageSchemes[1].LSchemeSources);
    }

    [Fact]
    public void LanguageLoad_MandarinPack_ReadsSourcedSchemes()
    {
        LLanguage language = TInterface.TLanguageLoad("Mandarin");

        Assert.Equal(["Pinyin", "Bopomofo"], language.LLanguageSchemes.Select(scheme => scheme.LSchemeName));
        Assert.All(language.LLanguageSchemes, scheme => Assert.NotEmpty(scheme.LSchemeSources));
    }

    [Fact]
    public void LanguageLoad_PackWithoutRespelling_ReadsNone()
    {
        LLanguage language = TInterface.TLanguageLoad("Spanish");

        Assert.Empty(language.LLanguageRespellings);
        Assert.Empty(language.LLanguageCleanups);
        Assert.False(language.LLanguagePhonemic);
    }

    [Theory]
    [InlineData("Mandarin", true)]
    [InlineData("Cantonese", true)]
    [InlineData("English", false)]
    public void LanguageLoad_PhonemicKey_ReadsFlag(string language, bool phonemic)
    {
        LLanguage pack = TInterface.TLanguageLoad(language);

        Assert.Equal(phonemic, pack.LLanguagePhonemic);
        Assert.NotEmpty(pack.LLanguageRespellings);
    }

    [Fact]
    public void LanguageLoad_CleanupBlock_ReadsRulesInOrder()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "cleanup": [ { "name": "Sites", "rules": [["g", "ɡ"], ["́", "ˈ"], ["bad"], [1, 2]] } ] }
            """);

        LRespelling group = Assert.Single(language.LLanguageCleanups);
        Assert.Empty(group.LRespellingVarieties);
        Assert.Equal(
            [("g", "ɡ"), ("́", "ˈ")],
            group.LRespellingRules.Select(rule => (rule.LRespellingRulePattern, rule.LRespellingRuleReplacement)));
        Assert.Empty(language.LLanguageRespellings);
    }

    [Fact]
    public void LanguageLoad_BrokenRegex_DropsThatGroupOnly()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "respelling": [
                { "name": "Good", "rules": [["ɛ", "e"]] },
                { "name": "Broken", "varieties": ["British"], "rules": [["(", "x"]] },
                { "name": "Empty", "rules": [] },
                { "name": "Last", "varieties": ["American", ""], "rules": [["æ", "a"]] } ] }
            """);

        Assert.Equal(
            ["", "American"],
            language.LLanguageRespellings.Select(group => string.Join(",", group.LRespellingVarieties)));
    }

    [Fact]
    public void LanguageLoad_SpellingList_StampsEverySourceKind()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "spelling": [["ā", "a"], ["bad"], ["ō", "o"]],
              "pronunciation": [
                { "name": "P", "attempts": [ { "urls": ["u"], "strategy": "span", "match": "<s>" } ] } ],
              "frequency": [ { "name": "F", "attempts": [ { "urls": ["u"], "strategy": "regex", "match": "(1)" } ] } ] }
            """);

        Assert.Equal(
            [("ā", "a"), ("ō", "o")],
            Assert.Single(language.LLanguageLookupSources).LSourceSpecSpelling
                .Select(rule => (rule.LRespellingRulePattern, rule.LRespellingRuleReplacement)));
        Assert.Equal(2, Assert.Single(language.LLanguageFrequencies).LSourceSpecSpelling.Count);
    }

    [Fact]
    public void LanguageLoad_SpellingBrokenRegex_StampsNone()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "spelling": [["ā", "a"], ["(", "x"]],
              "pronunciation": [
                { "name": "P", "attempts": [ { "urls": ["u"], "strategy": "span", "match": "<s>" } ] } ] }
            """);

        Assert.Empty(Assert.Single(language.LLanguageLookupSources).LSourceSpecSpelling);
    }

    [Fact]
    public void LanguageLoad_LatinPack_ReadsSpellingOnEveryList()
    {
        LLanguage language = TInterface.TLanguageLoad("Classical Latin");

        Assert.Equal(12, Assert.Single(language.LLanguageLookupSources).LSourceSpecSpelling.Count);
        Assert.Equal(12, Assert.Single(language.LLanguageHarvestSources).LSourceSpecSpelling.Count);
        Assert.Equal(12, Assert.Single(language.LLanguageFrequencies).LSourceSpecSpelling.Count);
    }

    [Fact]
    public void LanguageLoad_PackWithoutSpelling_ReadsNone()
    {
        LLanguage language = TInterface.TLanguageLoad("Spanish");

        Assert.All(language.LLanguageLookupSources, source => Assert.Empty(source.LSourceSpecSpelling));
    }

    [Fact]
    public void LanguageLoad_FlagNamesSvg_ReadsPackFilePath()
    {
        LLanguage own = TInterface.TLanguageLoad("Classical Chinese");
        LLanguage coded = TInterface.TLanguageLoad("Mandarin");

        Assert.True(Path.IsPathRooted(own.LLanguageFlag));
        Assert.EndsWith(Path.Combine("languages", "Classical Chinese", "flag.svg"), own.LLanguageFlag);
        Assert.True(File.Exists(own.LLanguageFlag));
        Assert.Equal("cn", coded.LLanguageFlag);
    }
}
