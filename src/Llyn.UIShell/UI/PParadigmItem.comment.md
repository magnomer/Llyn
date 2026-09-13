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

True when the form is unspecified, the lookup is on, no fetch is running, and no draft holds the entry, so the template says no source answered.
An unspecified form with the lookup off keeps every flag false, and the template says the lookup is off.

## `public bool PParadigmItemHeld`

True when the form is unspecified, the lookup is on, no fetch is running, and a draft holds the entry.
The engine declines to fetch while the entry is held, so the template says the form is looked up once the entry is saved.

## `internal static PParadigmItem PParadigmItemCreate(IReadOnlyList<LParadigmSlot> slots, bool grouped, bool pending, bool enabled, bool held)`

Maps the state of the first slot to the row's text and flags, naming every slot handed in.
`grouped` says whether the row heads a part group and carries the part's name.
`pending` says whether the engine is fetching for this entry, `enabled` whether the morphology switch is on, and `held` whether a draft holds the entry.
All three are read once by the caller for every row.

## `internal static IReadOnlyList<PParadigmItem> PParadigmItemScan(IReadOnlyList<LParadigmSlot> slots, bool pending, bool enabled, bool held)`

Turns the engine's slots into the rows one box draws, so the reading view and the editor fold and flag alike.
Consecutive slots of one part sharing one stored form fold into one row.
The part heads a row only when more than one part inflects, and only on its first row.

## `private static bool PParadigmItemMatch(LParadigmSlot one, LParadigmSlot other)`

Reports whether two slots belong to one part and are both specified with the same stored text.
Such slots fold into one row.
