using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIDeportment;

internal sealed partial class PSentence
{
    private IReadOnlyList<LMentionDraft> _pSentenceMention = [];

    public PMentionLine PSentenceChip { get; } = new();

    internal void PSentenceMentionShow(LWindow window, string silent)
    {
        PSentenceChip.PMentionLineShow(window, _pSentenceText.LStateValueShow(), _pSentenceMention, silent);
    }
}
