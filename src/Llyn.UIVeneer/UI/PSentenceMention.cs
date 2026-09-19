using System.Collections.Generic;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.UIVeneer;

internal sealed partial class PSentence
{
    private IReadOnlyList<LMentionDraft> _pSentenceMention = [];

    public PMentionLine PSentenceChip { get; } = new();

    internal IReadOnlyList<LMentionDraft> PSentenceMention => _pSentenceMention;

    internal void PSentenceMentionShow(LEngine engine, string silent)
    {
        PSentenceChip.PMentionLineShow(engine, _pSentenceText.LStateValueShow(), _pSentenceMention, silent);
    }
}
