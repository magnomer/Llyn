using System;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed class LQuillSentence
{
    private readonly LTenure _lQuillSentenceTenure;

    public LQuillSentence(LTenure tenure)
    {
        ArgumentNullException.ThrowIfNull(tenure);

        _lQuillSentenceTenure = tenure;
    }

    public void LSentenceGlossRemove(long card, long sentence, long gloss)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestGlossRemoval(_lQuillSentenceTenure.LTenureId, card, sentence, gloss));
    }

    public void LSentenceGlossSet(long card, long sentence, long gloss, string? language, string? text)
    {
        if (language is not null)
        {
            _lQuillSentenceTenure.LTenureRequestApply(
                new LRequestGlossLanguage(_lQuillSentenceTenure.LTenureId, card, sentence, gloss, language));
            return;
        }

        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(
            new LRequestGlossText(_lQuillSentenceTenure.LTenureId, card, sentence, gloss, new LStateWritten(text)));
    }

    public void LSentenceCitationSet(long card, long sentence, long reference)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestSentenceReference(_lQuillSentenceTenure.LTenureId, card, sentence, reference));
    }

    public void LQuillSentenceAdd(long card, int position)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestSentenceAddition(_lQuillSentenceTenure.LTenureId, card, position));
    }

    public void LQuillSentenceRemove(long card, long sentence)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestSentenceRemoval(_lQuillSentenceTenure.LTenureId, card, sentence));
    }

    public void LQuillSentenceSet(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(
            new LRequestSentenceText(_lQuillSentenceTenure.LTenureId, card, sentence, new LStateWritten(text)));
    }

    public void LSentenceParticleSet(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(
            new LRequestSentenceParticle(_lQuillSentenceTenure.LTenureId, card, sentence, new LStateWritten(text)));
    }

    public void LSentenceDependenceSet(long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(new LRequestSentenceDependence(
            _lQuillSentenceTenure.LTenureId, card, sentence, new LStateWritten(text)));
    }

    public void LQuillGlossAdd(long card, long sentence)
    {
        LQuillGlossInsert(card, sentence, int.MaxValue);
    }

    public void LQuillGlossInsert(long card, long sentence, int position)
    {
        _lQuillSentenceTenure.LTenureRequestApply(new LRequestGlossAddition(
            _lQuillSentenceTenure.LTenureId,
            card,
            sentence,
            _lQuillSentenceTenure.LTenureEngine.LEngineLanguage.LEngineGlossRead(),
            position));
    }

    public bool LQuillGlossPrepare(long card, long sentence)
    {
        LDraft? held = _lQuillSentenceTenure.LTenureRead();
        if (held is null || !LDraftClerkGloss.LGlossEmptyCheck(held, card, sentence))
        {
            return false;
        }

        LQuillGlossAdd(card, sentence);
        return true;
    }
}
