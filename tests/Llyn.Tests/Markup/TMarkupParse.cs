using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TMarkupParse
{
    [Fact]
    public void MarkupParse_FullEntry_ReadsEveryField()
    {
        IReadOnlyList<LMarkupEntry> entries = TInterface.TMarkupParse(
            TMarkupSample.TMarkupSampleText, out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Empty(omissions);
        LMarkupEntry entry = Assert.Single(entries);
        Assert.Equal("kindle", entry.LMarkupEntryHeadword);
        Assert.Equal("en", entry.LMarkupEntryLanguage);
        Assert.Equal(["verb", "noun"], entry.LMarkupEntrySpeech);
        Assert.Equal("kin·dles", Assert.Single(entry.LMarkupEntryForm).LFormLocal);
        Assert.Equal(["past", "participle"], Assert.Single(entry.LMarkupEntryInflection).LMarkupInflectionMorphology);
        LPronunciationDraft pronunciation = Assert.Single(entry.LMarkupEntryPronunciation);
        Assert.Equal("ˈkɪndl̩", pronunciation.LPronunciationDraftIpa);
        Assert.Equal("ˈkɪndəl", pronunciation.LPronunciationDraftRespelling);
        Assert.Equal("ɪ", Assert.Single(pronunciation.LPronunciationDraftSyllables).LSyllableNucleus);
        Assert.Equal("KIN-dl", Assert.Single(entry.LMarkupEntryTranscription).LTranscriptionDraftText);
        LReflexDraft reflex = Assert.Single(entry.LMarkupEntryReflex);
        Assert.Equal("[ʈ͡ʂɤŋ²¹⁴]", reflex.LReflexDraftText);
        Assert.Equal("/tʂəŋ²¹⁴/", reflex.LReflexDraftRespelling);
        Assert.True(reflex.LReflexDraftMain);
        Assert.Equal("Chiefly *literary*.", entry.LMarkupEntryNote);

        LMarkupCard meaning = Assert.Single(entry.LMarkupEntryMeaning);
        Assert.Equal("set something alight", meaning.LMarkupCardMeaning.LStateValueText);
        Assert.Equal("arouse a feeling", Assert.Single(meaning.LMarkupCardChild).LMarkupCardMeaning.LStateValueText);
        Assert.Equal("literary", Assert.Single(meaning.LMarkupCardRegister).LRegisterDraftName.LStateValueText);
        Assert.Equal("불붙이다", Assert.Single(meaning.LMarkupCardTranslation).LMarkupTranslationHeadword);
        Assert.Equal("fire", Assert.Single(meaning.LMarkupCardTag).LTagDraftText);
        Assert.Equal("00:10-00:20", Assert.Single(meaning.LMarkupCardVideo).LVideoDraftSpan.LStateValueText);

        LMarkupSentence sentence = Assert.Single(meaning.LMarkupCardSentence);
        Assert.Equal("with", sentence.LMarkupSentenceParticle.LStateValueText);
        Assert.NotNull(sentence.LMarkupSentenceExample);
        LMarkupExample example = sentence.LMarkupSentenceExample;
        Assert.Equal("ko", Assert.Single(example.LMarkupExampleGloss).LGlossDraftLanguage);
        LMarkupMention mention = Assert.Single(example.LMarkupExampleMention);
        Assert.Equal(
            (4, 7, "1"),
            (mention.LMarkupMentionOffset, mention.LMarkupMentionLength, mention.LMarkupMentionSense));
        Assert.NotNull(example.LMarkupExampleReference);
        Assert.Equal(LReferenceKind.LReferenceKindBook, example.LMarkupExampleReference.LMarkupReferenceKind);
        Assert.Equal(["Ada Ember", "Rhys Coal"], example.LMarkupExampleReference.LMarkupReferenceAuthor);

        LMarkupCard collocation = Assert.Single(entry.LMarkupEntryCollocation);
        Assert.Equal("kindle someone's interest", collocation.LMarkupCardExpression.LStateValueText);
    }

    [Fact]
    public void MarkupParse_UnknownElement_ReportsOmission()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <etymology>Old English</etymology>
              </entry>
            </llyn>
            """;

        IReadOnlyList<LMarkupEntry> entries = TInterface.TMarkupParse(
            text, out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Equal("ember", Assert.Single(entries).LMarkupEntryHeadword);
        LMarkupOmission omission = Assert.Single(omissions);
        Assert.Equal((4, "<etymology>"), (omission.LMarkupOmissionLine, omission.LMarkupOmissionText));
    }

    [Fact]
    public void MarkupParse_IdAttribute_ReportsOmission()
    {
        const string text = """
            <llyn>
              <entry id="42">
                <headword>ember</headword>
              </entry>
            </llyn>
            """;

        TInterface.TMarkupParse(text, out IReadOnlyList<LMarkupOmission> omissions);

        LMarkupOmission omission = Assert.Single(omissions);
        Assert.Equal((2, "id=\"42\""), (omission.LMarkupOmissionLine, omission.LMarkupOmissionText));
    }

    [Theory]
    [InlineData("<llyn><entry><headword>ember</entry></llyn>")]
    [InlineData("<lexicon><entry><headword>ember</headword></entry></lexicon>")]
    [InlineData("ember")]
    public void MarkupParse_MalformedText_Refuses(string text)
    {
        LRefusal refusal = Assert.Throws<LRefusal>(() => TInterface.TMarkupParse(text));

        Assert.Equal(LRefusal.LRefusalMarkup, refusal.LRefusalReason);
    }

    [Fact]
    public void MarkupParse_UnknownState_ReadsUnknown()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <meaning>
                  <title state="unknown" />
                  <definition></definition>
                </meaning>
              </entry>
            </llyn>
            """;

        LMarkupCard card = Assert.Single(TInterface.TMarkupParse(text).Single().LMarkupEntryMeaning);

        Assert.Equal(LState.LStateUnknown, card.LMarkupCardTitle.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LMarkupCardMeaning.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LMarkupCardExpression.LStateValueState);
    }

    [Fact]
    public void MarkupParse_NestedCollocation_ReportsOmission()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <collocation>
                  <title>glowing ember</title>
                  <meaning>
                    <title>nested</title>
                  </meaning>
                </collocation>
              </entry>
            </llyn>
            """;

        IReadOnlyList<LMarkupEntry> entries = TInterface.TMarkupParse(
            text, out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Empty(Assert.Single(Assert.Single(entries).LMarkupEntryCollocation).LMarkupCardChild);
        Assert.Equal("<meaning>", Assert.Single(omissions).LMarkupOmissionText);
    }

    [Fact]
    public void MarkupParse_OtherStateValue_ReportsOmission()
    {
        const string text = """
            <llyn>
              <entry>
                <headword>ember</headword>
                <meaning>
                  <title state="missing">glow</title>
                </meaning>
              </entry>
            </llyn>
            """;

        LMarkupEntry entry = Assert.Single(TInterface.TMarkupParse(text, out IReadOnlyList<LMarkupOmission> omissions));

        Assert.Equal("glow", Assert.Single(entry.LMarkupEntryMeaning).LMarkupCardTitle.LStateValueText);
        Assert.Contains("state=\"missing\"", Assert.Single(omissions).LMarkupOmissionText);
        Assert.Equal(2, entry.LMarkupEntryLine);
    }

    [Fact]
    public void MarkupParse_DeepLeafNesting_RefusesMarkup()
    {
        const int depth = 200_000;
        string text = "<llyn><entry><headword>"
            + string.Concat(Enumerable.Repeat("<a>", depth))
            + string.Concat(Enumerable.Repeat("</a>", depth))
            + "</headword></entry></llyn>";

        LRefusal refusal = Assert.Throws<LRefusal>(() => TInterface.TMarkupParse(text));

        Assert.Equal(LRefusal.LRefusalMarkup, refusal.LRefusalReason);
    }

    [Fact]
    public void MarkupParse_NestingAtCeiling_Reads()
    {
        int depth = LMarkup.LMarkupDepthCeiling - 3;
        string text = "<llyn><entry><headword>"
            + string.Concat(Enumerable.Repeat("<a>", depth))
            + "ember"
            + string.Concat(Enumerable.Repeat("</a>", depth))
            + "</headword></entry></llyn>";

        LMarkupEntry entry = Assert.Single(TInterface.TMarkupParse(text));

        Assert.Equal("ember", entry.LMarkupEntryHeadword);
    }

    [Fact]
    public void MarkupParse_FloodOfAttributes_CapsOmissions()
    {
        int count = LMarkup.LMarkupOmissionCeiling * 2;
        string text = "<llyn><entry><headword>ember</headword>"
            + string.Concat(Enumerable.Repeat("<note id=\"1\"/>", count))
            + "</entry></llyn>";

        TInterface.TMarkupParse(text, out IReadOnlyList<LMarkupOmission> omissions);

        Assert.Equal(LMarkup.LMarkupOmissionCeiling + 1, omissions.Count);
        Assert.Equal("...", omissions[^1].LMarkupOmissionText);
    }
}
