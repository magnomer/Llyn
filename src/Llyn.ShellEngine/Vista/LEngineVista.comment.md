# LEngineVista.cs

## `public sealed partial class LEngine`

Where a panel asks for a vista, the engine's view state for one catalog tab.
It is also where the panel asks for the rows that vista lists.
Before this every browse panel kept its own order and filter fields and applied them itself.
Now the engine holds the four choices and returns rows already filtered, sorted, numbered and marked.
This file moves the library onto it, and the other tabs follow.

## `private long _lEngineVistaCount;`

The ids handed out so far, so each vista's bulletin names one vista.

## `private readonly Dictionary<string, LVista> _lEngineVistas = [];`

The vista now standing for each tab, so a restart can detach the one it replaces.

## `public LVista LEngineVistaStart(`

Starts a vista for `tab` on the order, filter and mode the caller hands it.
The shell's posture resolves those from what it stored, so the engine holds no view state of its own.
`subject` names the stored kind the tab lists, so the engine holds no table of panel names.
`blank` makes an empty query list nothing, which only the duplex wings ask for.

## `public LVista? LEngineVistaRead(long id)`

The vista a bulletin names, or nothing once a restart replaced it.
The posture asks after each vista bulletin, so it stores the order and filter the vista now holds.
The vista is attached as an observer before it is returned, so it carries bulletins to the panel.
The vista started earlier for the same tab is detached first, so a switched workspace leaves no ghost.

## `public IReadOnlyList<LVistaRow> LEngineEntryFind(LVista vista)`

The entry rows the vista lists, ready to show.
The query, order and filter are the vista's, applied by the three-argument find.
A blank vista with nothing typed answers no rows before the lock is taken.
The rows are built by the shared builder under one lock.

## `private IReadOnlyList<LVistaRow> LEngineVistaBuild(IReadOnlyList<LEntry> entries, long? chosen)`

Turns entries in their listed order into rows ready to show: twin name, epithet and chosen mark.
Twins are numbered by entry id, so the older entry is `(1)` in every view.
The epithets come from one scan, so a long list costs one statement rather than one session per row.
The chosen row is the one whose id equals `chosen`.
Every catalog of entries builds its rows here, so no panel numbers twins or reads epithets itself.
Called under the lock.

## `public IReadOnlyList<string> LEngineNameResolve(IReadOnlyList<string> labels)`

The labels made distinct in their given order, numbered where two share a name, for the compass rows.

## `private static string[] LEngineTwinRead(IReadOnlyList<LEntry> entries)`

The twin name of each entry, by position, numbered by entry id, through `LEntryClerkTwin`.
The generic overload and the name read forward the same way for the catalog parts that still call them.

## `private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)`

The epithet of every listed entry that has one, keyed by id, or nothing while the workspace hides epithets.
Called under the lock.

## `public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query)`

Finds the marked entries whose headword carries the query, through the favorite clerk.
An empty query returns every marked entry.

## `public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order)`

The marked entries answering `query`, in `order`.
The mark carries its own stamp, so ordering by the mark is not ordering by the entry.

## `public IReadOnlyList<LCatalogFavorite> LEngineFavoriteFind(string query, LCatalogOrder order, LCatalogFilter filter)`

The same list with the marked entries in a hidden language left out.

## `public IReadOnlyList<LVistaRow> LEngineFavoriteFind(LVista vista)`

The rows the favorites panel's vista lists, with the query, order and filter read off the vista.
They come back as vista rows, twins numbered and epithets read in one scan, so the panel only copies them.
The row of the entry the vista stands on comes back marked chosen, so the panel keeps no choice.

## `public bool LEngineFavoriteCheck(long entryId)`

Whether the entry is marked.

## `public void LEngineFavoriteSave(long entryId)`

Marks the entry a favorite and announces the mark.
Marking changes no lexical data and keeps the entry's identity.

## `public void LEngineFavoriteDelete(long entryId)`

Unmarks the entry and announces it.
The entry stands, still reachable through the entry catalog.

## `internal LDraft? LEngineVistaLoad(LVista vista)`

Loads the selected record according to the vista subject under the engine gate.
The result is a snapshot, with no claim file, editing identity, or registered tenure.
Reference snapshots include credits.
Tag and register snapshots retain their own stored records.
