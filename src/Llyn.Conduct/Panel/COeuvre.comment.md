# COeuvre.cs

## `public sealed class COeuvre`

The source list of the authors panel: its rows, its query, its empty text, its tally and its colophon.
The rows are the Sources crediting the chosen Author, the uncredited ones under the orphan row, or all.
It keeps a handle on the roll vista too, because the engine narrows the rows by the roll's state.
It owns the panel over the oeuvre vista, with the Source keys and no delete scope.
It also holds the reference, author, vita and usage maps that other lists share.

## `internal COeuvre(LEntryPort entries, LSettingsPort settings, CEnvoy envoy, Func<bool> shownSeam)`

Builds the oeuvre over the entry port, with its panel asking through `envoy`.
The panel reads a failure's ready notice through `settings`.
The panel never edits, so it holds no changes and stores nothing on leaving.

## `public CPanel COeuvrePanel { get; }`

The panel over the oeuvre vista, whose loaded Source raises the colophon.

## `private void LOeuvreColophonUpdate(LDraft draft)`

Answers the panel's loaded draft with the colophon sheet it shapes.

## `public string COeuvreEmptyKey`

The key of the empty text: the plain empty text with no Author chosen, else unmatched while narrowed, else vacant.

## `internal void LOeuvreVistaRestore(LVista roll, LVista vista)`

Binds the roll vista the rows follow and the oeuvre vista that holds the query and the chosen Source.
The panel takes the oeuvre vista with it.
A workspace switch hands fresh vistas, and the comb query carries over, so the driver never replays it.

## `internal void LOeuvreObserverAttach(Action<Action> marshal)`

Attaches the oeuvre list's own observer plan: a Vista notice raises the oeuvre rows, run through `marshal`.
The guild attaches it after the vistas restore, so the observer stands on the fresh oeuvre vista.

## `public IReadOnlyList<CCatalogReference> COeuvreRowsRead()`

Reads the rows, counts them, and closes the panel when a chosen Source is no longer listed.
Before the vistas are restored the engine answers no rows.

## `public void COeuvreQuerySet(string query)`

The comb above the list, narrowing the Sources by the typed text.

## `public string COeuvreTallyRead()`

The citation sentence for the chosen Source, which the engine counts and words.

## `internal static CColophon LOeuvreColophonRead(LEntryPort entries, LDraft draft)`

The colophon of a Source draft, which the engine builds with its tally and this class shapes.
It is static, so the shelf reads its sheet through the same map without a second panel.

## `internal IReadOnlyList<CCatalogAuthor> LOeuvreAuthorRead(IReadOnlyList<LCatalogAuthor> rows)`

Maps the guild's roll or union rows to their shape, each work count worded by the engine.
The citation count arrives worded by Core.
The mark's icon key is chosen here, as Conduct chooses every key the driver looks up.

## `internal static CVita LOeuvreVitaRead(LVita vita)`

Maps the vita the engine built to its shape, with its fellows and citing places.

## `internal static CUsage COeuvreUsageRead(LUsage usage)`

Maps one citing place to its shape.
The lectern's usage rows build through it too, so both lists word a place alike.

## `internal static IReadOnlyList<CCatalogReference> COeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)`

The one map for reference rows, shared with the card, the anthology and the shelf.

## `private static CStateValue LOeuvreCreditRead(string? credit, bool uncertain)`

The authors as a written value, uncertain when the engine marks them unknown.
