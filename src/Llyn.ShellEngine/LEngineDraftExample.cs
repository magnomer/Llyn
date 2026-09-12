using System;
using System.Linq;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LDraft LEngineExampleStart(string origin, long? exampleId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            long example = exampleId is null or <= 0 ? 0 : exampleId.Value;
            LExample content = example == 0
                ? LEngineExampleBlank with { LExampleId = LEngineIdentityCreate() }
                : LEngineExampleRead(example) ?? throw new LRefusal(LRefusal.LRefusalExample);

            LDraft draft = new(
                LEngineIdentityCreate(),
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

    public LExample LEngineExampleCommit(long id)
    {
        LExample settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LExample sending = LEngineExampleNormalize(
                draft.LDraftExample ?? throw new LRefusal(LRefusal.LRefusalExample));

            LExample stored;
            if (draft.LDraftEntryId == 0 || LEngineExampleRead(draft.LDraftEntryId) is null)
            {
                stored = LEngineExampleCreate(sending);
            }
            else
            {
                LExample written = sending with { LExampleId = draft.LDraftEntryId };
                LEngineExampleUpdate(written);
                stored = LEngineExampleRead(draft.LDraftEntryId) ?? written;
            }

            LDraftArchive.LDraftArchiveSave(
                _lEngineWorkspace,
                draft with { LDraftEntryId = stored.LExampleId, LDraftExample = stored });

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
        LExample origin = draft.LDraftEntryId == 0
            ? blank
            : LEngineExampleRead(draft.LDraftEntryId) ?? blank;

        return !LEngineExampleMatch(origin, held);
    }

    private LExample LEngineExampleNormalize(LExample content)
    {
        return content.LExampleId == 0
            ? content with { LExampleId = LEngineIdentityCreate() }
            : content;
    }

    private static LExample LEngineExampleBlank =>
        new(
            0,
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateAnchor.LStateAnchorUnspecified);

    private static bool LEngineExampleMatch(LExample one, LExample other)
    {
        return string.Equals(one.LExampleLanguage, other.LExampleLanguage, StringComparison.Ordinal)
            && one.LExampleText == other.LExampleText
            && one.LExampleTranslation == other.LExampleTranslation
            && one.LExampleSource == other.LExampleSource
            && one.LExampleMention.SequenceEqual(other.LExampleMention);
    }
}
