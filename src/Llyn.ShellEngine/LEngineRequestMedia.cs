using System;
using System.Collections.Generic;
using Llyn.Core;
using Llyn.Infrastructure;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private LEntryDraft LEngineImageAdd(LEntryDraft content, LRequestImageAddition request)
    {
        LImageDraft image = new(LEngineValueRead(request.LRequestValue), LEngineIdentityCreate());

        return LEngineImageApply(
            content,
            request.LRequestCardId,
            images => LEngineListAdd(images, image, request.LRequestPosition));
    }

    private LEntryDraft LEngineImageInsert(LEntryDraft content, LRequestImagePick request)
    {
        if (request.LRequestImageId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LImage stored = new LImageArchive(_lEngineDatabase).LImageRead(request.LRequestImageId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LImageDraft image = new(stored.LImageLocation, stored.LImageId);

        return LEngineImageApply(
            content,
            request.LRequestCardId,
            images => LEngineListInsert(images, image, stored.LImageId, request.LRequestPosition, static row => row.LImageDraftId));
    }

    private static LEntryDraft LEngineImageRemove(LEntryDraft content, LRequestImageRemoval request)
    {
        return LEngineImageApply(
            content,
            request.LRequestCardId,
            images => LEngineListRemove(images, request.LRequestImageId, static row => row.LImageDraftId));
    }

    private static LEntryDraft LEngineImageMove(LEntryDraft content, LRequestImageShift request)
    {
        return LEngineImageApply(
            content,
            request.LRequestCardId,
            images => LEngineListMove(images, request.LRequestImageId, request.LRequestPosition, static row => row.LImageDraftId));
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

    private static LEntryDraft LEngineImageApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<LImageDraft>, IReadOnlyList<LImageDraft>> change)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftImage = change(card.LCardDraftImage) });
    }

    private LEntryDraft LEngineVideoAdd(LEntryDraft content, LRequestVideoAddition request)
    {
        LVideoDraft video = new(
            LEngineValueRead(request.LRequestValue), LStateValue.LStateValueUnspecified, LEngineIdentityCreate());

        return LEngineVideoApply(
            content,
            request.LRequestCardId,
            videos => LEngineListAdd(videos, video, request.LRequestPosition));
    }

    private LEntryDraft LEngineVideoInsert(LEntryDraft content, LRequestVideoPick request)
    {
        if (request.LRequestVideoId <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LVideo stored = new LVideoArchive(_lEngineDatabase).LVideoRead(request.LRequestVideoId)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        LVideoDraft video = new(stored.LVideoLocation, stored.LVideoSpan, stored.LVideoId);

        return LEngineVideoApply(
            content,
            request.LRequestCardId,
            videos => LEngineListInsert(videos, video, stored.LVideoId, request.LRequestPosition, static row => row.LVideoDraftId));
    }

    private static LEntryDraft LEngineVideoRemove(LEntryDraft content, LRequestVideoRemoval request)
    {
        return LEngineVideoApply(
            content,
            request.LRequestCardId,
            videos => LEngineListRemove(videos, request.LRequestVideoId, static row => row.LVideoDraftId));
    }

    private static LEntryDraft LEngineVideoMove(LEntryDraft content, LRequestVideoShift request)
    {
        return LEngineVideoApply(
            content,
            request.LRequestCardId,
            videos => LEngineListMove(videos, request.LRequestVideoId, request.LRequestPosition, static row => row.LVideoDraftId));
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

    private static LEntryDraft LEngineVideoApply(
        LEntryDraft content, long cardId, Func<IReadOnlyList<LVideoDraft>, IReadOnlyList<LVideoDraft>> change)
    {
        return LEngineCardChange(
            content, cardId, card => card with { LCardDraftVideo = change(card.LCardDraftVideo) });
    }
}
