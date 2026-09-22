using System.Collections.Generic;
using System.Globalization;

namespace Llyn.Core;

internal static class LMarkupReader
{
    internal static void LMarkupAttributeScan(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        foreach ((string name, string value) in element.LMarkupNodeAttribute)
        {
            if (name != LMarkup.LMarkupState || value != LMarkup.LMarkupUnknown)
            {
                LMarkupOmissionAdd(omissions, element.LMarkupNodeLine, $"{name}=\"{value}\"");
            }
        }

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            LMarkupAttributeScan(child, omissions);
        }
    }

    internal static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, LMarkupNode element)
    {
        LMarkupOmissionAdd(omissions, element.LMarkupNodeLine, $"<{element.LMarkupNodeName}>");
    }

    private static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, int line, string text)
    {
        if (omissions.Count < LMarkup.LMarkupOmissionCeiling)
        {
            omissions.Add(new LMarkupOmission(line, text));
        }
        else if (omissions.Count == LMarkup.LMarkupOmissionCeiling)
        {
            omissions.Add(new LMarkupOmission(line, "..."));
        }
    }

    internal static int? LMarkupNumberParse(LMarkupNode element)
    {
        return int.TryParse(
            element.LMarkupNodeText.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int number)
            ? number
            : null;
    }

    internal static LMarkupEntry LMarkupEntryParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string headword = string.Empty;
        string language = string.Empty;
        string note = string.Empty;
        List<string> speeches = [];
        List<LForm> forms = [];
        List<LMarkupInflection> inflections = [];
        List<LPronunciationDraft> pronunciations = [];
        List<LTranscriptionDraft> transcriptions = [];
        List<LReflexDraft> reflexes = [];
        List<LMarkupCard> meanings = [];
        List<LMarkupCard> collocations = [];

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "headword":
                    headword = LMarkup.LMarkupTextParse(child);
                    break;
                case "language":
                    language = LMarkup.LMarkupTextParse(child);
                    break;
                case "speech":
                    speeches.Add(LMarkup.LMarkupTextParse(child));
                    break;
                case "form":
                    forms.Add(LMarkupFormParse(child, forms.Count, omissions));
                    break;
                case "inflection":
                    inflections.Add(LMarkupInflectionParse(child, omissions));
                    break;
                case "pronunciation":
                    pronunciations.Add(LMarkupPronunciationParse(child, omissions));
                    break;
                case "transcription":
                    transcriptions.Add(LMarkupTranscriptionParse(child, omissions));
                    break;
                case "reflex":
                    reflexes.Add(LMarkupReflexParse(child, omissions));
                    break;
                case "meaning":
                    meanings.Add(LMarkupCardReader.LMarkupCardParse(child, true, omissions));
                    break;
                case "collocation":
                    collocations.Add(LMarkupCardReader.LMarkupCardParse(child, false, omissions));
                    break;
                case "note":
                    note = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupEntry(
            headword,
            language,
            speeches,
            forms,
            inflections,
            pronunciations,
            transcriptions,
            reflexes,
            meanings,
            collocations,
            note,
            element.LMarkupNodeLine);
    }

    private static LForm LMarkupFormParse(LMarkupNode element, int position, List<LMarkupOmission> omissions)
    {
        string text = string.Empty;
        string? local = null;
        string role = string.Empty;

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "text":
                    text = LMarkup.LMarkupTextParse(child);
                    break;
                case "local":
                    local = LMarkup.LMarkupTextParse(child);
                    break;
                case "role":
                    role = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LForm(0, position, text, local, role);
    }

    private static LMarkupInflection LMarkupInflectionParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string text = string.Empty;
        string? local = null;
        string speech = string.Empty;
        List<string> morphologies = [];

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "text":
                    text = LMarkup.LMarkupTextParse(child);
                    break;
                case "local":
                    local = LMarkup.LMarkupTextParse(child);
                    break;
                case "speech":
                    speech = LMarkup.LMarkupTextParse(child);
                    break;
                case "morphology":
                    morphologies.Add(LMarkup.LMarkupTextParse(child));
                    break;
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupInflection(text, local, speech, morphologies);
    }

    private static LPronunciationDraft LMarkupPronunciationParse(LMarkupNode element, List<LMarkupOmission> omissions)
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
                    LMarkupOmissionAdd(omissions, child);
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
                    tone = LMarkupNumberParse(child);
                    break;
                case "points":
                    points = LMarkup.LMarkupTextParse(child);
                    break;
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LSyllable(0, position, onset, medial, nucleus, coda, tone, points);
    }

    private static LTranscriptionDraft LMarkupTranscriptionParse(LMarkupNode element, List<LMarkupOmission> omissions)
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
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LTranscriptionDraft(scheme, text);
    }

    private static LReflexDraft LMarkupReflexParse(LMarkupNode element, List<LMarkupOmission> omissions)
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
                    LMarkupOmissionAdd(omissions, child);
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
