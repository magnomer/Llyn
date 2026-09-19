using System;
using System.Collections.Generic;
using Llyn.Application;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LSituation? LEngineSituationDispatch(LSituation situation, LRequest request)
    {
        if (LEngineImageApply(situation.LSituationImage, request) is IReadOnlyList<LImageDraft> images)
        {
            return situation with { LSituationImage = images };
        }

        if (LEngineVideoApply(situation.LSituationVideo, request) is IReadOnlyList<LVideoDraft> videos)
        {
            return situation with { LSituationVideo = videos };
        }

        return null;
    }

    private IReadOnlyList<LImageDraft>? LEngineImageApply(IReadOnlyList<LImageDraft> images, LRequest request)
    {
        return request switch
        {
            LRequestImageAddition sent => LEngineListAdd(
                images,
                new LImageDraft(LEngineValueRead(sent.LRequestValue), LEngineIdentityCreate()),
                sent.LRequestPosition),
            LRequestImagePick sent => LEngineListInsert(
                images,
                LEngineImageRead(sent.LRequestImageId),
                sent.LRequestImageId,
                sent.LRequestPosition,
                static row => row.LImageDraftId),
            LRequestImageRemoval sent => LEngineListRemove(
                images, sent.LRequestImageId, static row => row.LImageDraftId),
            LRequestImageShift sent => LEngineListMove(
                images, sent.LRequestImageId, sent.LRequestPosition, static row => row.LImageDraftId),
            LRequestImageLocation sent => LEngineListChange(
                images,
                sent.LRequestImageId,
                static row => row.LImageDraftId,
                image => image with { LImageDraftLocation = LEngineValueRead(sent.LRequestValue) })
                ?? throw new LRefusal(LRefusal.LRefusalItem),
            _ => null,
        };
    }

    private LEntryDraft LEngineImageApply(LEntryDraft content, long cardId, LRequest request)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftImage = LEngineImageApply(card.LCardDraftImage, request)! });
    }

    private LImageDraft LEngineImageRead(long id)
    {
        if (id <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LImage stored = new LImageArchive(_lEngineDatabase).LImageRead(id)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return new LImageDraft(stored.LImageLocation, stored.LImageId);
    }

    private static LEntryDraft LEngineImageChange(LEntryDraft content, LRequestImageLocation request)
    {
        LStateValue value = LEngineValueRead(request.LRequestValue);

        return LEngineRowChange(content, card =>
        {
            IReadOnlyList<LImageDraft>? images = LEngineListChange(
                card.LCardDraftImage,
                request.LRequestImageId,
                static row => row.LImageDraftId,
                image => image with { LImageDraftLocation = value });

            return images is null ? null : card with { LCardDraftImage = images };
        });
    }

    private IReadOnlyList<LVideoDraft>? LEngineVideoApply(IReadOnlyList<LVideoDraft> videos, LRequest request)
    {
        return request switch
        {
            LRequestVideoAddition sent => LEngineListAdd(
                videos,
                new LVideoDraft(
                    LEngineValueRead(sent.LRequestValue), LStateValue.LStateValueUnspecified, LEngineIdentityCreate()),
                sent.LRequestPosition),
            LRequestVideoPick sent => LEngineListInsert(
                videos,
                LEngineVideoRead(sent.LRequestVideoId),
                sent.LRequestVideoId,
                sent.LRequestPosition,
                static row => row.LVideoDraftId),
            LRequestVideoRemoval sent => LEngineListRemove(
                videos, sent.LRequestVideoId, static row => row.LVideoDraftId),
            LRequestVideoShift sent => LEngineListMove(
                videos, sent.LRequestVideoId, sent.LRequestPosition, static row => row.LVideoDraftId),
            LRequestVideoLocation sent => LEngineListChange(
                videos,
                sent.LRequestVideoId,
                static row => row.LVideoDraftId,
                video => video with { LVideoDraftLocation = LEngineValueRead(sent.LRequestValue) })
                ?? throw new LRefusal(LRefusal.LRefusalItem),
            LRequestVideoSpan sent => LEngineListChange(
                videos,
                sent.LRequestVideoId,
                static row => row.LVideoDraftId,
                video => video with { LVideoDraftSpan = LEngineValueRead(sent.LRequestValue) })
                ?? throw new LRefusal(LRefusal.LRefusalItem),
            _ => null,
        };
    }

    private LEntryDraft LEngineVideoApply(LEntryDraft content, long cardId, LRequest request)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftVideo = LEngineVideoApply(card.LCardDraftVideo, request)! });
    }

    private LVideoDraft LEngineVideoRead(long id)
    {
        if (id <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LVideo stored = new LVideoArchive(_lEngineDatabase).LVideoRead(id)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return new LVideoDraft(stored.LVideoLocation, stored.LVideoSpan, stored.LVideoId);
    }

    private static LEntryDraft LEngineVideoChange(
        LEntryDraft content, long videoId, Func<LVideoDraft, LVideoDraft> change)
    {
        return LEngineRowChange(content, card =>
        {
            IReadOnlyList<LVideoDraft>? videos = LEngineListChange(
                card.LCardDraftVideo, videoId, static row => row.LVideoDraftId, change);

            return videos is null ? null : card with { LCardDraftVideo = videos };
        });
    }
}
