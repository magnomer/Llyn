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

    public static LPortraitMedia LPortraitMediaCreate(byte[] data, string caption)
    {
        ArgumentNullException.ThrowIfNull(data);

        string media = data.Length >= 4 && data[0] == 0x89 && data[1] == 0x50 && data[2] == 0x4E && data[3] == 0x47
            ? "image/png"
            : data.Length >= 3 && data[0] == 0x47 && data[1] == 0x49 && data[2] == 0x46
                ? "image/gif"
                : "image/jpeg";

        return new LPortraitMedia(
            "data:" + media + ";base64," + Convert.ToBase64String(data), caption ?? string.Empty, false);
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
