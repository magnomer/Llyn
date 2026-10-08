# LDiweiPort.cs
Hash: `e787435741edfb65`

## `public interface LDiweiPort`

The slice of the engine a deportment sees when it browses the rime cells of a rime book.
`LFanqieFacade` implements it, since the diwei clerk sits beside the fanqie clerk.

## `(long, bool)? LEngineDiweiFind(string language, string kind, string key);`

The rime cell a pressed key names in `language`, and whether it is a rime cell.
Nothing is answered when no cell of that kind carries the key.

## `IReadOnlyList<LDiwei> LEngineDiweiFind(LVista vista, long? chosen, bool final);`

The onset or rime cells a column lists, narrowed by the column's own query.

## `LDiweiPage LEngineDiweiResolve(long? id, Func<string, string?> localize);`

The page of one rime cell with its labels localized through the given lookup.

## `long LEngineDiweiResolve(long? id, string character);`

The entry of a character shown on one rime cell, in the cell's language, made first when none exists.

## `IReadOnlyList<LVistaRow> LEngineXiaoyunFind(long? chosen, LVista onset, LVista rime, LVista vista);`

The entries under the chosen onset and rime, none when neither is chosen.
They are read in the language of the diwei the panel reads.

## `string? LEngineDiweiRead(bool initial, string key);`

The kind of the cell a pressed fanqie key opens, or none for a blank key.
Both reading views' gates ask it before the navigation opens.
