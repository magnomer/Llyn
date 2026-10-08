# COeuvre.cs
Hash: `fade34e12fe451ae`

## `public sealed class COeuvre`

The source list of the authors panel: its rows, its query, its empty text, its tally and its colophon.
The rows are the Sources crediting the chosen Author, the uncredited ones under the orphan row, or all.
It keeps a handle on the roll vista too, because the engine narrows the rows by the roll's state.
It owns the panel over the oeuvre vista, with the Source keys and no delete scope.
It also holds the reference, author, vita and usage maps that other lists share.

## `internal COeuvre(LEntryPort entries, LAuthorPort authors, LReferencePort references, LSettingsPort settings, LVistaPort vistas, CEnvoy envoy, Func<bool> shownSeam)`

Builds the oeuvre over its ports, with its panel asking through `envoy`.
`authors` lists the Sources and words the work count, `entries` words the tally and `references` reads the sheets.
The panel reads a failure's ready notice through `settings`.
`vistas` passes to the panel, which loads and deletes its chosen row through it.
The panel never edits, so it holds no changes and stores nothing on leaving.

## `public event Action<CColophon>? COeuvreColophonChanged;`

The colophon of each Source the panel loads, so the source side redraws it.

## `public CPanel COeuvrePanel { get; }`

The panel over the oeuvre vista, whose loaded Source raises the colophon.

## `private void LOeuvreColophonUpdate(LDraft draft)`

Answers the panel's loaded draft with the colophon sheet it shapes.

## `public bool COeuvreEmpty`

Whether the last rows read listed none, so it moves only when `COeuvreRowsRead` runs.

## `public string COeuvreEmptyKey`

The key of the empty text: the plain empty text with no Author chosen, else unmatched while narrowed, else vacant.

## `internal void LOeuvreVistaRestore(LVista roll, LVista vista)`

Binds the roll vista the rows follow and the oeuvre vista that holds the query and the chosen Source.
The panel takes the oeuvre vista with it.
A workspace switch hands fresh vistas, and the comb query carries over, so the driver never replays it.

## `internal void LOeuvreObserverAttach(Action<Action> marshal)`

Attaches the oeuvre list's own observer plan.
A Vista notice raises the oeuvre rows, run through `marshal`.
The guild attaches it after the vistas restore, so the observer stands on the fresh oeuvre vista.

## `public IReadOnlyList<CCatalogReference> COeuvreRowsRead()`

Reads the rows, counts them, and closes the panel when a chosen Source is no longer listed.
Before the vistas are restored the engine answers no rows.

## `public void COeuvreQuerySet(string query)`

The comb above the list, narrowing the Sources by the typed text.

## `public string COeuvreTallyRead()`

The citation sentence for the chosen Source, which the engine counts and words.

## `internal static CColophon LOeuvreColophonRead(LReferencePort references, LDraft draft)`

The colophon of a Source draft, which the engine builds with its tally and this class shapes.
It is static, so the shelf reads its sheet through the same map without a second panel.

## `internal IReadOnlyList<CReferenceKind> LOeuvreKindRead()`

The kind menu the authors panel's filter lists, read through the oeuvre's reference port.
Only `CGuild.CGuildRollRead` calls it, so the roll answer carries the menu.
It hands the engine's kind list to `CImprint.LImprintKindRead`, so both menus drop a repeated tag alike.

## `private static CColophon LOeuvreColophonRead(LColophon sheet)`

Maps the engine's colophon sheet to its shape, field by field, with no rule of its own.

## `internal IReadOnlyList<CCatalogAuthor> LOeuvreAuthorRead(IReadOnlyList<LCatalogAuthor> rows)`

Maps the guild's roll or union rows to their shape, each work count worded by the engine.
The citation count arrives worded by Core.
The mark's icon key is chosen here, as Conduct chooses every key the driver looks up.

## `internal static CVita LOeuvreVitaRead(LVita vita)`

Maps the vita the engine built to its shape, with its fellows and citing places.

## `internal static CUsage COeuvreUsageRead(LUsage usage)`

Maps one citing place to its shape.
The reading view's incoming rows build through it too, so both lists word a place alike.

## `internal static IReadOnlyList<CCatalogReference> COeuvreReferenceRead(IReadOnlyList<LCatalogReference> rows)`

The one map for reference rows, shared with the card and the shelf.
An unset author or year reads `Source.Unset`.

## `private static CStateValue LOeuvreCreditRead(string? credit, bool uncertain)`

The authors as a written value, uncertain when the engine marks them unknown.
