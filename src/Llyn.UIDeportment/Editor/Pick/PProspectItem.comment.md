# PProspectItem.cs
Hash: `0db2c8c692a1358d`

## `internal sealed class PProspectItem`

One row of the dropdown that opens when typed text matches more than one Entry.
Most rows stand for an Entry that already exists, so they carry its id.
The closing rows offer a fresh Entry for the typed word, one per language, with no id yet.
Their id is null, so no magic id stands for a fresh Entry.
Only those rows are marked fresh, read from the null id.
The flag is derived from the language so the dropdown reads the same as every other headword list.

## `internal PProspectItem(long? id, string headword, string language, string epithet = "", string? name = null)`

The shown name defaults to the headword, so a create row needs none passed.

## `public string PProspectItemName { get; }`

The headword as the row shows it, numbered `(1)`, `(2)` while another row carries the same headword.
`LTwin` writes it once the list is filled, because a repeat is only visible across rows.
`PProspectItemHeadword` keeps the plain headword for everything that is not display.

## `public string PProspectItemEpithet { get; }`

The epithet the row prints after the headword, small and muted, in the reading the language pack names.
It is empty when the setting is off or the entry keeps none.
