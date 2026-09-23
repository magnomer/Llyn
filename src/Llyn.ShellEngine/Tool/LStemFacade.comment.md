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

## `public LStem? LEngineStemFind(string language, string key)`

The series row of that language and key, the row a series chip opens.

## `public string? LEngineStemFind()`

The first loaded pack that declares a series source, or null when none does.
The panel is shown only while one answers, as the yunjing panel waits for rime books.

## `public IReadOnlyList<LStem> LEngineStemFind(LVista vista, string language)`

The series of the language as one column: narrowed by the query, sorted by the order, chosen marked.
A chosen series that the narrowing dropped is unchosen, so the column never points at a missing row.

## `public LStemPage LEngineStemResolve(long? id)`

The page of the chosen series, or the blank page when nothing was chosen.

## `public IReadOnlyList<LVistaRow> LEngineKindredFind(string language, LVista grove, LVista vista)`

The entry rows of the chosen series, narrowed by the query the list was typed into.
Nothing is listed while no series is chosen.

## `internal IReadOnlyList<LVistaRow> LEngineKindredFind(string language, IReadOnlyList<long> stemIds, string query, LVista? vista = null)`

The entry rows the series reach, built as catalog rows with the chosen one marked.

## `private bool LEngineStemCheck(string language)`

True while the pack of that language declares a series source.
