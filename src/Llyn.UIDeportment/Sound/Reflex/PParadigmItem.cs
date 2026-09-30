using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class PParadigmItem
{
    private PParadigmItem(string part, string name, string text, string? tip)
    {
        PParadigmItemPart = part;
        PParadigmItemName = name;
        PParadigmItemText = text;
        PParadigmItemTip = tip;
    }

    public string PParadigmItemPart { get; }

    public string PParadigmItemName { get; }

    public string PParadigmItemText { get; }

    public string? PParadigmItemTip { get; }

    internal static PParadigmItem PParadigmItemCreate(CParadigmSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);

        return new PParadigmItem(
            slot.CParadigmSlotPart, slot.CParadigmSlotName, slot.CParadigmSlotText, slot.CParadigmSlotTip);
    }

    internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(IReadOnlyList<CParadigmSlot> slots)
    {
        ArgumentNullException.ThrowIfNull(slots);

        List<PParadigmItem> items = new(slots.Count);
        foreach (CParadigmSlot slot in slots)
        {
            items.Add(PParadigmItemCreate(slot));
        }

        return items;
    }
}
