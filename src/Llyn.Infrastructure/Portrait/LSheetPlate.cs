using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetPlate
{
    public static void LSheetPlateAppend(StringBuilder page, LPortraitCard card)
    {
        ArgumentNullException.ThrowIfNull(card);

        LSheetPlateAppend(page, card.LPortraitCardImage, card.LPortraitCardVideo);
    }

    public static void LSheetPlateAppend(
        StringBuilder page, IReadOnlyList<LPortraitMedia> images, IReadOnlyList<LPortraitMedia> videos)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(videos);

        if (images.Count > 0)
        {
            page.Append("<div class=\"plates\">\n");

            foreach (LPortraitMedia image in images)
            {
                page.Append("<div class=\"plate\">");

                if (LSheetAddressRead(image.LPortraitMediaLocation) is string address)
                {
                    page.Append("<img src=\"").Append(LSheet.LSheetNormalize(address)).Append("\" alt=\"\">");
                }
                else
                {
                    page.Append("<span class=\"blank\"></span>");
                }

                page.Append("</div>\n");
            }

            page.Append("</div>\n");
        }

        if (videos.Count == 0)
        {
            return;
        }

        page.Append("<div class=\"plates\">\n");

        foreach (LPortraitMedia video in videos)
        {
            string location = video.LPortraitMediaLocation;
            string? film = LPortraitFilm.LPortraitFilmRead(location);

            page.Append("<div class=\"plate\"><a class=\"reel\" href=\"")
                .Append(LSheet.LSheetNormalize(location))
                .Append("\">");

            page.Append("<span class=\"blank\">");

            if (film is not null)
            {
                page.Append("<img src=\"https://img.youtube.com/vi/")
                    .Append(LSheet.LSheetNormalize(film))
                    .Append("/hqdefault.jpg\" alt=\"\">");
            }

            page.Append("</span>");

            page.Append("<span class=\"glyph\">▶</span><span class=\"said\">")
                .Append(LSheet.LSheetNormalize(location));

            if (video.LPortraitMediaSpan.Length > 0)
            {
                page.Append(" · ").Append(LSheet.LSheetNormalize(video.LPortraitMediaSpan));
            }

            page.Append("</span></a></div>\n");
        }

        page.Append("</div>\n");
    }

    private static string? LSheetAddressRead(string location)
    {
        return LPortraitAsset.LPortraitAssetLoad(location)?.LPortraitAssetAddress;
    }
}
