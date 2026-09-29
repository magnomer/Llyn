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
        !_cCardListDesk.CDeskFilling && _cCardListDesk.CDeskTenure is LTenure held ? new LQuillCard(held) : null;

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
        CCardListQuill?.LQuillCardMove(cardId, place);
    }

    public void CCardMove(long cardId, string ordinal)
    {
        ArgumentNullException.ThrowIfNull(ordinal);

        CCardListQuill?.LQuillCardMove(cardId, ordinal);
    }
}
