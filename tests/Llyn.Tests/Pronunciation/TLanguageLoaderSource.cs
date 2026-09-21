using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TLanguageLoaderSource
{
    [Fact]
    public void LanguageLoad_ReadingsList_KeepsWrittenOrder()
    {
        LSourceSpec oxford = TLanguageSourceFind("English", "Oxford");

        LSourceAttempt attempt = Assert.Single(oxford.LSourceSpecAttempts);
        Assert.Equal(
            ["British", "American"],
            attempt.LSourceAttemptReadings.Select(reading => reading.LSourceReadingVariety));
        Assert.All(attempt.LSourceAttemptReadings, reading => Assert.Equal(0, reading.LSourceReadingSkip));
        Assert.All(attempt.LSourceAttemptReadings, reading => Assert.False(reading.LSourceReadingEvery));
    }

    [Fact]
    public void LanguageLoad_EveryKey_ReadsEveryFlag()
    {
        LSourceSpec wiktionary = TLanguageSourceFind("Mandarin", "Wiktionary");

        LSourceAttempt attempt = Assert.Single(wiktionary.LSourceSpecAttempts);
        Assert.All(attempt.LSourceAttemptReadings, reading => Assert.True(reading.LSourceReadingEvery));
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
            attempt.LSourceAttemptReadings.Select(reading =>
                (reading.LSourceReadingVariety, reading.LSourceReadingStrategy)));
        Assert.All(attempt.LSourceAttemptReadings, reading => Assert.False(reading.LSourceReadingPhonetic));
    }

    [Fact]
    public void LanguageLoad_FollowObject_ReadsUnnormalizedReading()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "pronunciation": [ { "name": "Stub", "attempts": [
                { "urls": ["https://example.test/{word}"], "strategy": "regex", "match": "m=(.+)", "group": 1,
                  "follow": { "strategy": "regex", "match": "see=(.+)", "group": 1, "normalize": true } } ] } ] }
            """);

        LSourceAttempt attempt = Assert.Single(Assert.Single(language.LLanguageLookupSources).LSourceSpecAttempts);
        Assert.NotNull(attempt.LSourceAttemptFollow);
        Assert.Equal("see=(.+)", attempt.LSourceAttemptFollow.LSourceReadingPattern);
        Assert.False(attempt.LSourceAttemptFollow.LSourceReadingPhonetic);
    }

    [Fact]
    public void LanguageLoad_NoFollow_LeavesAttemptNull()
    {
        LSourceSpec longman = TLanguageSourceFind("English", "Longman");

        Assert.Null(Assert.Single(longman.LSourceSpecAttempts).LSourceAttemptFollow);
        Assert.False(Assert.Single(longman.LSourceSpecAttempts).LSourceAttemptDecoded);
    }

    [Fact]
    public void LanguageLoad_DecodeKey_MarksAttemptDecoded()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "pronunciation": [ { "name": "Stub", "attempts": [
                { "urls": ["https://example.test/{word}"], "strategy": "regex", "match": "m=(.+)", "group": 1,
                  "decode": true } ] } ] }
            """);

        LSourceAttempt attempt = Assert.Single(Assert.Single(language.LLanguageLookupSources).LSourceSpecAttempts);
        Assert.True(attempt.LSourceAttemptDecoded);
    }

    [Fact]
    public void LanguageLoad_FrequencyWithBandsAndOnce_ReadsEachSourceOwnScale()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "frequency": [
                { "name": "First", "attempts": [
                    { "urls": ["https://example.test/a/{word}"], "strategy": "regex",
                      "match": "a=(\\S+)", "group": 1 } ],
                  "once": { "total": 1000000 },
                  "bands": [
                    { "match": "^[SW]1$", "name": "Core" },
                    { "match": "^unranked$", "name": "Rare" } ] },
                { "name": "Second", "attempts": [
                    { "urls": ["https://example.test/b/{word}"], "strategy": "regex",
                      "match": "b=(\\S+)", "group": 1 } ],
                  "once": { "factor": 22, "base": 2 }, "unit": " Level " } ] }
            """);

        Assert.Equal(["First", "Second"], language.LLanguageFrequencies.Select(source => source.LSourceSpecName));
        LSourceSpec first = language.LLanguageFrequencies[0];
        Assert.Equal(
            [("Core", "^[SW]1$"), ("Rare", "^unranked$")],
            first.LSourceSpecBands.Select(band => (band.LBandName, band.LBandPattern)));
        Assert.Equal((1000000, null, null), (first.LSourceSpecTotal, first.LSourceSpecFactor, first.LSourceSpecBase));
        LSourceSpec second = language.LLanguageFrequencies[1];
        Assert.Empty(second.LSourceSpecBands);
        Assert.Equal((null, 22, 2), (second.LSourceSpecTotal, second.LSourceSpecFactor, second.LSourceSpecBase));
        Assert.Equal("Level", second.LSourceSpecUnit);
        Assert.Null(first.LSourceSpecUnit);
    }

    [Fact]
    public void LanguageLoad_MalformedBandRows_SkipsEachOne()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "frequency": [
                { "name": "First", "attempts": [
                    { "urls": ["https://example.test/a/{word}"], "strategy": "regex",
                      "match": "a=(\\S+)", "group": 1 } ],
                  "once": { "total": -5, "factor": "12" },
                  "bands": [
                    { "match": "^S1$" },
                    { "name": " ", "match": "^S1$" },
                    { "name": "Numeric", "upTo": 10 },
                    { "name": "Floored", "atLeast": 10 },
                    { "name": "Bare" },
                    { "name": "Broken", "match": "(" },
                    { "name": "Kept", "match": "^S1$" } ] } ] }
            """);

        LSourceSpec first = Assert.Single(language.LLanguageFrequencies);
        LBand kept = Assert.Single(first.LSourceSpecBands);
        Assert.Equal("Kept", kept.LBandName);
        Assert.Equal((null, null, null), (first.LSourceSpecTotal, first.LSourceSpecFactor, first.LSourceSpecBase));
    }

    [Fact]
    public void LanguageLoad_PackWithoutFrequency_ReadsEmptyLists()
    {
        LLanguage language = TInterface.TLanguageLoad("Spanish");

        Assert.Empty(language.LLanguageFrequencies);
    }

    [Fact]
    public void LanguageLoad_GlyphWithSources_ReadsNameLanguageAndSource()
    {
        LLanguage language = TInterface.TLanguageLoad("Mandarin");

        Assert.NotNull(language.LLanguageGlyph);
        Assert.Equal("Traditional", language.LLanguageGlyph.LGlyphName);
        Assert.Equal("Classical Chinese", language.LLanguageGlyph.LGlyphLanguage);
        Assert.Equal("Wiktionary", Assert.Single(language.LLanguageGlyph.LGlyphSources).LSourceSpecName);
        LFont? font = language.LLanguageGlyph.LGlyphFont;
        Assert.Equal("Microsoft JhengHei UI, Microsoft YaHei UI, Malgun Gothic", font?.LFontFamily);
        Assert.Equal(26, font?.LFontSize);
    }

    [Fact]
    public void LanguageLoad_GlyphWithoutSources_ReadsEmptySourceList()
    {
        LLanguage language = TInterface.TLanguageLoad("Japanese");

        Assert.NotNull(language.LLanguageGlyph);
        Assert.Equal("Kanji", language.LLanguageGlyph.LGlyphName);
        Assert.Empty(language.LLanguageGlyph.LGlyphSources);
    }

    [Fact]
    public void LanguageLoad_GlyphMissingLanguage_ReadsNull()
    {
        LLanguage language = TLanguageFixture.TLanguageFixtureLoad(
            """
            { "language": "Fixture", "glyph": { "name": "Traditional", "font": { "family": "Serif" } } }
            """);

        Assert.Null(language.LLanguageGlyph);
        Assert.Null(TInterface.TLanguageLoad("English").LLanguageGlyph);
        Assert.Null(TLanguageFixture.TLanguageFixtureLoad(
            """
            { "language": "Fixture", "glyph": { "name": "Kanji", "language": "Classical Chinese" } }
            """).LLanguageGlyph?.LGlyphFont);
    }

    private static LSourceSpec TLanguageSourceFind(string language, string name)
    {
        return Assert.Single(
            TInterface.TLanguageLoad(language).LLanguageLookupSources,
            source => source.LSourceSpecName == name);
    }
}
