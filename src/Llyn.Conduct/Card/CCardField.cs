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

    private LQuillCard? CCardFieldQuill =>
        !_cCardFieldDesk.CDeskDraft.CDeskDraftFilling && _cCardFieldDesk.CDeskDraft.CDeskDraftTenure is LTenure held
            ? new LQuillCard(held)
            : null;

    public void CCardTitleSet(long cardId, string text)
    {
        CCardFieldQuill?.LCardTitleSet(cardId, text);
    }

    public void CCardExpressionSet(long cardId, string text)
    {
        CCardFieldQuill?.LCardExpressionSet(cardId, text);
    }

    public void CCardMeaningSet(long cardId, string text)
    {
        CCardFieldQuill?.LCardMeaningSet(cardId, text);
    }
}
