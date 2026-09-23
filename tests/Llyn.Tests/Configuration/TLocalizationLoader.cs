using Xunit;

namespace Llyn.Tests;

public sealed class TLocalizationLoader
{
    [Fact]
    public void LocalizationLoaderRead_EmbeddedEnglish_YieldsTermsAndTexts()
    {
        IReadOnlyDictionary<string, string> pairs = TInterface.TLocalizationLoaderRead("en");

        Assert.Equal("Llyn", pairs["terms.product"]);
        Assert.Contains(pairs.Keys, key => !key.StartsWith("terms.", StringComparison.Ordinal));
    }

    [Fact]
    public void LocalizationLoaderRead_EveryCatalog_MatchesDefaultKeys()
    {
        IReadOnlyDictionary<string, string> standard = TInterface.TLocalizationLoaderRead("en");

        foreach (string language in TInterface.TLocalizationScan())
        {
            IReadOnlyDictionary<string, string> pairs = TInterface.TLocalizationLoaderRead(language);
            string[] missing = [.. standard.Keys.Where(key => !pairs.ContainsKey(key))];
            string[] extra = [.. pairs.Keys.Where(key => !standard.ContainsKey(key))];

            Assert.True(
                missing.Length == 0 && extra.Length == 0,
                $"{language}: missing [{string.Join(", ", missing)}], extra [{string.Join(", ", extra)}]");
        }
    }

    [Fact]
    public void LocalizationLoaderRead_UnknownLanguage_Throws()
    {
        Assert.Throws<InvalidDataException>(() => TInterface.TLocalizationLoaderRead("fr"));
    }
}
