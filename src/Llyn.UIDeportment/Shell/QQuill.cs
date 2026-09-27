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

    public void QQuillAuthorSet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        _qQuillDesk.LDeskDefer(new LRequestAuthorName(_qQuillDesk.LDeskId, name));
    }

    public void QQuillExampleChange(CExampleField field, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        switch (field)
        {
            case CExampleField.CExampleFieldLanguage:
                _qQuillDesk.LDeskSend(new LRequestExampleLanguage(_qQuillDesk.LDeskId, value));
                break;
            case CExampleField.CExampleFieldText:
                _qQuillDesk.LDeskDefer(new LRequestExampleText(_qQuillDesk.LDeskId, new LStateWritten(value, false)));
                break;
        }
    }

    public void QQuillCitationSet(long reference)
    {
        _qQuillDesk.LDeskSend(new LRequestExampleReference(_qQuillDesk.LDeskId, reference));
    }

    public void QQuillGlossAdd(long card, long sentence, string language, int position)
    {
        ArgumentNullException.ThrowIfNull(language);

        _qQuillDesk.LDeskSend(new LRequestGlossAddition(_qQuillDesk.LDeskId, card, sentence, language, position));
    }

    public void QQuillGlossRemove(long card, long sentence, long gloss)
    {
        _qQuillDesk.LDeskSend(new LRequestGlossRemoval(_qQuillDesk.LDeskId, card, sentence, gloss));
    }

    public void QQuillGlossChange(long card, long sentence, long gloss, CGlossField field, string value)
    {
        ArgumentNullException.ThrowIfNull(value);

        switch (field)
        {
            case CGlossField.CGlossFieldLanguage:
                _qQuillDesk.LDeskSend(new LRequestGlossLanguage(_qQuillDesk.LDeskId, card, sentence, gloss, value));
                break;
            case CGlossField.CGlossFieldText:
                _qQuillDesk.LDeskDefer(
                    new LRequestGlossText(_qQuillDesk.LDeskId, card, sentence, gloss, new LStateWritten(value)));
                break;
        }
    }

    public void QQuillMentionAdd(long card, long sentence, int offset, int length, long entry)
    {
        _qQuillDesk.LDeskSend(
            new LRequestMentionAddition(_qQuillDesk.LDeskId, card, sentence, offset, length, entry, 0));
    }

    public void QQuillMentionRemove(long card, long sentence, long mention)
    {
        _qQuillDesk.LDeskSend(new LRequestMentionRemoval(_qQuillDesk.LDeskId, card, sentence, mention));
    }

    public void QQuillMentionChange(long card, long sentence, long mention, long sense)
    {
        _qQuillDesk.LDeskSend(new LRequestMentionSense(_qQuillDesk.LDeskId, card, sentence, mention, sense));
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
