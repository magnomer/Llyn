using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PSentence
{
    private IReadOnlyList<CMentionDraft> _pSentenceMention = [];

    public PMentionLine PSentenceChip { get; } = new();

    internal void PSentenceMentionShow(LWindow window, string silent)
    {
        PSentenceChip.PMentionLineShow(window, _pSentenceText.CStateValueText, _pSentenceMention, silent);
    }
}
