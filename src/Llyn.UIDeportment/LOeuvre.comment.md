# LOeuvre.cs

## `public sealed class LOeuvre`

The deportment of the authors panel's source list: the panel state over the oeuvre vista and its rows.
The rows are the Sources crediting the chosen Author, the uncredited ones under the orphan row, or all.
It keeps a handle on the roll vista too, because the engine narrows the rows by the roll's state.
The list never edits, so its change seam always answers false and its delete seam always refuses.
A Source chosen here is read in the colophon, whose sheet this class composes.

## `public string LOeuvreEmptyKey`

The key of the empty text: the plain empty text with no Author chosen, else unmatched while narrowed, else vacant.

## `private IReadOnlyList<LCatalogReference> LOeuvreRowsApply(IReadOnlyList<LCatalogReference> rows)`

Counts the rows and drops a chosen Source the list no longer holds.
The rows cross as a parameter, so no local carries an engine answer into the clear.

## `public string LOeuvreTallyRead()`

The citation sentence for the chosen Source, composed from the usage the engine counts.

## `public LColophon LOeuvreColophonRead(LDraft draft)`

The read sheet of a loaded Source draft with the tally of the chosen row.
The draft arrives as a parameter from the notice, so the Source is never a local here.
