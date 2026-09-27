# PParadigmItem.cs

## `public sealed class PParadigmItem`

One row of the paradigm box: the part it belongs to, the form's name, and the form itself.
The row carries flags rather than wording, so the dash and ellipsis live in the row template.
A row is built once from one or more slots and never changes.
A new read hands the box new rows.
Several slots sharing one stored form fold into one row naming every form, so `walked` is not listed twice.

## `public string PParadigmItemPart`

The part of speech heading this row, or empty when the row is not the first of its part.
Only shown when more than one part inflects, so a lone part does not repeat the chip above.

## `public string PParadigmItemName`

The morphology value's name, such as `past` or `plural`, or several joined by a comma when the row folds them.

## `public string PParadigmItemText`

The stored form, or empty while the slot stands unspecified or unknown.

## `public bool PParadigmItemUnknown`

True when a reached source could not name the form, so the template draws a dash.

## `public bool PParadigmItemPending`

True when the form is unspecified and a fetch is running, so the template says it is being looked up.

## `public bool PParadigmItemLost`

True when the form is unspecified, the lookup is on, no fetch is running, and no draft holds the entry.
The template then says no source answered.
An unspecified form with the lookup off keeps every flag false, and the template says the lookup is off.

## `public bool PParadigmItemHeld`

True when the form is unspecified, the lookup is on, no fetch is running, and a draft holds the entry.
The engine declines to fetch while the entry is held.
The template then says the form is looked up once the entry is saved.

## `internal static PParadigmItem PParadigmItemCreate(CParadigmSlot slot, bool pending, bool enabled, bool held)`

Maps one paradigm row to its text and flags.
A row with an inflected form is filled, an unknown one is marked, and any other is open.
`pending` says whether the engine is fetching for this entry.
`enabled` says whether the morphology switch is on, and `held` whether a draft holds the entry.

## `internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(IReadOnlyList<CParadigmSlot> slots, bool pending, bool enabled, bool held)`

Turns the shaped rows into the items one box draws.
The engine already joined the slots, so the reading view and the editor draw alike.
