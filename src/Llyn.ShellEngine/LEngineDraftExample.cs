using System;
using System.Linq;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    internal LDraft LEngineExampleStart(string origin, long? exampleId)
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
                LEngineClockRead().LClockRead(),
                content);

            _lEngineDrafts.LDraftSave(draft);
            _lEngineClaims.LClaimSave(_lEngineClaims.LClaimCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    internal LExample LEngineExampleCommit(long id)
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
            using (LVaultSession session = _lEngineVault.LVaultSessionStart())
            {
                bool fresh = draft.LDraftEntryId == 0 || LEngineExampleRead(draft.LDraftEntryId) is null;
                if (fresh)
                {
                    stored = LEngineExampleCreate(sending);
                }
                else
                {
                    LExample written = sending with { LExampleId = draft.LDraftEntryId };
                    LEngineExampleUpdate(written);
                    stored = LEngineExampleRead(draft.LDraftEntryId) ?? written;
                }

                LEngineRevisionRecord(
                    stored.LExampleId, "example", fresh ? "create" : "update", stored.LExampleText.LStateValueShow());
                session.LVaultSessionCommit();
            }

            _lEngineDrafts.LDraftSave(
                draft with { LDraftEntryId = stored.LExampleId, LDraftExample = stored });

            _lEngineDraftHeld.Remove(id);
            _lEngineClaims.LClaimDelete(id);
            _lEngineDrafts.LDraftDelete(id);
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
            LStateAnchor.LStateAnchorUnspecified);

    private static bool LEngineExampleMatch(LExample one, LExample other)
    {
        return string.Equals(one.LExampleLanguage, other.LExampleLanguage, StringComparison.Ordinal)
            && one.LExampleText == other.LExampleText
            && one.LExampleSource == other.LExampleSource
            && one.LExampleGloss.SequenceEqual(other.LExampleGloss)
            && one.LExampleMention.SequenceEqual(other.LExampleMention);
    }
}
