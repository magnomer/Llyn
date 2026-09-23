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
        lock (LEngineGate)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentOutOfRangeException.ThrowIfZero(request.LRequestDraftId);
            LEngineDraftValidate(request.LRequestDraftId);

            LDraft held = _lEngineStaff.LEngineStaffClaim.LClaimClerkLoad(request.LRequestDraftId);
            LDraft draft = _lEngineStaff.LEngineStaffDraft.LDraftClerkApply(held, request);
            if (request is LRequestHeadword or LRequestLanguage)
            {
                draft = LEngineAudioClear(held, draft);
            }

            saved = draft with
            {
                LDraftContent = _lEngineStaff.LEngineStaffDraft.LDraftClerkNormalize(draft.LDraftContent),
            };
            if (saved == held)
            {
                return held;
            }

            LEngineChronicleRecord(held, saved, request);
            _lEngineStaff.LEngineStaffClaim.LDraftSave(saved);
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

        _lEngineStaff.LEngineStaffChronicle.LChronicleClerkRecord(held, saved, request);
    }

    internal LDraft? LEngineChronicleUndo(long id)
    {
        LDraft? restored;
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            restored = _lEngineStaff.LEngineStaffChronicle.LChronicleClerkUndo(id);
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
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);
            restored = _lEngineStaff.LEngineStaffChronicle.LChronicleClerkRedo(id);
        }

        if (restored is not null)
        {
            LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal bool LEngineUndoCheck(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffChronicle.LChronicleUndoCheck(id);
        }
    }

    internal bool LEngineRedoCheck(long id)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffChronicle.LChronicleRedoCheck(id);
        }
    }

    internal IReadOnlyList<LCardDraft> LEngineDraftMove(long id, bool collocation, int from, int target)
    {
        lock (LEngineGate)
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
        lock (LEngineGate)
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
        lock (LEngineGate)
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
        _lEngineStaff.LEngineStaffClaim.LDraftSave(saved);
        return cards;
    }

    internal LCourt LEngineCourtSave(
        long ownerId, long targetId, string headword, string language)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffCourt.LCourtClerkSave(ownerId, targetId, headword, language);
        }
    }

    public LCourt LEngineCourtStart(
        long ownerId, string origin, string headword, string language)
    {
        lock (LEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(ownerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);
            LEngineDraftValidate(ownerId);

            string named = language ?? string.Empty;
            LDraft target = LEngineDraftStart(origin, null);

            try
            {
                _lEngineStaff.LEngineStaffClaim.LDraftSave(target with
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
        lock (LEngineGate)
        {
            _lEngineStaff.LEngineStaffCourt.LCourtClerkDelete(linkId);
        }
    }

    public LCourt? LEngineCourtFind(long ownerId, long targetId)
    {
        lock (LEngineGate)
        {
            return _lEngineStaff.LEngineStaffCourt.LCourtClerkFind(ownerId, targetId);
        }
    }
}
