using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

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

    internal static PParadigmItem PParadigmItemCreate(CParadigmSlot slot, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(slot);

        string part = slot.CParadigmSlotPart;
        string name = slot.CParadigmSlotName;
        if (slot.CParadigmSlotText is string text)
        {
            return new PParadigmItem(part, name, text, false, false, false, false);
        }

        return slot.CParadigmSlotUncertain
            ? new PParadigmItem(part, name, string.Empty, true, false, false, false)
            : new PParadigmItem(
                part, name, string.Empty, false, pending, enabled && !pending && !held, enabled && !pending && held);
    }

    internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(
        IReadOnlyList<CParadigmSlot> slots, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(slots);

        List<PParadigmItem> items = new(slots.Count);
        foreach (CParadigmSlot slot in slots)
        {
            items.Add(PParadigmItemCreate(slot, pending, enabled, held));
        }

        return items;
    }
}
