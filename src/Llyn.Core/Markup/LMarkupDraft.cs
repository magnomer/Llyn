using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Llyn.Core;

public static class LMarkupDraft
{
    public static string LMarkupDraftFormat(
        LMarkup.LMarkupDocument document, IReadOnlyDictionary<string, string> keys)
    {
        ArgumentNullException.ThrowIfNull(keys);

        StringBuilder text = new StringBuilder();
        text.Append("<llyn>\n");

        LMarkupDraftAppend(text, document.LMarkupDocumentCatalog);

        foreach (LMarkup.LMarkupEntry entry in document.LMarkupDocumentEntry)
        {
            LMarkupDraftAppend(text, entry, keys);
        }

        text.Append("</llyn>\n");
        return text.ToString();
    }

    private static void LMarkupDraftAppend(StringBuilder text, LMarkup.LMarkupCatalog catalog)
    {
        if (catalog.LMarkupCatalogAuthor.Count == 0
            && catalog.LMarkupCatalogSource.Count == 0
            && catalog.LMarkupCatalogExample.Count == 0
            && catalog.LMarkupCatalogSituation.Count == 0
            && catalog.LMarkupCatalogRegister.Count == 0
            && catalog.LMarkupCatalogImage.Count == 0
            && catalog.LMarkupCatalogVideo.Count == 0)
        {
            return;
        }

        text.Append("  <catalog>\n");

        foreach (KeyValuePair<string, LAuthor> row in LMarkupDraftSort(catalog.LMarkupCatalogAuthor))
        {
            text.Append("    <author id=")
                .Append(LMarkupMark.LMarkupMarkNormalize(row.Key))
                .Append('>')
                .Append(LMarkupLeaf.LMarkupLeafNormalize(row.Value.LAuthorName, "author"))
                .Append("</author>\n");
        }

        foreach (KeyValuePair<string, LMarkup.LMarkupReference> row
            in LMarkupDraftSort(catalog.LMarkupCatalogSource))
        {
            LMarkupSource.LMarkupSourceAppend(text, 2, row.Key, row.Value);
        }

        foreach (KeyValuePair<string, LExample> row in LMarkupDraftSort(catalog.LMarkupCatalogExample))
        {
            LMarkupExample.LMarkupExampleAppend(text, 2, row.Key, row.Value);
        }

        foreach (KeyValuePair<string, LSituation> row in LMarkupDraftSort(catalog.LMarkupCatalogSituation))
        {
            text.Append("    <situation id=")
                .Append(LMarkupMark.LMarkupMarkNormalize(row.Key))
                .Append(">\n");
            LMarkupLeaf.LMarkupLeafAppend(text, 3, "title", row.Value.LSituationTitle);
            LMarkupLeaf.LMarkupLeafAppend(text, 3, "description", row.Value.LSituationDescription);
            LMarkupLeaf.LMarkupLeafAppend(text, 3, "kind", row.Value.LSituationKind);
            text.Append("    </situation>\n");
        }

        foreach (KeyValuePair<string, LRegister> row in LMarkupDraftSort(catalog.LMarkupCatalogRegister))
        {
            text.Append("    <register id=")
                .Append(LMarkupMark.LMarkupMarkNormalize(row.Key));

            if (row.Value.LRegisterLanguage.Length > 0)
            {
                text.Append(" lang=")
                    .Append(LMarkupMark.LMarkupMarkNormalize(row.Value.LRegisterLanguage));
            }

            if (row.Value.LRegisterBuiltin)
            {
                text.Append(" builtin=\"yes\"");
            }

            text.Append(">\n");
            LMarkupLeaf.LMarkupLeafAppend(text, 3, "name", row.Value.LRegisterName);
            text.Append("    </register>\n");
        }

        foreach (KeyValuePair<string, LImage> row in LMarkupDraftSort(catalog.LMarkupCatalogImage))
        {
            text.Append("    <image id=")
                .Append(LMarkupMark.LMarkupMarkNormalize(row.Key))
                .Append('>')
                .Append(LMarkupLeaf.LMarkupLeafNormalize(
                    row.Value.LImageLocation.LStateValueShow(), "image"))
                .Append("</image>\n");
        }

        foreach (KeyValuePair<string, LVideo> row in LMarkupDraftSort(catalog.LMarkupCatalogVideo))
        {
            text.Append("    <video id=")
                .Append(LMarkupMark.LMarkupMarkNormalize(row.Key))
                .Append(">\n");
            LMarkupLeaf.LMarkupLeafAppend(text, 3, "location", row.Value.LVideoLocation);
            LMarkupLeaf.LMarkupLeafAppend(text, 3, "span", row.Value.LVideoSpan);
            text.Append("    </video>\n");
        }

        text.Append("  </catalog>\n");
    }

    private static void LMarkupDraftAppend(
        StringBuilder text, LMarkup.LMarkupEntry entry, IReadOnlyDictionary<string, string> keys)
    {
        LEntryDraft draft = entry.LMarkupEntryDraft;

        text.Append("  <entry");
        if (entry.LMarkupEntryKey.Length > 0)
        {
            text.Append(" id=").Append(LMarkupMark.LMarkupMarkNormalize(entry.LMarkupEntryKey));
        }

        text.Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(
            text, 2, "headword", LStateValue.LStateValueRead(draft.LEntryDraftHeadword));
        LMarkupLeaf.LMarkupLeafAppend(
            text, 2, "lang", LStateValue.LStateValueRead(draft.LEntryDraftLanguage));
        LMarkupLeaf.LMarkupLeafAppend(
            text, 2, "note", LStateValue.LStateValueRead(draft.LEntryDraftNote));

        foreach (LForm form in draft.LEntryDraftForms)
        {
            LMarkupDraftAppend(text, form);
        }

        foreach (LSpeechDraft speech in draft.LEntryDraftSpeeches)
        {
            LMarkupDraftAppend(text, speech);
        }

        foreach (LInflection inflection in draft.LEntryDraftInflections)
        {
            LMarkupDraftAppend(text, inflection);
        }

        LMarkupDraftAppend(text, draft.LEntryDraftPronunciation);

        foreach (LCardDraft card in draft.LEntryDraftMeanings)
        {
            LMarkupCard.LMarkupCardAppend(text, 2, "sense", card, keys);
        }

        foreach (LCardDraft card in draft.LEntryDraftCollocations)
        {
            LMarkupCard.LMarkupCardAppend(text, 2, "collocation", card, keys);
        }

        text.Append("  </entry>\n");
    }

    private static void LMarkupDraftAppend(StringBuilder text, LForm form)
    {
        if (form.LFormText.Length == 0)
        {
            return;
        }

        text.Append("    <form");
        LMarkupMark.LMarkupMarkAppend(text, "role", form.LFormRole);
        LMarkupMark.LMarkupMarkAppend(text, "local", form.LFormLocal);
        text.Append('>')
            .Append(LMarkupLeaf.LMarkupLeafNormalize(form.LFormText, "form"))
            .Append("</form>\n");
    }

    private static void LMarkupDraftAppend(StringBuilder text, LSpeechDraft speech)
    {
        if (speech.LSpeechDraftEmpty)
        {
            return;
        }

        if (speech.LSpeechDraftValue is string value && value.Length > 0)
        {
            text.Append("    <pos id=")
                .Append(LMarkupMark.LMarkupMarkNormalize(value))
                .Append("/>\n");
            return;
        }

        text.Append("    <pos>")
            .Append(LMarkupLeaf.LMarkupLeafNormalize(speech.LSpeechDraftCustom ?? string.Empty, "pos"))
            .Append("</pos>\n");
    }

    private static void LMarkupDraftAppend(StringBuilder text, LInflection inflection)
    {
        if (inflection.LInflectionText.Length == 0)
        {
            return;
        }

        text.Append("    <inflection");
        LMarkupMark.LMarkupMarkAppend(text, "local", inflection.LInflectionLocal);
        LMarkupMark.LMarkupMarkAppend(text, "pos", inflection.LInflectionSpeechId);
        text.Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(
            text, 3, "text", LStateValue.LStateValueRead(inflection.LInflectionText));

        foreach (LFeature feature in inflection.LInflectionFeatures)
        {
            text.Append("      <feature");
            LMarkupMark.LMarkupMarkAppend(text, "id", feature.LFeatureId);
            LMarkupMark.LMarkupMarkAppend(text, "value", feature.LFeatureValueId);
            text.Append("/>\n");
        }

        text.Append("    </inflection>\n");
    }

    private static void LMarkupDraftAppend(StringBuilder text, LPronunciationDraft? pronunciation)
    {
        if (pronunciation is null || pronunciation.LPronunciationDraftEmpty)
        {
            return;
        }

        text.Append("    <pronunciation");
        LMarkupMark.LMarkupMarkAppend(text, "level", pronunciation.LPronunciationDraftLevel);
        text.Append(">\n");

        LMarkupLeaf.LMarkupLeafAppend(
            text, 3, "ipa", LStateValue.LStateValueRead(pronunciation.LPronunciationDraftIpa));

        foreach (LSyllable syllable in pronunciation.LPronunciationDraftSyllables)
        {
            LMarkupDraftAppend(text, syllable);
        }

        foreach (LRepresentation representation in pronunciation.LPronunciationDraftRepresentations)
        {
            LMarkupDraftAppend(text, representation);
        }

        LMarkupLeaf.LMarkupLeafAppend(
            text,
            3,
            "audio",
            LStateValue.LStateValueRead(pronunciation.LPronunciationDraftAudio),
            "source",
            pronunciation.LPronunciationDraftSource);

        text.Append("    </pronunciation>\n");
    }

    private static void LMarkupDraftAppend(StringBuilder text, LSyllable syllable)
    {
        if (syllable.LSyllableNucleus.Length == 0)
        {
            return;
        }

        text.Append("      <syllable");
        LMarkupMark.LMarkupMarkAppend(text, "onset", syllable.LSyllableOnset);
        LMarkupMark.LMarkupMarkAppend(text, "medial", syllable.LSyllableMedial);
        LMarkupMark.LMarkupMarkAppend(text, "nucleus", syllable.LSyllableNucleus);
        LMarkupMark.LMarkupMarkAppend(text, "coda", syllable.LSyllableCoda);
        LMarkupMark.LMarkupMarkAppend(text, "orthography", syllable.LSyllableOrthography);
        LMarkupMark.LMarkupMarkAppend(text, "local", syllable.LSyllableLocal);
        LMarkupMark.LMarkupMarkAppend(
            text,
            "tone",
            syllable.LSyllableToneNumber?.ToString(CultureInfo.InvariantCulture));
        LMarkupMark.LMarkupMarkAppend(text, "tone-local", syllable.LSyllableToneLocal);
        LMarkupMark.LMarkupMarkAppend(text, "tone-points", syllable.LSyllableTonePoints);
        text.Append("/>\n");
    }

    private static void LMarkupDraftAppend(StringBuilder text, LRepresentation representation)
    {
        if (representation.LRepresentationText.Length == 0)
        {
            return;
        }

        text.Append("      <representation");
        LMarkupMark.LMarkupMarkAppend(text, "system", representation.LRepresentationSystem);
        LMarkupMark.LMarkupMarkAppend(text, "role", representation.LRepresentationRole);
        LMarkupMark.LMarkupMarkAppend(text, "tone", representation.LRepresentationLocalTone);
        text.Append('>')
            .Append(LMarkupLeaf.LMarkupLeafNormalize(
                representation.LRepresentationText, "representation"))
            .Append("</representation>\n");
    }

    private static List<KeyValuePair<string, LMarkupRow>> LMarkupDraftSort<LMarkupRow>(
        IReadOnlyDictionary<string, LMarkupRow> rows)
    {
        List<KeyValuePair<string, LMarkupRow>> sorted = new List<KeyValuePair<string, LMarkupRow>>(rows.Count);
        foreach (KeyValuePair<string, LMarkupRow> row in rows)
        {
            sorted.Add(row);
        }

        sorted.Sort((one, other) => string.CompareOrdinal(one.Key, other.Key));
        return sorted;
    }
}
