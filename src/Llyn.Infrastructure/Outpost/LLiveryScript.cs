using System;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

internal static class LLiveryScript
{
    public static void LLiveryScriptAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)
    {
        ArgumentNullException.ThrowIfNull(sheet);
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(lookup);

        if (page.LLiveryPageScript.Count == 0)
        {
            return;
        }

        sheet.Append("<div class=\"llyn-card\">\n\n");
        foreach (LScriptGroup group in page.LLiveryPageScript)
        {
            if (group.LScriptGroupHeading.Length > 0)
            {
                sheet.Append("<span class=\"llyn-heading\">")
                    .Append(LLiveryHeader.LLiveryTextFormat(group.LScriptGroupHeading)).Append("</span>\n\n");
            }

            sheet.Append("<span class=\"llyn-style\">")
                .Append(LLiveryHeader.LLiveryTextFormat(group.LScriptGroupStyle)).Append("</span>");
            foreach (LScriptImage image in group.LScriptGroupImages)
            {
                sheet.Append(' ').Append(LLiveryFigureFormat(image, lookup));
            }

            sheet.Append("\n\n");
            if (group.LScriptGroupGloss.Length > 0)
            {
                sheet.Append("<span class=\"llyn-quote\">")
                    .Append(LLiveryHeader.LLiveryTextFormat(group.LScriptGroupGloss)).Append("</span>\n\n");
            }
        }

        sheet.Append("</div>\n\n");
    }

    private static string LLiveryFigureFormat(LScriptImage image, Func<string, string> lookup)
    {
        StringBuilder figure = new StringBuilder("<span class=\"llyn-figure\">");
        if (image.LScriptImageData.Length > 0)
        {
            figure.Append("<img src=\"data:image/png;base64,")
                .Append(Convert.ToBase64String(image.LScriptImageData)).Append("\" class=\"llyn-glyph\">");
        }

        string epoch = image.LScriptImageEpoch.Length > 0 ? lookup("Epoch." + image.LScriptImageEpoch) : string.Empty;
        if (epoch.Length > 0 || image.LScriptImageCaption.Length > 0)
        {
            figure.Append("<span class=\"llyn-caption\">");
            if (epoch.Length > 0)
            {
                figure.Append("<span class=\"llyn-epoch\">").Append(LLiveryHeader.LLiveryTextFormat(epoch))
                    .Append("</span>");
            }

            figure.Append(LLiveryHeader.LLiveryTextFormat(image.LScriptImageCaption)).Append("</span>");
        }

        return figure.Append("</span>").ToString();
    }
}
