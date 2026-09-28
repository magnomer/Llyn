using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.Conduct;

internal static class CSounding
{
    internal static IReadOnlyList<CPronunciationDraft> CSoundingPronunciationRead(
        IReadOnlyList<LPronunciationDraft> spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        return spoken.Select(CSoundingPronunciationRead).ToList();
    }

    internal static CPronunciationDraft CSoundingPronunciationRead(LPronunciationDraft spoken)
    {
        ArgumentNullException.ThrowIfNull(spoken);

        return new CPronunciationDraft(
            spoken.LPronunciationDraftId,
            spoken.LPronunciationDraftIpa,
            spoken.LPronunciationDraftRespelling,
            spoken.LPronunciationDraftVariety,
            spoken.LPronunciationDraftAudio);
    }

    internal static IReadOnlyList<CTranscriptionDraft> CSoundingTranscriptionRead(
        IReadOnlyList<LTranscriptionDraft> transcriptions)
    {
        ArgumentNullException.ThrowIfNull(transcriptions);

        return transcriptions
            .Select(static row => new CTranscriptionDraft(
                row.LTranscriptionDraftId, row.LTranscriptionDraftScheme, row.LTranscriptionDraftText))
            .ToList();
    }

    internal static IReadOnlyList<CReflexDraft> CSoundingReflexRead(IReadOnlyList<LReflexDraft> reflexes)
    {
        ArgumentNullException.ThrowIfNull(reflexes);

        return reflexes.Select(CSoundingReflexRead).ToList();
    }

    private static CReflexDraft CSoundingReflexRead(LReflexDraft reflex)
    {
        return new CReflexDraft(
            reflex.LReflexDraftId,
            reflex.LReflexDraftLanguage,
            reflex.LReflexDraftKind,
            reflex.LReflexDraftText,
            reflex.LReflexDraftRespelling,
            reflex.LReflexDraftRomanization,
            reflex.LReflexDraftMeaning,
            reflex.LReflexDraftNote,
            reflex.LReflexDraftMain,
            reflex.LReflexDraftRegion,
            reflex.LReflexDraftAnchors,
            reflex.LReflexDraftAnatomy.LAnatomyToneIpa);
    }
}
