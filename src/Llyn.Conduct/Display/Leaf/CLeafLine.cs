using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed record CLeafLine(
    string CLeafLineHead,
    long CLeafLineSentence,
    IReadOnlyList<CMentionPiece> CLeafLinePiece,
    string CLeafLineCitation,
    IReadOnlyList<CGlossDraft> CLeafLineGloss)
{
    internal static CLeafLine LLeafLineRead(
        LSentenceDraft sentence,
        LSentenceOrder order,
        string mark,
        IReadOnlyDictionary<long, string> citations,
        LExamplePort examples)
    {
        ArgumentNullException.ThrowIfNull(examples);

        (string head, IReadOnlyList<LMentionPiece> pieces, string citation) =
            examples.LEngineLineRead(sentence, order, mark, citations);
        return new CLeafLine(
            head,
            sentence.LSentenceDraftId,
            CMention.LMentionPieceRead(pieces),
            citation,
            CFolio.CFolioGlossRead(sentence.LSentenceDraftGloss));
    }
}
