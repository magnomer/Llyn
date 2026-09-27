using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

internal sealed partial class PSentence
{
    private IReadOnlyList<CMentionDraft> _pSentenceMention = [];

    public PMentionLine PSentenceChip { get; } = new();

    internal void PSentenceMentionShow(CAtelier atelier, string silent)
    {
        PSentenceChip.PMentionLineShow(atelier, _pSentenceText.CStateValueText, _pSentenceMention, silent);
    }
}
