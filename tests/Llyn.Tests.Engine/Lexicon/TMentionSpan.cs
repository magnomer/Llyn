using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TMentionSpan
{
    [Fact]
    public void MentionSpanResolve_MidWord_ReturnsTheWholeWord()
    {
        Assert.Equal((8, 3), TInterfaceMentionSpan.TMentionSpanResolve("he said the word", 9, true));
        Assert.Equal((12, 4), TInterfaceMentionSpan.TMentionSpanResolve("he said the word", 15, true));
        Assert.Equal((3, 6), TInterfaceMentionSpan.TMentionSpanResolve("he didn't go", 5, true));
    }

    [Fact]
    public void MentionSpanResolve_OnSpace_ReturnsEmptySpan()
    {
        Assert.Equal((7, 0), TInterfaceMentionSpan.TMentionSpanResolve("he said the word", 7, true));
        Assert.Equal((16, 0), TInterfaceMentionSpan.TMentionSpanResolve("he said the word.", 16, true));
    }

    [Fact]
    public void MentionSpanResolve_AtEndOfText_ReturnsEmptySpan()
    {
        Assert.Equal((16, 0), TInterfaceMentionSpan.TMentionSpanResolve("he said the word", 16, true));
        Assert.Equal((20, 0), TInterfaceMentionSpan.TMentionSpanResolve("he said the word", 20, true));
    }

    [Fact]
    public void MentionSpanResolve_SurrogatePairInWord_CountsItOnce()
    {
        Assert.Equal((3, 3), TInterfaceMentionSpan.TMentionSpanResolve("go 𝒜bc now", 4, true));
        Assert.Equal((7, 3), TInterfaceMentionSpan.TMentionSpanResolve("go 𝒜bc now", 8, true));
    }

    [Fact]
    public void MentionSpanResolve_JapaneseRun_StopsAtScriptChange()
    {
        Assert.Equal((0, 2), TInterfaceMentionSpan.TMentionSpanResolve("学校に行きます。", 1, false));
        Assert.Equal((2, 1), TInterfaceMentionSpan.TMentionSpanResolve("学校に行きます。", 2, false));
        Assert.Equal((4, 3), TInterfaceMentionSpan.TMentionSpanResolve("学校に行きます。", 5, false));
        Assert.Equal((7, 0), TInterfaceMentionSpan.TMentionSpanResolve("学校に行きます。", 7, false));
    }
    [Fact]
    public void MentionSpanDivide_TwoMentionsAndGap_CutsFivePiecesInOrder()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterfaceMentionSpan.TMentionSpanDivide(
            "he said the word.",
            [TInterfaceMentionSpan.TMentionCreate(2, 12, 4, 7), TInterfaceMentionSpan.TMentionCreate(1, 3, 4, 5)]);

        Assert.Equal([0, 3, 7, 12, 16], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal(["he ", "said", " the ", "word", "."], pieces.Select(piece => piece.LMentionPieceText));
        Assert.Equal([0L, 1L, 0L, 2L, 0L], pieces.Select(piece => piece.LMentionPieceStored?.LMentionId ?? 0));
    }

    [Fact]
    public void MentionSpanDivide_MentionsAtBothEnds_CutsNoEmptyPiece()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterfaceMentionSpan.TMentionSpanDivide(
            "he said the word",
            [TInterfaceMentionSpan.TMentionCreate(1, 0, 2, 5), TInterfaceMentionSpan.TMentionCreate(2, 12, 4, 7)]);

        Assert.Equal([0, 2, 12], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal(["he", " said the ", "word"], pieces.Select(piece => piece.LMentionPieceText));
    }

    [Fact]
    public void MentionSpanDivide_SurrogatePairBeforeMention_KeepsCodePointOffsets()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterfaceMentionSpan.TMentionSpanDivide(
            "go 𝒜bc now",
            [TInterfaceMentionSpan.TMentionCreate(1, 7, 3, 5)]);

        Assert.Equal([0, 7], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal(["go 𝒜bc ", "now"], pieces.Select(piece => piece.LMentionPieceText));
        Assert.Equal(1, pieces[1].LMentionPieceStored!.LMentionId);
    }

    [Fact]
    public void MentionSpanDivide_NoMention_CutsOnePiece()
    {
        LMentionPiece piece = Assert.Single(TInterfaceMentionSpan.TMentionSpanDivide("he said", []));

        Assert.Equal((0, "he said"), (piece.LMentionPieceOffset, piece.LMentionPieceText));
        Assert.Null(piece.LMentionPieceStored);
        Assert.Empty(TInterfaceMentionSpan.TMentionSpanDivide(string.Empty, []));
    }

    [Fact]
    public void MentionSpanDivide_OverlappingMentions_Throws()
    {
        Assert.Throws<ArgumentException>(() => TInterfaceMentionSpan.TMentionSpanDivide(
            "he said the word",
            [TInterfaceMentionSpan.TMentionCreate(1, 3, 4, 5), TInterfaceMentionSpan.TMentionCreate(2, 5, 4, 7)]));
    }

    [Fact]
    public void MentionOffsetRead_AcrossSurrogatePair_RoundTripsWithUnitRead()
    {
        const string text = "go 𝒜bc";

        Assert.Equal(0, TInterfaceMentionSpan.TMentionOffsetRead(text, 0));
        Assert.Equal(3, TInterfaceMentionSpan.TMentionOffsetRead(text, 3));
        Assert.Equal(3, TInterfaceMentionSpan.TMentionOffsetRead(text, 4));
        Assert.Equal(4, TInterfaceMentionSpan.TMentionOffsetRead(text, 5));
        Assert.Equal(6, TInterfaceMentionSpan.TMentionOffsetRead(text, 7));
        Assert.Equal(6, TInterfaceMentionSpan.TMentionOffsetRead(text, 9));

        Assert.Equal(0, TInterfaceMentionSpan.TMentionUnitRead(text, 0));
        Assert.Equal(3, TInterfaceMentionSpan.TMentionUnitRead(text, 3));
        Assert.Equal(5, TInterfaceMentionSpan.TMentionUnitRead(text, 4));
        Assert.Equal(7, TInterfaceMentionSpan.TMentionUnitRead(text, 6));
        Assert.Equal(7, TInterfaceMentionSpan.TMentionUnitRead(text, 9));

        for (int offset = 0; offset <= 6; offset++)
        {
            Assert.Equal(
                offset,
                TInterfaceMentionSpan.TMentionOffsetRead(text, TInterfaceMentionSpan.TMentionUnitRead(text, offset)));
        }
    }

    [Fact]
    public void MentionSpanRead_PaddedSelection_DropsTheOuterSpaces()
    {
        LMentionDraft span = TInterfaceMentionSpan.TMentionSpanRead("he said the word", 2, 6);

        Assert.Equal(3, span.LMentionDraftOffset);
        Assert.Equal(4, span.LMentionDraftLength);
    }

    [Fact]
    public void MentionSpanRead_SurrogatePair_CountsCodePoints()
    {
        LMentionDraft span = TInterfaceMentionSpan.TMentionSpanRead("go 𝒜bc now", 3, 4);

        Assert.Equal(3, span.LMentionDraftOffset);
        Assert.Equal(3, span.LMentionDraftLength);
    }

    [Fact]
    public void MentionSpanRead_OnlySpaces_ReadsAnEmptySpan()
    {
        Assert.Equal(0, TInterfaceMentionSpan.TMentionSpanRead("he said the word", 2, 1).LMentionDraftLength);
    }

    [Fact]
    public void EtymologyDraftFind_CaretInsideSpan_ReturnsThatMention()
    {
        LMentionDraft mention = TInterfaceMentionSpan.TMentionDraftCreate(7, 3, 4, 42);
        LEtymologyDraft etymology = new("he said the word", [mention]);
        LMentionDraft caret = TInterfaceMentionSpan.TMentionSpanRead(etymology.LEtymologyDraftText, 5, 0);
        LMentionDraft whole = TInterfaceMentionSpan.TMentionSpanRead(etymology.LEtymologyDraftText, 3, 4);

        Assert.Same(mention, TInterfaceMentionSpan.TEtymologyDraftFind(etymology, caret));
        Assert.Same(mention, TInterfaceMentionSpan.TEtymologyDraftFind(etymology, whole));
    }

    [Fact]
    public void EtymologyDraftFind_SpanPastTheMention_ReturnsNothing()
    {
        LEtymologyDraft etymology = new("he said the word", [TInterfaceMentionSpan.TMentionDraftCreate(7, 3, 4, 42)]);
        LMentionDraft wider = TInterfaceMentionSpan.TMentionSpanRead(etymology.LEtymologyDraftText, 3, 8);
        LMentionDraft later = TInterfaceMentionSpan.TMentionSpanRead(etymology.LEtymologyDraftText, 12, 4);

        Assert.Null(TInterfaceMentionSpan.TEtymologyDraftFind(etymology, wider));
        Assert.Null(TInterfaceMentionSpan.TEtymologyDraftFind(etymology, later));
    }

    [Fact]
    public void ExampleDraftFind_SelectionInsideMention_ReturnsIt()
    {
        LMentionDraft held = TInterfaceMentionSpan.TMentionDraftCreate(1, 4, 3, 7);
        LExampleDraft example = TInterfaceExample.TExampleDraftCreate("he said the word") with
        {
            LExampleDraftMention = [held],
        };

        Assert.Same(
            held,
            TInterfaceMentionSpan.TExampleDraftFind(example, TInterfaceMentionSpan.TMentionDraftCreate(0, 5, 2, 0)));
        Assert.Null(
            TInterfaceMentionSpan.TExampleDraftFind(example, TInterfaceMentionSpan.TMentionDraftCreate(0, 5, 3, 0)));
    }

    [Fact]
    public void DraftExampleRead_HeldCardAndSentence_ReturnsTheSentenceExample()
    {
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");
        LCardDraft card = draft.LDraftContent.LEntryDraftMeanings[0];
        LSentenceDraft sentence = card.LCardDraftSentence[0];

        LExampleDraft? read =
            TInterfaceMentionSpan.TDraftExampleRead(draft, card.LCardDraftId, sentence.LSentenceDraftId);

        Assert.Same(sentence.LSentenceDraftExample, read);
    }

    [Fact]
    public void DraftExampleRead_UnknownSentence_ReturnsNothing()
    {
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");
        long cardId = draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;

        Assert.Null(TInterfaceMentionSpan.TDraftExampleRead(draft, cardId, TInterface.TIdentityCreate()));
        Assert.Null(TInterfaceMentionSpan.TDraftExampleRead(draft, 0, 0));
    }
}
