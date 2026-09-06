using System;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LDraft LEngineSituationStart(string origin, string? situationId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            string situation = string.IsNullOrWhiteSpace(situationId) ? string.Empty : situationId;
            LSituation content = situation.Length == 0
                ? LEngineSituationBlank with { LSituationId = LIdentity.LIdentityCreate() }
                : LEngineSituationRead(situation) ?? throw new LRefusal(LRefusal.LRefusalSituation);

            LDraft draft = new(
                LIdentity.LIdentityCreate(),
                origin,
                situation,
                LEngineDraftBlank,
                DateTimeOffset.UtcNow,
                null,
                content);

            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft);
            LClaimArchive.LClaimArchiveSave(
                _lEngineWorkspace, LClaimArchive.LClaimArchiveCreate(draft.LDraftId));
            _lEngineDraftHeld.Add(draft.LDraftId);
            return draft;
        }
    }

    public LSituation LEngineSituationSave(LDraft draft)
    {
        lock (_lEngineGate)
        {
            ArgumentNullException.ThrowIfNull(draft);
            LEngineDraftValidate(draft.LDraftId);

            LSituation sent = draft.LDraftSituation ?? throw new LRefusal(LRefusal.LRefusalSituation);
            LSituation content = LEngineSituationNormalize(sent);
            LDraftArchive.LDraftArchiveSave(_lEngineWorkspace, draft with { LDraftSituation = content });
            return content;
        }
    }

    public LSituation LEngineSituationCommit(string id)
    {
        LSituation settled;
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LSituation sending = LEngineSituationNormalize(
                draft.LDraftSituation ?? throw new LRefusal(LRefusal.LRefusalSituation));

            LSituation stored;
            if (draft.LDraftEntry.Length == 0 || LEngineSituationRead(draft.LDraftEntry) is null)
            {
                stored = LEngineSituationCreate(sending);
            }
            else
            {
                LSituation written = sending with { LSituationId = draft.LDraftEntry };
                LEngineSituationUpdate(written);
                stored = LEngineSituationRead(draft.LDraftEntry) ?? written;
            }

            LDraftArchive.LDraftArchiveSave(
                _lEngineWorkspace,
                draft with { LDraftEntry = stored.LSituationId, LDraftSituation = stored });

            _lEngineDraftHeld.Remove(id);
            LClaimArchive.LClaimArchiveDelete(_lEngineWorkspace, id);
            LDraftArchive.LDraftArchiveDelete(_lEngineWorkspace, id);
            settled = stored;
        }

        LEngineBulletinRaise(LSubject.LSubjectSituation, settled.LSituationId);
        return settled;
    }

    private bool LEngineSituationCheck(LDraft draft, LSituation held)
    {
        LSituation origin = draft.LDraftEntry.Length == 0
            ? LEngineSituationBlank
            : LEngineSituationRead(draft.LDraftEntry) ?? LEngineSituationBlank;

        return !LEngineSituationMatch(origin, held);
    }

    private static LSituation LEngineSituationNormalize(LSituation content)
    {
        return content.LSituationId.Length == 0
            ? content with { LSituationId = LIdentity.LIdentityCreate() }
            : content;
    }

    private static LSituation LEngineSituationBlank =>
        new(
            string.Empty,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

    private static bool LEngineSituationMatch(LSituation one, LSituation other)
    {
        return one.LSituationTitle == other.LSituationTitle
            && one.LSituationDescription == other.LSituationDescription
            && one.LSituationKind == other.LSituationKind;
    }
}
