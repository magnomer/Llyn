using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineDraftNormalize(LEntryDraft content)
    {
        LPronunciationDraft? spoken = LEngineSoundNormalize(content.LEntryDraftPronunciation);
        IReadOnlyList<LCardDraft> meanings = LEngineCardNormalize(content.LEntryDraftMeanings);
        IReadOnlyList<LCardDraft> collocations = LEngineCardNormalize(content.LEntryDraftCollocations);

        return ReferenceEquals(spoken, content.LEntryDraftPronunciation)
            && ReferenceEquals(meanings, content.LEntryDraftMeanings)
            && ReferenceEquals(collocations, content.LEntryDraftCollocations)
            ? content
            : content with
            {
                LEntryDraftPronunciation = spoken,
                LEntryDraftMeanings = meanings,
                LEntryDraftCollocations = collocations,
            };
    }

    private LPronunciationDraft? LEngineSoundNormalize(LPronunciationDraft? spoken)
    {
        if (spoken is null || spoken.LPronunciationDraftEmpty)
        {
            return null;
        }

        return spoken.LPronunciationDraftId == 0
            ? spoken with { LPronunciationDraftId = LEngineIdentityCreate() }
            : spoken;
    }

    private IReadOnlyList<LCardDraft> LEngineCardNormalize(IReadOnlyList<LCardDraft> cards)
    {
        return LEngineListNormalize(cards, static _ => false, card =>
        {
            IReadOnlyList<LSentenceDraft> sentences = LEngineSentenceNormalize(card.LCardDraftSentence);
            IReadOnlyList<LSituationDraft> situations = LEngineSituationNormalize(card.LCardDraftSituation);
            IReadOnlyList<LRegisterDraft> registers = LEngineRegisterNormalize(card.LCardDraftRegister);
            IReadOnlyList<LTagDraft> tags = LEngineTagNormalize(card.LCardDraftTag);
            IReadOnlyList<LImageDraft> images = LEngineImageNormalize(card.LCardDraftImage);
            IReadOnlyList<LVideoDraft> videos = LEngineVideoNormalize(card.LCardDraftVideo);
            IReadOnlyList<LRelationDraft> relations = LEngineRelationNormalize(card.LCardDraftRelation);
            IReadOnlyList<LSynonymDraft> synonyms = LEngineSynonymNormalize(card.LCardDraftInterlink);
            IReadOnlyList<LCardDraft> children = LEngineCardNormalize(card.LCardDraftChild);

            if (card.LCardDraftId != 0
                && ReferenceEquals(sentences, card.LCardDraftSentence)
                && ReferenceEquals(situations, card.LCardDraftSituation)
                && ReferenceEquals(registers, card.LCardDraftRegister)
                && ReferenceEquals(tags, card.LCardDraftTag)
                && ReferenceEquals(images, card.LCardDraftImage)
                && ReferenceEquals(videos, card.LCardDraftVideo)
                && ReferenceEquals(relations, card.LCardDraftRelation)
                && ReferenceEquals(synonyms, card.LCardDraftInterlink)
                && ReferenceEquals(children, card.LCardDraftChild))
            {
                return card;
            }

            return card with
            {
                LCardDraftId = card.LCardDraftId == 0 ? LEngineIdentityCreate() : card.LCardDraftId,
                LCardDraftSentence = sentences,
                LCardDraftSituation = situations,
                LCardDraftRegister = registers,
                LCardDraftTag = tags,
                LCardDraftImage = images,
                LCardDraftVideo = videos,
                LCardDraftRelation = relations,
                LCardDraftInterlink = synonyms,
                LCardDraftChild = children,
            };
        });
    }

    private IReadOnlyList<LSentenceDraft> LEngineSentenceNormalize(IReadOnlyList<LSentenceDraft> drafts)
    {
        return LEngineListNormalize(drafts, static draft => draft.LSentenceDraftEmpty, draft =>
        {
            LExampleDraft? example = draft.LSentenceDraftExample;
            if (example is not null && example.LExampleDraftId == 0)
            {
                example = example.LExampleDraftText.LStateValueEmpty
                    ? null
                    : example with { LExampleDraftId = LEngineIdentityCreate() };
            }

            if (draft.LSentenceDraftId != 0 && ReferenceEquals(example, draft.LSentenceDraftExample))
            {
                return draft;
            }

            return draft with
            {
                LSentenceDraftExample = example,
                LSentenceDraftId = draft.LSentenceDraftId == 0 ? LEngineIdentityCreate() : draft.LSentenceDraftId,
            };
        });
    }

    private IReadOnlyList<LSituationDraft> LEngineSituationNormalize(IReadOnlyList<LSituationDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static draft => draft.LSituationDraftTitle.LStateValueEmpty,
            draft => draft.LSituationDraftId == 0
                ? draft with { LSituationDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LRegisterDraft> LEngineRegisterNormalize(IReadOnlyList<LRegisterDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static draft => draft.LRegisterDraftName.LStateValueEmpty,
            draft => draft.LRegisterDraftId == 0
                ? draft with { LRegisterDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LTagDraft> LEngineTagNormalize(IReadOnlyList<LTagDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static draft => draft.LTagDraftId == 0 && string.IsNullOrWhiteSpace(draft.LTagDraftText),
            draft => draft.LTagDraftId == 0
                ? draft with { LTagDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LImageDraft> LEngineImageNormalize(IReadOnlyList<LImageDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static draft => draft.LImageDraftEmpty,
            draft => draft.LImageDraftId == 0
                ? draft with { LImageDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LVideoDraft> LEngineVideoNormalize(IReadOnlyList<LVideoDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static draft => draft.LVideoDraftEmpty,
            draft => draft.LVideoDraftId == 0
                ? draft with { LVideoDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LRelationDraft> LEngineRelationNormalize(IReadOnlyList<LRelationDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static _ => false,
            draft => draft.LRelationDraftId == 0
                ? draft with { LRelationDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LSynonymDraft> LEngineSynonymNormalize(IReadOnlyList<LSynonymDraft> drafts)
    {
        return LEngineListNormalize(
            drafts,
            static _ => false,
            draft => draft.LSynonymDraftId == 0
                ? draft with { LSynonymDraftId = LEngineIdentityCreate() }
                : draft);
    }

    private static IReadOnlyList<LEngineItem> LEngineListNormalize<LEngineItem>(
        IReadOnlyList<LEngineItem> items,
        Func<LEngineItem, bool> blank,
        Func<LEngineItem, LEngineItem> name)
        where LEngineItem : class
    {
        List<LEngineItem>? named = null;
        for (int index = 0; index < items.Count; index++)
        {
            LEngineItem item = items[index];
            LEngineItem? written = blank(item) ? null : name(item);
            if (ReferenceEquals(written, item))
            {
                named?.Add(item);
                continue;
            }

            if (named is null)
            {
                named = new List<LEngineItem>(items.Count);
                for (int earlier = 0; earlier < index; earlier++)
                {
                    named.Add(items[earlier]);
                }
            }

            if (written is not null)
            {
                named.Add(written);
            }
        }

        return named ?? items;
    }

    private static void LEngineIdentityRecord(Dictionary<long, long> identity, long draftId, long rowId)
    {
        if (draftId < 0)
        {
            identity[draftId] = rowId;
        }
    }
}
