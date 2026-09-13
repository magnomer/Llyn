using System;
using System.Collections.Generic;
using Llyn.Core;

namespace Llyn.UIShell;

public partial class PDisplay
{
    private void PDisplayParadigmShow(long id)
    {
        IReadOnlyList<LParadigmSlot> slots;
        bool enabled;
        bool pending;
        try
        {
            slots = _lEngine.LEngineParadigmShow(id);
            enabled = _lEngine.LEngineSettingsRead().LSettingsMorphology;
            if (enabled)
            {
                _lEngine.LEngineInflectionStart(id);
            }

            pending = _lEngine.LEngineInflectionCheck(id);
        }
        catch (Exception)
        {
            slots = [];
            enabled = false;
            pending = false;
        }

        if (slots.Count == 0)
        {
            PDisplayParadigm.PParadigmItems = null;
            return;
        }

        HashSet<long> parts = [];
        foreach (LParadigmSlot slot in slots)
        {
            parts.Add(slot.LParadigmSlotSpeech.LSpeechValueId);
        }

        List<PParadigmItem> items = new(slots.Count);
        List<LParadigmSlot> row = [];
        long? previous = null;
        foreach (LParadigmSlot slot in slots)
        {
            if (row.Count > 0 && !PDisplayParadigmMatch(row[0], slot))
            {
                items.Add(PParadigmItem.PParadigmItemCreate(row, parts.Count > 1 && previous != row[0].LParadigmSlotSpeech.LSpeechValueId, pending, enabled));
                previous = row[0].LParadigmSlotSpeech.LSpeechValueId;
                row = [];
            }

            row.Add(slot);
        }

        items.Add(PParadigmItem.PParadigmItemCreate(row, parts.Count > 1 && previous != row[0].LParadigmSlotSpeech.LSpeechValueId, pending, enabled));

        PFont.PFontApply(_lEngine, slots[0].LParadigmSlotSpeech.LSpeechValueLanguage, PDisplayParadigm);
        PDisplayParadigm.PParadigmItems = items;
    }

    private static bool PDisplayParadigmMatch(LParadigmSlot one, LParadigmSlot other)
    {
        return one.LParadigmSlotSpeech.LSpeechValueId == other.LParadigmSlotSpeech.LSpeechValueId
            && one.LParadigmSlotState == LState.LStateSpecified
            && other.LParadigmSlotState == LState.LStateSpecified
            && one.LParadigmSlotInflection is LInflection first
            && other.LParadigmSlotInflection is LInflection second
            && string.Equals(first.LInflectionText, second.LInflectionText, StringComparison.Ordinal);
    }
}
