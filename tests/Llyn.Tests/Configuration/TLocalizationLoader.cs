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
    public void LocalizationLoaderRead_UnknownLanguage_Throws()
    {
        Assert.Throws<InvalidDataException>(() => TInterface.TLocalizationLoaderRead("fr"));
    }
}
