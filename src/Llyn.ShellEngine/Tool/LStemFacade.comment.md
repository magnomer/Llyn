# LStemFacade.cs

## `internal sealed class LStemFacade`

The engine's facade for stem, the phonetic series the xiesheng panel browses by.
It reads the series and the entries they reach, and composes one series page.
The links themselves are written where a series is stored, so nothing here writes.

## `public LStemFacade(LEngine engine)`

The facade bound to its engine and the engine's gate.

## `internal IReadOnlyList<LStem> LEngineStemRead(string language)`

The stored series of the language, unsorted and unnarrowed.

## `public LStem? LEngineStemRead(long? id)`

The series row that identity names, or null when nothing was chosen.

## `public long? LEngineStemFind(string language, string? key)`

The identity of the series of that language and key, the series a chip opens.
A blank key or a series never stored answers null.

## `public string? LEngineStemFind()`

The first loaded pack that declares a series source, or null when none does.
The panel is shown only while one answers, as the yunjing panel waits for rime books.

## `public bool LEngineStemCheck()`

True while a loaded pack declares a series source, the verdict the xiesheng tab is shown by.

## `public IReadOnlyList<LStem> LEngineStemFind(LVista vista)`

The series of the column's language, narrowed by the query, sorted by the order, chosen marked.
Nothing is listed while no pack declares a series source.
A chosen series that the narrowing dropped is unchosen, so the column never points at a missing row.

## `public LStemPage LEngineStemResolve(long? id)`

The page of the chosen series, or the blank page when nothing was chosen.

## `public IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista)`

The entry rows of the series the column chose, in its language, narrowed by the list's own query.
Nothing is listed while no series is chosen.

## `internal IReadOnlyList<LVistaRow> LEngineKindredFind(string language, IReadOnlyList<long> stemIds, string query, LVista? vista = null)`

The entry rows the series reach, built as catalog rows with the chosen one marked.

## `private string LEngineLanguageRead(long? chosen)`

The language a column lists, that of the chosen series, else the first pack declaring a series source.
Empty while neither answers.

## `private bool LEngineStemCheck(string language)`

True while the pack of that language declares a series source.
