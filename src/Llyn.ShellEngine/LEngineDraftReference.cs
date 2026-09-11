using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            long reference = referenceId is null or <= 0 ? 0 : referenceId.Value;
            LReference content = reference == 0
                ? LEngineReferenceBlank with { LReferenceId = LIdentity.LIdentityCreate() }
                : LEngineReferenceRead(reference) ?? throw new LRefusal(LRefusal.LRefusalReference);

            LDraft draft = new(
                LIdentity.LIdentityCreate(),
                origin,
                reference,
                LEngineDraftBlank,
                DateTimeOffset.UtcNow,
                null,
                null,
                content);

            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
            LClaimArchive.LClaimArchiveSave(
                _lEngineWorkspace, LClaimArchive.LClaimArchiveCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    public LReference LEngineReferenceSave(LDraft draft)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);
            LEngineDraftValidate(draft.LDraftId);

            LReference sent = draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference);
            LReference content = LEngineReferenceNormalize(sent);
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftReference = content });
            return content;
        }
    }

    public LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LReference sending = LEngineReferenceNormalize(
                draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference));

            LReference stored;
            if (draft.LDraftEntry == 0 || LEngineReferenceRead(draft.LDraftEntry) is null)
            {
                stored = LEngineReferenceCreate(sending);
            }
            else
            {
                LReference written = sending with { LReferenceId = draft.LDraftEntry };
                LEngineReferenceUpdate(written);
                stored = LEngineReferenceRead(draft.LDraftEntry) ?? written;
            }

            LDraftArchive.LDraftArchiveSave(
                _lEngineWorkspace,
                draft with { LDraftEntry = stored.LReferenceId, LDraftReference = stored });

            _lEngineDraftHeld.Remove(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
            settled = stored;
        }

        LEngineBulletinRaise(LSubject.LSubjectReference, settled.LReferenceId);
        return settled;
    }

    private bool LEngineReferenceCheck(LDraft draft, LReference held)
    {
        LReference origin = draft.LDraftEntry == 0
            ? LEngineReferenceBlank
            : LEngineReferenceRead(draft.LDraftEntry) ?? LEngineReferenceBlank;

        return !LEngineReferenceMatch(origin, held);
    }

    private static LReference LEngineReferenceNormalize(LReference content)
    {
        return content.LReferenceId == 0
            ? content with { LReferenceId = LIdentity.LIdentityCreate() }
            : content;
    }

    private static LReference LEngineReferenceBlank =>
        new(
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LReferenceKind.LReferenceKindUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified);

    private static bool LEngineReferenceMatch(LReference one, LReference other)
    {
        return one.LReferenceTitle == other.LReferenceTitle
            && one.LReferenceYear == other.LReferenceYear
            && one.LReferenceKind == other.LReferenceKind
            && one.LReferenceNote == other.LReferenceNote
            && one.LReferenceUrl == other.LReferenceUrl
            && one.LReferenceAuthorState == other.LReferenceAuthorState;
    }
}
