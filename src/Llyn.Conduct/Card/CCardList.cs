using System;
using Llyn.Core;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardList
{
    private readonly CDesk _cCardListDesk;

    internal CCardList(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cCardListDesk = desk;
    }

    private LQuillCard? CCardListQuill =>
        !_cCardListDesk.CDeskDraft.CDeskDraftFilling && _cCardListDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new LQuillCard(held)
            : null;

    public void CCardMeaningAdd()
    {
        CCardListQuill?.LQuillCardAdd(LCardKind.LCardKindMeaning);
    }

    public void CCardCollocationAdd()
    {
        CCardListQuill?.LQuillCardAdd(LCardKind.LCardKindCollocation);
    }

    public void CCardRemove(long cardId)
    {
        CCardListQuill?.LQuillCardRemove(cardId);
    }

    public void CCardMove(long cardId, int place)
    {
        if (CCardListQuill is LQuillCard quill
            && _cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is LDraft held
            && CFolio.CFolioPlaceRead(held.LDraftContent, cardId, place) is int target)
        {
            quill.LQuillCardMove(cardId, target);
        }
    }

    public void CCardMove(long cardId, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(ordinal);

        if (_cCardListDesk.CDeskDraft.CDeskDraftTenure?.LTenureRead() is LDraft held
            && CFolio.CFolioOrdinalRead(held.LDraftContent, cardId, ordinal) is int place)
        {
            CCardMove(cardId, place);
        }
    }
}
