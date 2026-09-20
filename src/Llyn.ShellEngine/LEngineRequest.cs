using System;
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

            if (!_lEngineDraftHeld.Contains(request.LRequestDraftId))
            {
                throw new LRefusal(LRefusal.LRefusalDraft);
            }

            LDraft held = LEngineDraftLoad(request.LRequestDraftId);
            LDraft draft = _lEngineDraftClerk.LDraftClerkApply(held, request);
            if (request is LRequestHeadword or LRequestLanguage)
            {
                draft = LEngineAudioClear(held, draft);
            }

            saved = draft with { LDraftContent = LEngineDraftNormalize(draft.LDraftContent) };
            if (saved == held)
            {
                return held;
            }

            LEngineChronicleRecord(held, saved, request);
            _lEngineDrafts.LDraftSave(saved);
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
}
