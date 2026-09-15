# PProspectItem.cs

## `internal sealed class PProspectItem`

One row of the dropdown that opens when typed text matches more than one Entry.
Most rows stand for an Entry that already exists, so they carry its id.
The last row stands for the Entry the typed word would create, and carries no id yet.
That row is the only one marked fresh.
The flag is derived from the language so the dropdown reads the same as every other headword list.

## `public string PProspectItemName`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
`PTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PProspectItemHeadword` keeps the plain headword for everything that is not display.

## `public string PProspectItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
The epithet is the reading the pack names, empty when the setting is off or the entry keeps none.
