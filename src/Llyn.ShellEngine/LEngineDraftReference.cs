using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LDraft LEngineReferenceStart(string origin, long? referenceId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            long reference = referenceId is null or <= 0 ? 0 : referenceId.Value;
            LReference content = reference == 0
                ? LEngineReferenceBlank with { LReferenceId = LEngineIdentityCreate() }
                : LEngineReferenceRead(reference) ?? throw new LRefusal(LRefusal.LRefusalReference);

            LDraft draft = new(
                LEngineIdentityCreate(),
                origin,
                reference,
                LEngineDraftBlank,
                DateTimeOffset.UtcNow,
                null,
                null,
                content)
            {
                LDraftAuthor = reference == 0 ? [] : LEngineCreditRead(reference),
            };

            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
            LClaimArchive.LClaimArchiveSave(
                _lEngineWorkspace, LClaimArchive.LClaimArchiveCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    public LReference LEngineReferenceCommit(long id)
    {
        LReference settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LReference sending = LEngineReferenceNormalize(
                draft.LDraftReference ?? throw new LRefusal(LRefusal.LRefusalReference));

            LReference stored;
            using (LDatabaseSession session = _lEngineDatabase.LDatabaseSessionStart())
            {
                bool fresh = draft.LDraftEntryId == 0 || LEngineReferenceRead(draft.LDraftEntryId) is null;
                if (fresh)
                {
                    stored = LEngineReferenceCreate(sending);
                }
                else
                {
                    LReference written = sending with { LReferenceId = draft.LDraftEntryId };
                    LEngineReferenceUpdate(written);
                    stored = LEngineReferenceRead(draft.LDraftEntryId) ?? written;
                }

                LEngineCreditSave(stored.LReferenceId, draft.LDraftAuthor);
                LEngineRevisionRecord(
                    stored.LReferenceId,
                    "reference",
                    fresh ? "create" : "update",
                    stored.LReferenceTitle.LStateValueShow());
                session.LDatabaseSessionCommit();
            }

            LDraftArchive.LDraftArchiveSave(
                _lEngineWorkspace,
                draft with { LDraftEntryId = stored.LReferenceId, LDraftReference = stored });

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
        LReference origin = draft.LDraftEntryId == 0
            ? LEngineReferenceBlank
            : LEngineReferenceRead(draft.LDraftEntryId) ?? LEngineReferenceBlank;

        IReadOnlyList<LAuthor> credited = draft.LDraftEntryId == 0 ? [] : LEngineCreditRead(draft.LDraftEntryId);

        return !LEngineReferenceMatch(origin, held) || !LEngineCreditMatch(credited, draft.LDraftAuthor);
    }

    private IReadOnlyList<LAuthor> LEngineCreditRead(long referenceId)
    {
        return new LAuthorArchive(_lEngineDatabase).LAuthorReferenceRead(referenceId);
    }

    private void LEngineCreditSave(long referenceId, IReadOnlyList<LAuthor> authors)
    {
        LAuthorArchive archive = new(_lEngineDatabase);
        LReferenceArchive references = new(_lEngineDatabase);

        List<long> kept = new(authors.Count);
        foreach (LAuthor author in authors)
        {
            long id = author.LAuthorId switch
            {
                > 0 when archive.LAuthorRead(author.LAuthorId) is not null => author.LAuthorId,
                > 0 => throw new LRefusal(LRefusal.LRefusalLink),
                _ => archive.LAuthorCreate(new LAuthor(0, author.LAuthorName)).LAuthorId,
            };

            if (kept.Contains(id))
            {
                continue;
            }

            references.LReferenceAuthorAttach(referenceId, id, kept.Count);
            kept.Add(id);
        }

        foreach (LAuthor credited in archive.LAuthorReferenceRead(referenceId))
        {
            if (!kept.Contains(credited.LAuthorId))
            {
                references.LReferenceAuthorDetach(referenceId, credited.LAuthorId);
            }
        }
    }

    private static bool LEngineCreditMatch(IReadOnlyList<LAuthor> one, IReadOnlyList<LAuthor> other)
    {
        if (one.Count != other.Count)
        {
            return false;
        }

        for (int index = 0; index < one.Count; index++)
        {
            if (one[index].LAuthorId != other[index].LAuthorId)
            {
                return false;
            }
        }

        return true;
    }

    private LReference LEngineReferenceNormalize(LReference content)
    {
        return content.LReferenceId == 0
            ? content with { LReferenceId = LEngineIdentityCreate() }
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
            LStateMark.LStateMarkUnspecified);

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
