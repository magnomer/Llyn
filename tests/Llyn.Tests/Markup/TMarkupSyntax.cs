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
              <expression>kindle a fire</expression>
              <meaning>to set something burning</meaning>
              <synonym>ignite, light</synonym>
              <tag>literal</tag>
              <tag>old</tag>
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
    }

    [Fact]
    public void MarkupCardRead_EmptyTag_ReturnsNoTag()
    {
        LCardDraft card = TInterface.TMarkupCardRead(TInterface.TMarkupScan(
            "<sense><meaning></meaning><tag></tag></sense>"), 1);

        Assert.Equal(LState.LStateUnknown, card.LCardDraftMeaning.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftTitle.LStateValueState);
        Assert.Equal(LState.LStateUnspecified, card.LCardDraftExpression.LStateValueState);
        Assert.Equal(string.Empty, card.LCardDraftSynonym);
        Assert.Empty(card.LCardDraftTag);
        Assert.Empty(card.LCardDraftImage);
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
