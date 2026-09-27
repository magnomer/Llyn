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

## `private void LOeuvreColophonUpdate(LDraft draft)`

Composes the read sheet of a loaded Source draft with the tally of the chosen row, and announces it.
The draft passes only between controllers, so the view receives the sheet and never the draft.
The draft arrives as a parameter from the notice, so the Source is never a local here.
The shelf composes its sheet through the same map.

## `public event Action<CColophon>? LOeuvreColophonChanged;`

The sheet of the Source just loaded, for the guild view's colophon.

## `internal static CColophon LOeuvreColophonRead(`

Reads the sheet of a Source draft and shapes it, failing with the caller's message when no Source is held.
The oeuvre and the shelf both call it, so the colophon map is declared once.

## `internal static IReadOnlyList<CCatalogReference> LOeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)`

The one map for reference rows, shared with the card, the anthology and the shelf.

## `internal static IReadOnlyList<CCatalogAuthor> LOeuvreAuthorRead(`

Maps the guild's roll or union rows to their shape, each source count worded by `localize`.
The maps of the guild sit here, since `LGuild` is near its line ceiling.

## `internal static CVita LOeuvreVitaRead(LVita vita)`

Maps the vita the engine built to its shape, with its fellows and citing places.

## `internal static CUsage LOeuvreUsageRead(LUsage usage)`

Maps one citing place to its shape.
The lectern's usage rows build through it too, so both lists word a place alike.

## `private static CStateValue LOeuvreCreditRead(string? credit, bool uncertain)`

The authors as a written value, uncertain when the engine marks them unknown.
