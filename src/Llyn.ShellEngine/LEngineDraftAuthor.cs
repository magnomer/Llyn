using System;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LDraft LEngineAuthorStart(string origin, long? authorId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            long author = authorId is null or <= 0 ? 0 : authorId.Value;
            LAuthor content = author == 0
                ? new LAuthor(0, string.Empty)
                : LEngineAuthorRead(author) ?? throw new LRefusal(LRefusal.LRefusalLink);

            LDraft draft = new(
                LEngineIdentityCreate(),
                origin,
                author,
                LEngineDraftBlank,
                DateTimeOffset.UtcNow)
            {
                LDraftAuthorHeld = content,
            };

            _lEngineDrafts.LDraftSave(draft);
            _lEngineClaims.LClaimSave(_lEngineClaims.LClaimCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    internal LAuthor LEngineAuthorCommit(long id)
    {
        LAuthor settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LAuthor held = draft.LDraftAuthorHeld ?? throw new LRefusal(LRefusal.LRefusalLink);
            string name = held.LAuthorName.Trim();
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            LAuthor stored;
            using (LVaultSession session = _lEngineVault.LVaultSessionStart())
            {
                LAuthorVault archive = _lEngineAuthors;
                bool fresh = draft.LDraftEntryId == 0 || archive.LAuthorRead(draft.LDraftEntryId) is null;
                if (fresh)
                {
                    stored = archive.LAuthorCreate(new LAuthor(0, name));
                }
                else
                {
                    stored = new LAuthor(draft.LDraftEntryId, name);
                    archive.LAuthorUpdate(stored);
                }

                LEngineRevisionRecord(stored.LAuthorId, "author", fresh ? "create" : "update", stored.LAuthorName);
                session.LVaultSessionCommit();
            }

            _lEngineDrafts.LDraftSave(
                draft with { LDraftEntryId = stored.LAuthorId, LDraftAuthorHeld = stored });

            _lEngineDraftHeld.Remove(id);
            LEngineChronicleClear(id);
            _lEngineClaims.LClaimDelete(id);
            _lEngineDrafts.LDraftDelete(id);
            settled = stored;
        }

        LEngineBulletinRaise(LSubject.LSubjectAuthor, settled.LAuthorId);
        return settled;
    }

    private bool LEngineAuthorCheck(LDraft draft, LAuthor held)
    {
        string origin = draft.LDraftEntryId == 0
            ? string.Empty
            : LEngineAuthorRead(draft.LDraftEntryId)?.LAuthorName ?? string.Empty;

        return !string.Equals(origin.Trim(), held.LAuthorName.Trim(), StringComparison.Ordinal);
    }
}
