using System;
using Llyn.ShellEngine;

namespace Llyn.Conduct;

public sealed class CCardField
{
    private readonly CDesk _cCardFieldDesk;

    internal CCardField(CDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _cCardFieldDesk = desk;
    }

    public void CCardTitleSet(long cardId, string text)
    {
        _cCardFieldDesk.CDeskQuill?.LQuillTitleSet(cardId, text);
    }

    public void CCardExpressionSet(long cardId, string text)
    {
        _cCardFieldDesk.CDeskQuill?.LQuillExpressionSet(cardId, text);
    }

    public void CCardMeaningSet(long cardId, string text)
    {
        _cCardFieldDesk.CDeskQuill?.LQuillMeaningSet(cardId, text);
    }
}
