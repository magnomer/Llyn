using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TEpoch
{
    private const string TEpochLanguage = "Classical Chinese";

    private static IReadOnlyList<LEpoch> TEpochTableRead() =>
        TInterface.TEpochTableRead(TInterface.TLanguageLoad(TEpochLanguage));

    [Fact]
    public void LanguageLoaderLoad_ClassicalChinesePack_ReadsEpochLabels()
    {
        IReadOnlyList<LEpoch> epochs = TEpochTableRead();

        Assert.NotEmpty(epochs);
        Assert.Contains(epochs, epoch => epoch.LEpochLabel == "西周早期" && epoch.LEpochCode == "ZhouWesternEarly");
        Assert.Empty(TInterface.TEpochTableRead(TInterface.TLanguageLoad("Korean")));
    }

    [Fact]
    public void LanguageLoaderLoad_ClassicalChinesePack_GivesEveryStyleTheSharedTable()
    {
        IReadOnlyList<LScriptStyle> styles = TInterface.TLanguageLoad(TEpochLanguage).LLanguageScripts;

        Assert.NotEmpty(styles);
        foreach (LScriptStyle style in styles)
        {
            Assert.Equal(TEpochTableRead(), style.LScriptStyleEpoch);
        }
    }

    [Fact]
    public void EpochResolve_ClericalCaption_ReadsAnAgeAfterTheBronzes()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "老子乙227上 西漢");

        Assert.Equal("HanWestern", code);
        Assert.Equal("老子乙227上", caption);
    }

    [Fact]
    public void EpochResolve_DatedCaption_CutsChronologyAndKeepsName()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "西周早期 大盂鼎");

        Assert.Equal("ZhouWesternEarly", code);
        Assert.Equal("大盂鼎", caption);
    }

    [Fact]
    public void EpochResolve_DynastyAndPeriod_TakesTheLongerLabel()
    {
        Assert.Equal("ZhouWesternLate", TInterface.TEpochResolve(TEpochTableRead(), "西周晚期 頌壺").LEpochFound);
        Assert.Equal("ZhouWestern", TInterface.TEpochResolve(TEpochTableRead(), "西周 頌壺").LEpochFound);
    }

    [Fact]
    public void EpochResolve_BareChronology_LeavesNoCaption()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "春秋晚期");

        Assert.Equal("SpringAutumnLate", code);
        Assert.Equal(string.Empty, caption);
    }

    [Fact]
    public void EpochResolve_ChronologyInsideCaption_CutsItAndJoinsWhatRemains()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "天尹鐘 西周晚期 集成5");

        Assert.Equal("ZhouWesternLate", code);
        Assert.Equal("天尹鐘 集成5", caption);
    }

    [Fact]
    public void EpochResolve_DynastyWrittenWithItsAge_ReadsTheDynastyForm()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "王作女乙瓦弄卣 商代晚期 集成5102");

        Assert.Equal("ShangLate", code);
        Assert.Equal("王作女乙瓦弄卣 集成5102", caption);
    }

    [Fact]
    public void EpochResolve_ChronologyBetweenTwoAges_TakesTheEarlierOne()
    {
        (string code, string caption) = TInterface.TEpochResolve(
            TEpochTableRead(), "君子之弄鼎 春秋晚期或戰國早期 集成2086");

        Assert.Equal("SpringAutumnLate", code);
        Assert.Equal("君子之弄鼎 集成2086", caption);
    }

    [Fact]
    public void EpochResolve_LabelStartingAName_LeavesTheCaptionWhole()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "商鞅方升");

        Assert.Equal(string.Empty, code);
        Assert.Equal("商鞅方升", caption);
    }

    [Fact]
    public void EpochResolve_UnlistedChronology_LeavesTheCaptionWhole()
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), "新石器時代 陶文");

        Assert.Equal(string.Empty, code);
        Assert.Equal("新石器時代 陶文", caption);
    }

    [Fact]
    public void EpochResolve_NoTable_LeavesTheCaptionWhole()
    {
        (string code, string caption) = TInterface.TEpochResolve([], "西周早期 大盂鼎");

        Assert.Equal(string.Empty, code);
        Assert.Equal("西周早期 大盂鼎", caption);
    }

    [Theory]
    [InlineData("en")]
    [InlineData("ko")]
    public void LocalizationLoaderRead_EveryEpochCode_HasItsOwnText(string language)
    {
        IReadOnlyDictionary<string, string> texts = TInterface.TLocalizationLoaderRead(language);

        foreach (LEpoch epoch in TEpochTableRead())
        {
            Assert.True(texts.ContainsKey("Epoch." + epoch.LEpochCode), epoch.LEpochCode);
        }
    }
}
