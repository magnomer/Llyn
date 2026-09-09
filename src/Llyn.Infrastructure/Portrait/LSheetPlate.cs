using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LSheetPlate
{
    public static void LSheetPlateAppend(StringBuilder page, LPortraitCard card)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(card);

        if (card.LPortraitCardImage.Count > 0)
        {
            page.Append("<div class=\"plates\">\n");

            foreach (LPortraitMedia image in card.LPortraitCardImage)
            {
                string address = LPortraitAsset.LPortraitAssetLoad(image.LPortraitMediaLocation)
                    is LPortraitAsset held
                    ? held.LPortraitAssetAddress
                    : image.LPortraitMediaLocation;

                page.Append("<div class=\"plate\"><img src=\"")
                    .Append(LSheet.LSheetNormalize(address))
                    .Append("\" alt=\"\"></div>\n");
            }

            page.Append("</div>\n");
        }

        if (card.LPortraitCardVideo.Count == 0)
        {
            return;
        }

        page.Append("<div class=\"plates\">\n");

        foreach (LPortraitMedia video in card.LPortraitCardVideo)
        {
            string location = video.LPortraitMediaLocation;
            string? film = LPortraitFilm.LPortraitFilmRead(location);

            page.Append("<div class=\"plate\"><a class=\"reel\" href=\"")
                .Append(LSheet.LSheetNormalize(location))
                .Append("\">");

            if (film is not null)
            {
                page.Append("<img src=\"https://img.youtube.com/vi/")
                    .Append(LSheet.LSheetNormalize(film))
                    .Append("/hqdefault.jpg\" alt=\"\">");
            }
            else
            {
                page.Append("<span class=\"blank\"></span>");
            }

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
}
