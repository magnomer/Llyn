using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Infrastructure;

public static class LFolioBody
{
    public static string LFolioBodyFormat(
        LPortrait portrait, LTheme theme, List<LPortraitAsset> plates)
    {
        ArgumentNullException.ThrowIfNull(portrait);
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

        string crest = portrait.LPortraitHeadword;
        if (portrait.LPortraitFavorite)
        {
            crest += "  ★";
        }

        LFolioLine.LFolioLineAppend(body, "Headword", crest);

        List<string> marks = new List<string>();
        if (portrait.LPortraitLanguage.Length > 0)
        {
            marks.Add(portrait.LPortraitLanguage);
        }

        foreach (LPortraitReading reading in portrait.LPortraitPronunciation)
        {
            marks.Add(LFolioReadingFormat(reading, "[ ", " ]"));
        }

        foreach (LPortraitReading reading in portrait.LPortraitTranscription)
        {
            marks.Add(LFolioReadingFormat(reading, string.Empty, string.Empty));
        }

        if (portrait.LPortraitSpeech.Count > 0)
        {
            marks.Add(string.Join(", ", portrait.LPortraitSpeech));
        }

        if (marks.Count > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Sound", string.Join("   ·   ", marks));
        }

        LFolioBodyAppend(
            body, portrait.LPortraitLabel.LPortraitLabelMeanings,
            portrait.LPortraitMeaning, plates, theme);
        LFolioBodyAppend(
            body, portrait.LPortraitLabel.LPortraitLabelCollocations,
            portrait.LPortraitCollocation, plates, theme);

        if (portrait.LPortraitIncoming.Count > 0)
        {
            LFolioLine.LFolioLineAppend(
                body, "Band", portrait.LPortraitLabel.LPortraitLabelIncoming);

            foreach (LPortraitUsage usage in portrait.LPortraitIncoming)
            {
                LFolioLine.LFolioLineAppend(body, "Row", "→  " + usage.LPortraitUsageHeadword, string.Empty);
                LFolioLine.LFolioLineAppend(
                    body,
                    "RowDetail",
                    "     " + usage.LPortraitUsageTitle
                        + "   ·   " + usage.LPortraitUsageOwner
                        + "   ·   " + usage.LPortraitUsageLanguage);
            }
        }

        if (portrait.LPortraitNote.Length > 0)
        {
            LFolioLine.LFolioLineAppend(body, "Band", portrait.LPortraitLabel.LPortraitLabelNote);
            LFolioNote.LFolioNoteAppend(body, portrait.LPortraitNote);
        }

        body.Append("<w:sectPr><w:pgSz w:w=\"11906\" w:h=\"16838\"/>")
            .Append("<w:pgMar w:top=\"1134\" w:right=\"1134\" w:bottom=\"1134\" w:left=\"1134\" ")
            .Append("w:header=\"0\" w:footer=\"0\" w:gutter=\"0\"/></w:sectPr>")
            .Append("</w:body></w:document>");

        return body.ToString();
    }

    private static string LFolioReadingFormat(LPortraitReading reading, string open, string close)
    {
        string text = open + reading.LPortraitReadingText + close;
        return reading.LPortraitReadingLabel.Length == 0 ? text : reading.LPortraitReadingLabel + " " + text;
    }

    private static void LFolioBodyAppend(
        StringBuilder body,
        string heading,
        IReadOnlyList<LPortraitCard> cards,
        List<LPortraitAsset> plates,
        LTheme theme)
    {
        if (cards.Count == 0)
        {
            return;
        }

        LFolioLine.LFolioLineAppend(body, "Band", heading);

        foreach (LPortraitCard card in cards)
        {
            LFolioCard.LFolioCardAppend(body, card, plates, theme);
        }
    }
}
