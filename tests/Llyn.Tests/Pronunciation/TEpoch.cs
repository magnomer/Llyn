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
