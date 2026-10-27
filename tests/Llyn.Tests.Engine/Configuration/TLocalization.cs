using Xunit;

namespace Llyn.Tests;

public sealed class TLocalization
{
    private const string TLocalizationSample = """
        {
          "terms.product": "Llyn",
          "terms.entry": "entry",
          "texts": {
            "Display.Note": "Note of {Terms.entry}",
            "Display.FanqieTone": "{0} class"
          }
        }
        """;

    [Fact]
    public void LocalizationLoad_SampleCatalog_ResolvesTermsIntoTexts()
    {
        IReadOnlyDictionary<string, string> texts = TInterface.TLocalizationLoad(TLocalizationSample, "en");

        Assert.Equal("Note of Entry", texts["Display.Note"]);
        Assert.Equal("Llyn", texts["Terms.Product"]);
        Assert.Equal("Note of Entry", TInterface.TLocalizationTextRead("Display.Note"));
    }

    [Fact]
    public void LocalizationTextRead_MissingKey_ReturnsKey()
    {
        TInterface.TLocalizationLoad(TLocalizationSample, "en");

        Assert.Equal("Display.Missing", TInterface.TLocalizationTextRead("Display.Missing"));
        Assert.Null(TInterface.TLocalizationTextFind("Display.Missing"));
    }

    [Fact]
    public void LocalizationLoad_EmbeddedKorean_SwitchesCatalog()
    {
        IReadOnlyDictionary<string, string> english = TInterface.TLocalizationLoad("en");
        IReadOnlyDictionary<string, string> korean = TInterface.TLocalizationLoad("ko");

        Assert.NotEqual(english["Display.Note"], korean["Display.Note"]);
        Assert.Equal(korean["Display.Note"], TInterface.TLocalizationTextRead("Display.Note"));
    }

    [Fact]
    public void LocalizationNormalize_UnknownLanguage_ReturnsEnglish()
    {
        Assert.Equal("en", TInterface.TLocalizationNormalize("fr"));
        Assert.Equal("ko", TInterface.TLocalizationNormalize("ko"));
        Assert.True(TInterface.TLocalizationDefaultCheck(null));
        Assert.False(TInterface.TLocalizationDefaultCheck("ko"));
    }

    [Fact]
    public void LocalizationGroupFind_SearchText_ReadsMatchingGroups()
    {
        TInterface.TLocalizationLoad(TLocalizationSample, "en");
        IReadOnlyList<(string, IReadOnlyList<string>)> groups =
        [
            ("Note", ["Display.Note"]),
            ("Tone", ["Display.Missing", "Display.FanqieTone"]),
        ];

        Assert.Equal(["Note"], TInterface.TLocalizationGroupFind(groups, "  NOTE "));
        Assert.Equal(["Tone"], TInterface.TLocalizationGroupFind(groups, "class"));
        Assert.Equal(["Note"], TInterface.TLocalizationGroupFind(groups, "note*entry"));
        Assert.Empty(TInterface.TLocalizationGroupFind(groups, "absent"));
    }

    [Fact]
    public void LocalizationGroupFind_BlankOrNull_ReadsAllGroupsInOrder()
    {
        IReadOnlyList<(string, IReadOnlyList<string>)> groups = [("One", []), ("Two", ["Display.Note"])];

        Assert.Equal(["One", "Two"], TInterface.TLocalizationGroupFind(groups, "   "));
        Assert.Equal(["One", "Two"], TInterface.TLocalizationGroupFind(groups, null));
    }
}
