using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LDraft LEngineReferenceStart(string origin, string? referenceId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            string reference = string.IsNullOrWhiteSpace(referenceId) ? string.Empty : referenceId;
            LReference content = reference.Length == 0
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

    public LReference LEngineReferenceCommit(string id)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LReference sending = LEngineReferenceNormalize(
                draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference));

            LReference stored;
            if (draft.LDraftEntry.Length == 0 || LEngineReferenceRead(draft.LDraftEntry) is null)
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
            return stored;
        }
    }

    private bool LEngineReferenceCheck(LDraft draft, LReference held)
    {
        LReference origin = draft.LDraftEntry.Length == 0
            ? LEngineReferenceBlank
            : LEngineReferenceRead(draft.LDraftEntry) ?? LEngineReferenceBlank;

        return !LEngineReferenceMatch(origin, held);
    }

    private static LReference LEngineReferenceNormalize(LReference content)
    {
        return content.LReferenceId.Length == 0
            ? content with { LReferenceId = LIdentity.LIdentityCreate() }
            : content;
    }

    private static LReference LEngineReferenceBlank =>
        new(
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LState.LStateUnspecified);

    private static bool LEngineReferenceMatch(LReference one, LReference other)
    {
        return one.LReferenceTitle == other.LReferenceTitle
            && one.LReferenceProgram == other.LReferenceProgram
            && one.LReferenceChannel == other.LReferenceChannel
            && one.LReferenceYear == other.LReferenceYear
            && one.LReferenceUrl == other.LReferenceUrl
            && one.LReferenceAuthorState == other.LReferenceAuthorState;
    }
}
