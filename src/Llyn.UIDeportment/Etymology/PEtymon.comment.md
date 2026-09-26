# PEtymon.cs

## `internal sealed class PEtymon`

The entry at the end of the source chips, where a word is typed before it becomes a link.
It is one item of the field's collection, so the chips and the entry wrap as one line.
The word lives here rather than on the box, which is rebuilt whenever a chip is added.
Its two values are dependency properties, so the drawn box follows them without a hand-raised change event.

## `public string PEtymonText`

The word standing in the entry, bound two ways to the box.
Clearing it after a pick clears the box.

## `public bool PEtymonShown`

Whether the entry is drawn, true only on an editable field.
The read-only field keeps the item but collapses its box, so no branch picks the collection.
