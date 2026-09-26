using System;
using System.Collections.Generic;
using System.Linq;
using Llyn.Core;

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

    internal static PParadigmItem PParadigmItemCreate(LParadigmRow row, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(row);

        LParadigmSlot first = row.LParadigmRowFirst;
        string part = row.LParadigmRowPart;
        string name = row.LParadigmRowName;
        if (first.LParadigmSlotInflection is LInflection inflection)
        {
            return new PParadigmItem(part, name, inflection.LInflectionText, false, false, false, false);
        }

        return first.LParadigmSlotUncertain
            ? new PParadigmItem(part, name, string.Empty, true, false, false, false)
            : new PParadigmItem(
                part, name, string.Empty, false, pending, enabled && !pending && !held, enabled && !pending && held);
    }

    internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(
        IReadOnlyList<LParadigmSlot> slots, bool pending, bool enabled, bool held)
    {
        ArgumentNullException.ThrowIfNull(slots);

        List<PParadigmItem> items = new(slots.Count);
        foreach (LParadigmRow row in LParadigmRow.LParadigmRowScan(slots))
        {
            items.Add(PParadigmItemCreate(row, pending, enabled, held));
        }

        return items;
    }
}
