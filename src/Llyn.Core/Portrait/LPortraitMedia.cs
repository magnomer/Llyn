using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitMedia(
    string LPortraitMediaLocation,
    string LPortraitMediaSpan,
    bool LPortraitMediaMoving)
{
    public static IReadOnlyList<LPortraitMedia> LPortraitMediaCreate(
        IReadOnlyList<LImageDraft> images, string mark)
    {
        ArgumentNullException.ThrowIfNull(images);

        List<LPortraitMedia> shown = new List<LPortraitMedia>();
        foreach (LImageDraft image in images)
        {
            if (!image.LImageDraftEmpty)
            {
                shown.Add(new LPortraitMedia(
                    LPortraitText.LPortraitTextRead(image.LImageDraftLocation, mark),
                    string.Empty,
                    false));
            }
        }

        return shown;
    }

    public static IReadOnlyList<LPortraitMedia> LPortraitMediaCreate(
        IReadOnlyList<LVideoDraft> videos, string mark)
    {
        ArgumentNullException.ThrowIfNull(videos);

        List<LPortraitMedia> shown = new List<LPortraitMedia>();
        foreach (LVideoDraft video in videos)
        {
            if (!video.LVideoDraftEmpty)
            {
                shown.Add(new LPortraitMedia(
                    LPortraitText.LPortraitTextRead(video.LVideoDraftLocation, mark),
                    LPortraitText.LPortraitTextRead(video.LVideoDraftSpan, mark),
                    true));
            }
        }

        return shown;
    }
}
