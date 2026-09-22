using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkMint
{
    private readonly LIdentity _lDraftMintIdentity;

    public LDraftClerkMint(LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftMintIdentity = identity;
    }

    public LEntryDraft LDraftNormalize(LEntryDraft content)
    {
        ArgumentNullException.ThrowIfNull(content);

        IReadOnlyList<LPronunciationDraft> spoken = LPronunciationNormalize(content.LEntryDraftPronunciations);
        IReadOnlyList<LTranscriptionDraft> spelled = LTranscriptionNormalize(content.LEntryDraftTranscriptions);
        IReadOnlyList<LReflexDraft> reflexes = LReflexNormalize(content.LEntryDraftReflexes);
        LEtymologyDraft etymology = LEtymologyNormalize(content.LEntryDraftEtymology);
        IReadOnlyList<LCardDraft> meanings = LCardNormalize(content.LEntryDraftMeanings);
        IReadOnlyList<LCardDraft> collocations = LCardNormalize(content.LEntryDraftCollocations);

        return ReferenceEquals(etymology, content.LEntryDraftEtymology)
            && ReferenceEquals(spoken, content.LEntryDraftPronunciations)
            && ReferenceEquals(spelled, content.LEntryDraftTranscriptions)
            && ReferenceEquals(reflexes, content.LEntryDraftReflexes)
            && ReferenceEquals(meanings, content.LEntryDraftMeanings)
            && ReferenceEquals(collocations, content.LEntryDraftCollocations)
            ? content
            : content with
            {
                LEntryDraftPronunciations = spoken,
                LEntryDraftTranscriptions = spelled,
                LEntryDraftReflexes = reflexes,
                LEntryDraftEtymology = etymology,
                LEntryDraftMeanings = meanings,
                LEntryDraftCollocations = collocations,
            };
    }

    public IReadOnlyList<LReflexDraft> LReflexNormalize(IReadOnlyList<LReflexDraft> drafts)
    {
        ArgumentNullException.ThrowIfNull(drafts);

        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static draft => draft.LReflexDraftId == 0 && draft.LReflexDraftEmpty,
            draft => draft.LReflexDraftId == 0 ? draft with { LReflexDraftId = LIdentityCreate() } : draft);
    }

    public LEtymologyDraft LEtymologyNormalize(LEtymologyDraft etymology)
    {
        ArgumentNullException.ThrowIfNull(etymology);

        IReadOnlyList<LMentionDraft> mentions = LDraftClerkList.LDraftListNormalize(
            etymology.LEtymologyDraftMentions,
            static mention => !mention.LMentionDraftLinked,
            mention => mention.LMentionDraftId == 0
                ? mention with { LMentionDraftId = LIdentityCreate() }
                : mention);

        return ReferenceEquals(mentions, etymology.LEtymologyDraftMentions)
            ? etymology
            : etymology with { LEtymologyDraftMentions = mentions };
    }

    private long LIdentityCreate()
    {
        return _lDraftMintIdentity.LIdentityCreate();
    }

    private IReadOnlyList<LPronunciationDraft> LPronunciationNormalize(IReadOnlyList<LPronunciationDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static draft => draft.LPronunciationDraftId == 0 && draft.LPronunciationDraftEmpty,
            draft => draft.LPronunciationDraftId == 0
                ? draft with { LPronunciationDraftId = LIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LTranscriptionDraft> LTranscriptionNormalize(IReadOnlyList<LTranscriptionDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static draft => draft.LTranscriptionDraftId == 0
                && draft.LTranscriptionDraftEmpty
                && string.IsNullOrWhiteSpace(draft.LTranscriptionDraftScheme),
            draft => draft.LTranscriptionDraftId == 0
                ? draft with { LTranscriptionDraftId = LIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LCardDraft> LCardNormalize(IReadOnlyList<LCardDraft> cards)
    {
        return LDraftClerkList.LDraftListNormalize(cards, static _ => false, card =>
        {
            IReadOnlyList<LSentenceDraft> sentences = LSentenceNormalize(card.LCardDraftSentence);
            IReadOnlyList<LSituationDraft> situations = LSituationNormalize(card.LCardDraftSituation);
            IReadOnlyList<LRegisterDraft> registers = LRegisterNormalize(card.LCardDraftRegister);
            IReadOnlyList<LTagDraft> tags = LTagNormalize(card.LCardDraftTag);
            IReadOnlyList<LImageDraft> images = LImageNormalize(card.LCardDraftImage);
            IReadOnlyList<LVideoDraft> videos = LVideoNormalize(card.LCardDraftVideo);
            IReadOnlyList<LCardDraft> children = LCardNormalize(card.LCardDraftChild);

            if (card.LCardDraftId != 0
                && ReferenceEquals(sentences, card.LCardDraftSentence)
                && ReferenceEquals(situations, card.LCardDraftSituation)
                && ReferenceEquals(registers, card.LCardDraftRegister)
                && ReferenceEquals(tags, card.LCardDraftTag)
                && ReferenceEquals(images, card.LCardDraftImage)
                && ReferenceEquals(videos, card.LCardDraftVideo)
                && ReferenceEquals(children, card.LCardDraftChild))
            {
                return card;
            }

            return card with
            {
                LCardDraftId = card.LCardDraftId == 0 ? LIdentityCreate() : card.LCardDraftId,
                LCardDraftSentence = sentences,
                LCardDraftSituation = situations,
                LCardDraftRegister = registers,
                LCardDraftTag = tags,
                LCardDraftImage = images,
                LCardDraftVideo = videos,
                LCardDraftChild = children,
            };
        });
    }

    private IReadOnlyList<LSentenceDraft> LSentenceNormalize(IReadOnlyList<LSentenceDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(drafts, static _ => false, draft =>
        {
            LExampleDraft? example = draft.LSentenceDraftExample;
            if (example is not null && example.LExampleDraftId == 0)
            {
                example = example.LExampleDraftText.LStateValueEmpty
                    ? null
                    : example with { LExampleDraftId = LIdentityCreate() };
            }

            if (draft.LSentenceDraftId != 0 && ReferenceEquals(example, draft.LSentenceDraftExample))
            {
                return draft;
            }

            return draft with
            {
                LSentenceDraftExample = example,
                LSentenceDraftId = draft.LSentenceDraftId == 0 ? LIdentityCreate() : draft.LSentenceDraftId,
            };
        });
    }

    private IReadOnlyList<LSituationDraft> LSituationNormalize(IReadOnlyList<LSituationDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static draft => draft.LSituationDraftTitle.LStateValueEmpty,
            draft => draft.LSituationDraftId == 0
                ? draft with { LSituationDraftId = LIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LRegisterDraft> LRegisterNormalize(IReadOnlyList<LRegisterDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static draft => draft.LRegisterDraftName.LStateValueEmpty,
            draft => draft.LRegisterDraftId == 0
                ? draft with { LRegisterDraftId = LIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LTagDraft> LTagNormalize(IReadOnlyList<LTagDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static draft => draft.LTagDraftId == 0 && string.IsNullOrWhiteSpace(draft.LTagDraftText),
            draft => draft.LTagDraftId == 0
                ? draft with { LTagDraftId = LIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LImageDraft> LImageNormalize(IReadOnlyList<LImageDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static _ => false,
            draft => draft.LImageDraftId == 0
                ? draft with { LImageDraftId = LIdentityCreate() }
                : draft);
    }

    private IReadOnlyList<LVideoDraft> LVideoNormalize(IReadOnlyList<LVideoDraft> drafts)
    {
        return LDraftClerkList.LDraftListNormalize(
            drafts,
            static _ => false,
            draft => draft.LVideoDraftId == 0
                ? draft with { LVideoDraftId = LIdentityCreate() }
                : draft);
    }
}
