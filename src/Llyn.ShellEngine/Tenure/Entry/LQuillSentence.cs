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

    public void LSentenceGlossRemove(LCardKind kind, long card, long sentence, long gloss)
    {
        LSentenceGlossRemove(new LExampleSiteCard(kind, card, sentence), gloss);
    }

    public void LSentenceGlossRemove(long gloss)
    {
        LSentenceGlossRemove(new LExampleSiteDraft(), gloss);
    }

    public void LSentenceGlossSet(
        LCardKind kind, long card, long sentence, long gloss, string? language, string? text)
    {
        LSentenceGlossSet(new LExampleSiteCard(kind, card, sentence), gloss, language, text);
    }

    public void LSentenceGlossSet(long gloss, string? language, string? text)
    {
        LSentenceGlossSet(new LExampleSiteDraft(), gloss, language, text);
    }

    public void LSentenceCitationSet(LCardKind kind, long card, long sentence, long reference)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestSentenceReference(_lQuillSentenceTenure.LTenureId, kind, card, sentence, reference));
    }

    public void LQuillSentenceAdd(LCardKind kind, long card, int position)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestSentenceAddition(_lQuillSentenceTenure.LTenureId, kind, card, position));
    }

    public void LQuillSentenceRemove(LCardKind kind, long card, long sentence)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestSentenceRemoval(_lQuillSentenceTenure.LTenureId, kind, card, sentence));
    }

    public void LQuillSentenceSet(LCardKind kind, long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(new LRequestSentenceText(
            _lQuillSentenceTenure.LTenureId, kind, card, sentence, new LStateWritten(text)));
    }

    public void LSentenceParticleSet(LCardKind kind, long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(new LRequestSentenceParticle(
            _lQuillSentenceTenure.LTenureId, kind, card, sentence, new LStateWritten(text)));
    }

    public void LSentenceDependenceSet(LCardKind kind, long card, long sentence, string text)
    {
        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(new LRequestSentenceDependence(
            _lQuillSentenceTenure.LTenureId, kind, card, sentence, new LStateWritten(text)));
    }

    public void LQuillGlossAdd(LCardKind kind, long card, long sentence)
    {
        LQuillGlossInsert(new LExampleSiteCard(kind, card, sentence), int.MaxValue);
    }

    public void LQuillGlossInsert(int position)
    {
        LQuillGlossInsert(new LExampleSiteDraft(), position);
    }

    public bool LQuillGlossPrepare()
    {
        return LQuillGlossPrepare(new LExampleSiteDraft());
    }

    private void LSentenceGlossRemove(LExampleSite site, long gloss)
    {
        _lQuillSentenceTenure.LTenureRequestApply(
            new LRequestGlossRemoval(_lQuillSentenceTenure.LTenureId, site, gloss));
    }

    private void LSentenceGlossSet(LExampleSite site, long gloss, string? language, string? text)
    {
        if (language is not null)
        {
            _lQuillSentenceTenure.LTenureRequestApply(
                new LRequestGlossLanguage(_lQuillSentenceTenure.LTenureId, site, gloss, language));
            return;
        }

        ArgumentNullException.ThrowIfNull(text);

        _lQuillSentenceTenure.LTenureRequestDefer(
            new LRequestGlossText(_lQuillSentenceTenure.LTenureId, site, gloss, new LStateWritten(text)));
    }

    private void LQuillGlossInsert(LExampleSite site, int position)
    {
        _lQuillSentenceTenure.LTenureRequestApply(new LRequestGlossAddition(
            _lQuillSentenceTenure.LTenureId,
            site,
            _lQuillSentenceTenure.LTenureEngine.LEngineLanguage.LEngineGlossRead(),
            position));
    }

    private bool LQuillGlossPrepare(LExampleSite site)
    {
        LDraft? held = _lQuillSentenceTenure.LTenureRead();
        if (held is null || !LDraftClerkGloss.LGlossEmptyCheck(held, site))
        {
            return false;
        }

        LQuillGlossInsert(site, int.MaxValue);
        return true;
    }
}
