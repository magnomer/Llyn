# LStemPort.cs
Hash: `a38fe66e80a15880`

## `public interface LStemPort`

The slice of the engine a deportment sees when it browses phonetic series.
`LStemFacade` implements it.

## `long? LEngineStemFind(string language, string? key);`

The phonetic series a pressed key names in `language`, or none for a blank key or an unknown one.

## `IReadOnlyList<LStem> LEngineStemFind(LVista vista);`

The phonetic series a column lists, narrowed by the column's own query.

## `bool LEngineStemCheck();`

True while a loaded pack declares a phonetic series source, the verdict the xiesheng tab shows by.

## `LStemPage LEngineStemResolve(long? id);`

The page of the chosen series, or the blank page when nothing was chosen.

## `long LEngineStemResolve(long? id, string character);`

The entry of a character in one phonetic series, in the language of the series, made first when none exists.

## `IReadOnlyList<LVistaRow> LEngineKindredFind(LVista grove, LVista vista);`

The entry rows of the series the column chose, in its language, narrowed by the list's own query.
Nothing is listed while no series is chosen.
