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
        LEntryDraft shown,
        LSentenceDraft sentence,
        LSentenceOrder order,
        string mark,
        IReadOnlyDictionary<long, string> citations,
        LExamplePort examples)
    {
        ArgumentNullException.ThrowIfNull(examples);

        (string head, IReadOnlyList<LMentionPiece> pieces, string citation, long line) =
            examples.LEngineLineRead(shown, sentence, order, mark, citations);
        return new CLeafLine(
            head,
            line,
            CMention.LMentionPieceRead(pieces),
            citation,
            CFolio.CFolioGlossRead(sentence.LSentenceDraftGloss));
    }
}
