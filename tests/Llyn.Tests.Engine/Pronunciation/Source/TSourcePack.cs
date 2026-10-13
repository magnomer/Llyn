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
}
