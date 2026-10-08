using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LMarkupClerkCard
{
    private readonly LEntryVault _lMarkupCardEntries;
    private readonly LMarkupClerkExample _lMarkupCardExample;

    public LMarkupClerkCard(LRig rig, LMarkupClerkExample example)
    {
        ArgumentNullException.ThrowIfNull(rig);
        ArgumentNullException.ThrowIfNull(example);
        _lMarkupCardEntries = rig.LRigEntries;
        _lMarkupCardExample = example;
    }

    public IReadOnlyList<LMarkupCard> LMarkupCardCreate(IReadOnlyList<LCardDraft> cards)
    {
        List<LMarkupCard> written = new(cards.Count);
        foreach (LCardDraft card in cards)
        {
            List<LMarkupSentence> sentences = new(card.LCardDraftSentence.Count);
            foreach (LSentenceDraft sentence in card.LCardDraftSentence)
            {
                sentences.Add(new LMarkupSentence(
                    sentence.LSentenceDraftExample is LExampleDraft example
                        ? _lMarkupCardExample.LMarkupExampleCreate(example)
                        : null,
                    sentence.LSentenceDraftParticle,
                    sentence.LSentenceDraftDependence));
            }

            List<LSituationDraft> situations = new(card.LCardDraftSituation.Count);
            foreach (LSituationDraft situation in card.LCardDraftSituation)
            {
                situations.Add(situation with { LSituationDraftId = 0 });
            }

            List<LRegisterDraft> registers = new(card.LCardDraftRegister.Count);
            foreach (LRegisterDraft register in card.LCardDraftRegister)
            {
                registers.Add(register with { LRegisterDraftId = 0 });
            }

            List<LTagDraft> tags = new(card.LCardDraftTag.Count);
            foreach (LTagDraft tag in card.LCardDraftTag)
            {
                tags.Add(tag with { LTagDraftId = 0 });
            }

            List<LImageDraft> images = new(card.LCardDraftImage.Count);
            foreach (LImageDraft image in card.LCardDraftImage)
            {
                images.Add(image with { LImageDraftId = 0 });
            }

            List<LVideoDraft> videos = new(card.LCardDraftVideo.Count);
            foreach (LVideoDraft video in card.LCardDraftVideo)
            {
                videos.Add(video with { LVideoDraftId = 0 });
            }

            written.Add(new LMarkupCard(
                card.LCardDraftTitle,
                card.LCardDraftExpression,
                card.LCardDraftMeaning,
                sentences,
                situations,
                registers,
                LMarkupTranslationCreate(card.LCardDraftTranslation),
                tags,
                images,
                videos,
                LMarkupCardCreate(card.LCardDraftChild)));
        }

        return written;
    }

    private IReadOnlyList<LMarkupTranslation> LMarkupTranslationCreate(IReadOnlyList<long> ids)
    {
        List<LMarkupTranslation> translations = new(ids.Count);
        foreach (long id in ids)
        {
            if (_lMarkupCardEntries.LEntryRead(id) is LEntry entry)
            {
                translations.Add(new LMarkupTranslation(entry.LEntryHeadword, entry.LEntryLanguage));
            }
        }

        return translations;
    }
}
