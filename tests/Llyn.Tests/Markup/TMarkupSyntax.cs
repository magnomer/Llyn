using Llyn.Core;
using Xunit;

using LMarkupToken = Llyn.Core.LMarkup.LMarkupToken;
using LMarkupTokenKind = Llyn.Core.LMarkup.LMarkupTokenKind;
using LMarkupReference = Llyn.Core.LMarkup.LMarkupReference;

namespace Llyn.Tests;

public sealed class TMarkupSyntax
{
    [Fact]
    public void ATextTagBecomesOneTokenCarryingItsTrimmedText()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan("<meaning>  a unit of language  </meaning>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Equal(LMarkupTokenKind.LMarkupTokenText, token.LMarkupTokenKind);
        Assert.Equal("meaning", token.LMarkupTokenName);
        Assert.Equal("a unit of language", token.LMarkupTokenText);
        Assert.False(token.LMarkupTokenEmpty);
        Assert.Null(token.LMarkupTokenSource);
    }

    [Fact]
    public void AnEmptyTagCarriesTheEmptyFlagInBothOfItsForms()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan("<meaning></meaning><tag>\n  </tag><image/>");

        Assert.Equal(3, tokens.Count);
        Assert.All(tokens, token =>
        {
            Assert.True(token.LMarkupTokenEmpty);
            Assert.Equal(string.Empty, token.LMarkupTokenText);
        });
        Assert.Equal(["meaning", "tag", "image"], tokens.Select(token => token.LMarkupTokenName));
    }

    [Fact]
    public void AnAbsentSourceAnEmptySourceAndANamedSourceStayApart()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<example>plain</example><example src=\"\">unknown</example><example src=\"oed\">cited</example>");

        Assert.Equal(3, tokens.Count);
        Assert.Null(tokens[0].LMarkupTokenSource);
        Assert.Equal(string.Empty, tokens[1].LMarkupTokenSource);
        Assert.Equal("oed", tokens[2].LMarkupTokenSource);
        Assert.Equal(["plain", "unknown", "cited"], tokens.Select(token => token.LMarkupTokenText));
    }

    [Fact]
    public void TextIsTakenLiterallySoAmpersandsAndAnglesAreNeverInterpreted()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<meaning>a mark ( & ) joining two clauses, as in a &amp; b < c</meaning>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Equal("a mark ( & ) joining two clauses, as in a &amp; b < c", token.LMarkupTokenText);
    }

    [Fact]
    public void ABlockTagIsBoundedByAnEnterAndALeaveToken()
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
        Assert.Equal(["entry", "headword", "source", "year", "source", "entry"], tokens.Select(token => token.LMarkupTokenName));
        Assert.Equal("oed", tokens[2].LMarkupTokenSource);
    }

    [Fact]
    public void OrderIsKeptAcrossRepeatedTags()
    {
        IReadOnlyList<LMarkupToken> tokens = TInterface.TMarkupScan(
            "<sense><tag>literal</tag><tag></tag><tag>figurative</tag></sense>");

        Assert.Equal(["", "literal", "", "figurative", ""], tokens.Select(token => token.LMarkupTokenText));
        Assert.Equal([false, false, true, false, false], tokens.Select(token => token.LMarkupTokenEmpty));
    }

    [Fact]
    public void AnUnclosedTextTagThrowsAFormatExceptionNamingItsPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(
            () => TInterface.TMarkupScan("<entry><meaning>a unit of language</entry>"));

        Assert.Contains("meaning", failure.Message);
        Assert.Contains("7", failure.Message);
    }

    [Fact]
    public void AStrayAngleThrowsAFormatExceptionNamingItsPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupScan("<sense>< </sense>"));

        Assert.Contains("Stray", failure.Message);
        Assert.Contains("7", failure.Message);
    }

    [Fact]
    public void AnUnclosedBlockThrowsAFormatExceptionNamingItsPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(
            () => TInterface.TMarkupScan("<entry><headword>kindle</headword>"));

        Assert.Contains("entry", failure.Message);
        Assert.Contains("0", failure.Message);
    }

    [Fact]
    public void AClosingTagWithNoOpenBlockThrowsAFormatException()
    {
        Assert.Throws<FormatException>(() => TInterface.TMarkupScan("<entry></entry></sense>"));
    }

    [Fact]
    public void ACardCarriesEveryTagItDeclaresInTheOrderItDeclaresThem()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            """
            <sense>
              <title>set alight</title>
              <expression>kindle a fire</expression>
              <meaning>to set something burning</meaning>
              <synonym>ignite, light</synonym>
              <tag>literal</tag>
              <tag>old</tag>
              <example src="oed">she knelt to kindle the damp logs</example>
              <example>a second example</example>
              <situation src="">around a hearth</situation>
              <image>media/one.jpg</image>
              <image>media/two.jpg</image>
            </sense>
            """), 1);

        Assert.Equal("set alight", card.LCardDraftTitle.TStateValueShow());
        Assert.Equal("kindle a fire", card.LCardDraftExpression.TStateValueShow());
        Assert.Equal("to set something burning", card.LCardDraftMeaning.TStateValueShow());
        Assert.Equal("ignite, light", card.LCardDraftSynonym);
        Assert.Equal(["literal", "old"], card.LCardDraftTag);
        Assert.Equal(
            ["media/one.jpg", "media/two.jpg"],
            card.LCardDraftImage.Select(image => image.TStateValueShow()));
        Assert.Equal(2, card.LCardDraftExample.Count);
        Assert.Equal("she knelt to kindle the damp logs", card.LCardDraftExample[0].LExampleDraftText.TStateValueShow());
        Assert.Equal("oed", card.LCardDraftExample[0].LExampleDraftReference.TStateValueShow());
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftExample[1].LExampleDraftReference.LStateValueState);
        LSituationDraft situation = Assert.Single(card.LCardDraftSituation);
        Assert.Equal("around a hearth", situation.LSituationDraftText.TStateValueShow());
        Assert.Equal(LState.LStateUnknown, situation.LSituationDraftReference.LStateValueState);
    }

    [Fact]
    public void AnEmptyTagCarriesNoNameAndSoCarriesNoTag()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<sense><meaning></meaning><tag></tag></sense>"), 1);

        Assert.Equal(LState.LStateUnknown, card.LCardDraftMeaning.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftTitle.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftExpression.LStateValueState);
        Assert.Equal(string.Empty, card.LCardDraftSynonym);
        Assert.Empty(card.LCardDraftTag);
        Assert.Empty(card.LCardDraftExample);
        Assert.Empty(card.LCardDraftSituation);
        Assert.Empty(card.LCardDraftImage);
    }

    [Fact]
    public void AnUnrecognisedTagInsideACardIsIgnored()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<collocation><future>not a field</future><meaning>to cause interest</meaning></collocation>"), 1);

        Assert.Equal("to cause interest", card.LCardDraftMeaning.TStateValueShow());
    }

    [Fact]
    public void ASourceKeepsItsIdAndItsKnownAndUnknownFieldsApart()
    {
        LMarkupReference read = TInterface.TMarkupReferenceRead(TInterface.TMarkupScan(
            """
            <source id="field">
              <title>A Field Guide to Rivers</title>
              <author></author>
              <year>2011</year>
              <url></url>
            </source>
            """));

        Assert.Equal("field", read.LMarkupReferenceId);
        LReference reference = read.LMarkupReferenceValue;
        Assert.Equal("field", reference.LReferenceId);
        Assert.Equal("A Field Guide to Rivers", reference.LReferenceTitle.TStateValueShow());
        Assert.Equal("2011", reference.LReferenceYear.TStateValueShow());
        Assert.Equal(LState.LStateUnknown, reference.LReferenceUrl.LStateValueState);
        Assert.Equal(LState.LStateUnknown, reference.LReferenceAuthorState);
        Assert.Empty(read.LMarkupReferenceAuthor);
        Assert.Equal(LState.LStateUnspecified, reference.LReferenceProgram.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, reference.LReferenceChannel.LStateValueState);
    }

    [Fact]
    public void ASourceWithoutAnIdIsRefused()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupReferenceRead(
            TInterface.TMarkupScan("<source><title>A Field Guide to Rivers</title></source>")));

        Assert.Contains("id", failure.Message);
    }

    [Fact]
    public void ASourceWithAnEmptyIdIsRefused()
    {
        Assert.Throws<FormatException>(() => TInterface.TMarkupReferenceRead(
            TInterface.TMarkupScan("<source id=\"\"><title>A Field Guide</title></source>")));
    }

    [Fact]
    public void ASourceNestedInACardLeavesNoFieldOfItsOwnOnThatCard()
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
    public void AnIdAttributeOnAnExampleIsNotACitation()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<sense><example id=\"oed\">she knelt to kindle the damp logs</example></sense>"), 1);

        Assert.Equal(
            LState.LStateUnspecified,
            Assert.Single(card.LCardDraftExample).LExampleDraftReference.LStateValueState);
    }

    [Fact]
    public void AnEntryThatDeclaresOneSourceIdTwiceIsRefused()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            """
            <entry>
              <headword>kindle</headword>
              <source id="oed"><title>Oxford English Dictionary</title></source>
              <source id="oed"><title>A Field Guide to Rivers</title></source>
            </entry>
            """));

        Assert.Contains("oed", failure.Message);
    }

    [Fact]
    public void ASourceCarriesItsAuthorNameBesideTheReferenceThatStatesIt()
    {
        LMarkupReference read = TInterface.TMarkupReferenceRead(TInterface.TMarkupScan(
            """
            <source id="oed">
              <title>Oxford English Dictionary</title>
              <author>Murray, James</author>
              <program>Word of Mouth</program>
              <channel>Radio 4</channel>
            </source>
            """));

        Assert.Equal("oed", read.LMarkupReferenceId);
        Assert.Equal(["Murray, James"], read.LMarkupReferenceAuthor);
        Assert.Equal(LState.LStateSpecified, read.LMarkupReferenceValue.LReferenceAuthorState);
        Assert.Equal("Word of Mouth", read.LMarkupReferenceValue.LReferenceProgram.TStateValueShow());
        Assert.Equal("Radio 4", read.LMarkupReferenceValue.LReferenceChannel.TStateValueShow());
    }

    private const string TMarkupSample =
        """
        <entry>
          <headword>kindle</headword>
          <lang>English</lang>
          <ipa>/ˈkɪnd(ə)l/</ipa>
          <audio>media/kindle.mp3</audio>
          <pos>verb</pos>
          <note>Chiefly literary in its figurative senses.</note>

          <sense>
            <title>set alight</title>
            <expression>kindle a fire</expression>
            <meaning>to set something burning; to start a flame</meaning>
            <synonym>ignite, light</synonym>
            <tag>literal</tag>
            <example src="oed">she knelt to kindle the damp logs</example>
            <situation src="oed">around a hearth on a cold evening</situation>
            <image>media/kindle-hearth.jpg</image>
          </sense>

          <sense>
            <title>rouse a feeling</title>
            <meaning>to stir up an emotion or interest</meaning>
            <synonym>arouse, awaken, spark</synonym>
            <tag>figurative</tag>
            <tag></tag>
            <example>the teacher kindled a love of poetry in her class</example>
            <example src="">a remark that kindled old resentments</example>
          </sense>

          <collocation>
            <expression>kindle interest</expression>
            <meaning>to cause interest to begin</meaning>
            <example>the exhibition kindled fresh interest in the painter</example>
          </collocation>

          <source id="oed">
            <title>Oxford English Dictionary</title>
            <author>Murray, James</author>
            <year>1928</year>
            <url>https://www.oed.com/</url>
            <program></program>
            <channel></channel>
          </source>
        </entry>

        <entry>
          <headword>brook</headword>
          <lang>English</lang>
          <ipa>/brʊk/</ipa>
          <pos>noun, verb</pos>

          <sense>
            <title>a small stream</title>
            <meaning>a small natural watercourse</meaning>
            <synonym>stream, creek, rivulet</synonym>
            <tag>nature</tag>
            <example src="field">the path followed a shallow brook down the valley</example>
          </sense>

          <sense>
            <title>to tolerate</title>
            <meaning>to bear or put up with, usually in the negative</meaning>
            <tag>formal</tag>
            <tag>usually negative</tag>
            <example>she would brook no argument on the matter</example>
            <situation></situation>
          </sense>

          <source id="field">
            <title>A Field Guide to Rivers</title>
            <author></author>
            <year>2011</year>
            <url></url>
          </source>
        </entry>
        """;

    [Fact]
    public void TheCompleteSampleReadsAsTwoEntriesCarryingEveryFieldItWrites()
    {
        IReadOnlyList<LEntryDraft> entries = TInterface.TMarkupRead(TMarkupSample);

        Assert.Equal(2, entries.Count);

        LEntryDraft kindle = entries[0];
        Assert.Equal("kindle", kindle.LEntryDraftHeadword);
        Assert.Equal("English", kindle.LEntryDraftLanguage);
        Assert.Equal("/\u02c8k\u026And(\u0259)l/", kindle.LEntryDraftPronunciation);
        Assert.Equal("media/kindle.mp3", kindle.LEntryDraftAudio);
        Assert.Equal(["verb"], kindle.LEntryDraftSpeeches);
        Assert.Equal("Chiefly literary in its figurative senses.", kindle.LEntryDraftNote);
        Assert.Equal(2, kindle.LEntryDraftSenses.Count);
        Assert.Equal("kindle interest", Assert.Single(kindle.LEntryDraftCollocations).LCardDraftExpression.TStateValueShow());

        LCardDraft alight = kindle.LEntryDraftSenses[0];
        Assert.Equal("set alight", alight.LCardDraftTitle.TStateValueShow());
        Assert.Equal("ignite, light", alight.LCardDraftSynonym);
        Assert.Equal(["literal"], alight.LCardDraftTag);
        Assert.Equal(["media/kindle-hearth.jpg"], alight.LCardDraftImage.Select(image => image.TStateValueShow()));
        Assert.Equal("oed", Assert.Single(alight.LCardDraftExample).LExampleDraftReference.TStateValueShow());
        Assert.Equal("oed", Assert.Single(alight.LCardDraftSituation).LSituationDraftReference.TStateValueShow());

        LCardDraft rouse = kindle.LEntryDraftSenses[1];
        Assert.Equal(["figurative"], rouse.LCardDraftTag);
        Assert.Equal(
            [LState.LStateUnspecified, LState.LStateUnknown],
            rouse.LCardDraftExample.Select(example => example.LExampleDraftReference.LStateValueState));

        LEntryDraft brook = entries[1];
        Assert.Equal("brook", brook.LEntryDraftHeadword);
        Assert.Equal(["noun", "verb"], brook.LEntryDraftSpeeches);
        Assert.Equal(string.Empty, brook.LEntryDraftAudio);
        Assert.Equal(string.Empty, brook.LEntryDraftNote);
        Assert.Equal("field", Assert.Single(brook.LEntryDraftSenses[0].LCardDraftExample).LExampleDraftReference.TStateValueShow());
        Assert.Equal(
            LState.LStateUnknown,
            Assert.Single(brook.LEntryDraftSenses[1].LCardDraftSituation).LSituationDraftText.LStateValueState);
    }

    [Fact]
    public void ASharedSourceIsResolvedOnceAndCitedByEveryTagThatNamesIt()
    {
        LMarkup.LMarkupEntry entry = Assert.Single(TInterface.TMarkupEntryRead(
            """
            <entry>
              <headword>kindle</headword>
              <sense>
                <example src="oed">she knelt to kindle the damp logs</example>
                <situation src="oed">around a hearth on a cold evening</situation>
              </sense>
              <source id="oed">
                <title>Oxford English Dictionary</title>
                <author>Murray, James</author>
                <author>Bradley, Henry</author>
              </source>
            </entry>
            """));

        LMarkupReference source = Assert.Single(entry.LMarkupEntrySource);
        Assert.Equal("oed", source.LMarkupReferenceId);
        Assert.Equal(["Murray, James", "Bradley, Henry"], source.LMarkupReferenceAuthor);
        Assert.Equal(LState.LStateSpecified, source.LMarkupReferenceValue.LReferenceAuthorState);

        LCardDraft card = Assert.Single(entry.LMarkupEntryDraft.LEntryDraftSenses);
        Assert.Equal(
            source.LMarkupReferenceValue.LReferenceId,
            Assert.Single(card.LCardDraftExample).LExampleDraftReference.TStateValueShow());
        Assert.Equal(
            source.LMarkupReferenceValue.LReferenceId,
            Assert.Single(card.LCardDraftSituation).LSituationDraftReference.TStateValueShow());
    }

    [Fact]
    public void AnIdIsMeaningfulOnlyInsideItsOwnEntry()
    {
        IReadOnlyList<LMarkup.LMarkupEntry> entries = TInterface.TMarkupEntryRead(
            """
            <entry>
              <headword>one</headword>
              <sense><example src="key">first</example></sense>
              <source id="key"><title>First Work</title></source>
            </entry>
            <entry>
              <headword>two</headword>
              <sense><example src="key">second</example></sense>
              <source id="key"><title>Second Work</title></source>
            </entry>
            """);

        Assert.Equal(2, entries.Count);
        Assert.Equal("First Work", Assert.Single(entries[0].LMarkupEntrySource).LMarkupReferenceValue.LReferenceTitle.TStateValueShow());
        Assert.Equal("Second Work", Assert.Single(entries[1].LMarkupEntrySource).LMarkupReferenceValue.LReferenceTitle.TStateValueShow());
    }

    [Fact]
    public void AMissingHeadwordThrowsAFormatExceptionNamingTheEntryIndex()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            "<entry><headword>kindle</headword></entry><entry><lang>English</lang></entry>"));

        Assert.Contains("headword", failure.Message);
        Assert.Contains("2", failure.Message);
    }

    [Fact]
    public void AnEmptyHeadwordIsAsMissingAsAnAbsentOne()
    {
        Assert.Throws<FormatException>(() => TInterface.TMarkupRead("<entry><headword></headword></entry>"));
    }

    [Fact]
    public void AnUnknownSourceKeyThrowsAFormatExceptionNamingTheKey()
    {
        FormatException failure = Assert.Throws<FormatException>(() => TInterface.TMarkupRead(
            """
            <entry>
              <headword>brook</headword>
              <sense><example src="field">the path followed a shallow brook</example></sense>
            </entry>
            """));

        Assert.Contains("field", failure.Message);
    }
}
