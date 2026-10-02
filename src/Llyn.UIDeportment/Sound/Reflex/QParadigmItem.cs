using System;
using System.Collections.Generic;
using Llyn.Conduct;

namespace Llyn.UIDeportment;

public sealed class QParadigmItem
{
    private QParadigmItem(string part, string name, string text, string? tip)
    {
        QParadigmItemPart = part;
        QParadigmItemName = name;
        QParadigmItemText = text;
        QParadigmItemTip = tip;
    }

    public string QParadigmItemPart { get; }

    public string QParadigmItemName { get; }

    public string QParadigmItemText { get; }

    public string? QParadigmItemTip { get; }

    internal static QParadigmItem QParadigmItemCreate(CParadigmSlot slot)
    {
        ArgumentNullException.ThrowIfNull(slot);

        return new QParadigmItem(
            slot.CParadigmSlotPart, slot.CParadigmSlotName, slot.CParadigmSlotText, slot.CParadigmSlotTip);
    }

    internal static IReadOnlyList<QParadigmItem> QParadigmItemScan(IReadOnlyList<CParadigmSlot> slots)
    {
        ArgumentNullException.ThrowIfNull(slots);

        List<QParadigmItem> items = new(slots.Count);
        foreach (CParadigmSlot slot in slots)
        {
            items.Add(QParadigmItemCreate(slot));
        }

        return items;
    }
}
