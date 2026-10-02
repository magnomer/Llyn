# QDirectoryItem.cs

## `internal sealed class QDirectoryItem`

Presentation item for one tag row in `QDirectory`.
Carries the id of the stored Tag, the text the row shows and whether that row is the chosen one.
The id is what the membership list is looked up by, and the text is only what is shown.
The chosen flag is read when the row is painted, which tints the tag the membership list currently stands on.

## `public bool QDirectoryItemChosen { get; }`

Whether this row is the chosen one, as Conduct's row said.
A pick goes to the gate, and the directory is rebuilt from the rows it answers.
So the item never changes after it is built and announces nothing.
