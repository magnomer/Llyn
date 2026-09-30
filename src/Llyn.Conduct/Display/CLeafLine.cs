using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CLeafLine(
    string CLeafLineHead,
    string CLeafLineText,
    string CLeafLineLanguage,
    IReadOnlyList<CMentionMark> CLeafLineMention,
    string CLeafLineCitation,
    IReadOnlyList<CGlossDraft> CLeafLineGloss)
{
    internal static CLeafLine LLeafLineRead(
        LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)
    {
        (string head, string text, string citation) = LEntryPort.LEngineLineRead(sentence, order, mark, citations);
        return new CLeafLine(
            head,
            text,
            sentence.LSentenceDraftLanguage,
            CMention.CMentionMarkRead(sentence.LSentenceDraftMention),
            citation,
            CFolio.CFolioGlossRead(sentence.LSentenceDraftGloss));
    }
}
