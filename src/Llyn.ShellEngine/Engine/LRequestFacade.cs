using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;

namespace Llyn.ShellEngine;

internal sealed class LRequestFacade
{
    private readonly LEngine _lRequestFacadeEngine;
    private readonly object _lRequestFacadeGate;

    private LEngineStaff LRequestFacadeStaff => _lRequestFacadeEngine.LEngineStaffHeld;

    public LRequestFacade(LEngine engine)
    {
        ArgumentNullException.ThrowIfNull(engine);
        _lRequestFacadeEngine = engine;
        _lRequestFacadeGate = engine.LEngineGate;
    }

    internal LDraft LEngineRequestApply(LRequest request)
    {
        LDraft saved;
        lock (_lRequestFacadeGate)
        {
            ArgumentNullException.ThrowIfNull(request);
            ArgumentOutOfRangeException.ThrowIfZero(request.LRequestDraftId);
            _lRequestFacadeEngine.LEngineDraft.LEngineDraftValidate(request.LRequestDraftId);

            LDraft held = LRequestFacadeStaff.LEngineStaffClaim.LClaimClerkLoad(request.LRequestDraftId);
            LDraft draft = LRequestFacadeStaff.LEngineStaffDraft.LDraftClerkApply(held, request);
            if (request is LRequestHeadword or LRequestLanguage)
            {
                draft = LEngineAudioClear(held, draft);
            }

            saved = draft with
            {
                LDraftContent = LRequestFacadeStaff.LEngineStaffDraft.LDraftClerkNormalize(draft.LDraftContent),
            };
            if (saved == held)
            {
                return held;
            }

            LEngineChronicleRecord(held, saved, request);
            LRequestFacadeStaff.LEngineStaffClaim.LDraftSave(saved);
        }

        _lRequestFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, saved.LDraftId);
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
            LEntryDraft? stored = draft.LDraftEntryId <= 0
                ? null
                : _lRequestFacadeEngine.LEngineEntry.LEngineEntryLoad(draft.LDraftEntryId);
            content = LDraftClerkReading.LAudioClear(content, stored);
        }

        return draft with { LDraftContent = content };
    }

    private void LEngineChronicleRecord(LDraft held, LDraft saved, LRequest? request)
    {
        LDraftFacade drafts = _lRequestFacadeEngine.LEngineDraft;
        if (LDraftClerkEquality.LDraftMatch(held, saved)
            || (!drafts.LEngineDraftCheck(held) && !drafts.LEngineDraftCheck(saved)))
        {
            return;
        }

        LRequestFacadeStaff.LEngineStaffChronicle.LChronicleClerkRecord(held, saved, request);
    }

    internal LDraft? LEngineChronicleUndo(long id)
    {
        LDraft? restored;
        lock (_lRequestFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lRequestFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            restored = LRequestFacadeStaff.LEngineStaffChronicle.LChronicleClerkUndo(id);
        }

        if (restored is not null)
        {
            _lRequestFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal LDraft? LEngineChronicleRedo(long id)
    {
        LDraft? restored;
        lock (_lRequestFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            _lRequestFacadeEngine.LEngineDraft.LEngineDraftValidate(id);
            restored = LRequestFacadeStaff.LEngineStaffChronicle.LChronicleClerkRedo(id);
        }

        if (restored is not null)
        {
            _lRequestFacadeEngine.LEngineBulletinRaise(LSubject.LSubjectDraft, restored.LDraftId);
        }

        return restored;
    }

    internal bool LEngineUndoCheck(long id)
    {
        lock (_lRequestFacadeGate)
        {
            return LRequestFacadeStaff.LEngineStaffChronicle.LChronicleUndoCheck(id);
        }
    }

    internal bool LEngineRedoCheck(long id)
    {
        lock (_lRequestFacadeGate)
        {
            return LRequestFacadeStaff.LEngineStaffChronicle.LChronicleRedoCheck(id);
        }
    }

    internal LCourt LEngineCourtSave(
        long ownerId, long targetId, string headword, string language)
    {
        lock (_lRequestFacadeGate)
        {
            return LRequestFacadeStaff.LEngineStaffCourt.LCourtClerkSave(ownerId, targetId, headword, language);
        }
    }

    internal LCourt LEngineCourtStart(
        long ownerId, string origin, string headword, string language)
    {
        lock (_lRequestFacadeGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(ownerId);
            ArgumentException.ThrowIfNullOrWhiteSpace(headword);
            _lRequestFacadeEngine.LEngineDraft.LEngineDraftValidate(ownerId);

            string named = language ?? string.Empty;
            LDraft target = _lRequestFacadeEngine.LEngineDraft.LEngineDraftStart(origin, null);

            try
            {
                LRequestFacadeStaff.LEngineStaffClaim.LDraftSave(target with
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
                _lRequestFacadeEngine.LEngineDraft.LEngineDraftCancel(target.LDraftId);
                throw;
            }
        }
    }

    internal void LEngineCourtDelete(long linkId)
    {
        lock (_lRequestFacadeGate)
        {
            LRequestFacadeStaff.LEngineStaffCourt.LCourtClerkDelete(linkId);
        }
    }

    internal LCourt? LEngineCourtFind(long ownerId, long targetId)
    {
        lock (_lRequestFacadeGate)
        {
            return LRequestFacadeStaff.LEngineStaffCourt.LCourtClerkFind(ownerId, targetId);
        }
    }
}

