using System.Globalization;
using System.Xml.Linq;

namespace Llyn.Core;

internal static class LMarkupWriter
{
    internal static XElement LMarkupEntryFormat(LMarkupEntry entry)
    {
        XElement element = new("entry");
        LMarkup.LMarkupTextFormat(element, "headword", entry.LMarkupEntryHeadword);
        LMarkup.LMarkupTextFormat(element, "language", entry.LMarkupEntryLanguage);

        foreach (string speech in entry.LMarkupEntrySpeech)
        {
            element.Add(new XElement("speech", LMarkup.LMarkupTextNormalize(speech)));
        }

        foreach (LForm form in entry.LMarkupEntryForm)
        {
            element.Add(LMarkupFormFormat(form));
        }

        foreach (LMarkupInflection inflection in entry.LMarkupEntryInflection)
        {
            element.Add(LMarkupInflectionFormat(inflection));
        }

        foreach (LPronunciationDraft pronunciation in entry.LMarkupEntryPronunciation)
        {
            element.Add(LMarkupPronunciationFormat(pronunciation));
        }

        foreach (LTranscriptionDraft transcription in entry.LMarkupEntryTranscription)
        {
            element.Add(LMarkupTranscriptionFormat(transcription));
        }

        foreach (LMarkupCard card in entry.LMarkupEntryMeaning)
        {
            element.Add(LMarkupCardWriter.LMarkupCardFormat(card, "meaning"));
        }

        foreach (LMarkupCard card in entry.LMarkupEntryCollocation)
        {
            element.Add(LMarkupCardWriter.LMarkupCardFormat(card, "collocation"));
        }

        LMarkup.LMarkupTextFormat(element, "note", entry.LMarkupEntryNote);
        return element;
    }

    private static XElement LMarkupFormFormat(LForm form)
    {
        XElement element = new("form");
        LMarkup.LMarkupTextFormat(element, "text", form.LFormText);
        LMarkupLocalFormat(element, form.LFormLocal);
        LMarkup.LMarkupTextFormat(element, "role", form.LFormRole);
        return element;
    }

    private static void LMarkupLocalFormat(XElement parent, string? local)
    {
        if (local is not null)
        {
            parent.Add(new XElement("local", LMarkup.LMarkupTextNormalize(local)));
        }
    }

    private static XElement LMarkupInflectionFormat(LMarkupInflection inflection)
    {
        XElement element = new("inflection");
        LMarkup.LMarkupTextFormat(element, "text", inflection.LMarkupInflectionText);
        LMarkupLocalFormat(element, inflection.LMarkupInflectionLocal);
        LMarkup.LMarkupTextFormat(element, "speech", inflection.LMarkupInflectionSpeech);

        foreach (string morphology in inflection.LMarkupInflectionMorphology)
        {
            element.Add(new XElement("morphology", LMarkup.LMarkupTextNormalize(morphology)));
        }

        return element;
    }

    private static XElement LMarkupPronunciationFormat(LPronunciationDraft pronunciation)
    {
        XElement element = new("pronunciation");
        LMarkup.LMarkupTextFormat(element, "ipa", pronunciation.LPronunciationDraftIpa);
        LMarkup.LMarkupTextFormat(element, "variety", pronunciation.LPronunciationDraftVariety);

        foreach (LSyllable syllable in pronunciation.LPronunciationDraftSyllables)
        {
            element.Add(LMarkupSyllableFormat(syllable));
        }

        LMarkup.LMarkupTextFormat(element, "audio", pronunciation.LPronunciationDraftAudio);
        LMarkup.LMarkupTextFormat(element, "source", pronunciation.LPronunciationDraftSource);
        return element;
    }

    private static XElement LMarkupSyllableFormat(LSyllable syllable)
    {
        XElement element = new("syllable");
        LMarkup.LMarkupTextFormat(element, "onset", syllable.LSyllableOnset);
        LMarkup.LMarkupTextFormat(element, "medial", syllable.LSyllableMedial);
        LMarkup.LMarkupTextFormat(element, "nucleus", syllable.LSyllableNucleus);
        LMarkup.LMarkupTextFormat(element, "coda", syllable.LSyllableCoda);

        if (syllable.LSyllableToneNumber is int tone)
        {
            element.Add(new XElement("tone", tone.ToString(CultureInfo.InvariantCulture)));
        }

        LMarkup.LMarkupTextFormat(element, "points", syllable.LSyllableTonePoints);
        return element;
    }

    private static XElement LMarkupTranscriptionFormat(LTranscriptionDraft transcription)
    {
        XElement element = new("transcription");
        LMarkup.LMarkupTextFormat(element, "scheme", transcription.LTranscriptionDraftScheme);
        LMarkup.LMarkupTextFormat(element, "text", transcription.LTranscriptionDraftText);
        return element;
    }
}
