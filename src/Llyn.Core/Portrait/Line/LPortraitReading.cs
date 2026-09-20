using System;
using System.Collections.Generic;
using System.Linq;

namespace Llyn.Core;

public static class LPortraitReading
{
    private const string LPortraitReadingMain = "●";

    public static IReadOnlyList<LPortraitLine> LPortraitReadingCreate(
        IReadOnlyList<LPronunciationDraft> pronunciations, bool respelled, bool phonemic)
    {
        ArgumentNullException.ThrowIfNull(pronunciations);

        List<LPortraitLine> shown = new List<LPortraitLine>();
        foreach (LPronunciationDraft pronunciation in pronunciations)
        {
            string text = LPortraitReadingRead(
                pronunciation.LPronunciationDraftIpa, pronunciation.LPronunciationDraftRespelling, respelled);
            if (text.Length > 0)
            {
                (string open, string close) = LPortraitMarkRead(phonemic, true);
                shown.Add(new LPortraitLine(pronunciation.LPronunciationDraftVariety, text, open, close));
            }
        }

        return shown;
    }

    public static IReadOnlyList<LPortraitLine> LPortraitReadingCreate(
        IReadOnlyList<LTranscriptionDraft> transcriptions)
    {
        ArgumentNullException.ThrowIfNull(transcriptions);

        List<LPortraitLine> shown = new List<LPortraitLine>();
        foreach (LTranscriptionDraft transcription in transcriptions)
        {
            if (!transcription.LTranscriptionDraftEmpty)
            {
                shown.Add(new LPortraitLine(
                    transcription.LTranscriptionDraftScheme, transcription.LTranscriptionDraftText));
            }
        }

        return shown;
    }

    public static IReadOnlyList<LPortraitLine> LPortraitReadingCreate(
        IReadOnlyList<LReflexDraft> reflexes,
        Func<string, bool> respelled,
        Func<string, bool> phonemic,
        IReadOnlyList<LFanqieRow> fanqie)
    {
        ArgumentNullException.ThrowIfNull(reflexes);
        ArgumentNullException.ThrowIfNull(respelled);
        ArgumentNullException.ThrowIfNull(phonemic);
        ArgumentNullException.ThrowIfNull(fanqie);

        List<LPortraitLine> shown = new List<LPortraitLine>();
        foreach (LReflexDraft reflex in reflexes)
        {
            string language = reflex.LReflexDraftLanguage.Trim();
            string text = LPortraitReadingRead(
                reflex.LReflexDraftText, reflex.LReflexDraftRespelling, respelled(language));
            if (text.Length == 0)
            {
                continue;
            }

            List<string> parts = [reflex.LReflexDraftLanguage];
            if (reflex.LReflexDraftKind.Length > 0)
            {
                parts.Add(reflex.LReflexDraftKind);
            }

            if (reflex.LReflexDraftRegion.Length > 0)
            {
                parts.Add(reflex.LReflexDraftRegion);
            }

            if (reflex.LReflexDraftMain)
            {
                parts.Add(LPortraitReadingMain);
            }

            (string open, string close) = LPortraitMarkRead(phonemic(language), false);
            List<string> tail = [open + text + close];
            LPortraitReadingAdd(tail, reflex.LReflexDraftNote);
            LPortraitReadingAdd(tail, reflex.LReflexDraftRemark);
            LPortraitReadingAdd(tail, LPortraitAnchorRead(fanqie, reflex.LReflexDraftAnchors));

            shown.Add(new LPortraitLine(string.Join(" ", parts), string.Join(" ", tail)));
        }

        return shown;
    }

    public static string LPortraitAnchorRead(IReadOnlyList<LFanqieRow> fanqie, IReadOnlyList<long> anchors)
    {
        ArgumentNullException.ThrowIfNull(fanqie);
        ArgumentNullException.ThrowIfNull(anchors);

        List<string> readings = [];
        foreach (LFanqieRow row in fanqie)
        {
            if (row.LFanqieRowId <= 0 || !anchors.Contains(row.LFanqieRowId))
            {
                continue;
            }

            string reading = row.LFanqieRowReading.Length > 0
                ? "/" + row.LFanqieRowReading + "/"
                : row.LFanqieRowBook;
            if (!readings.Contains(reading))
            {
                readings.Add(reading);
            }
        }

        return readings.Count == 0 ? string.Empty : "⚓ " + string.Join(" · ", readings);
    }

    private static void LPortraitReadingAdd(List<string> parts, string text)
    {
        if (text.Length > 0)
        {
            parts.Add(text);
        }
    }

    private static string LPortraitReadingRead(string phonetic, string respelling, bool respelled)
    {
        return (respelled && respelling.Length > 0 ? respelling : phonetic).Trim();
    }

    private static (string, string) LPortraitMarkRead(bool phonemic, bool bracketed)
    {
        return phonemic ? ("/", "/") : bracketed ? ("[", "]") : (string.Empty, string.Empty);
    }
}
