using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillCard
{
    private readonly LTenure _lQuillCardTenure;

    public LQuillCard(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillCardTenure = tenure;
    }

    public void LQuillCardAdd(LCardKind kind)
    {
        if (_lQuillCardTenure.LTenureRead() is not LDraft held)
        {
            return;
        }

        _lQuillCardTenure.LTenureRequestApply(new LRequestCardAddition(
            _lQuillCardTenure.LTenureId, kind, 0, LDraftClerkCard.LCardEndRead(held.LDraftContent, kind)));
    }

    public void LQuillCardRemove(LCardKind kind, long card)
    {
        if (_lQuillCardTenure.LTenureRead() is not LDraft held
            || LDraftClerkCard.LCardLoneCheck(held.LDraftContent, kind, card))
        {
            return;
        }

        _lQuillCardTenure.LTenureRequestApply(new LRequestCardRemoval(_lQuillCardTenure.LTenureId, kind, card));
    }

    public void LQuillCardMove(LCardKind kind, long card, int place)
    {
        if (_lQuillCardTenure.LTenureRead() is LDraft held
            && LDraftClerkCard.LCardShiftRead(held.LDraftContent, kind, card, place) is int target)
        {
            _lQuillCardTenure.LTenureRequestApply(
                new LRequestCardShift(_lQuillCardTenure.LTenureId, kind, card, 0, target));
        }
    }

    public void LCardTitleSet(LCardKind kind, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillCardTenure.LTenureRequestDefer(
            new LRequestCardTitle(_lQuillCardTenure.LTenureId, kind, card, new LStateWritten(text)));
    }

    public void LCardExpressionSet(LCardKind kind, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillCardTenure.LTenureRequestDefer(
            new LRequestCardExpression(_lQuillCardTenure.LTenureId, kind, card, new LStateWritten(text)));
    }

    public void LCardMeaningSet(LCardKind kind, long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillCardTenure.LTenureRequestDefer(
            new LRequestCardMeaning(_lQuillCardTenure.LTenureId, kind, card, new LStateWritten(text)));
    }

    public void LCardSenseAttach(long card, long sense)
    {
        _lQuillCardTenure.LTenureRequestApply(
            new LRequestSenseAddition(_lQuillCardTenure.LTenureId, card, sense));
    }

    public void LCardSenseDetach(long card, long sense)
    {
        _lQuillCardTenure.LTenureRequestApply(
            new LRequestSenseRemoval(_lQuillCardTenure.LTenureId, card, sense));
    }
}
