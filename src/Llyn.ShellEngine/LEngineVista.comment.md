# LEngineVista.cs

## `public sealed partial class LEngine`

Where a panel asks for a vista, the engine's view state for one catalog tab.
It is also where the panel asks for the rows that vista lists.
Before this every browse panel kept its own order and filter fields and applied them itself.
Now the engine holds the four choices and returns rows already filtered, sorted, numbered and marked.
This file moves the library onto it, and the other tabs follow.

## `private long _lEngineVistaCount;`

The ids handed out so far, so each vista's bulletin names one vista.

## `public LVista LEngineVistaStart(string tab, LCatalogOrder fallback, bool blank = false)`

Starts a vista for `tab`, reading its order and filter from the stored layout.
A tab with no stored order lists by `fallback`, and one with no stored filter hides nothing.
`blank` makes an empty query list nothing, which only the duplex wings ask for.
Every start reads the layout afresh, so a switched workspace hands the panel its own choices.

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

## `private static string[] LEngineTwinRead(IReadOnlyList<LEntry> entries)`

The twin name of each entry, by position, numbered by entry id.

## `private IReadOnlyDictionary<long, string> LEngineEpithetScan(IReadOnlyList<LEntry> entries)`

The epithet of every listed entry that has one, keyed by id, or nothing while the workspace hides epithets.
Called under the lock.
