using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

internal static class LMarkupWriter
{
    internal static LMarkupNode LMarkupEntryFormat(LMarkupEntry entry)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "headword", entry.LMarkupEntryHeadword);
        LMarkup.LMarkupTextFormat(element, "language", entry.LMarkupEntryLanguage);

        foreach (string speech in entry.LMarkupEntrySpeech)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("speech", speech));
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

        foreach (LReflexDraft reflex in entry.LMarkupEntryReflex)
        {
            element.Add(LMarkupReflexFormat(reflex));
        }

        foreach (LMarkupEtymon etymon in entry.LMarkupEntryEtymon)
        {
            element.Add(LMarkupEtymonFormat(etymon));
        }

        if (entry.LMarkupEntryEtymology is LMarkupEtymology etymology
            && etymology.LMarkupEtymologyText.Length > 0)
        {
            element.Add(LMarkupEtymologyFormat(etymology));
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
        return LMarkupNode.LMarkupNodeCreate("entry", element);
    }

    private static LMarkupNode LMarkupEtymonFormat(LMarkupEtymon etymon)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "headword", etymon.LMarkupEtymonHeadword);
        LMarkup.LMarkupTextFormat(element, "language", etymon.LMarkupEtymonLanguage);
        return LMarkupNode.LMarkupNodeCreate("etymon", element);
    }

    private static LMarkupNode LMarkupEtymologyFormat(LMarkupEtymology etymology)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "text", etymology.LMarkupEtymologyText);

        foreach (LMarkupMention mention in etymology.LMarkupEtymologyMention)
        {
            element.Add(LMarkupCardWriter.LMarkupMentionFormat(mention));
        }

        return LMarkupNode.LMarkupNodeCreate("etymology", element);
    }

    private static LMarkupNode LMarkupFormFormat(LForm form)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "text", form.LFormText);
        LMarkupLocalFormat(element, form.LFormLocal);
        LMarkup.LMarkupTextFormat(element, "role", form.LFormRole);
        return LMarkupNode.LMarkupNodeCreate("form", element);
    }

    private static void LMarkupLocalFormat(List<LMarkupNode> parent, string? local)
    {
        if (local is not null)
        {
            parent.Add(LMarkupNode.LMarkupNodeCreate("local", local));
        }
    }

    private static LMarkupNode LMarkupInflectionFormat(LMarkupInflection inflection)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "text", inflection.LMarkupInflectionText);
        LMarkupLocalFormat(element, inflection.LMarkupInflectionLocal);
        LMarkup.LMarkupTextFormat(element, "speech", inflection.LMarkupInflectionSpeech);

        foreach (string morphology in inflection.LMarkupInflectionMorphology)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("morphology", morphology));
        }

        return LMarkupNode.LMarkupNodeCreate("inflection", element);
    }

    private static LMarkupNode LMarkupPronunciationFormat(LPronunciationDraft pronunciation)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "ipa", pronunciation.LPronunciationDraftIpa);
        LMarkup.LMarkupTextFormat(element, "respelling", pronunciation.LPronunciationDraftRespelling);
        LMarkup.LMarkupTextFormat(element, "variety", pronunciation.LPronunciationDraftVariety);

        foreach (LSyllable syllable in pronunciation.LPronunciationDraftSyllables)
        {
            element.Add(LMarkupSyllableFormat(syllable));
        }

        LMarkup.LMarkupTextFormat(element, "audio", pronunciation.LPronunciationDraftAudio);
        LMarkup.LMarkupTextFormat(element, "source", pronunciation.LPronunciationDraftSource);
        return LMarkupNode.LMarkupNodeCreate("pronunciation", element);
    }

    private static LMarkupNode LMarkupSyllableFormat(LSyllable syllable)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "onset", syllable.LSyllableOnset);
        LMarkup.LMarkupTextFormat(element, "medial", syllable.LSyllableMedial);
        LMarkup.LMarkupTextFormat(element, "nucleus", syllable.LSyllableNucleus);
        LMarkup.LMarkupTextFormat(element, "coda", syllable.LSyllableCoda);

        if (syllable.LSyllableToneNumber is int tone)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("tone", tone.ToString(CultureInfo.InvariantCulture)));
        }

        LMarkup.LMarkupTextFormat(element, "points", syllable.LSyllableTonePoints);
        return LMarkupNode.LMarkupNodeCreate("syllable", element);
    }

    private static LMarkupNode LMarkupTranscriptionFormat(LTranscriptionDraft transcription)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "scheme", transcription.LTranscriptionDraftScheme);
        LMarkup.LMarkupTextFormat(element, "text", transcription.LTranscriptionDraftText);
        return LMarkupNode.LMarkupNodeCreate("transcription", element);
    }

    private static LMarkupNode LMarkupReflexFormat(LReflexDraft reflex)
    {
        List<LMarkupNode> element = [];
        LMarkup.LMarkupTextFormat(element, "language", reflex.LReflexDraftLanguage);
        LMarkup.LMarkupTextFormat(element, "kind", reflex.LReflexDraftKind);
        LMarkup.LMarkupTextFormat(element, "text", reflex.LReflexDraftText);
        LMarkup.LMarkupTextFormat(element, "respelling", reflex.LReflexDraftRespelling);
        LMarkup.LMarkupTextFormat(element, "romanization", reflex.LReflexDraftRomanization);
        LMarkup.LMarkupTextFormat(element, "meaning", reflex.LReflexDraftMeaning);
        LMarkup.LMarkupTextFormat(element, "note", reflex.LReflexDraftNote);
        LMarkup.LMarkupTextFormat(element, "region", reflex.LReflexDraftRegion);
        if (reflex.LReflexDraftOwned)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("owned"));
        }

        if (reflex.LReflexDraftMain)
        {
            element.Add(LMarkupNode.LMarkupNodeCreate("main"));
        }

        return LMarkupNode.LMarkupNodeCreate("reflex", element);
    }
}
