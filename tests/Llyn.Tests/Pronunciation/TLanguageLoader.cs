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
    public void LanguageLoad_PackWithoutVarieties_ReadsNoneUnflagged()
    {
        LLanguage language = TInterface.TLanguageLoad("Spanish");

        Assert.Empty(language.LLanguageVarieties);
        Assert.False(language.LLanguageVarietyFlagged);
    }

    [Fact]
    public void LanguageLoad_ReadingsList_KeepsWrittenOrder()
    {
        LSourceSpec oxford = TLanguageSourceFind("English", "Oxford");

        LSourceAttempt attempt = Assert.Single(oxford.LSourceSpecAttempts);
        Assert.Equal(
            [("British", 0), ("American", 1)],
            attempt.LSourceAttemptReadings.Select(reading => (reading.LSourceReadingVariety, reading.LSourceReadingSkip)));
    }

    [Fact]
    public void LanguageLoad_FlatAttempt_ReadsOneUntagged()
    {
        LSourceSpec longman = TLanguageSourceFind("English", "Longman");

        LSourceAttempt attempt = Assert.Single(longman.LSourceSpecAttempts);
        LSourceReading reading = Assert.Single(attempt.LSourceAttemptReadings);
        Assert.Equal(string.Empty, reading.LSourceReadingVariety);
        Assert.Equal("span", reading.LSourceReadingStrategy);
    }

    [Fact]
    public void LanguageLoad_AudioReadingsList_ReadsTwoVarieties()
    {
        LSourceSpec oxford = Assert.Single(
            TInterface.TLanguageLoad("English").LLanguageHarvestSources,
            source => source.LSourceSpecName == "Oxford");

        LSourceAttempt attempt = Assert.Single(oxford.LSourceSpecAttempts);
        Assert.Equal(
            [("British", "regex"), ("American", "regex")],
            attempt.LSourceAttemptReadings.Select(reading => (reading.LSourceReadingVariety, reading.LSourceReadingStrategy)));
        Assert.All(attempt.LSourceAttemptReadings, reading => Assert.False(reading.LSourceReadingPhonetic));
    }

    [Fact]
    public void LanguageLoad_EnglishPack_ReadsTwoScopedRespellingGroups()
    {
        LLanguage language = TInterface.TLanguageLoad("English");

        Assert.Equal(
            [("British", "British"), ("American", "American")],
            language.LLanguageRespellings.Select(group => (group.LRespellingName, string.Join(",", group.LRespellingVarieties))));
        Assert.All(language.LLanguageRespellings, group => Assert.NotEmpty(group.LRespellingRules));
        Assert.All(language.LLanguageRespellings, group => Assert.NotEmpty(group.LRespellingVarieties));
        Assert.Empty(language.LLanguageCleanups);
    }

    [Fact]
    public void LanguageLoad_VarietiesDeclared_DropsUnscopedGroup()
    {
        LLanguage language = TLanguageFixtureLoad(
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

        Assert.Equal(["Tight"], language.LLanguageCleanups.Select(group => group.LRespellingName));
        Assert.Equal(["British", "American"], language.LLanguageRespellings.Select(group => group.LRespellingName));
    }

    [Fact]
    public void LanguageLoad_NoVarieties_KeepsUnscopedGroup()
    {
        LLanguage language = TLanguageFixtureLoad(
            """
            { "cleanup": [ { "name": "Loose", "rules": [["g", "ɡ"]] } ],
              "respelling": [ { "name": "Shared", "rules": [["ɛ", "e"]] } ] }
            """);

        Assert.Equal(["Loose"], language.LLanguageCleanups.Select(group => group.LRespellingName));
        Assert.Equal(["Shared"], language.LLanguageRespellings.Select(group => group.LRespellingName));
    }

    [Fact]
    public void LanguageLoad_PackWithoutRespelling_ReadsNone()
    {
        LLanguage language = TInterface.TLanguageLoad("Spanish");

        Assert.Empty(language.LLanguageRespellings);
        Assert.Empty(language.LLanguageCleanups);
    }

    [Fact]
    public void LanguageLoad_CleanupBlock_ReadsRulesInOrder()
    {
        LLanguage language = TLanguageFixtureLoad(
            """
            { "cleanup": [ { "name": "Sites", "rules": [["g", "ɡ"], ["́", "ˈ"], ["bad"], [1, 2]] } ] }
            """);

        LRespelling group = Assert.Single(language.LLanguageCleanups);
        Assert.Equal("Sites", group.LRespellingName);
        Assert.Empty(group.LRespellingVarieties);
        Assert.Equal(
            [("g", "ɡ"), ("́", "ˈ")],
            group.LRespellingRules.Select(rule => (rule.LRespellingRulePattern, rule.LRespellingRuleReplacement)));
        Assert.Empty(language.LLanguageRespellings);
    }

    [Fact]
    public void LanguageLoad_BrokenRegex_DropsThatGroupOnly()
    {
        LLanguage language = TLanguageFixtureLoad(
            """
            { "respelling": [
                { "name": "Good", "rules": [["ɛ", "e"]] },
                { "name": "Broken", "varieties": ["British"], "rules": [["(", "x"]] },
                { "name": "Empty", "rules": [] },
                { "name": "Last", "varieties": ["American", ""], "rules": [["æ", "a"]] } ] }
            """);

        Assert.Equal(
            [("Good", ""), ("Last", "American")],
            language.LLanguageRespellings.Select(group => (group.LRespellingName, string.Join(",", group.LRespellingVarieties))));
    }

    private static LLanguage TLanguageFixtureLoad(string json)
    {
        string name = "Fixture" + Guid.NewGuid().ToString("N");
        string folder = Path.Combine(AppContext.BaseDirectory, "languages", name);
        Directory.CreateDirectory(folder);
        try
        {
            File.WriteAllText(Path.Combine(folder, "source.json"), json);
            return TInterface.TLanguageLoad(name);
        }
        finally
        {
            Directory.Delete(folder, true);
        }
    }

    private static LSourceSpec TLanguageSourceFind(string language, string name)
    {
        return Assert.Single(
            TInterface.TLanguageLoad(language).LLanguageLookupSources,
            source => source.LSourceSpecName == name);
    }
}
