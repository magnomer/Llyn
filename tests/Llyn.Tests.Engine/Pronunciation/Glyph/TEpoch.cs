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

    [Theory]
    [InlineData("老子乙227上 西漢", "HanWestern", "老子乙227上")]
    [InlineData("西周早期 大盂鼎", "ZhouWesternEarly", "大盂鼎")]
    [InlineData("天尹鐘 西周晚期 集成5", "ZhouWesternLate", "天尹鐘 集成5")]
    [InlineData("王作女乙瓦弄卣 商代晚期 集成5102", "ShangLate", "王作女乙瓦弄卣 集成5102")]
    [InlineData("君子之弄鼎 春秋晚期或戰國早期 集成2086", "SpringAutumnLate", "君子之弄鼎 集成2086")]
    public void EpochResolve_ChronologyInCaption_CutsChronologyOut(
        string text, string expectedCode, string expectedCaption)
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), text);

        Assert.Equal(expectedCode, code);
        Assert.Equal(expectedCaption, caption);
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

    [Theory]
    [InlineData("商鞅方升")]
    [InlineData("新石器時代 陶文")]
    public void EpochResolve_UnlistedWording_KeepsCaptionWhole(string text)
    {
        (string code, string caption) = TInterface.TEpochResolve(TEpochTableRead(), text);

        Assert.Equal(string.Empty, code);
        Assert.Equal(text, caption);
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

        LEpoch[] epochs = [.. TEpochTableRead()];
        Assert.NotEmpty(epochs);

        foreach (LEpoch epoch in epochs)
        {
            Assert.True(texts.ContainsKey("Epoch." + epoch.LEpochCode), epoch.LEpochCode);
        }
    }
}
