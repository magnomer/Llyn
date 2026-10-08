using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.Infrastructure;

public sealed class LEntryLoaderReading
{
    private readonly LDatabase _lEntryReadingDatabase;

    public LEntryLoaderReading(LDatabase database)
    {
        ArgumentNullException.ThrowIfNull(database);
        _lEntryReadingDatabase = database;
    }

    public IReadOnlyList<LPronunciationDraft> LEntrySoundRead(long id)
    {
        LPronunciationArchive pronunciations = new(_lEntryReadingDatabase);
        List<LPronunciationDraft> drafts = [];
        foreach (LPronunciation pronunciation in pronunciations.LPronunciationRead(id))
        {
            LPronunciationAudio? audio = pronunciations.LPronunciationAudioRead(pronunciation.LPronunciationId);
            drafts.Add(new LPronunciationDraft(
                pronunciation.LPronunciationIpa ?? string.Empty,
                pronunciation.LPronunciationSyllables,
                audio?.LPronunciationAudioFile ?? string.Empty,
                audio?.LPronunciationAudioSource,
                pronunciation.LPronunciationId,
                pronunciation.LPronunciationVariety ?? string.Empty,
                LPronunciationDraftRespelling: pronunciation.LPronunciationRespelling ?? string.Empty));
        }

        return drafts;
    }

    public IReadOnlyList<LTranscriptionDraft> LEntrySpellingRead(long id)
    {
        List<LTranscriptionDraft> drafts = [];
        LTranscriptionArchive transcriptions = new(_lEntryReadingDatabase);
        foreach (LTranscription transcription in transcriptions.LTranscriptionRead(id))
        {
            drafts.Add(new LTranscriptionDraft(
                transcription.LTranscriptionScheme,
                transcription.LTranscriptionText,
                transcription.LTranscriptionId));
        }

        return drafts;
    }

    public IReadOnlyList<LReflexDraft> LEntryReflexRead(long id)
    {
        List<LReflexDraft> drafts = [];
        foreach (LReflex reflex in new LReflexArchive(_lEntryReadingDatabase).LReflexRead(id))
        {
            drafts.Add(new LReflexDraft(
                reflex.LReflexLanguage,
                reflex.LReflexKind,
                reflex.LReflexText,
                reflex.LReflexMain,
                reflex.LReflexId,
                reflex.LReflexRomanization,
                reflex.LReflexMeaning,
                reflex.LReflexOwned,
                reflex.LReflexNote,
                reflex.LReflexRespelling,
                reflex.LReflexRegion,
                reflex.LReflexAnatomy,
                reflex.LReflexAnchors));
        }

        return drafts;
    }
}
