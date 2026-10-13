using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillEtymology
{
    private readonly LTenure _lQuillEtymologyTenure;

    public LQuillEtymology(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillEtymologyTenure = tenure;
    }

    public void LQuillEtymologySet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillEtymologyTenure.LTenureRequestDefer(new LRequestEtymologyText(_lQuillEtymologyTenure.LTenureId, text));
    }

    public void LEtymonAdd(long entry, int position)
    {
        _lQuillEtymologyTenure.LTenureRequestApply(
            new LRequestEtymonAddition(_lQuillEtymologyTenure.LTenureId, entry, position));
    }

    public void LEtymonRemove(long entry)
    {
        _lQuillEtymologyTenure.LTenureRequestApply(new LRequestEtymonRemoval(_lQuillEtymologyTenure.LTenureId, entry));
    }

    public void LEtymologyMentionSave(int offset, int length, long entry)
    {
        _lQuillEtymologyTenure.LTenureRequestApply(
            new LRequestEtymologyMention(_lQuillEtymologyTenure.LTenureId, offset, length, entry));
    }

    public IReadOnlyList<LTranslationTarget> LQuillEtymonRead()
    {
        if (_lQuillEtymologyTenure.LTenureRead()?.LDraftContent is not LEntryDraft draft)
        {
            return [];
        }

        try
        {
            return _lQuillEtymologyTenure.LTenureEngine.LEngineCard.LEngineEtymonRead(draft);
        }
        catch (Exception exception) when (LWorkspaceClerk.LWorkspaceRefusedCheck(exception))
        {
            return [];
        }
    }

    public LMentionDraft? LQuillEtymologyFind(LMentionDraft span)
    {
        ArgumentNullException.ThrowIfNull(span);

        return LMentionClerk.LMentionEtymologyFind(_lQuillEtymologyTenure.LTenureRead(), span);
    }
}
