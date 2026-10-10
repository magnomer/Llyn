# LStemFacade.cs
Hash: `7768740ca6bbd9e7`

## `public sealed class LStemFacade : LStemPort`

The engine's facade for stem, the phonetic series the xiesheng panel browses by.
It implements the stem port itself, so Host hands it to Conduct with no outlet between.
It reads the series and the entries they reach, and composes one series page.
The links themselves are written where a series is stored, so nothing here writes them.
Its own write is a member's fold, through the fold clerk.
The character resolve may make an entry, but through `LEntryFacade`.

## `internal LStemFacade(LEngineHearth hearth, LEntryFacade entry, LLanguageFacade language, LVistaRowFacade row, LReflexFacade reflex)`

Stores the hearth, its gate and the sibling facades it calls, all built by `LEngine` before this one.
The gate, the staff and the shared state are read through the hearth.
It takes its siblings rather than the engine, so it names only the facades it uses.

## `public LStem? LEngineStemRead(long? id)`

The series row that identity names, or null when nothing was chosen.

## `public long? LEngineStemFind(string language, string? key)`

The identity of the series of that language and key, the series a chip opens.
A blank key or a series never stored answers null.

## `public string? LEngineStemFind()`

The language of the first loaded pack that declares a series source, or null when none does.
The panel is shown only while one answers, as the yunjing panel waits for rime books.

## `public bool LEngineStemCheck()`

True while a loaded pack declares a series source, the verdict the xiesheng tab is shown by.

## `public IReadOnlyList<LStem> LEngineStemFind(LVista vista)`

The series of the column's language, narrowed by the query, sorted by the order, chosen marked.
Nothing is listed while no pack declares a series source.
A chosen series that the narrowing dropped is unchosen, so the column never points at a missing row.

## `public LStemPage LEngineStemResolve(long? id)`

The page of the chosen series, or the blank page when nothing was chosen.
Its members come from `LStemClerk` with their own folds, and pass through given the guises of their rows.
The guises come from `LReflexFacade.LEngineGuiseRead`, the read the entry page's reflex rows take.
So the reader maps the whole page from this one call and asks the engine nothing more.

## `public long LEngineStemResolve(long? id, string character)`

The entry of a character on the series' page, in the page's language, made first when none exists.
The entry resolve is the entry facade's own, so a glyph chip anywhere lands on the same entry.
The series is read under this facade's gate, and the resolve takes the entry facade's gate afterwards.
With no series the page's language is blank, which the resolve refuses.

## `public void LEngineStemSpread(long? id, string character, bool opened)`

Opens or closes one member's fold: reads the series, finds the member's entry, then writes the fold.
`LStemClerk.LStemEntryFind` finds the entry, and `LFoldClerk` writes it under the series key.
A StemFold notice is raised on the entry afterwards, outside the gate, as `LReflexFacade` raises its own.
Its own subject keeps entry page and card folds from redrawing the series page.
No series or a bare member writes nothing and raises nothing.

## `public IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista)`

The entry rows of the series the column chose, in its language, narrowed by the list's own query.
Nothing is listed while no series is chosen.

## `internal IReadOnlyList<LVistaRow> LEngineKindredFind(string language, IReadOnlyList<long> stemIds, string query, LVista? vista = null)`

The entry rows the series reach, built as catalog rows with the chosen one marked.
They follow the order of `vista`, the panel's order setting, as the catalog entry list does.
Without a vista they fall back to headword order.

## `private string LEngineLanguageRead(long? chosen)`

The language a column lists, that of the chosen series, else the first pack declaring a series source.
Empty while neither answers.

## `private bool LEngineStemCheck(string language)`

True while the pack of that language declares a series source.
