using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

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

            LDraft held = _lEngineClaimClerk.LClaimClerkLoad(request.LRequestDraftId);
            LDraft draft = _lEngineDraftClerk.LDraftClerkApply(held, request);
            if (request is LRequestHeadword or LRequestLanguage)
            {
                draft = LEngineAudioClear(held, draft);
            }

            saved = draft with { LDraftContent = _lEngineDraftClerk.LDraftClerkNormalize(draft.LDraftContent) };
            if (saved == held)
            {
                return held;
            }

            LEngineChronicleRecord(held, saved, request);
            _lEngineClaimClerk.LDraftSave(saved);
        }

        LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
        return saved;
    }

    private LDraft LEngineAudioClear(LDraft held, LDraft draft)
    {
        LEntryDraft before = held.LDraftContent;
        LEntryDraft content = draft.LDraftContent;
        if (!string.Equals(before.LEntryDraftLanguage, content.LEntryDraftLanguage, StringComparison.Ordinal))
        {
            content = LDraftClerkReading.LAudioClear(content, null);
        }
        else if (!string.Equals(before.LEntryDraftHeadword, content.LEntryDraftHeadword, StringComparison.Ordinal))
        {
            LEntryDraft? stored = draft.LDraftEntryId <= 0 ? null : LEngineEntryLoad(draft.LDraftEntryId);
            content = LDraftClerkReading.LAudioClear(content, stored);
        }

        return draft with { LDraftContent = content };
    }

    private void LEngineChronicleRecord(LDraft held, LDraft saved, LRequest? request)
    {
        if (LDraftClerkEquality.LDraftMatch(held, saved) || (!LEngineDraftCheck(held) && !LEngineDraftCheck(saved)))
        {
            return;
        }

        _lEngineChronicleClerk.LChronicleClerkRecord(held, saved, request);
    }

    internal LDraft? LEngineChronicleUndo(long id)
    {
        LDraft? restored;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            restored = _lEngineChronicleClerk.LChronicleClerkUndo(id);
        }

        if (restored is not null)
        {
            LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal LDraft? LEngineChronicleRedo(long id)
    {
        LDraft? restored;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            restored = _lEngineChronicleClerk.LChronicleClerkRedo(id);
        }

        if (restored is not null)
        {
            LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal bool LEngineUndoCheck(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineChronicleClerk.LChronicleUndoCheck(id);
        }
    }

    internal bool LEngineRedoCheck(long id)
    {
        lock (_lEngineGate)
        {
            return _lEngineChronicleClerk.LChronicleRedoCheck(id);
        }
    }

    internal IReadOnlyList<LCardDraft> LEngineDraftMove(long id, bool collocation, int from, int target)
    {
        lock (_lEngineGate)
        {
            LEngineDraftValidate(id);
            LDraft draft = LEngineDraftLoad(id);
            List<LCardDraft> cards = new(
                collocation
                    ? draft.LDraftContent.LEntryDraftCollocations
                    : draft.LDraftContent.LEntryDraftMeanings);

            if (cards.Count != 0)
            {
                int origin = Math.Clamp(from, 0, cards.Count - 1);
                int landing = Math.Clamp(target, 0, cards.Count - 1);

                LCardDraft moved = cards[origin];
                cards.RemoveAt(origin);
                cards.Insert(landing, moved);
            }

            return LEngineCardApply(draft, collocation, cards);
        }
    }

    internal IReadOnlyList<LCardDraft> LEngineDraftNormalize(long id, bool collocation)
    {
        lock (_lEngineGate)
        {
            LEngineDraftValidate(id);
            LDraft draft = LEngineDraftLoad(id);
            List<LCardDraft> cards = new(
                collocation
                    ? draft.LDraftContent.LEntryDraftCollocations
                    : draft.LDraftContent.LEntryDraftMeanings);

            return LEngineCardApply(draft, collocation, cards);
        }
    }

    internal long LEngineCardCreate()
    {
        lock (_lEngineGate)
        {
            return LEngineIdentityCreate();
        }
    }

    private IReadOnlyList<LCardDraft> LEngineCardApply(
        LDraft draft, bool collocation, List<LCardDraft> cards)
    {
        for (int index = 0; index < cards.Count; index++)
        {
            LCardDraft card = cards[index];
            cards[index] = card with
            {
                LCardDraftPosition = index + 1,
                LCardDraftId = card.LCardDraftId == 0
                    ? LEngineIdentityCreate()
                    : card.LCardDraftId,
            };
        }

        LEntryDraft content = collocation
            ? draft.LDraftContent with { LEntryDraftCollocations = cards }
            : draft.LDraftContent with { LEntryDraftMeanings = cards };

        LDraft saved = draft with { LDraftContent = content };
        LEngineChronicleRecord(draft, saved, null);
        _lEngineClaimClerk.LDraftSave(saved);
        return cards;
    }

    internal LCourt LEngineCourtSave(
        long ownerId, long targetId, string headword, string language)
    {
        lock (_lEngineGate)
        {
            return _lEngineCourtClerk.LCourtClerkSave(ownerId, targetId, headword, language);
        }
    }

    public LCourt LEngineCourtStart(
        long ownerId, string origin, string headword, string language)
    {
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(ownerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);
            LEngineDraftValidate(ownerId);

            string named = language ?? string.Empty;
            LDraft target = LEngineDraftStart(origin, null);

            try
            {
                _lEngineClaimClerk.LDraftSave(target with
                {
                    LDraftContent = target.LDraftContent with
                    {
                        LEntryDraftHeadword = headword,
                        LEntryDraftLanguage = named,
                    },
                });

                return LEngineCourtSave(ownerId, target.LDraftId, headword, named);
            }
            catch (Exception)
            {
                LEngineDraftCancel(target.LDraftId);
                throw;
            }
        }
    }

    public void LEngineCourtDelete(long linkId)
    {
        lock (_lEngineGate)
        {
            _lEngineCourtClerk.LCourtClerkDelete(linkId);
        }
    }

    public LCourt? LEngineCourtFind(long ownerId, long targetId)
    {
        lock (_lEngineGate)
        {
            return _lEngineCourtClerk.LCourtClerkFind(ownerId, targetId);
        }
    }
}
