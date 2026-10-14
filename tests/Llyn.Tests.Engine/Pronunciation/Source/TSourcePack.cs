using System.Net;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TSourcePack
{
    private const string TSourcePackKorean =
        "<h2 data-mw-wikitext=\"\" id=\"Korean\">Korean</h2>" +
        "<a rel=\"mw:WikiLink\" href=\"./Wiktionary:International_Phonetic_Alphabet\">IPA</a>" +
        "<sup>(<a rel=\"mw:WikiLink\" href=\"./Appendix:Korean_pronunciation\" " +
        "title=\"Appendix:Korean pronunciation\">key</a>)</sup>: " +
        "<span class=\"IPA\">[ˈsʰa̠(ː)ɾa̠m]<span class=\"mw-empty-elt\">" +
        "<link rel=\"mw:PageProp/Category\" href=\"./Category:Korean\"/></span></span>";

    private const string TSourcePackSpanish =
        "<h2 data-mw-wikitext=\"\" id=\"Spanish\">Spanish</h2>" +
        "<a rel=\"mw:WikiLink\" href=\"./Wiktionary:International_Phonetic_Alphabet\">IPA</a>" +
        "<sup>(<a rel=\"mw:WikiLink\" href=\"./Appendix:Spanish_pronunciation\" " +
        "title=\"Appendix:Spanish pronunciation\">key</a>)</sup>:<span typeof=\"mw:Entity\"> </span>" +
        "<span class=\"IPA nowrap\">/ˈaɡwa/</span> <span class=\"IPA nowrap\">[ˈa.ɣ̞wa]</span>";

    private const string TSourcePackTraditional =
        "{\"parse\":{\"title\":\"\\u8d77\\u5e8a\",\"pageid\":166734,\"wikitext\":{\"*\":\"==Chinese==\\n" +
        "{{zh-forms}}\\n\\n===Pronunciation===\\n{{zh-pron\\n|m=q\\u01d0chu\\u00e1ng\\n|c=hei2 cong4\\n" +
        "|cat=v\\n}}\\n\\n===Verb===\\n{{head|zh|verb}}\\n\\n# to [[get up]]; to [[get out of bed]]\"}}}";

    private const string TSourcePackSimplified =
        "{\"parse\":{\"title\":\"\\u540e\\u6765\",\"pageid\":166791,\"wikitext\":{\"*\":\"==Chinese==\\n" +
        "{{zh-see|\\u5f8c\\u4f86}}\"}}}";

    private const string TSourcePackMissing =
        "{\"error\":{\"code\":\"missingtitle\",\"info\":\"The page you specified doesn't exist.\"," +
        "\"*\":\"See https://en.wiktionary.org/w/api.php for API usage.\"}}";

    [Theory]
    [InlineData("Korean", "사람", TSourcePackKorean, "ˈsʰa̠(ː)ɾa̠m")]
    [InlineData("Spanish", "agua", TSourcePackSpanish, "ˈaɡwa")]
    public async Task SourceFind_RestHeading_ReadsPackIpa(string language, string word, string body, string ipa)
    {
        LSourceSpec spec = TInterface.TLanguageLoad(language).LLanguageLookupSources[0];
        LSource source = TInterfaceSource.TSourceGenericCreate(
            spec, TPronunciationHelper.TSourceClientCreate(body, HttpStatusCode.OK));

        LAnswer answer = await source.TSourceFind(word, CancellationToken.None);

        Assert.Equal(ipa, Assert.Single(answer.LAnswerReadings).LReadingPhonetic);
    }

    [Theory]
    [InlineData("Mandarin", "起床", TSourcePackTraditional, "起床")]
    [InlineData("Cantonese", "起床", TSourcePackTraditional, "起床")]
    [InlineData("Mandarin", "后来", TSourcePackSimplified, "後來")]
    [InlineData("Cantonese", "后来", TSourcePackSimplified, "後來")]
    public async Task SourceFind_ChinesePage_ReturnsTraditionalForm(
        string language, string word, string body, string traditional)
    {
        LSourceSpec spec = TInterface.TLanguageLoad(language).LLanguageGlyph!.LGlyphSources[0];
        LSource source = TInterfaceSource.TSourceGenericCreate(
            spec, TPronunciationHelper.TSourceClientCreate(body, HttpStatusCode.OK));

        LAnswer answer = await source.TSourceFind(word, CancellationToken.None);

        Assert.Equal(traditional, Assert.Single(answer.LAnswerReadings).LReadingPhonetic);
    }

    [Theory]
    [InlineData("Mandarin")]
    [InlineData("Cantonese")]
    public async Task SourceFind_MissingChinesePage_ReturnsBlank(string language)
    {
        LSourceSpec spec = TInterface.TLanguageLoad(language).LLanguageGlyph!.LGlyphSources[0];
        LSource source = TInterfaceSource.TSourceGenericCreate(
            spec, TPronunciationHelper.TSourceClientCreate(TSourcePackMissing, HttpStatusCode.OK));

        LAnswer answer = await source.TSourceFind("起牀床", CancellationToken.None);

        Assert.Empty(answer.LAnswerReadings);
        Assert.True(answer.LAnswerReached);
    }
}
