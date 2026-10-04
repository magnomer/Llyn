using System.Collections.Generic;
using System.Linq;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TExampleLine
{
    [Fact]
    public void EngineLineRead_TrailingOrder_WritesTheRoleBeforeTheMarker()
    {
        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate(
            "an example", 0, LStateAnchor.LStateAnchorUnspecified, "of", "Something");

        (string head, IReadOnlyList<LMentionPiece> pieces, _) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(0, 1), "?", new Dictionary<long, string>());
        (string trailing, _, _) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(1, 0), "?", new Dictionary<long, string>());

        Assert.Equal(("(+of Something)", "an example"), (head, TExampleLineFormat(pieces)));
        Assert.Equal("(+Something of)", trailing);
    }

    [Fact]
    public void EngineLineRead_UnknownMarker_WritesTheMark()
    {
        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate(
            LStateValue.LStateValueUnknown, LStateValue.LStateValueUnspecified);

        (string head, IReadOnlyList<LMentionPiece> pieces, string citation) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(0, 1), "?", new Dictionary<long, string>());

        Assert.Equal(("(+?)", string.Empty, string.Empty), (head, TExampleLineFormat(pieces), citation));
    }

    [Fact]
    public void EngineLineRead_EmptyFields_DropsTheHead()
    {
        LSentenceDraft marker = TInterfaceExample.TSentenceDraftCreate("to", LStateValue.LStateValueUnspecified);
        LSentenceDraft none = TInterfaceExample.TSentenceDraftCreate(
            LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified);
        LSentenceDraft written = TInterfaceExample.TSentenceDraftCreate("text");
        LSentenceOrder order = TInterface.TSentenceOrderCreate(0, 1);
        Dictionary<long, string> citations = [];

        Assert.Equal(("(+to)", string.Empty), TEngineHeadRead(marker, order, citations));
        Assert.Equal((string.Empty, string.Empty), TEngineHeadRead(none, order, citations));
        Assert.Equal((string.Empty, "text"), TEngineHeadRead(written, order, citations));
    }

    [Fact]
    public void EngineLineRead_CitedSource_AnswersItsLine()
    {
        LSentenceDraft cited = TInterfaceExample.TSentenceDraftCreate("a line", 0, TInterface.TStateAnchorCreate(5));
        LSentenceDraft lost = TInterfaceExample.TSentenceDraftCreate("a line", 0, TInterface.TStateAnchorCreate(6));
        LSentenceDraft plain = TInterfaceExample.TSentenceDraftCreate("a line");
        LSentenceOrder order = TInterface.TSentenceOrderCreate(0, 1);
        Dictionary<long, string> citations = new() { [5] = "Field notes, 1999" };

        Assert.Equal("Field notes, 1999", TInterface.TEngineLineRead(cited, order, "?", citations).Item3);
        Assert.Equal(string.Empty, TInterface.TEngineLineRead(lost, order, "?", citations).Item3);
        Assert.Equal(string.Empty, TInterface.TEngineLineRead(plain, order, "?", citations).Item3);
    }

    [Fact]
    public void EngineLineRead_LinkedMention_DividesTheSentenceAroundIt()
    {
        LSentenceDraft sentence = TInterfaceExample.TSentenceDraftCreate("the cat sat");
        sentence = sentence with
        {
            LSentenceDraftExample = sentence.LSentenceDraftExample! with
            {
                LExampleDraftMention = [TInterfaceMentionSpan.TMentionDraftCreate(1, 4, 3, 7)],
            },
        };

        (_, IReadOnlyList<LMentionPiece> pieces, _) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(0, 1), "?", new Dictionary<long, string>());

        Assert.Equal(["the ", "cat", " sat"], pieces.Select(static piece => piece.LMentionPieceText));
        Assert.Equal(7, pieces[1].LMentionPieceStored?.LMentionEntryId);
        Assert.Null(pieces[0].LMentionPieceStored);
    }

    private static (string, string) TEngineHeadRead(
        LSentenceDraft sentence, LSentenceOrder order, IReadOnlyDictionary<long, string> citations)
    {
        (string head, IReadOnlyList<LMentionPiece> pieces, _) =
            TInterface.TEngineLineRead(sentence, order, "?", citations);
        return (head, TExampleLineFormat(pieces));
    }

    private static string TExampleLineFormat(IReadOnlyList<LMentionPiece> pieces)
    {
        return string.Concat(pieces.Select(static piece => piece.LMentionPieceText));
    }
}
