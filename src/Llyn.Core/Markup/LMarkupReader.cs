using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.Linq;

namespace Llyn.Core;

internal static class LMarkupReader
{
    internal static void LMarkupDepthValidate(string text)
    {
        XmlReaderSettings settings = new()
        {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = null,
        };

        try
        {
            using XmlReader reader = XmlReader.Create(new StringReader(text), settings);
            while (reader.Read())
            {
                if (reader.NodeType == XmlNodeType.Element && reader.Depth >= LMarkup.LMarkupDepthCeiling)
                {
                    throw new LRefusal(LRefusal.LRefusalMarkup);
                }
            }
        }
        catch (XmlException)
        {
            throw new LRefusal(LRefusal.LRefusalMarkup);
        }
    }

    internal static void LMarkupAttributeScan(XElement root, List<LMarkupOmission> omissions)
    {
        foreach (XElement element in root.DescendantsAndSelf())
        {
            foreach (XAttribute attribute in element.Attributes())
            {
                if (attribute.Name.LocalName != LMarkup.LMarkupState || attribute.Value != LMarkup.LMarkupUnknown)
                {
                    LMarkupOmissionAdd(
                        omissions,
                        LMarkupLineRead(element),
                        $"{attribute.Name.LocalName}=\"{attribute.Value}\"");
                }
            }
        }
    }

    internal static void LMarkupOmissionAdd(List<LMarkupOmission> omissions, XElement element)
    {
        LMarkupOmissionAdd(omissions, LMarkupLineRead(element), $"<{element.Name.LocalName}>");
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

    internal static int LMarkupLineRead(XElement element)
    {
        IXmlLineInfo info = element;
        return info.HasLineInfo() ? info.LineNumber : 0;
    }

    internal static int? LMarkupNumberParse(XElement element)
    {
        return int.TryParse(element.Value.Trim(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int number)
            ? number
            : null;
    }

    internal static LMarkupEntry LMarkupEntryParse(XElement element, List<LMarkupOmission> omissions)
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

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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
            LMarkupLineRead(element));
    }

    private static LForm LMarkupFormParse(XElement element, int position, List<LMarkupOmission> omissions)
    {
        string text = string.Empty;
        string? local = null;
        string role = string.Empty;

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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

    private static LMarkupInflection LMarkupInflectionParse(XElement element, List<LMarkupOmission> omissions)
    {
        string text = string.Empty;
        string? local = null;
        string speech = string.Empty;
        List<string> morphologies = [];

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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

    private static LPronunciationDraft LMarkupPronunciationParse(XElement element, List<LMarkupOmission> omissions)
    {
        string ipa = string.Empty;
        string respelling = string.Empty;
        string variety = string.Empty;
        string audio = string.Empty;
        string? source = null;
        List<LSyllable> syllables = [];

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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

    private static LSyllable LMarkupSyllableParse(XElement element, int position, List<LMarkupOmission> omissions)
    {
        string? onset = null;
        string? medial = null;
        string nucleus = string.Empty;
        string? coda = null;
        int? tone = null;
        string? points = null;

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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

    private static LTranscriptionDraft LMarkupTranscriptionParse(XElement element, List<LMarkupOmission> omissions)
    {
        string scheme = string.Empty;
        string text = string.Empty;

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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

    private static LReflexDraft LMarkupReflexParse(XElement element, List<LMarkupOmission> omissions)
    {
        string language = string.Empty;
        string kind = string.Empty;
        string text = string.Empty;
        string respelling = string.Empty;
        string note = string.Empty;
        bool main = false;

        foreach (XElement child in element.Elements())
        {
            switch (child.Name.LocalName)
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
                case "note":
                    note = LMarkup.LMarkupTextParse(child);
                    break;
                case "main":
                    main = true;
                    break;
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LReflexDraft(language, kind, text, main, LReflexDraftNote: note, LReflexDraftRespelling: respelling);
    }
}
