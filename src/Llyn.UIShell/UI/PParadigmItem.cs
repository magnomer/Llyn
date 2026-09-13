using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

namespace Llyn.UIShell;

public sealed class PParadigmItem
{
    private PParadigmItem(string part, string name, string text, bool unknown, bool pending, bool lost)
    {
        PParadigmItemPart = part;
        PParadigmItemName = name;
        PParadigmItemText = text;
        PParadigmItemUnknown = unknown;
        PParadigmItemPending = pending;
        PParadigmItemLost = lost;
    }

    public string PParadigmItemPart { get; }

    public string PParadigmItemName { get; }

    public string PParadigmItemText { get; }

    public bool PParadigmItemUnknown { get; }

    public bool PParadigmItemPending { get; }

    public bool PParadigmItemLost { get; }

    internal static PParadigmItem PParadigmItemCreate(
        IReadOnlyList<LParadigmSlot> slots, bool grouped, bool pending, bool enabled)
    {
        ArgumentNullException.ThrowIfNull(slots);
        ArgumentOutOfRangeException.ThrowIfZero(slots.Count);

        LParadigmSlot first = slots[0];
        string part = grouped ? first.LParadigmSlotSpeech.LSpeechValueName : string.Empty;
        string name = string.Join(", ", slots.Select(slot => slot.LParadigmSlotMorphology.LMorphologyName));
        return first.LParadigmSlotState switch
        {
            LState.LStateSpecified when first.LParadigmSlotInflection is LInflection inflection
                => new PParadigmItem(part, name, inflection.LInflectionText, false, false, false),
            LState.LStateUnknown => new PParadigmItem(part, name, string.Empty, true, false, false),
            _ => new PParadigmItem(part, name, string.Empty, false, pending, enabled && !pending),
        };
    }
}
