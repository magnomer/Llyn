using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Application;

public sealed class LDraftClerkMedia
{
    private readonly LImageVault _lDraftClerkImages;
    private readonly LVideoVault _lDraftClerkVideos;
    private readonly LIdentity _lDraftClerkIdentity;

    public LDraftClerkMedia(LImageVault images, LVideoVault videos, LIdentity identity)
    {
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(videos);
        ArgumentNullException.ThrowIfNull(identity);
        _lDraftClerkImages = images;
        _lDraftClerkVideos = videos;
        _lDraftClerkIdentity = identity;
    }

    public LSituation? LSituationDispatch(LSituation situation, LRequest request)
    {
        ArgumentNullException.ThrowIfNull(situation);

        if (LImageApply(situation.LSituationImage, request) is IReadOnlyList<LImageDraft> images)
        {
            return situation with { LSituationImage = images };
        }

        if (LVideoApply(situation.LSituationVideo, request) is IReadOnlyList<LVideoDraft> videos)
        {
            return situation with { LSituationVideo = videos };
        }

        return null;
    }

    private IReadOnlyList<LImageDraft>? LImageApply(IReadOnlyList<LImageDraft> images, LRequest request)
    {
        return request switch
        {
            LRequestImageAddition sent => LDraftClerkList.LDraftListAdd(
                images,
                new LImageDraft(
                    LStateValue.LStateValueRead(sent.LRequestValue), _lDraftClerkIdentity.LIdentityCreate()),
                sent.LRequestPosition),
            LRequestImagePick sent => LDraftClerkList.LDraftListInsert(
                images,
                LImageRead(sent.LRequestImageId),
                sent.LRequestImageId,
                sent.LRequestPosition,
                static row => row.LImageDraftId),
            LRequestImageRemoval sent => LDraftClerkList.LDraftListRemove(
                images, sent.LRequestImageId, static row => row.LImageDraftId),
            LRequestImageShift sent => LDraftClerkList.LDraftListMove(
                images, sent.LRequestImageId, sent.LRequestPosition, static row => row.LImageDraftId),
            LRequestImageLocation sent => LDraftClerkList.LDraftListChange(
                images,
                sent.LRequestImageId,
                static row => row.LImageDraftId,
                image => image with { LImageDraftLocation = LStateValue.LStateValueRead(sent.LRequestValue) })
                ?? throw new LRefusal(LRefusal.LRefusalItem),
            _ => null,
        };
    }

    public LEntryDraft LImageApply(LEntryDraft content, long cardId, LRequest request)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftImage = LImageApply(card.LCardDraftImage, request)! });
    }

    private LImageDraft LImageRead(long id)
    {
        if (id <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LImage stored = _lDraftClerkImages.LImageRead(id)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return new LImageDraft(stored.LImageLocation, stored.LImageId);
    }

    public static LEntryDraft LImageChange(LEntryDraft content, LRequestImageLocation request)
    {
        ArgumentNullException.ThrowIfNull(request);

        LStateValue value = LStateValue.LStateValueRead(request.LRequestValue);

        return LDraftClerkList.LDraftRowChange(content, card =>
        {
            IReadOnlyList<LImageDraft>? images = LDraftClerkList.LDraftListChange(
                card.LCardDraftImage,
                request.LRequestImageId,
                static row => row.LImageDraftId,
                image => image with { LImageDraftLocation = value });

            return images is null ? null : card with { LCardDraftImage = images };
        });
    }

    private IReadOnlyList<LVideoDraft>? LVideoApply(IReadOnlyList<LVideoDraft> videos, LRequest request)
    {
        return request switch
        {
            LRequestVideoAddition sent => LDraftClerkList.LDraftListAdd(
                videos,
                new LVideoDraft(
                    LStateValue.LStateValueRead(sent.LRequestValue),
                    LStateValue.LStateValueUnspecified,
                    _lDraftClerkIdentity.LIdentityCreate()),
                sent.LRequestPosition),
            LRequestVideoPick sent => LDraftClerkList.LDraftListInsert(
                videos,
                LVideoRead(sent.LRequestVideoId),
                sent.LRequestVideoId,
                sent.LRequestPosition,
                static row => row.LVideoDraftId),
            LRequestVideoRemoval sent => LDraftClerkList.LDraftListRemove(
                videos, sent.LRequestVideoId, static row => row.LVideoDraftId),
            LRequestVideoShift sent => LDraftClerkList.LDraftListMove(
                videos, sent.LRequestVideoId, sent.LRequestPosition, static row => row.LVideoDraftId),
            LRequestVideoLocation sent => LDraftClerkList.LDraftListChange(
                videos,
                sent.LRequestVideoId,
                static row => row.LVideoDraftId,
                video => video with { LVideoDraftLocation = LStateValue.LStateValueRead(sent.LRequestValue) })
                ?? throw new LRefusal(LRefusal.LRefusalItem),
            LRequestVideoSpan sent => LDraftClerkList.LDraftListChange(
                videos,
                sent.LRequestVideoId,
                static row => row.LVideoDraftId,
                video => video with { LVideoDraftSpan = LStateValue.LStateValueRead(sent.LRequestValue) })
                ?? throw new LRefusal(LRefusal.LRefusalItem),
            _ => null,
        };
    }

    public LEntryDraft LVideoApply(LEntryDraft content, long cardId, LRequest request)
    {
        return LDraftClerkCard.LCardChange(
            content, cardId, card => card with { LCardDraftVideo = LVideoApply(card.LCardDraftVideo, request)! });
    }

    private LVideoDraft LVideoRead(long id)
    {
        if (id <= 0)
        {
            throw new LRefusal(LRefusal.LRefusalItem);
        }

        LVideo stored = _lDraftClerkVideos.LVideoRead(id)
            ?? throw new LRefusal(LRefusal.LRefusalItem);

        return new LVideoDraft(stored.LVideoLocation, stored.LVideoSpan, stored.LVideoId);
    }

    public static LEntryDraft LVideoChange(
        LEntryDraft content, long videoId, Func<LVideoDraft, LVideoDraft> change)
    {
        return LDraftClerkList.LDraftRowChange(content, card =>
        {
            IReadOnlyList<LVideoDraft>? videos = LDraftClerkList.LDraftListChange(
                card.LCardDraftVideo, videoId, static row => row.LVideoDraftId, change);

            return videos is null ? null : card with { LCardDraftVideo = videos };
        });
    }
}
