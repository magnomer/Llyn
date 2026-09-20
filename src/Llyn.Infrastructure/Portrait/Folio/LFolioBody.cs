using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolioBody
{
    public static string LFolioBodyFormat(
        LPortraitPage page, LTheme theme, List<LPortraitAsset> plates)
    {
        ArgumentNullException.ThrowIfNull(page);
        ArgumentNullException.ThrowIfNull(theme);
        ArgumentNullException.ThrowIfNull(plates);

        StringBuilder body = new StringBuilder();

        body.Append("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\"?>")
            .Append("<w:document ")
            .Append("xmlns:w=\"http://schemas.openxmlformats.org/wordprocessingml/2006/main\" ")
            .Append("xmlns:r=\"http://schemas.openxmlformats.org/officeDocument/2006/relationships\" ")
            .Append("xmlns:wp=\"http://schemas.openxmlformats.org/drawingml/2006/wordprocessingDrawing\" ")
            .Append("xmlns:a=\"http://schemas.openxmlformats.org/drawingml/2006/main\" ")
            .Append("xmlns:pic=\"http://schemas.openxmlformats.org/drawingml/2006/picture\">")
            .Append("<w:body>");

        string crest = page.LPortraitPageTitle;
        if (page.LPortraitPageFavorite)
        {
            crest += "  ★";
        }

        LFolioLine.LFolioLineAppend(body, "Headword", crest);

        List<string> marks = new List<string>();
        if (page.LPortraitPageLanguage.Length > 0)
        {
            marks.Add(page.LPortraitPageLanguage);
        }

        foreach (LPortraitLine line in page.LPortraitPageLine)
        {
            string text = line.LPortraitLineOpener + line.LPortraitLineText + line.LPortraitLineCloser;
            marks.Add(line.LPortraitLineLabel.Length == 0 ? text : line.LPortraitLineLabel + " " + text);
        }

        foreach (string chip in page.LPortraitPageChip)
        {
            marks.Add(chip);
        }

        if (marks.Count > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Sound", string.Join("   ·   ", marks));
        }

        foreach (LPortraitSection section in page.LPortraitPageSection)
        {
            LFolioSection.LFolioSectionAppend(body, section, plates, theme);
        }

        body.Append("<w:sectPr><w:pgSz w:w=\"11906\" w:h=\"16838\"/>")
            .Append("<w:pgMar w:top=\"1134\" w:right=\"1134\" w:bottom=\"1134\" w:left=\"1134\" ")
            .Append("w:header=\"0\" w:footer=\"0\" w:gutter=\"0\"/></w:sectPr>")
            .Append("</w:body></w:document>");

        return body.ToString();
    }
}
