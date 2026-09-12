using System;
using System.Collections.Generic;

namespace Llyn.Core;

public sealed record LPortraitReading(
    string LPortraitReadingLabel,
    string LPortraitReadingText)
{
    public static IReadOnlyList<LPortraitReading> LPortraitReadingCreate(
        IReadOnlyList<LPronunciationDraft> pronunciations)
    {
        ArgumentNullException.ThrowIfNull(pronunciations);

        List<LPortraitReading> shown = new List<LPortraitReading>();
        foreach (LPronunciationDraft pronunciation in pronunciations)
        {
            if (pronunciation.LPronunciationDraftIpa.Length > 0)
            {
                shown.Add(new LPortraitReading(
                    pronunciation.LPronunciationDraftVariety, pronunciation.LPronunciationDraftIpa));
            }
        }

        return shown;
    }

    public static IReadOnlyList<LPortraitReading> LPortraitReadingCreate(
        IReadOnlyList<LTranscriptionDraft> transcriptions)
    {
        ArgumentNullException.ThrowIfNull(transcriptions);

        List<LPortraitReading> shown = new List<LPortraitReading>();
        foreach (LTranscriptionDraft transcription in transcriptions)
        {
            if (!transcription.LTranscriptionDraftEmpty)
            {
                shown.Add(new LPortraitReading(
                    transcription.LTranscriptionDraftScheme, transcription.LTranscriptionDraftText));
            }
        }

        return shown;
    }
}
