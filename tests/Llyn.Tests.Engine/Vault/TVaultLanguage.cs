using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TVaultLanguage
{
    [Fact]
    public void LanguageRead_ShippedPack_ReturnsItsSources()
    {
        LLanguageVault languages = TInterface.TLanguageVaultCreate();

        LLanguage pack = languages.TLanguageRead("English");

        Assert.NotEmpty(pack.LLanguageLookupSources);
    }

    [Fact]
    public void LanguageScan_ShippedPacks_ListsEnglishFirst()
    {
        LLanguageVault languages = TInterface.TLanguageVaultCreate();

        IReadOnlyList<string> listed = languages.TLanguageScan();

        Assert.Equal("English", listed[0]);
        Assert.Contains("Korean", listed);
    }

    [Fact]
    public void LanguageRead_UnknownPack_ReturnsBlankPack()
    {
        LLanguageVault languages = TInterface.TLanguageVaultCreate();

        LLanguage pack = languages.TLanguageRead("Atlantean");

        Assert.Empty(pack.LLanguageLookupSources);
        Assert.False(languages.TLanguageNameValidate("../English"));
    }
}
