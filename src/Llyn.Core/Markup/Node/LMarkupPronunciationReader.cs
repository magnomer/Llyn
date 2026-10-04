using System.Collections.Generic;

namespace Llyn.Core;

internal static class LMarkupPronunciationReader
{
    internal static LPronunciationDraft LMarkupPronunciationParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string ipa = string.Empty;
        string respelling = string.Empty;
        string variety = string.Empty;
        string audio = string.Empty;
        string? source = null;
        List<LSyllable> syllables = [];

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "ipa":
                    ipa = LMarkup.LMarkupTextParse(child);
                    break;
                case "respelling":
                    respelling = LMarkup.LMarkupTextParse(child);
                    break;
                case "variety":
                    variety = LMarkup.LMarkupTextParse(child);
                    break;
                case "syllable":
                    syllables.Add(LMarkupSyllableParse(child, syllables.Count, omissions));
                    break;
                case "audio":
                    audio = LMarkup.LMarkupTextParse(child);
                    break;
                case "source":
                    source = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LPronunciationDraft(
            ipa,
            syllables,
            audio,
            source,
            0,
            variety,
            LPronunciationDraftRespelling: respelling);
    }

    private static LSyllable LMarkupSyllableParse(LMarkupNode element, int position, List<LMarkupOmission> omissions)
    {
        string? onset = null;
        string? medial = null;
        string nucleus = string.Empty;
        string? coda = null;
        int? tone = null;
        string? points = null;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "onset":
                    onset = LMarkup.LMarkupTextParse(child);
                    break;
                case "medial":
                    medial = LMarkup.LMarkupTextParse(child);
                    break;
                case "nucleus":
                    nucleus = LMarkup.LMarkupTextParse(child);
                    break;
                case "coda":
                    coda = LMarkup.LMarkupTextParse(child);
                    break;
                case "tone":
                    tone = LMarkup.LMarkupNumberParse(child);
                    break;
                case "points":
                    points = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LSyllable(0, position, onset, medial, nucleus, coda, tone, points);
    }

    internal static LTranscriptionDraft LMarkupTranscriptionParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string scheme = string.Empty;
        string text = string.Empty;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "scheme":
                    scheme = LMarkup.LMarkupTextParse(child);
                    break;
                case "text":
                    text = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LTranscriptionDraft(scheme, text);
    }

    internal static LReflexDraft LMarkupReflexParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string language = string.Empty;
        string kind = string.Empty;
        string text = string.Empty;
        string respelling = string.Empty;
        string romanization = string.Empty;
        string meaning = string.Empty;
        string note = string.Empty;
        string region = string.Empty;
        bool main = false;
        bool owned = false;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "language":
                    language = LMarkup.LMarkupTextParse(child);
                    break;
                case "kind":
                    kind = LMarkup.LMarkupTextParse(child);
                    break;
                case "text":
                    text = LMarkup.LMarkupTextParse(child);
                    break;
                case "respelling":
                    respelling = LMarkup.LMarkupTextParse(child);
                    break;
                case "romanization":
                    romanization = LMarkup.LMarkupTextParse(child);
                    break;
                case "meaning":
                    meaning = LMarkup.LMarkupTextParse(child);
                    break;
                case "note":
                    note = LMarkup.LMarkupTextParse(child);
                    break;
                case "region":
                    region = LMarkup.LMarkupTextParse(child);
                    break;
                case "owned":
                    owned = true;
                    break;
                case "main":
                    main = true;
                    break;
                default:
                    LMarkupReader.LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LReflexDraft(
            language,
            kind,
            text,
            main,
            LReflexDraftRomanization: romanization,
            LReflexDraftMeaning: meaning,
            LReflexDraftOwned: owned,
            LReflexDraftNote: note,
            LReflexDraftRespelling: respelling,
            LReflexDraftRegion: region);
    }
}
