# PParadigmItem.cs

## `public sealed class PParadigmItem`

One row of the paradigm box.
It holds the part it belongs to, the form's name, and the text shown for the form.
The row copies a ready slot, so the mark and the tip key were chosen below the driver.
A row is built once from one slot and never changes.
A new read hands the box new rows.
Several slots sharing one stored form fold into one row naming every form, so `walked` is not listed twice.

## `public string PParadigmItemPart`

The part of speech heading this row, or empty when the row is not the first of its part.
Only shown when more than one part inflects, so a lone part does not repeat the chip above.

## `public string PParadigmItemName`

The morphology value's name, such as `past` or `plural`, or several joined by a comma when the row folds them.

## `public string PParadigmItemText`

The stored form, or the mark that stands in for a missing one.

## `public string? PParadigmItemTip`

The localization key of the tooltip, or null when the row shows its form.

## `internal static PParadigmItem PParadigmItemCreate(CParadigmSlot slot)`

Copies one ready slot into a row.

## `internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(IReadOnlyList<CParadigmSlot> slots)`

Turns the ready slots into the items one box draws.
The lectern and the editor draw alike, since the read already chose each row's mark and tip.
