using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TLanguageLoaderHan
{
    [Fact]
    public void LanguageLoad_ScriptStyles_ReadsFormPatternGroupsAndRewrite()
    {
        LLanguage language = TInterface.TLanguageLoad("Classical Chinese");

        Assert.Equal(
            ["金文", "小篆", "隸書", "異體"],
            language.LLanguageScripts.Select(style => style.LScriptStyleName).ToList());
        LScriptStyle seal = language.LLanguageScripts[1];
        Assert.Equal("https://xiaoxue.iis.sinica.edu.tw/xiaozhuan/PageResult/PageResult", seal.LScriptStyleUrl);
        Assert.Equal("{word}", seal.LScriptStyleForm["EudcFontChar"]);
        Assert.Equal((1, 2), (seal.LScriptStyleImage, seal.LScriptStyleCaption));
        Assert.Equal("https://xiaoxue.iis.sinica.edu.tw", seal.LScriptStylePrefix);
        Assert.Equal("size=200", Assert.Single(seal.LScriptStyleRewrite).LRespellingRuleReplacement);
        Assert.NotNull(seal.LScriptStyleGloss);
        Assert.Null(language.LLanguageScripts[0].LScriptStyleGloss);
        Assert.Empty(TInterface.TLanguageLoad("English").LLanguageScripts);
    }
    [Fact]
    public void LanguageLoad_FanqieBooks_ReadsFormMarkerBusyAndInterval()
    {
        LLanguage language = TInterface.TLanguageLoad("Classical Chinese");

        Assert.Equal(["廣韻", "廣韻", "集韻"], language.LLanguageFanqieBooks.Select(book => book.LFanqieBookName).ToList());
        Assert.Equal(
            ["Kaom", "Wiktionary", "Kaom"],
            language.LLanguageFanqieBooks.Select(book => book.LFanqieBookSource).ToList());
        LFanqieBook wiki = language.LLanguageFanqieBooks[1];
        Assert.Empty(wiki.LFanqieBookForm);
        Assert.Contains("(?<spelling>", wiki.LFanqieBookLine);
        Assert.Null(wiki.LFanqieBookSpelling);
        LFanqieBook rhymes = language.LLanguageFanqieBooks[2];
        Assert.Equal("http://www.kaom.net/zgy_dw8.php", rhymes.LFanqieBookUrl);
        Assert.Equal(("{word}", "j"), (rhymes.LFanqieBookForm["word"], rhymes.LFanqieBookForm["t"]));
        Assert.Contains("{word}", rhymes.LFanqieBookPattern);
        Assert.Equal("點擊過頻", rhymes.LFanqieBookBusy);
        Assert.Equal(4, rhymes.LFanqieBookInterval);
        Assert.NotNull(rhymes.LFanqieBookSplit);
        Assert.Contains("(?<rime>", rhymes.LFanqieBookHead);
        Assert.Contains("(?<tone>", rhymes.LFanqieBookColumn);
        Assert.Contains("{word}", rhymes.LFanqieBookRounded);
        Assert.NotNull(rhymes.LFanqieBookSpelling);
        Assert.Empty(TInterface.TLanguageLoad("English").LLanguageFanqieBooks);
    }
    [Fact]
    public void LanguageLoad_Hypothesis_ReadsInitialFinalAndToneTables()
    {
        LHypothesis? hypothesis = TInterface.TLanguageLoad("Classical Chinese").LLanguageHypothesis;

        Assert.NotNull(hypothesis);
        Assert.Equal("ng", hypothesis.LHypothesisInitials["疑"]);
        Assert.Equal("o", hypothesis.LHypothesisFinals["模 一"]);
        Assert.Equal("wan", hypothesis.LHypothesisFinals["寒 一 合"]);
        LHypothesisTone entering = hypothesis.LHypothesisTones["入"][0];
        Assert.Equal("7", entering.LHypothesisToneClass);
        LRespellingRule velar = Assert.Single(
            entering.LHypothesisToneRules, rule => rule.LRespellingRulePattern == "ng$");
        Assert.Equal("k", velar.LRespellingRuleReplacement);
        Assert.Equal("4S", hypothesis.LHypothesisTones["上"][1].LHypothesisToneClass);
        Assert.Equal(
            ["labial", "dental", "retroflex", "palatal", "velar", "laryngeal", "other"],
            hypothesis.LHypothesisPlaces.Select(place => place.LHypothesisLocusName));
        Assert.Equal(["明", "幫", "並", "滂"], hypothesis.LHypothesisPlaces[0].LHypothesisLocusInitials);
        Assert.Null(TInterface.TLanguageLoad("English").LLanguageHypothesis);
    }
    [Fact]
    public void LanguageLoad_HypothesisMissingFinals_ReadsNull()
    {
        LLanguage language = TLanguageHanLoad(
            """
            { "language": "Fixture", "hypothesis": { "initial": { "疑": "ng" } } }
            """);

        Assert.Null(language.LLanguageHypothesis);
    }
    [Fact]
    public void LanguageLoad_HypothesisNamesFile_ReadsTablesFromPackFolder()
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "language": "Fixture", "hypothesis": "tables.json" }
            """);
        pack.TLanguageFixtureSave(
            "tables.json",
            """
            { "initial": { "疑": "ng" }, "final": { "模 一": "o" } }
            """);

        LHypothesis? hypothesis = TInterface.TLanguageLoad(pack.TLanguageFixtureName).LLanguageHypothesis;

        Assert.NotNull(hypothesis);
        Assert.Equal("o", hypothesis.LHypothesisFinals["模 一"]);
    }
    [Fact]
    public void LanguageLoad_HypothesisNamesMissingOrOutsideFile_ReadsNull()
    {
        using TLanguageFixture missing = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "language": "Fixture", "hypothesis": "absent.json" }
            """);
        using TLanguageFixture outside = TLanguageFixture.TLanguageFixtureCreate(
            """
            { "language": "Fixture", "hypothesis": "../English/source.json" }
            """);

        Assert.Null(TInterface.TLanguageLoad(missing.TLanguageFixtureName).LLanguageHypothesis);
        Assert.Null(TInterface.TLanguageLoad(outside.TLanguageFixtureName).LLanguageHypothesis);
    }
    [Fact]
    public void LanguageLoad_FanqieMissingMatch_SkipsThatBook()
    {
        LLanguage language = TLanguageHanLoad(
            """
            { "language": "Fixture", "fanqie": [
                { "name": "Broad", "url": "https://example.test/broad" },
                { "name": "Collected", "url": "https://example.test/collected", "match": "<u>{word}</u>" } ] }
            """);

        LFanqieBook book = Assert.Single(language.LLanguageFanqieBooks);
        Assert.Equal("Collected", book.LFanqieBookName);
        Assert.Equal("Collected", book.LFanqieBookSource);
        Assert.Null(book.LFanqieBookLine);
        Assert.Empty(book.LFanqieBookForm);
        Assert.Null(book.LFanqieBookBusy);
        Assert.Equal(0, book.LFanqieBookInterval);
    }
    [Fact]
    public void LanguageLoad_ScriptMissingMatch_SkipsThatStyle()
    {
        LLanguage language = TLanguageHanLoad(
            """
            { "language": "Fixture", "script": [
                { "name": "Seal", "url": "https://example.test/seal" },
                { "name": "Clerical", "url": "https://example.test/clerical",
                  "match": "<img src=\"([^\"]+)\"", "image": 1 } ] }
            """);

        LScriptStyle style = Assert.Single(language.LLanguageScripts);
        Assert.Equal("Clerical", style.LScriptStyleName);
        Assert.Empty(style.LScriptStyleForm);
        Assert.Equal(0, style.LScriptStyleCaption);
        Assert.Empty(style.LScriptStyleRewrite);
    }

    private static LLanguage TLanguageHanLoad(string json)
    {
        using TLanguageFixture pack = TLanguageFixture.TLanguageFixtureCreate(json);
        return TInterface.TLanguageLoad(pack.TLanguageFixtureName);
    }
}
