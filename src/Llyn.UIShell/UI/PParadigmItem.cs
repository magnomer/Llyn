using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PParadigmItem
{
    private PParadigmItem(string part, string name, string text, bool unknown, bool pending, bool lost, bool held)
    {
        PParadigmItemPart = part;
        PParadigmItemName = name;
        PParadigmItemText = text;
        PParadigmItemUnknown = unknown;
        PParadigmItemPending = pending;
        PParadigmItemLost = lost;
        PParadigmItemHeld = held;
    }

    public string PParadigmItemPart { get; }

    public string PParadigmItemName { get; }

    public string PParadigmItemText { get; }

    public bool PParadigmItemUnknown { get; }

    public bool PParadigmItemPending { get; }

    public bool PParadigmItemLost { get; }

    public bool PParadigmItemHeld { get; }

    internal static PParadigmItem PParadigmItemCreate(
        IReadOnlyList<LParadigmSlot> slots, bool grouped, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(slots);
        ArgumentOutOfRangeException.ThrowIfZero(slots.Count);

        LParadigmSlot first = slots[0];
        string part = grouped ? first.LParadigmSlotSpeech.LSpeechValueName : string.Empty;
        string name = string.Join(", ", slots.Select(slot => slot.LParadigmSlotMorphology.LMorphologyName));
        return first.LParadigmSlotState switch
        {
            LState.LStateSpecified when first.LParadigmSlotInflection is LInflection inflection
                => new PParadigmItem(part, name, inflection.LInflectionText, false, false, false, false),
            LState.LStateUnknown => new PParadigmItem(part, name, string.Empty, true, false, false, false),
            _ => new PParadigmItem(
                part, name, string.Empty, false, pending, enabled && !pending && !held, enabled && !pending && held),
        };
    }

    internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(
        IReadOnlyList<LParadigmSlot> slots, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(slots);
        if (slots.Count == 0)
        {
            return [];
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
            if (row.Count > 0 && !PParadigmItemMatch(row[0], slot))
            {
                items.Add(PParadigmItemCreate(row, parts.Count > 1 && previous != row[0].LParadigmSlotSpeech.LSpeechValueId, pending, enabled, held));
                previous = row[0].LParadigmSlotSpeech.LSpeechValueId;
                row = [];
            }

            row.Add(slot);
        }

        items.Add(PParadigmItemCreate(row, parts.Count > 1 && previous != row[0].LParadigmSlotSpeech.LSpeechValueId, pending, enabled, held));
        return items;
    }

    private static bool PParadigmItemMatch(LParadigmSlot one, LParadigmSlot other)
    {
        return one.LParadigmSlotSpeech.LSpeechValueId == other.LParadigmSlotSpeech.LSpeechValueId
            && one.LParadigmSlotState == LState.LStateSpecified
            && other.LParadigmSlotState == LState.LStateSpecified
            && one.LParadigmSlotInflection is LInflection first
            && other.LParadigmSlotInflection is LInflection second
            && string.Equals(first.LInflectionText, second.LInflectionText, StringComparison.Ordinal);
    }
}
