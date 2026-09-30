using System.Collections.Generic;
using Llyn.Core;
using Xunit;

namespace Llyn.Tests;

public sealed class TExampleLine
{
    [Fact]
    public void EngineLineRead_TrailingOrder_WritesTheRoleBeforeTheMarker()
    {
        LSentenceDraft sentence = TInterface.TSentenceDraftCreate(
            "an example", 0, LStateAnchor.LStateAnchorUnspecified, "of", "Something");

        (string head, string text, _) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(0, 1), "?", new Dictionary<long, string>());
        (string trailing, _, _) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(1, 0), "?", new Dictionary<long, string>());

        Assert.Equal(("(+of Something)", "an example"), (head, text));
        Assert.Equal("(+Something of)", trailing);
    }

    [Fact]
    public void EngineLineRead_UnknownMarker_WritesTheMark()
    {
        LSentenceDraft sentence = TInterface.TSentenceDraftCreate(
            LStateValue.LStateValueUnknown, LStateValue.LStateValueUnspecified);

        (string head, string text, string citation) = TInterface.TEngineLineRead(
            sentence, TInterface.TSentenceOrderCreate(0, 1), "?", new Dictionary<long, string>());

        Assert.Equal(("(+?)", string.Empty, string.Empty), (head, text, citation));
    }

    [Fact]
    public void EngineLineRead_EmptyFields_DropsTheHead()
    {
        LSentenceDraft marker = TInterface.TSentenceDraftCreate("to", LStateValue.LStateValueUnspecified);
        LSentenceDraft none = TInterface.TSentenceDraftCreate(
            LStateValue.LStateValueUnspecified, LStateValue.LStateValueUnspecified);
        LSentenceDraft written = TInterface.TSentenceDraftCreate("text");
        LSentenceOrder order = TInterface.TSentenceOrderCreate(0, 1);
        Dictionary<long, string> citations = [];

        Assert.Equal(("(+to)", string.Empty), TEngineHeadRead(marker, order, citations));
        Assert.Equal((string.Empty, string.Empty), TEngineHeadRead(none, order, citations));
        Assert.Equal((string.Empty, "text"), TEngineHeadRead(written, order, citations));
    }

    [Fact]
    public void EngineLineRead_CitedSource_AnswersItsLine()
    {
        LSentenceDraft cited = TInterface.TSentenceDraftCreate("a line", 0, TInterface.TStateAnchorCreate(5));
        LSentenceDraft lost = TInterface.TSentenceDraftCreate("a line", 0, TInterface.TStateAnchorCreate(6));
        LSentenceDraft plain = TInterface.TSentenceDraftCreate("a line");
        LSentenceOrder order = TInterface.TSentenceOrderCreate(0, 1);
        Dictionary<long, string> citations = new() { [5] = "Field notes, 1999" };

        Assert.Equal("Field notes, 1999", TInterface.TEngineLineRead(cited, order, "?", citations).Item3);
        Assert.Equal(string.Empty, TInterface.TEngineLineRead(lost, order, "?", citations).Item3);
        Assert.Equal(string.Empty, TInterface.TEngineLineRead(plain, order, "?", citations).Item3);
    }

    private static (string, string) TEngineHeadRead(
        LSentenceDraft sentence, LSentenceOrder order, IReadOnlyDictionary<long, string> citations)
    {
        (string head, string text, _) = TInterface.TEngineLineRead(sentence, order, "?", citations);
        return (head, text);
    }
}
