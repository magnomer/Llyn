using Llyn.Core;
using Xunit;

using LMarkupToken = Llyn.Core.LMarkup.LMarkupToken;
using LMarkupTokenKind = Llyn.Core.LMarkup.LMarkupTokenKind;
using LMarkupReference = Llyn.Core.LMarkup.LMarkupReference;

namespace Llyn.Tests;

public sealed class TMarkupSyntax
{
    [Fact]
    public void MarkupScan_TextTag_ReturnsTokenWithTrimmedText()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan("<meaning>  a unit of language  </meaning>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Equal(LMarkupTokenKind.LMarkupTokenText, token.LMarkupTokenKind);
        Assert.Equal("meaning", token.LMarkupTokenName);
        Assert.Equal("a unit of language", token.LMarkupTokenText);
        Assert.False(token.LMarkupTokenEmpty);
        Assert.Null(TInterface.TMarkupTokenRead(token, "src"));
    }

    [Fact]
    public void MarkupScan_EmptyTagBothForms_CarriesEmptyFlag()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan("<meaning></meaning><tag>\n  </tag><use/>");

        Assert.Equal(3, tokens.Count);
        Assert.All(tokens, token =>
        {
            Assert.True(token.LMarkupTokenEmpty);
            Assert.Equal(string.Empty, token.LMarkupTokenText);
        });
        Assert.Equal(["meaning", "tag", "use"], tokens.Select(token => token.LMarkupTokenName));
    }

    [Fact]
    public void MarkupScan_ThreeReferenceForms_KeepsThemApart()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<use/><use ref=\"\"/><use ref=\"ex1\"/>");

        Assert.Equal(3, tokens.Count);
        Assert.Null(TInterface.TMarkupTokenRead(tokens[0], "ref"));
        Assert.Equal(string.Empty, TInterface.TMarkupTokenRead(tokens[1], "ref"));
        Assert.Equal("ex1", TInterface.TMarkupTokenRead(tokens[2], "ref"));
    }

    [Fact]
    public void MarkupScan_AmpersandsAndAngles_TakesLiterally()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<meaning>a mark ( & ) joining two clauses, as in a &amp; b < c</meaning>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Equal("a mark ( & ) joining two clauses, as in a &amp; b < c", token.LMarkupTokenText);
    }

    [Fact]
    public void MarkupScan_BlockTag_BoundsWithEnterAndLeave()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<entry><headword>kindle</headword><source id=\"oed\"><year>1928</year></source></entry>");

        Assert.Equal(
            [
                LMarkupTokenKind.LMarkupTokenEnter,
                LMarkupTokenKind.LMarkupTokenText,
                LMarkupTokenKind.LMarkupTokenEnter,
                LMarkupTokenKind.LMarkupTokenText,
                LMarkupTokenKind.LMarkupTokenLeave,
                LMarkupTokenKind.LMarkupTokenLeave,
            ],
            tokens.Select(token => token.LMarkupTokenKind));
        Assert.Equal(
            ["entry", "headword", "source", "year", "source", "entry"],
            tokens.Select(token => token.LMarkupTokenName));
        Assert.Equal("oed", TInterface.TMarkupTokenRead(tokens[2], "id"));
    }

    [Fact]
    public void MarkupScan_NestedSense_BoundsTwoBlocks()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            """
            <sense id="alight">
              <sense id="figurative"><meaning>to rouse a feeling</meaning></sense>
            </sense>
            """);

        Assert.Equal(
            [
                LMarkupTokenKind.LMarkupTokenEnter,
                LMarkupTokenKind.LMarkupTokenEnter,
                LMarkupTokenKind.LMarkupTokenText,
                LMarkupTokenKind.LMarkupTokenLeave,
                LMarkupTokenKind.LMarkupTokenLeave,
            ],
            tokens.Select(token => token.LMarkupTokenKind));
        Assert.Equal("alight", TInterface.TMarkupTokenRead(tokens[0], "id"));
        Assert.Equal("figurative", TInterface.TMarkupTokenRead(tokens[1], "id"));
    }

    [Fact]
    public void MarkupScan_RepeatedTags_KeepsOrder()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<sense><tag>literal</tag><tag></tag><tag>figurative</tag></sense>");

        Assert.Equal(["", "literal", "", "figurative", ""], tokens.Select(token => token.LMarkupTokenText));
        Assert.Equal([false, false, true, false, false], tokens.Select(token => token.LMarkupTokenEmpty));
    }

    [Fact]
    public void MarkupScan_UnknownAttribute_DropsIt()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<use mood=\"wry\" par=\"for\" dep=\"Agent\"/>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Null(TInterface.TMarkupTokenRead(token, "mood"));
        Assert.Equal("for", TInterface.TMarkupTokenRead(token, "par"));
        Assert.Equal("Agent", TInterface.TMarkupTokenRead(token, "dep"));
    }

    [Fact]
    public void MarkupScan_SyllableAttributes_KeepsEveryOne()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<syllable onset=\"k\" nucleus=\"ɪ\" coda=\"n\" orthography=\"kin\" local=\"킨\" tone-points=\"55\"/>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Equal("k", TInterface.TMarkupTokenRead(token, "onset"));
        Assert.Equal("ɪ", TInterface.TMarkupTokenRead(token, "nucleus"));
        Assert.Equal("n", TInterface.TMarkupTokenRead(token, "coda"));
        Assert.Equal("kin", TInterface.TMarkupTokenRead(token, "orthography"));
        Assert.Equal("킨", TInterface.TMarkupTokenRead(token, "local"));
        Assert.Equal("55", TInterface.TMarkupTokenRead(token, "tone-points"));
    }

    [Fact]
    public void MarkupScan_UnclosedTextTag_ThrowsNamingPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(
            () => TInterface.TMarkupScan("<entry><meaning>a unit of language</entry>"));

        Assert.Contains("meaning", failure.Message);
        Assert.Contains("7", failure.Message);
    }

    [Fact]
    public void MarkupScan_StrayAngle_ThrowsNamingPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupScan("<sense>< </sense>"));

        Assert.Contains("Stray", failure.Message);
        Assert.Contains("7", failure.Message);
    }

    [Fact]
    public void MarkupScan_UnclosedBlock_ThrowsNamingPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(
            () => TInterface.TMarkupScan("<entry><headword>kindle</headword>"));

        Assert.Contains("entry", failure.Message);
        Assert.Contains("0", failure.Message);
    }

    [Fact]
    public void MarkupScan_ClosingTagNoOpenBlock_Throws()
    {
        Assert.Throws<FormatException>(() => TInterface.TMarkupScan("<entry></entry></sense>"));
    }

    [Fact]
    public void MarkupCardRead_DeclaredTags_ReturnsDeclaredOrder()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            """
            <sense>
              <title>set alight</title>
              <gloss>ignite</gloss>
              <meaning lang="English">to set something burning</meaning>
              <labels>literal</labels>
              <tag>literal</tag>
              <tag> old </tag>
              <tag>literal</tag>
            </sense>
            """), 1);

        Assert.Equal("set alight", card.LCardDraftTitle.TStateValueShow());
        Assert.Equal("ignite", card.LCardDraftGloss);
        Assert.Equal("to set something burning", card.LCardDraftMeaning.TStateValueShow());
        Assert.Equal("English", card.LCardDraftLanguage);
        Assert.Equal("literal", card.LCardDraftLabels);
        Assert.Equal(["literal", "old"], card.LCardDraftTag);
    }

    [Fact]
    public void MarkupCardRead_EmptyTag_ReturnsNoTag()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<sense><meaning></meaning><gloss></gloss><tag></tag></sense>"), 1);

        Assert.Equal(LState.LStateUnknown, card.LCardDraftMeaning.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftTitle.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftExpression.LStateValueState);
        Assert.Null(card.LCardDraftGloss);
        Assert.Empty(card.LCardDraftTag);
        Assert.Empty(card.LCardDraftImage);
    }

    [Fact]
    public void MarkupCardRead_NestedSense_KeepsTheChildOrder()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            """
            <sense>
              <meaning>to set alight</meaning>
              <sense><meaning>to rouse a feeling</meaning></sense>
              <sense><meaning>to bear young</meaning></sense>
            </sense>
            """), 1);

        Assert.Equal(
            ["to rouse a feeling", "to bear young"],
            card.LCardDraftChild.Select(child => child.LCardDraftMeaning.TStateValueShow()));
        Assert.Equal([1, 2], card.LCardDraftChild.Select(child => child.LCardDraftPosition));
    }

    [Fact]
    public void MarkupCardRead_FrameWithoutExample_ReturnsTheUse()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<sense><use par=\"for\" dep=\"Agent\"/><use par=\"\"/></sense>"), 1);

        Assert.Equal(2, card.LCardDraftSentence.Count);
        Assert.Null(card.LCardDraftSentence[0].LSentenceDraftExample);
        Assert.Equal("for", card.LCardDraftSentence[0].LSentenceDraftParticle.TStateValueShow());
        Assert.Equal("Agent", card.LCardDraftSentence[0].LSentenceDraftDependence.TStateValueShow());
        Assert.Equal(
            LState.LStateUnknown,
            card.LCardDraftSentence[1].LSentenceDraftParticle.LStateValueState);
    }

    [Fact]
    public void MarkupRead_EntryDetail_ReturnsEveryField()
    {
        LEntryDraft entry = Assert.Single(TInterface.TMarkupRead(
            """
            <llyn>
              <entry>
                <headword>kindle</headword>
                <form role="past" local="과거형">kindled</form>
                <form role="participle">kindling</form>
                <pos id="verb"/>
                <pos>서술어</pos>
                <inflection local="3인칭 단수" pos="verb">
                  <text>kindles</text>
                  <feature id="person" value="third"/>
                  <feature id="number" value="singular"/>
                </inflection>
                <pronunciation level="standard">
                  <ipa>/ˈkɪnd(ə)l/</ipa>
                  <syllable onset="k" nucleus="ɪ" coda="n" orthography="kin" local="킨" tone="1"/>
                  <representation system="revised" role="transcription" tone="평성">kindeul</representation>
                  <audio source="recorded by the author">media/kindle.mp3</audio>
                </pronunciation>
              </entry>
            </llyn>
            """));

        Assert.Equal(["kindled", "kindling"], entry.LEntryDraftForms.Select(form => form.LFormText));
        Assert.Equal(["past", "participle"], entry.LEntryDraftForms.Select(form => form.LFormRole));
        Assert.Equal("과거형", entry.LEntryDraftForms[0].LFormLocal);
        Assert.Null(entry.LEntryDraftForms[1].LFormLocal);

        Assert.Equal("verb", entry.LEntryDraftSpeeches[0].LSpeechDraftValue);
        Assert.Null(entry.LEntryDraftSpeeches[0].LSpeechDraftCustom);
        Assert.Null(entry.LEntryDraftSpeeches[1].LSpeechDraftValue);
        Assert.Equal("서술어", entry.LEntryDraftSpeeches[1].LSpeechDraftCustom);

        LInflection inflection = Assert.Single(entry.LEntryDraftInflections);
        Assert.Equal("kindles", inflection.LInflectionText);
        Assert.Equal("3인칭 단수", inflection.LInflectionLocal);
        Assert.Equal("verb", inflection.LInflectionSpeechId);
        Assert.Equal(
            ["person", "number"], inflection.LInflectionFeatures.Select(feature => feature.LFeatureId));
        Assert.Equal(
            ["third", "singular"],
            inflection.LInflectionFeatures.Select(feature => feature.LFeatureValueId));

        Assert.NotNull(entry.LEntryDraftPronunciation);
        LPronunciationDraft spoken = entry.LEntryDraftPronunciation;
        Assert.Equal("standard", spoken.LPronunciationDraftLevel);
        Assert.Equal("/ˈkɪnd(ə)l/", spoken.LPronunciationDraftIpa);
        Assert.Equal("media/kindle.mp3", spoken.LPronunciationDraftAudio);
        Assert.Equal("recorded by the author", spoken.LPronunciationDraftSource);

        LSyllable syllable = Assert.Single(spoken.LPronunciationDraftSyllables);
        Assert.Equal("k", syllable.LSyllableOnset);
        Assert.Equal("ɪ", syllable.LSyllableNucleus);
        Assert.Equal("n", syllable.LSyllableCoda);
        Assert.Equal("kin", syllable.LSyllableOrthography);
        Assert.Equal("킨", syllable.LSyllableLocal);
        Assert.Equal(1, syllable.LSyllableToneNumber);

        LRepresentation representation = Assert.Single(spoken.LPronunciationDraftRepresentations);
        Assert.Equal("revised", representation.LRepresentationSystem);
        Assert.Equal("transcription", representation.LRepresentationRole);
        Assert.Equal("kindeul", representation.LRepresentationText);
        Assert.Equal("평성", representation.LRepresentationLocalTone);
    }

    [Fact]
    public void MarkupRead_SpeechCarryingIdAndName_Throws()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            "<llyn><entry><headword>kindle</headword><pos id=\"verb\">서술어</pos></entry></llyn>"));

        Assert.Contains("<pos>", failure.Message);
    }

    [Fact]
    public void MarkupRead_SpeechCarryingNeither_Throws()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            "<llyn><entry><headword>kindle</headword><pos/></entry></llyn>"));

        Assert.Contains("<pos>", failure.Message);
    }

    [Fact]
    public void MarkupRead_SyllableWithoutNucleus_Throws()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            "<llyn><entry><headword>kindle</headword>"
            + "<pronunciation><syllable onset=\"k\"/></pronunciation></entry></llyn>"));

        Assert.Contains("<syllable>", failure.Message);
    }

    [Fact]
    public void MarkupRead_SyllablesWithoutIpa_KeepsTheSyllables()
    {
        LEntryDraft entry = Assert.Single(TInterface.TMarkupRead(
            "<llyn><entry><headword>kindle</headword>"
            + "<pronunciation><syllable nucleus=\"ɪ\"/></pronunciation></entry></llyn>"));

        Assert.NotNull(entry.LEntryDraftPronunciation);
        LPronunciationDraft spoken = entry.LEntryDraftPronunciation;
        Assert.Equal(string.Empty, spoken.LPronunciationDraftIpa);
        Assert.Equal("ɪ", Assert.Single(spoken.LPronunciationDraftSyllables).LSyllableNucleus);
    }

    [Fact]
    public void MarkupRead_IpaWithoutSyllables_KeepsTheReading()
    {
        LEntryDraft entry = Assert.Single(TInterface.TMarkupRead(
            "<llyn><entry><headword>kindle</headword>"
            + "<pronunciation><ipa>/ˈkɪnd(ə)l/</ipa></pronunciation></entry></llyn>"));

        Assert.NotNull(entry.LEntryDraftPronunciation);
        LPronunciationDraft spoken = entry.LEntryDraftPronunciation;
        Assert.Equal("/ˈkɪnd(ə)l/", spoken.LPronunciationDraftIpa);
        Assert.Empty(spoken.LPronunciationDraftSyllables);
    }

    [Fact]
    public void MarkupCardRead_UseWithoutReferenceOrFrame_Throws()
    {
        FormatException failure = Assert.Throws<FormatException>(() =>
            TInterface.TMarkupCardRead(TInterface.TMarkupScan("<sense><use/></sense>"), 1));

        Assert.Contains("Use 1", failure.Message);
    }

    [Fact]
    public void MarkupCardRead_UnreadableReference_QuotesAnUnreadableExample()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<sense><use ref=\"\"/></sense>"), 1);

        LSentenceDraft use = Assert.Single(card.LCardDraftSentence);
        Assert.NotNull(use.LSentenceDraftExample);
        Assert.Equal(
            LState.LStateUnknown, use.LSentenceDraftExample.LExampleDraftText.LStateValueState);
        Assert.False(use.LSentenceDraftEmpty);
    }

    [Fact]
    public void MarkupCardRead_UnrecognisedTag_IgnoresIt()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<collocation><future>not a field</future><meaning>to cause interest</meaning></collocation>"), 1);

        Assert.Equal("to cause interest", card.LCardDraftMeaning.TStateValueShow());
    }

    [Fact]
    public void MarkupCardRead_NestedSource_LeavesNoFieldOnCard()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            """
            <sense>
              <meaning>to set something burning</meaning>
              <source id="oed"><title>Oxford English Dictionary</title></source>
            </sense>
            """), 1);

        Assert.Equal("to set something burning", card.LCardDraftMeaning.TStateValueShow());
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftTitle.LStateValueState);
    }

    [Fact]
    public void MarkupReferenceRead_MixedFields_KeepsThemApart()
    {
        LMarkupReference read = TInterface.TMarkupReferenceRead(TInterface.TMarkupScan(
            """
            <source id="field">
              <title>A Field Guide to Rivers</title>
              <author/>
              <year>2011</year>
              <url></url>
            </source>
            """));

        Assert.Equal("field", read.LMarkupReferenceId);
        LReference reference = read.LMarkupReferenceValue;
        Assert.Equal("A Field Guide to Rivers", reference.LReferenceTitle.TStateValueShow());
        Assert.Equal("2011", reference.LReferenceYear.TStateValueShow());
        Assert.Equal(LState.LStateUnknown, reference.LReferenceUrl.LStateValueState);
        Assert.Equal(LState.LStateUnknown, reference.LReferenceAuthorState);
        Assert.Empty(read.LMarkupReferenceAuthor);
        Assert.Equal(LReferenceKind.LReferenceKindUnspecified, reference.LReferenceKind);
        Assert.Equal(LState.LStateUnspecified, reference.LReferenceNote.LStateValueState);
    }

    [Fact]
    public void MarkupReferenceRead_SourceWithoutId_Throws()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupReferenceRead(
            TInterface.TMarkupScan("<source><title>A Field Guide to Rivers</title></source>")));

        Assert.Contains("id", failure.Message);
    }

    [Fact]
    public void MarkupReferenceRead_SourceWithEmptyId_Throws()
    {
        Assert.Throws<FormatException>(() => TInterface.TMarkupReferenceRead(
            TInterface.TMarkupScan("<source id=\"\"><title>A Field Guide</title></source>")));
    }

    [Fact]
    public void MarkupReferenceRead_CitedAuthors_KeepsCreditOrder()
    {
        LMarkupReference read = TInterface.TMarkupReferenceRead(TInterface.TMarkupScan(
            """
            <source id="oed">
              <title>Oxford English Dictionary</title>
              <author ref="murray"/>
              <author ref="bradley"/>
              <kind>video</kind>
              <note>Word of Mouth, Radio 4</note>
            </source>
            """));

        Assert.Equal("oed", read.LMarkupReferenceId);
        Assert.Equal(["murray", "bradley"], read.LMarkupReferenceAuthor);
        Assert.Equal(LState.LStateSpecified, read.LMarkupReferenceValue.LReferenceAuthorState);
        Assert.Equal(LReferenceKind.LReferenceKindVideo, read.LMarkupReferenceValue.LReferenceKind);
        Assert.Equal(
            "Word of Mouth, Radio 4",
            read.LMarkupReferenceValue.LReferenceNote.TStateValueShow());
    }
}
