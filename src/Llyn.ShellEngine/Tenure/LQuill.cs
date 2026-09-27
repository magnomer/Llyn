using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuill
{
    private readonly LTenure _lQuillTenure;

    public LQuill(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillTenure = tenure;
    }

    public void LQuillAuthorSet(string name)
    {
        ArgumentNullException.ThrowIfNull(name);

        _lQuillTenure.LTenureRequestDefer(new LRequestAuthorName(_lQuillTenure.LTenureId, name));
    }

    public void LQuillExampleSet(string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestExampleText(_lQuillTenure.LTenureId, new LStateWritten(text, false)));
    }

    public void LQuillSpeakerSet(string language)
    {
        ArgumentNullException.ThrowIfNull(language);

        _lQuillTenure.LTenureRequestApply(new LRequestExampleLanguage(_lQuillTenure.LTenureId, language));
    }

    public void LQuillReferenceSet(long reference)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestExampleReference(_lQuillTenure.LTenureId, reference));
    }

    public void LQuillGlossAdd(long card, long sentence, string language, int position)
    {
        ArgumentNullException.ThrowIfNull(language);

        _lQuillTenure.LTenureRequestApply(
            new LRequestGlossAddition(_lQuillTenure.LTenureId, card, sentence, language, position));
    }

    public void LQuillGlossRemove(long card, long sentence, long gloss)
    {
        _lQuillTenure.LTenureRequestApply(new LRequestGlossRemoval(_lQuillTenure.LTenureId, card, sentence, gloss));
    }

    public void LQuillGlossSet(long card, long sentence, long gloss, string? language, string? text)
    {
        if (language is not null)
        {
            _lQuillTenure.LTenureRequestApply(
                new LRequestGlossLanguage(_lQuillTenure.LTenureId, card, sentence, gloss, language));
            return;
        }

        ArgumentNullException.ThrowIfNull(text);

        _lQuillTenure.LTenureRequestDefer(
            new LRequestGlossText(_lQuillTenure.LTenureId, card, sentence, gloss, new LStateWritten(text)));
    }

    public void LQuillMentionAdd(long card, long sentence, int offset, int length, long entry)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestMentionAddition(_lQuillTenure.LTenureId, card, sentence, offset, length, entry, 0));
    }

    public void LQuillMentionRemove(long card, long sentence, long mention)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestMentionRemoval(_lQuillTenure.LTenureId, card, sentence, mention));
    }

    public void LQuillMentionSet(long card, long sentence, long mention, long sense)
    {
        _lQuillTenure.LTenureRequestApply(
            new LRequestMentionSense(_lQuillTenure.LTenureId, card, sentence, mention, sense));
    }

    public void LQuillSituationSet(string title, string description, string kind)
    {
        ArgumentNullException.ThrowIfNull(title);
        ArgumentNullException.ThrowIfNull(description);
        ArgumentNullException.ThrowIfNull(kind);

        LQuillSituationDefer(_lQuillTenure.LTenureRead()?.LDraftSituation, title, description, kind);
    }

    private void LQuillSituationDefer(LSituation? held, string title, string description, string kind)
    {
        _lQuillTenure.LTenureRequestDefer(new LRequestSituationBody(
            _lQuillTenure.LTenureId,
            0,
            LQuillWrittenRead(title, held?.LSituationTitle),
            LQuillWrittenRead(description, held?.LSituationDescription),
            LQuillWrittenRead(kind, held?.LSituationKind)));
    }

    private static LStateWritten LQuillWrittenRead(string text, LStateValue? held)
    {
        return new LStateWritten(text, text.Length == 0 && (held?.LStateValueUncertain ?? false));
    }
}
