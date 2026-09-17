using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LDraft LEngineRequestApply(LRequest request)
    {
        LDraft saved;
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentOutOfRangeException.ThrowIfZero(request.LRequestDraftId);
            LEngineDraftValidate(request.LRequestDraftId);

            if (!_lEngineDraftHeld.Contains(request.LRequestDraftId))
            {
                throw new LRefusal(LRefusal.LRefusalDraft);
            }

            LDraft held = LEngineDraftLoad(request.LRequestDraftId);
            LDraft draft = LEngineRequestApply(held, request);
            saved = draft with { LDraftContent = LEngineDraftNormalize(draft.LDraftContent) };
            if (saved == held)
            {
                return held;
            }

            LEngineChronicleRecord(held, saved, request);
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, saved);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
        return saved;
    }

    private LDraft LEngineRequestApply(LDraft draft, LRequest request)
    {
        if (draft.LDraftSituation is LSituation held
            && LEngineSituationDispatch(held, request) is LSituation dispatched)
        {
            return draft with { LDraftSituation = dispatched };
        }

        return request switch
        {
            LRequestExampleText sent => LEngineExampleChange(
                draft, example => LEngineMentionUpdate(example, LEngineValueRead(sent.LRequestValue))),
            LRequestMentionAddition sent => LEngineMentionAdd(draft, sent),
            LRequestMentionRemoval sent => LEngineMentionRemove(draft, sent),
            LRequestMentionSense sent => LEngineMentionChange(draft, sent),
            LRequestGlossAddition sent => LEngineGlossAdd(draft, sent),
            LRequestGlossRemoval sent => LEngineGlossRemove(draft, sent),
            LRequestGlossText sent => LEngineGlossChange(draft, sent),
            LRequestGlossLanguage sent => LEngineGlossChange(draft, sent),
            LRequestExampleLanguage sent => LEngineExampleChange(
                draft, example => example with { LExampleLanguage = sent.LRequestLanguage ?? string.Empty }),
            LRequestExampleReference sent => LEngineExampleChange(
                draft,
                example => example with { LExampleSource = LStateAnchor.LStateAnchorRead(sent.LRequestReferenceId) }),
            LRequestReferenceTitle sent => LEngineReferenceChange(
                draft, reference => reference with { LReferenceTitle = LEngineValueRead(sent.LRequestValue) }),
            LRequestReferenceYear sent => LEngineReferenceChange(
                draft, reference => reference with { LReferenceYear = LEngineValueRead(sent.LRequestValue) }),
            LRequestReferenceKind sent => LEngineReferenceChange(
                draft, reference => reference with { LReferenceKind = sent.LRequestKind }),
            LRequestReferenceNote sent => LEngineReferenceChange(
                draft, reference => reference with { LReferenceNote = LEngineValueRead(sent.LRequestValue) }),
            LRequestReferenceUrl sent => LEngineReferenceChange(
                draft, reference => reference with { LReferenceUrl = LEngineValueRead(sent.LRequestValue) }),
            LRequestAuthorState sent => LEngineReferenceChange(
                draft,
                reference => reference with
                {
                    LReferenceAuthorState = LStateMark.LStateMarkRead(sent.LRequestState),
                }),
            LRequestReferenceBody sent => LEngineReferenceChange(
                draft, reference => LEngineBodyApply(reference, sent)),
            LRequestExampleBody sent => LEngineExampleChange(
                draft, example => LEngineBodyApply(example, sent)),
            LRequestSituationBody sent => LEngineSituationChange(
                draft,
                draft.LDraftSituation?.LSituationId ?? sent.LRequestSituationId,
                situation => LEngineBodyApply(situation, sent)),
            LRequestAuthorAddition sent => LEngineAuthorAdd(draft, sent),
            LRequestAuthorPick sent => LEngineAuthorInsert(draft, sent),
            LRequestAuthorRemoval sent => LEngineAuthorRemove(draft, sent.LRequestAuthorId),
            LRequestAuthorShift sent => LEngineAuthorMove(draft, sent),
            LRequestSituationTitle sent => LEngineSituationChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationTitle = LEngineValueRead(sent.LRequestValue) }),
            LRequestSituationDescription sent => LEngineSituationChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationDescription = LEngineValueRead(sent.LRequestValue) }),
            LRequestSituationKind sent => LEngineSituationChange(
                draft,
                sent.LRequestSituationId,
                situation => situation with { LSituationKind = LEngineValueRead(sent.LRequestValue) }),
            LRequestHeadword or LRequestLanguage => LEngineAudioClear(draft, request),
            _ => draft with { LDraftContent = LEngineRequestApply(draft.LDraftContent, request) },
        };
    }

    private LEntryDraft LEngineRequestApply(LEntryDraft content, LRequest request)
    {
        return request switch
        {
            LRequestHeadword sent => content with { LEntryDraftHeadword = sent.LRequestText ?? string.Empty },
            LRequestLanguage sent => LEngineAnatomyRebuild(LEngineRespellingRebuild(
                content with { LEntryDraftLanguage = sent.LRequestText ?? string.Empty })),
            LRequestNote sent => content with { LEntryDraftNote = sent.LRequestText ?? string.Empty },
            LRequestSpeech sent => content with { LEntryDraftSpeeches = sent.LRequestSpeeches ?? [] },
            LRequestCardAddition sent => LEngineCardInsert(content, sent),
            LRequestCardRemoval sent => LEngineCardRemove(content, sent.LRequestCardId),
            LRequestCardShift sent => LEngineCardMove(content, sent),
            LRequestCardTitle sent => LEngineCardChange(
                content,
                sent.LRequestCardId,
                card => card with { LCardDraftTitle = LEngineValueRead(sent.LRequestValue) }),
            LRequestCardExpression sent => LEngineCardChange(
                content,
                sent.LRequestCardId,
                card => card with { LCardDraftExpression = LEngineValueRead(sent.LRequestValue) }),
            LRequestCardMeaning sent => LEngineCardChange(
                content,
                sent.LRequestCardId,
                card => card with { LCardDraftMeaning = LEngineValueRead(sent.LRequestValue) }),
            _ => LEngineReadingApply(content, request),
        };
    }

    private static LStateValue LEngineValueRead(LStateWritten? written)
    {
        return written?.LStateWrittenResolve() ?? LStateValue.LStateValueUnspecified;
    }

    private LEntryDraft LEngineCardInsert(LEntryDraft content, LRequestCardAddition request)
    {
        LCardDraft card = new(
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            [],
            [],
            [],
            [],
            [],
            [],
            [],
            0,
            LEngineIdentityCreate());

        return LEngineCardInsert(
            content, request.LRequestKind, request.LRequestParentId, request.LRequestPosition, card);
    }

    private static LEntryDraft LEngineCardInsert(
        LEntryDraft content, LCardKind kind, long parentId, int position, LCardDraft card)
    {
        if (kind == LCardKind.LCardKindCollocation)
        {
            if (parentId != 0)
            {
                throw new LRefusal(LRefusal.LRefusalCollocation);
            }

            return content with
            {
                LEntryDraftCollocations = LEngineCardInsert(content.LEntryDraftCollocations, card, position),
            };
        }

        if (parentId == 0)
        {
            return content with
            {
                LEntryDraftMeanings = LEngineCardInsert(content.LEntryDraftMeanings, card, position),
            };
        }

        IReadOnlyList<LCardDraft> meanings = LEngineCardChange(
            content.LEntryDraftMeanings,
            parentId,
            parent => parent with { LCardDraftChild = LEngineCardInsert(parent.LCardDraftChild, card, position) })
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return content with { LEntryDraftMeanings = meanings };
    }

    private static IReadOnlyList<LCardDraft> LEngineCardInsert(
        IReadOnlyList<LCardDraft> cards, LCardDraft card, int position)
    {
        List<LCardDraft> written = new(cards);
        written.Insert(Math.Clamp(position, 0, written.Count), card);
        return LEnginePositionUpdate(written);
    }

    private static LEntryDraft LEngineCardRemove(LEntryDraft content, long id)
    {
        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        IReadOnlyList<LCardDraft>? meanings = LEngineCardRemove(content.LEntryDraftMeanings, id, out _);
        if (meanings is not null)
        {
            return content with { LEntryDraftMeanings = meanings };
        }

        IReadOnlyList<LCardDraft> collocations = LEngineCardRemove(content.LEntryDraftCollocations, id, out _)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return content with { LEntryDraftCollocations = collocations };
    }

    private static LEntryDraft LEngineCardMove(LEntryDraft content, LRequestCardShift request)
    {
        if (request.LRequestCardId == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        LCardDraft? moved;
        LCardKind kind = LCardKind.LCardKindMeaning;
        IReadOnlyList<LCardDraft>? cards = LEngineCardRemove(
            content.LEntryDraftMeanings, request.LRequestCardId, out moved);
        if (cards is not null)
        {
            content = content with { LEntryDraftMeanings = cards };
        }
        else
        {
            kind = LCardKind.LCardKindCollocation;
            cards = LEngineCardRemove(content.LEntryDraftCollocations, request.LRequestCardId, out moved)
                ?? throw new LRefusal(LRefusal.LRefusalCard);
            content = content with { LEntryDraftCollocations = cards };
        }

        return LEngineCardInsert(content, kind, request.LRequestParentId, request.LRequestPosition, moved!);
    }

    private static IReadOnlyList<LCardDraft>? LEngineCardRemove(
        IReadOnlyList<LCardDraft> cards, long id, out LCardDraft? removed)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            if (card.LCardDraftId == id)
            {
                removed = card;
                List<LCardDraft> written = new(cards);
                written.RemoveAt(index);
                return LEnginePositionUpdate(written);
            }

            IReadOnlyList<LCardDraft>? children = LEngineCardRemove(card.LCardDraftChild, id, out removed);
            if (children is null)
            {
                continue;
            }

            List<LCardDraft> kept = new(cards);
            kept[index] = card with { LCardDraftChild = children };
            return kept;
        }

        removed = null;
        return null;
    }

    private static LEntryDraft LEngineCardChange(LEntryDraft content, long id, Func<LCardDraft, LCardDraft> change)
    {
        if (id == 0)
        {
            throw new LRefusal(LRefusal.LRefusalCard);
        }

        IReadOnlyList<LCardDraft>? meanings = LEngineCardChange(content.LEntryDraftMeanings, id, change);
        if (meanings is not null)
        {
            return content with { LEntryDraftMeanings = meanings };
        }

        IReadOnlyList<LCardDraft> collocations = LEngineCardChange(content.LEntryDraftCollocations, id, change)
            ?? throw new LRefusal(LRefusal.LRefusalCard);

        return content with { LEntryDraftCollocations = collocations };
    }

    private static IReadOnlyList<LCardDraft>? LEngineCardChange(
        IReadOnlyList<LCardDraft> cards, long id, Func<LCardDraft, LCardDraft> change)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            LCardDraft? written = null;

            if (card.LCardDraftId == id)
            {
                written = change(card);
            }
            else if (LEngineCardChange(card.LCardDraftChild, id, change) is IReadOnlyList<LCardDraft> children)
            {
                written = card with { LCardDraftChild = children };
            }

            if (written is null)
            {
                continue;
            }

            List<LCardDraft> kept = new(cards);
            kept[index] = written;
            return kept;
        }

        return null;
    }

    private static IReadOnlyList<LCardDraft> LEnginePositionUpdate(List<LCardDraft> cards)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            if (cards[index].LCardDraftPosition != index + 1)
            {
                cards[index] = cards[index] with { LCardDraftPosition = index + 1 };
            }
        }

        return cards;
    }
}
