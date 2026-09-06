using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LDraft LEngineExampleStart(string origin, string? exampleId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            string example = string.IsNullOrWhiteSpace(exampleId) ? string.Empty : exampleId;
            LExample content = example.Length == 0
                ? LEngineExampleBlank with { LExampleId = LIdentity.LIdentityCreate() }
                : LEngineExampleRead(example) ?? throw new LRefusal(LRefusal.LRefusalExample);

            LDraft draft = new(
                LIdentity.LIdentityCreate(),
                origin,
                example,
                LEngineDraftBlank,
                DateTimeOffset.UtcNow,
                content);

            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
            LClaimArchive.LClaimArchiveSave(
                _lEngineWorkspace, LClaimArchive.LClaimArchiveCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    public LExample LEngineExampleSave(LDraft draft)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);
            LEngineDraftValidate(draft.LDraftId);

            LExample sent = draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample);
            LExample content = LEngineExampleNormalize(sent);
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftExample = content });
            return content;
        }
    }

    public LExample LEngineExampleCommit(string id)
    {
        LExample settled;
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LExample sending = LEngineExampleNormalize(
                draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample));

            LExample stored;
            if (draft.LDraftEntry.Length == 0 || LEngineExampleRead(draft.LDraftEntry) is null)
            {
                stored = LEngineExampleCreate(sending);
            }
            else
            {
                LExample written = sending with { LExampleId = draft.LDraftEntry };
                LEngineExampleUpdate(written);
                stored = LEngineExampleRead(draft.LDraftEntry) ?? written;
            }

            LDraftArchive.LDraftArchiveSave(
                _lEngineWorkspace,
                draft with { LDraftEntry = stored.LExampleId, LDraftExample = stored });

            _lEngineDraftHeld.Remove(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
            settled = stored;
        }

        LEngineBulletinRaise(LSubject.LSubjectExample, settled.LExampleId);
        return settled;
    }

    private bool LEngineExampleCheck(LDraft draft, LExample held)
    {
        LExample blank = LEngineExampleBlank with { LExampleLanguage = held.LExampleLanguage };
        LExample origin = draft.LDraftEntry.Length == 0
            ? blank
            : LEngineExampleRead(draft.LDraftEntry) ?? blank;

        return !LEngineExampleMatch(origin, held);
    }

    private static LExample LEngineExampleNormalize(LExample content)
    {
        return content.LExampleId.Length == 0
            ? content with { LExampleId = LIdentity.LIdentityCreate() }
            : content;
    }

    private static LExample LEngineExampleBlank =>
        new(
            string.Empty,
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

    private static bool LEngineExampleMatch(LExample one, LExample other)
    {
        return string.Equals(one.LExampleLanguage, other.LExampleLanguage, StringComparison.Ordinal)
            && one.LExampleText == other.LExampleText
            && one.LExampleTranslation == other.LExampleTranslation
            && one.LExampleSource == other.LExampleSource;
    }
}
