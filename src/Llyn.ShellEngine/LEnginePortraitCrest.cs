using System;
using System.Collections.Generic;
using System.Text;
using Llyn.Core;

namespace Llyn.ShellEngine;

public sealed partial class LEngine
{
    private void LEngineGlyphAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)
    {
        LGlyph? glyph;
        try
        {
            glyph = draft.LEntryDraftLanguage.Length == 0 ? null : LEngineGlyphRead(draft.LEntryDraftLanguage);
        }
        catch (Exception)
        {
            glyph = null;
        }

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

    private void LEngineFrequencyAdd(List<LPortraitSection> sections, long entryId, LPortraitLabel label)
    {
        IReadOnlyList<LFrequency> rows;
        try
        {
            rows = LEngineFrequencyRead(entryId);
        }
        catch (Exception)
        {
            rows = [];
        }

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

    private static void LEngineFormAdd(List<LPortraitSection> sections, LEntryDraft draft, LPortraitLabel label)
    {
        List<LPortraitLine> lines = [];
        foreach (LForm form in draft.LEntryDraftForms)
        {
            if (form.LFormText.Length == 0)
            {
                continue;
            }

            lines.Add(new LPortraitLine(form.LFormRole, LEngineLocalShow(form.LFormText, form.LFormLocal)));
        }

        if (lines.Count > 0)
        {
            sections.Add(LPortraitSection.LPortraitSectionCreate(label.LPortraitLabelForm, lines));
        }
    }

    private static string LEngineLocalShow(string text, string? local)
    {
        return string.IsNullOrEmpty(local) ? text : text + " " + local;
    }

    private void LEngineParadigmAdd(List<LPortraitSection> sections, long entryId, LPortraitLabel label)
    {
        IReadOnlyList<LParadigmSlot> slots;
        try
        {
            slots = LEngineParadigmShow(entryId);
        }
        catch (Exception)
        {
            slots = [];
        }

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
                    ? LEngineLocalShow(inflection.LInflectionText, inflection.LInflectionLocal)
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

    private static void LEngineFanqieAdd(
        List<LPortraitSection> sections, IReadOnlyList<LFanqieRow> rows, LPortraitLabel label)
    {
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
            LEnginePartAdd(parts, parted ? row.LFanqieRowInitial + row.LFanqieRowRime : row.LFanqieRowText);
            LEnginePartAdd(parts, row.LFanqieRowHeading, "[", "]");
            LEnginePartAdd(parts, row.LFanqieRowDivision);
            LEnginePartAdd(parts, row.LFanqieRowTone);
            LEnginePartAdd(parts, row.LFanqieRowSpelling);
            LEnginePartAdd(parts, row.LFanqieRowReading, "/", "/");

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

    private static void LEnginePartAdd(List<string> parts, string text, string open = "", string close = "")
    {
        if (text.Length > 0)
        {
            parts.Add(open + text + close);
        }
    }

    private void LEngineScriptAdd(
        List<LPortraitSection> sections, long entryId, string language, LPortraitLabel label)
    {
        IReadOnlyList<LScriptImage> images;
        try
        {
            images = LEngineStyleRead(language).Count == 0 ? [] : LEngineScriptRead(entryId);
        }
        catch (Exception)
        {
            images = [];
        }

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
}
