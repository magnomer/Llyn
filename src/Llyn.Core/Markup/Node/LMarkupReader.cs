using System;
using System.Collections.Generic;

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
        List<LMarkupEtymon> etymons = [];
        LMarkupEtymology? etymology = null;
        LUnit unit = LUnit.LUnitEmpty;
        LFreedom freedom = LFreedom.LFreedomEmpty;

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
                case "unit":
                    string unitText = LMarkup.LMarkupTextParse(child);
                    unit = LUnitKey.LUnitKeyParse(unitText);
                    if (unit == LUnit.LUnitEmpty && !string.IsNullOrWhiteSpace(unitText))
                    {
                        LMarkupOmissionAdd(omissions, child);
                    }

                    break;
                case "freedom":
                    string freedomText = LMarkup.LMarkupTextParse(child);
                    freedom = LFreedomKey.LFreedomKeyParse(freedomText);
                    if (freedom == LFreedom.LFreedomEmpty && !string.IsNullOrWhiteSpace(freedomText))
                    {
                        LMarkupOmissionAdd(omissions, child);
                    }

                    break;
                case "form":
                    forms.Add(LMarkupFormParse(child, forms.Count, omissions));
                    break;
                case "inflection":
                    inflections.Add(LMarkupInflectionParse(child, omissions));
                    break;
                case "pronunciation":
                    pronunciations.Add(LMarkupPronunciationReader.LMarkupPronunciationParse(child, omissions));
                    break;
                case "transcription":
                    LMarkupTranscriptionAdd(transcriptions, child, omissions);
                    break;
                case "reflex":
                    reflexes.Add(LMarkupPronunciationReader.LMarkupReflexParse(child, omissions));
                    break;
                case "etymon":
                    etymons.Add(LMarkupEtymonParse(child, omissions));
                    break;
                case "etymology":
                    etymology = LMarkupEtymologyParse(child, omissions);
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
            element.LMarkupNodeLine,
            etymology,
            etymons,
            unit,
            freedom);
    }

    private static void LMarkupTranscriptionAdd(
        List<LTranscriptionDraft> transcriptions, LMarkupNode element, List<LMarkupOmission> omissions)
    {
        LTranscriptionDraft transcription = LMarkupPronunciationReader.LMarkupTranscriptionParse(element, omissions);
        string scheme = transcription.LTranscriptionDraftScheme.Trim();
        if (scheme.Length == 0 || transcriptions.Exists(held => string.Equals(
                held.LTranscriptionDraftScheme.Trim(), scheme, StringComparison.Ordinal)))
        {
            LMarkupOmissionAdd(omissions, element);
            return;
        }

        transcriptions.Add(transcription);
    }

    private static LMarkupEtymon LMarkupEtymonParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string headword = string.Empty;
        string language = string.Empty;

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
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupEtymon(headword, language);
    }

    private static LMarkupEtymology LMarkupEtymologyParse(LMarkupNode element, List<LMarkupOmission> omissions)
    {
        string text = string.Empty;
        List<LMarkupMention> mentions = [];

        foreach (LMarkupNode child in element.LMarkupNodeChild)
        {
            switch (child.LMarkupNodeName)
            {
                case "text":
                    text = LMarkup.LMarkupTextParse(child);
                    break;
                case "mention":
                    mentions.Add(LMarkupCardReader.LMarkupMentionParse(child, false, omissions));
                    break;
                default:
                    LMarkupOmissionAdd(omissions, child);
                    break;
            }
        }

        return new LMarkupEtymology(text, mentions);
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
}
