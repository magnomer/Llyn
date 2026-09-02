# PDirectoryItem.cs

## `internal sealed class PDirectoryItem`

Presentation item for one tag row in `PDirectory`.
Carries the tag text the row shows and whether that row is the chosen one.
A tag has no id, so its text is both what is shown and what is looked up by.
The chosen flag is read by the row template, which tints the tag the membership list currently stands on.
The item is immutable, so a changed selection is a rebuilt catalog rather than a mutated row.
