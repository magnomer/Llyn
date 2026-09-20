using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.Application;

public static class LPortraitClerkCrest
{
    public static void LPortraitGlyphAdd(
        List<LPortraitSection> sections, LEntryDraft draft, LGlyph? glyph, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(label);

        if (glyph is null)
        {
            return;
        }

        string text = draft.LEntryDraftHeadword;
        foreach (LTranscriptionDraft spelled in draft.LEntryDraftTranscriptions)
        {
            if (string.Equals(spelled.LTranscriptionDraftScheme, glyph.LGlyphName, StringComparison.Ordinal)
                && !spelled.LTranscriptionDraftEmpty)
            {
                text = spelled.LTranscriptionDraftText;
                break;
            }
        }

        List<string> chips = [];
        foreach (Rune rune in text.EnumerateRunes())
        {
            chips.Add(rune.ToString());
        }

        if (chips.Count > 0)
        {
            sections.Add(new LPortraitSection(
                label.LPortraitLabelGlyph, [], string.Empty, [], [], LPortraitSectionChip: chips));
        }
    }

    public static void LPortraitFrequencyAdd(
        List<LPortraitSection> sections, IReadOnlyList<LFrequency> rows, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(label);

        if (rows.Count == 0)
        {
            return;
        }

        List<string> chips = [];
        List<LPortraitLine> lines = [];
        foreach (LFrequency row in rows)
        {
            if (row.LFrequencyBand is string band && chips.Count == 0)
            {
                chips.Add(band);
            }

            string figure = row.LFrequencyUnit is string unit
                ? unit + " " + row.LFrequencyRaw
                : row.LFrequencyRaw;
            lines.Add(new LPortraitLine(row.LFrequencySource, figure));
        }

        sections.Add(new LPortraitSection(
            label.LPortraitLabelFrequency, lines, string.Empty, [], [], LPortraitSectionChip: chips));
    }

    public static void LPortraitFormAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(draft);
        ArgumentNullException.ThrowIfNull(label);

        List<LPortraitLine> lines = [];
        foreach (LForm form in draft.LEntryDraftForms)
        {
            if (form.LFormText.Length == 0)
            {
                continue;
            }

            lines.Add(new LPortraitLine(form.LFormRole, LPortraitLocalShow(form.LFormText, form.LFormLocal)));
        }

        if (lines.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelForm, lines));
        }
    }

    public static void LPortraitParadigmAdd(
        List<LPortraitSection> sections, IReadOnlyList<LParadigmSlot> slots, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(slots);
        ArgumentNullException.ThrowIfNull(label);

        HashSet<long> parts = [];
        foreach (LParadigmSlot slot in slots)
        {
            parts.Add(slot.LParadigmSlotSpeech.LSpeechValueId);
        }

        List<LPortraitLine> lines = [];
        foreach (LParadigmSlot slot in slots)
        {
            string text = slot.LParadigmSlotState switch
            {
                LState.LStateSpecified => slot.LParadigmSlotInflection is LInflection inflection
                    ? LPortraitLocalShow(inflection.LInflectionText, inflection.LInflectionLocal)
                    : string.Empty,
                LState.LStateUnknown => label.LPortraitLabelUnknown,
                _ => string.Empty,
            };

            if (text.Length == 0)
            {
                continue;
            }

            string tag = parts.Count > 1
                ? slot.LParadigmSlotSpeech.LSpeechValueName + " " + slot.LParadigmSlotMorphology.LMorphologyName
                : slot.LParadigmSlotMorphology.LMorphologyName;
            lines.Add(new LPortraitLine(tag, text));
        }

        if (lines.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelParadigm, lines));
        }
    }

    public static void LPortraitFanqieAdd(
        List<LPortraitSection> sections, IReadOnlyList<LFanqieRow> rows, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(rows);
        ArgumentNullException.ThrowIfNull(label);

        HashSet<string> characters = [];
        foreach (LFanqieRow row in rows)
        {
            characters.Add(row.LFanqieRowCharacter);
        }

        List<LPortraitLine> lines = [];
        foreach (LFanqieRow row in rows)
        {
            bool parted = row.LFanqieRowInitial.Length > 0 || row.LFanqieRowRime.Length > 0;
            List<string> parts = [];
            LPortraitPartAdd(parts, parted ? row.LFanqieRowInitial + row.LFanqieRowRime : row.LFanqieRowText);
            LPortraitPartAdd(parts, row.LFanqieRowHeading, "[", "]");
            LPortraitPartAdd(parts, row.LFanqieRowDivision);
            LPortraitPartAdd(parts, row.LFanqieRowTone);
            LPortraitPartAdd(parts, row.LFanqieRowSpelling);
            LPortraitPartAdd(parts, row.LFanqieRowReading, "/", "/");

            string tag = characters.Count > 1
                ? row.LFanqieRowCharacter + " " + row.LFanqieRowBook
                : row.LFanqieRowBook;
            lines.Add(new LPortraitLine(tag, string.Join(" ", parts)));
        }

        if (lines.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelFanqie, lines));
        }
    }

    public static void LPortraitScriptAdd(
        List<LPortraitSection> sections, IReadOnlyList<LScriptImage> images, LPortraitLabel label)
    {
        ArgumentNullException.ThrowIfNull(sections);
        ArgumentNullException.ThrowIfNull(images);
        ArgumentNullException.ThrowIfNull(label);

        List<LPortraitMedia> plates = [];
        foreach (LScriptImage image in images)
        {
            plates.Add(LPortraitMedia.LPortraitMediaCreate(image.LScriptImageData, image.LScriptImageCaption));
        }

        if (plates.Count > 0)
        {
            sections.Add(new LPortraitSection(label.LPortraitLabelScript, [], string.Empty, plates, []));
        }
    }

    private static string LPortraitLocalShow(string text, string? local)
    {
        return string.IsNullOrEmpty(local) ? text : text + " " + local;
    }

    private static void LPortraitPartAdd(List<string> parts, string text, string open = "", string close = "")
    {
        if (text.Length > 0)
        {
            parts.Add(open + text + close);
        }
    }
}
