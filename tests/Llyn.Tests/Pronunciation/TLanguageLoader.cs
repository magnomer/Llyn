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

    private static LSourceSpec TLanguageSourceFind(string language, string name)
    {
        return Assert.Single(
            TInterface.TLanguageLoad(language).LLanguageLookupSources,
            source => source.LSourceSpecName == name);
    }
}
