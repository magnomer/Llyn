# COeuvre.cs

## `public sealed class COeuvre`

The source list of the authors panel: its rows, its query, its empty text, its tally and its colophon.
The rows are the Sources crediting the chosen Author, the uncredited ones under the orphan row, or all.
It keeps a handle on the roll vista too, because the engine narrows the rows by the roll's state.
The panel over the oeuvre vista still lives in the guild's deportment, so the stray notice asks it to clear.
It also holds the reference, author, vita and usage maps that other lists share.

## `public event Action? COeuvreStrayed;`

The chosen Source is no longer among the rows, so the panel drops it.

## `public string COeuvreEmptyKey`

The key of the empty text: the plain empty text with no Author chosen, else unmatched while narrowed, else vacant.

## `internal void COeuvreVistaRestore(LVista roll, LVista vista)`

Binds the roll vista the rows follow and the oeuvre vista that holds the query and the chosen Source.
A workspace switch hands fresh vistas, and the comb query carries over, so the driver never replays it.

## `public IReadOnlyList<CCatalogReference> COeuvreRowsRead()`

Reads the rows, counts them, and raises the stray notice when a chosen Source is no longer listed.
Before the vistas are restored the engine answers no rows.

## `public void COeuvreQuerySet(string query)`

The comb above the list, narrowing the Sources by the typed text.

## `public string COeuvreTallyRead()`

The citation sentence for the chosen Source, which the engine counts and words.

## `internal void COeuvreColophonUpdate(LDraft draft)`

Turns a loaded Source draft into the colophon and announces it.
The panel hands the draft over, so the driver receives the sheet and never the draft.

## `internal CColophon COeuvreColophonRead(LDraft draft)`

The colophon of a Source draft, which the engine builds with its tally and this class shapes.
The shelf composes its sheet through the same read.

## `internal IReadOnlyList<CCatalogAuthor> COeuvreAuthorRead(IReadOnlyList<LCatalogAuthor> rows)`

Maps the guild's roll or union rows to their shape, each work count worded by the engine.

## `internal static CVita COeuvreVitaRead(LVita vita)`

Maps the vita the engine built to its shape, with its fellows and citing places.

## `internal static CUsage COeuvreUsageRead(LUsage usage)`

Maps one citing place to its shape.
The lectern's usage rows build through it too, so both lists word a place alike.

## `internal static IReadOnlyList<CCatalogReference> COeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)`

The one map for reference rows, shared with the card, the anthology and the shelf.

## `private static CStateValue COeuvreCreditRead(string? credit, bool uncertain)`

The authors as a written value, uncertain when the engine marks them unknown.
