using System;
using Llyn.Application;
using Llyn.Conduct;
using Llyn.Core;

namespace Llyn.UIDeportment;

public sealed class QQuill
{
    private readonly LDesk _qQuillDesk;

    internal QQuill(LDesk desk)
    {
        ArgumentNullException.ThrowIfNull(desk);

        _qQuillDesk = desk;
    }

    public void QQuillSituationChange(string title, string description, string kind)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(kind);

        QQuillSituationDefer(
            LAtlas.LAtlasSituationRead(_qQuillDesk.LDeskDraft?.LDraftSituation),
            title,
            description,
            kind);
    }

    private void QQuillSituationDefer(CSituationDraft? held, string title, string description, string kind)
    {
        _qQuillDesk.LDeskDefer(new LRequestSituationBody(
            _qQuillDesk.LDeskId,
            0,
            QQuillWrittenRead(title, held?.CSituationDraftTitle),
            QQuillWrittenRead(description, held?.CSituationDraftDescription),
            QQuillWrittenRead(kind, held?.CSituationDraftKind)));
    }

    private static LStateWritten QQuillWrittenRead(string text, CStateValue? held)
    {
        return new LStateWritten(text, text.Length == 0 && (held?.CStateValueUncertain ?? false));
    }
}
