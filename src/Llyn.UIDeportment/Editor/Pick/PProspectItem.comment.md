# PProspectItem.cs
Hash: `d90c7fe9dac1860d`

## `internal sealed class PProspectItem`

One row of the dropdown that opens when typed text matches more than one Entry.
Most rows stand for an Entry that already exists, so they carry its id.
The closing rows offer a fresh Entry for the typed word, one per language, with no id yet.
Only those rows are marked fresh.
The flag is derived from the language so the dropdown reads the same as every other headword list.

## `internal PProspectItem(`

The shown name defaults to the headword, so a create row needs none passed.

## `public string PProspectItemName { get; }`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
`LTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PProspectItemHeadword` keeps the plain headword for everything that is not display.

## `public string PProspectItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
It is empty when the setting is off or the entry keeps none.
