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

    public void LQuillCardRemove(long card)
    {
        if (_lQuillCardTenure.LTenureRead() is not LDraft held
            || LDraftClerkCard.LCardLoneCheck(held.LDraftContent, card))
        {
            return;
        }

        _lQuillCardTenure.LTenureRequestApply(new LRequestCardRemoval(_lQuillCardTenure.LTenureId, card));
    }

    public void LQuillCardMove(long card, int place)
    {
        if (_lQuillCardTenure.LTenureRead() is LDraft held
            && LDraftClerkCard.LCardShiftRead(held.LDraftContent, card, place) is int target)
        {
            _lQuillCardTenure.LTenureRequestApply(
                new LRequestCardShift(_lQuillCardTenure.LTenureId, card, 0, target));
        }
    }

    public void LCardTitleSet(long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillCardTenure.LTenureRequestDefer(
            new LRequestCardTitle(_lQuillCardTenure.LTenureId, card, new LStateWritten(text)));
    }

    public void LCardExpressionSet(long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillCardTenure.LTenureRequestDefer(
            new LRequestCardExpression(_lQuillCardTenure.LTenureId, card, new LStateWritten(text)));
    }

    public void LCardMeaningSet(long card, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillCardTenure.LTenureRequestDefer(
            new LRequestCardMeaning(_lQuillCardTenure.LTenureId, card, new LStateWritten(text)));
    }
}
