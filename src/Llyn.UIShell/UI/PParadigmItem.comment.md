# PParadigmItem.cs

## `public sealed class PParadigmItem`

One row of the paradigm box: the part it belongs to, the form's name, and the form itself.
The row carries flags rather than wording, so the dash and ellipsis live in the row template.
A row is built once from one or more slots and never changes, since a new read hands the box new rows.
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

True when the form is unspecified, the lookup is on, and no fetch is running, so the template says no source answered.
An unspecified form with the lookup off keeps every flag false, and the template says the lookup is off.

## `internal static PParadigmItem PParadigmItemCreate(IReadOnlyList<LParadigmSlot> slots, bool grouped, bool pending, bool enabled)`

Maps the state of the first slot to the row's text and flags, naming every slot handed in.
`grouped` says whether the row heads a part group and carries the part's name.
`pending` says whether the engine is fetching for this entry, and `enabled` whether the morphology switch is on.
Both are read once by the caller for every row.
