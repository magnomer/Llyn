using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    public LDraft LEngineSituationStart(string origin, long? situationId)
    {
        lock (_lEngineGate)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(origin);

            long situation = situationId is null or <= 0 ? 0 : situationId.Value;
            LSituation content = situation == 0
                ? LEngineSituationBlank with { LSituationId = LEngineIdentityCreate() }
                : LEngineSituationRead(situation) ?? throw new LRefusal(LRefusal.LRefusalSituation);

            LDraft draft = new(
                LEngineIdentityCreate(),
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

    public LSituation LEngineSituationCommit(long id)
    {
        LSituation settled;
        lock (_lEngineGate)
        {
            ArgumentOutOfRangeException.ThrowIfZero(id);
            LEngineDraftValidate(id);

            LDraft draft = LEngineDraftLoad(id);
            LSituation sending = LEngineSituationNormalize(
                draft.LDraftSituation ?? throw new LRefusal(LRefusal.LRefusalSituation));

            LSituation stored;
            if (draft.LDraftEntryId == 0 || LEngineSituationRead(draft.LDraftEntryId) is null)
            {
                stored = LEngineSituationCreate(sending);
            }
            else
            {
                LSituation written = sending with { LSituationId = draft.LDraftEntryId };
                LEngineSituationUpdate(written);
                stored = LEngineSituationRead(draft.LDraftEntryId) ?? written;
            }

            LDraftArchive.LDraftArchiveSave(
                _lEngineWorkspace,
                draft with { LDraftEntryId = stored.LSituationId, LDraftSituation = stored });

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
        LSituation origin = draft.LDraftEntryId == 0
            ? LEngineSituationBlank
            : LEngineSituationRead(draft.LDraftEntryId) ?? LEngineSituationBlank;

        return !LEngineSituationMatch(origin, held);
    }

    private LSituation LEngineSituationNormalize(LSituation content)
    {
        IReadOnlyList<LImageDraft> images = LEngineListNormalize(
            content.LSituationImage,
            static row => row.LImageDraftEmpty,
            row => row.LImageDraftId == 0 ? row with { LImageDraftId = LEngineIdentityCreate() } : row);
        IReadOnlyList<LVideoDraft> videos = LEngineListNormalize(
            content.LSituationVideo,
            static row => row.LVideoDraftEmpty,
            row => row.LVideoDraftId == 0 ? row with { LVideoDraftId = LEngineIdentityCreate() } : row);

        if (content.LSituationId != 0
            && ReferenceEquals(images, content.LSituationImage)
            && ReferenceEquals(videos, content.LSituationVideo))
        {
            return content;
        }

        return content with
        {
            LSituationId = content.LSituationId == 0 ? LEngineIdentityCreate() : content.LSituationId,
            LSituationImage = images,
            LSituationVideo = videos,
        };
    }

    private static LSituation LEngineSituationBlank =>
        new(
            0,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified,
            LStateValue.LStateValueUnspecified);

    private static bool LEngineSituationMatch(LSituation one, LSituation other)
    {
        return one.LSituationTitle == other.LSituationTitle
            && one.LSituationDescription == other.LSituationDescription
            && one.LSituationKind == other.LSituationKind
            && LEngineImageMatch(one.LSituationImage, other.LSituationImage)
            && LEngineVideoMatch(one.LSituationVideo, other.LSituationVideo);
    }
}
