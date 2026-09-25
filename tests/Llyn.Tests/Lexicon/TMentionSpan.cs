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
        Assert.Equal((8, 3), TInterface.TMentionSpanResolve("he said the word", 9, true));
        Assert.Equal((12, 4), TInterface.TMentionSpanResolve("he said the word", 15, true));
        Assert.Equal((3, 6), TInterface.TMentionSpanResolve("he didn't go", 5, true));
    }

    [Fact]
    public void MentionSpanResolve_OnSpace_ReturnsEmptySpan()
    {
        Assert.Equal((7, 0), TInterface.TMentionSpanResolve("he said the word", 7, true));
        Assert.Equal((16, 0), TInterface.TMentionSpanResolve("he said the word.", 16, true));
    }

    [Fact]
    public void MentionSpanResolve_AtEndOfText_ReturnsEmptySpan()
    {
        Assert.Equal((16, 0), TInterface.TMentionSpanResolve("he said the word", 16, true));
        Assert.Equal((20, 0), TInterface.TMentionSpanResolve("he said the word", 20, true));
    }

    [Fact]
    public void MentionSpanResolve_SurrogatePairInWord_CountsItOnce()
    {
        Assert.Equal((3, 3), TInterface.TMentionSpanResolve("go 𝒜bc now", 4, true));
        Assert.Equal((7, 3), TInterface.TMentionSpanResolve("go 𝒜bc now", 8, true));
    }

    [Fact]
    public void MentionSpanResolve_JapaneseRun_StopsAtScriptChange()
    {
        Assert.Equal((0, 2), TInterface.TMentionSpanResolve("学校に行きます。", 1, false));
        Assert.Equal((2, 1), TInterface.TMentionSpanResolve("学校に行きます。", 2, false));
        Assert.Equal((4, 3), TInterface.TMentionSpanResolve("学校に行きます。", 5, false));
        Assert.Equal((7, 0), TInterface.TMentionSpanResolve("学校に行きます。", 7, false));
    }
    [Fact]
    public void MentionSpanDivide_TwoMentionsAndGap_CutsFivePiecesInOrder()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterface.TMentionSpanDivide(
            "he said the word.",
            [TInterface.TMentionCreate(2, 12, 4, 7), TInterface.TMentionCreate(1, 3, 4, 5)]);

        Assert.Equal([0, 3, 7, 12, 16], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal([3, 4, 5, 4, 1], pieces.Select(piece => piece.LMentionPieceLength));
        Assert.Equal([0L, 1L, 0L, 2L, 0L], pieces.Select(piece => piece.LMentionPieceStored?.LMentionId ?? 0));
    }

    [Fact]
    public void MentionSpanDivide_MentionsAtBothEnds_CutsNoEmptyPiece()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterface.TMentionSpanDivide(
            "he said the word",
            [TInterface.TMentionCreate(1, 0, 2, 5), TInterface.TMentionCreate(2, 12, 4, 7)]);

        Assert.Equal([0, 2, 12], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal([2, 10, 4], pieces.Select(piece => piece.LMentionPieceLength));
        Assert.Equal(16, pieces.Sum(piece => piece.LMentionPieceLength));
    }

    [Fact]
    public void MentionSpanDivide_SurrogatePairBeforeMention_KeepsCodePointOffsets()
    {
        IReadOnlyList<LMentionPiece> pieces = TInterface.TMentionSpanDivide(
            "go 𝒜bc now",
            [TInterface.TMentionCreate(1, 7, 3, 5)]);

        Assert.Equal([0, 7], pieces.Select(piece => piece.LMentionPieceOffset));
        Assert.Equal([7, 3], pieces.Select(piece => piece.LMentionPieceLength));
        Assert.Equal(["go 𝒜bc ", "now"], pieces.Select(piece => piece.LMentionPieceText));
        Assert.Equal(1, pieces[1].LMentionPieceStored!.LMentionId);
    }

    [Fact]
    public void MentionSpanDivide_NoMention_CutsOnePiece()
    {
        LMentionPiece piece = Assert.Single(TInterface.TMentionSpanDivide("he said", []));

        Assert.Equal((0, 7), (piece.LMentionPieceOffset, piece.LMentionPieceLength));
        Assert.Null(piece.LMentionPieceStored);
        Assert.Empty(TInterface.TMentionSpanDivide(string.Empty, []));
    }

    [Fact]
    public void MentionSpanDivide_OverlappingMentions_Throws()
    {
        Assert.Throws<ArgumentException>(() => TInterface.TMentionSpanDivide(
            "he said the word",
            [TInterface.TMentionCreate(1, 3, 4, 5), TInterface.TMentionCreate(2, 5, 4, 7)]));
    }

    [Fact]
    public void MentionOffsetRead_AcrossSurrogatePair_RoundTripsWithUnitRead()
    {
        const string text = "go 𝒜bc";

        Assert.Equal(0, TInterface.TMentionOffsetRead(text, 0));
        Assert.Equal(3, TInterface.TMentionOffsetRead(text, 3));
        Assert.Equal(3, TInterface.TMentionOffsetRead(text, 4));
        Assert.Equal(4, TInterface.TMentionOffsetRead(text, 5));
        Assert.Equal(6, TInterface.TMentionOffsetRead(text, 7));
        Assert.Equal(6, TInterface.TMentionOffsetRead(text, 9));

        Assert.Equal(0, TInterface.TMentionUnitRead(text, 0));
        Assert.Equal(3, TInterface.TMentionUnitRead(text, 3));
        Assert.Equal(5, TInterface.TMentionUnitRead(text, 4));
        Assert.Equal(7, TInterface.TMentionUnitRead(text, 6));
        Assert.Equal(7, TInterface.TMentionUnitRead(text, 9));

        for (int offset = 0; offset <= 6; offset++)
        {
            Assert.Equal(offset, TInterface.TMentionOffsetRead(text, TInterface.TMentionUnitRead(text, offset)));
        }
    }

    [Fact]
    public void MentionSpanRead_PaddedSelection_DropsTheOuterSpaces()
    {
        LMentionDraft span = TInterface.TMentionSpanRead("he said the word", 2, 6);

        Assert.Equal(3, span.LMentionDraftOffset);
        Assert.Equal(4, span.LMentionDraftLength);
    }

    [Fact]
    public void MentionSpanRead_SurrogatePair_CountsCodePoints()
    {
        LMentionDraft span = TInterface.TMentionSpanRead("go 𝒜bc now", 3, 4);

        Assert.Equal(3, span.LMentionDraftOffset);
        Assert.Equal(3, span.LMentionDraftLength);
    }

    [Fact]
    public void MentionSpanRead_OnlySpaces_ReadsAnEmptySpan()
    {
        Assert.Equal(0, TInterface.TMentionSpanRead("he said the word", 2, 1).LMentionDraftLength);
    }

    [Fact]
    public void EtymologyDraftFind_CaretInsideSpan_ReturnsThatMention()
    {
        LMentionDraft mention = TInterface.TMentionDraftCreate(7, 3, 4, 42);
        LEtymologyDraft etymology = new("he said the word", [mention]);
        LMentionDraft caret = TInterface.TMentionSpanRead(etymology.LEtymologyDraftText, 5, 0);
        LMentionDraft whole = TInterface.TMentionSpanRead(etymology.LEtymologyDraftText, 3, 4);

        Assert.Same(mention, TInterface.TEtymologyDraftFind(etymology, caret));
        Assert.Same(mention, TInterface.TEtymologyDraftFind(etymology, whole));
    }

    [Fact]
    public void EtymologyDraftFind_SpanPastTheMention_ReturnsNothing()
    {
        LEtymologyDraft etymology = new("he said the word", [TInterface.TMentionDraftCreate(7, 3, 4, 42)]);
        LMentionDraft wider = TInterface.TMentionSpanRead(etymology.LEtymologyDraftText, 3, 8);
        LMentionDraft later = TInterface.TMentionSpanRead(etymology.LEtymologyDraftText, 12, 4);

        Assert.Null(TInterface.TEtymologyDraftFind(etymology, wider));
        Assert.Null(TInterface.TEtymologyDraftFind(etymology, later));
    }

    [Fact]
    public void ExampleDraftFind_SelectionInsideMention_ReturnsIt()
    {
        LMentionDraft held = TInterface.TMentionDraftCreate(1, 4, 3, 7);
        LExampleDraft example = TInterface.TExampleDraftCreate("he said the word") with
        {
            LExampleDraftMention = [held],
        };

        Assert.Same(held, TInterface.TExampleDraftFind(example, TInterface.TMentionDraftCreate(0, 5, 2, 0)));
        Assert.Null(TInterface.TExampleDraftFind(example, TInterface.TMentionDraftCreate(0, 5, 3, 0)));
    }

    [Fact]
    public void DraftExampleRead_HeldCardAndSentence_ReturnsTheSentenceExample()
    {
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");
        LCardDraft card = draft.LDraftContent.LEntryDraftMeanings[0];
        LSentenceDraft sentence = card.LCardDraftSentence[0];

        LExampleDraft? read = TInterface.TDraftExampleRead(draft, card.LCardDraftId, sentence.LSentenceDraftId);

        Assert.Same(sentence.LSentenceDraftExample, read);
    }

    [Fact]
    public void DraftExampleRead_UnknownSentence_ReturnsNothing()
    {
        LDraft draft = TInterface.TDraftNestedCreate("editor", "kindle");
        long cardId = draft.LDraftContent.LEntryDraftMeanings[0].LCardDraftId;

        Assert.Null(TInterface.TDraftExampleRead(draft, cardId, TInterface.TIdentityCreate()));
        Assert.Null(TInterface.TDraftExampleRead(draft, 0, 0));
    }
}
