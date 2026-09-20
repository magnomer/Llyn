using System.Collections.Generic;
using Llyn.Core;
using Llyn.UIDeportment;

namespace Llyn.UIVeneer;

internal sealed partial class PSentence
{
    private IReadOnlyList<LMentionDraft> _pSentenceMention = [];

    public PMentionLine PSentenceChip { get; } = new();

    internal IReadOnlyList<LMentionDraft> PSentenceMention => _pSentenceMention;

    internal void PSentenceMentionShow(LWindow window, string silent)
    {
        PSentenceChip.PMentionLineShow(window, _pSentenceText.LStateValueShow(), _pSentenceMention, silent);
    }
}
