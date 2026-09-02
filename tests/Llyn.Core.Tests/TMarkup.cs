using Llyn.Core;
using Xunit;

using LMarkupToken = Llyn.Core.LMarkup.LMarkupToken;
using LMarkupTokenKind = Llyn.Core.LMarkup.LMarkupTokenKind;
using LMarkupReference = Llyn.Core.LMarkup.LMarkupReference;

namespace Llyn.Core.Tests;

public sealed class TMarkup
{
    [Fact]
    public void ATextTagBecomesOneTokenCarryingItsTrimmedText()
    {
        IReadOnlyList<LMarkupToken> tokens = LMarkup.LMarkupScan("<meaning>  a unit of language  </meaning>");

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
        IReadOnlyList<LMarkupToken> tokens = LMarkup.LMarkupScan("<meaning></meaning><tag>\n  </tag><image/>");

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
        IReadOnlyList<LMarkupToken> tokens = LMarkup.LMarkupScan(
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
        IReadOnlyList<LMarkupToken> tokens = LMarkup.LMarkupScan(
            "<meaning>a mark ( & ) joining two clauses, as in a &amp; b < c</meaning>");

        LMarkupToken token = Assert.Single(tokens);
        Assert.Equal("a mark ( & ) joining two clauses, as in a &amp; b < c", token.LMarkupTokenText);
    }

    [Fact]
    public void ABlockTagIsBoundedByAnEnterAndALeaveToken()
    {
        IReadOnlyList<LMarkupToken> tokens = LMarkup.LMarkupScan(
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
        IReadOnlyList<LMarkupToken> tokens = LMarkup.LMarkupScan(
            "<sense><tag>literal</tag><tag></tag><tag>figurative</tag></sense>");

        Assert.Equal(["", "literal", "", "figurative", ""], tokens.Select(token => token.LMarkupTokenText));
        Assert.Equal([false, false, true, false, false], tokens.Select(token => token.LMarkupTokenEmpty));
    }

    [Fact]
    public void AnUnclosedTextTagThrowsAFormatExceptionNamingItsPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(
            () => LMarkup.LMarkupScan("<entry><meaning>a unit of language</entry>"));

        Assert.Contains("meaning", failure.Message);
        Assert.Contains("7", failure.Message);
    }

    [Fact]
    public void AStrayAngleThrowsAFormatExceptionNamingItsPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(() => LMarkup.LMarkupScan("<sense>< </sense>"));

        Assert.Contains("Stray", failure.Message);
        Assert.Contains("7", failure.Message);
    }

    [Fact]
    public void AnUnclosedBlockThrowsAFormatExceptionNamingItsPosition()
    {
        FormatException failure = Assert.Throws<FormatException>(
            () => LMarkup.LMarkupScan("<entry><headword>kindle</headword>"));

        Assert.Contains("entry", failure.Message);
        Assert.Contains("0", failure.Message);
    }

    [Fact]
    public void AClosingTagWithNoOpenBlockThrowsAFormatException()
    {
        Assert.Throws<FormatException>(() => LMarkup.LMarkupScan("<entry></entry></sense>"));
    }

    [Fact]
    public void ACardCarriesEveryTagItDeclaresInTheOrderItDeclaresThem()
    {
        LCardDraft card = LMarkup.LMarkupCardRead(LMarkup.LMarkupScan(
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
            """));

        Assert.Equal("set alight", card.LCardDraftTitle.LStateValueShow());
        Assert.Equal("kindle a fire", card.LCardDraftExpression.LStateValueShow());
        Assert.Equal("to set something burning", card.LCardDraftMeaning.LStateValueShow());
        Assert.Equal("ignite, light", card.LCardDraftSynonym);
        Assert.Equal(["literal", "old"], card.LCardDraftTag.Select(tag => tag.LStateValueShow()));
        Assert.Equal(
            ["media/one.jpg", "media/two.jpg"],
            card.LCardDraftImage.Select(image => image.LStateValueShow()));
        Assert.Equal(2, card.LCardDraftExample.Count);
        Assert.Equal("she knelt to kindle the damp logs", card.LCardDraftExample[0].LExampleDraftText.LStateValueShow());
        Assert.Equal("oed", card.LCardDraftExample[0].LExampleDraftReference.LStateValueShow());
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftExample[1].LExampleDraftReference.LStateValueState);
        LSituationDraft situation = Assert.Single(card.LCardDraftSituation);
        Assert.Equal("around a hearth", situation.LSituationDraftText.LStateValueShow());
        Assert.Equal(LState.LStateUnknown, situation.LSituationDraftReference.LStateValueState);
    }

    [Fact]
    public void AnEmptyTagIsUnknownAndAnAbsentTagIsUnspecified()
    {
        LCardDraft card = LMarkup.LMarkupCardRead(LMarkup.LMarkupScan(
            "<sense><meaning></meaning><tag></tag></sense>"));

        Assert.Equal(LState.LStateUnknown, card.LCardDraftMeaning.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftTitle.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftExpression.LStateValueState);
        Assert.Equal(string.Empty, card.LCardDraftSynonym);
        Assert.Equal(LState.LStateUnknown, Assert.Single(card.LCardDraftTag).LStateValueState);
        Assert.Empty(card.LCardDraftExample);
        Assert.Empty(card.LCardDraftSituation);
        Assert.Empty(card.LCardDraftImage);
    }

    [Fact]
    public void AnUnrecognisedTagInsideACardIsIgnored()
    {
        LCardDraft card = LMarkup.LMarkupCardRead(LMarkup.LMarkupScan(
            "<collocation><future>not a field</future><meaning>to cause interest</meaning></collocation>"));

        Assert.Equal("to cause interest", card.LCardDraftMeaning.LStateValueShow());
    }

    [Fact]
    public void ASourceKeepsItsIdAndItsKnownAndUnknownFieldsApart()
    {
        LMarkupReference read = LMarkup.LMarkupReferenceRead(LMarkup.LMarkupScan(
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
        Assert.Equal("A Field Guide to Rivers", reference.LReferenceTitle.LStateValueShow());
        Assert.Equal("2011", reference.LReferenceYear.LStateValueShow());
        Assert.Equal(LState.LStateUnknown, reference.LReferenceUrl.LStateValueState);
        Assert.Equal(LState.LStateUnknown, reference.LReferenceAuthorState);
        Assert.Equal(LState.LStateUnknown, read.LMarkupReferenceAuthor.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, reference.LReferenceProgram.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, reference.LReferenceChannel.LStateValueState);
    }

    [Fact]
    public void ASourceCarriesItsAuthorNameBesideTheReferenceThatStatesIt()
    {
        LMarkupReference read = LMarkup.LMarkupReferenceRead(LMarkup.LMarkupScan(
            """
            <source id="oed">
              <title>Oxford English Dictionary</title>
              <author>Murray, James</author>
              <program>Word of Mouth</program>
              <channel>Radio 4</channel>
            </source>
            """));

        Assert.Equal("oed", read.LMarkupReferenceId);
        Assert.Equal("Murray, James", read.LMarkupReferenceAuthor.LStateValueShow());
        Assert.Equal(LState.LStateSpecified, read.LMarkupReferenceValue.LReferenceAuthorState);
        Assert.Equal("Word of Mouth", read.LMarkupReferenceValue.LReferenceProgram.LStateValueShow());
        Assert.Equal("Radio 4", read.LMarkupReferenceValue.LReferenceChannel.LStateValueShow());
    }
}
