# TInterfaceNavigation.cs

## `internal static class TInterfaceNavigation`

The relays for the navigation's internal rules and the arrival opens it runs on each area.
Each relay is transparent and carries no test logic of its own.

## `internal static void TNavigationTabAdd(`

Registers a tab on a navigation as a panel area does, with its hooks handed in.
The landed record is handed to `arrival`, as the area's own open would take it.

## `internal static void TNavigationStationAdd(this CNavigation navigation) => navigation.LNavigationStationAdd();`

Relays the station record the areas' row gates make.

## `internal static void TNavigationDiweiAttach(this CNavigation navigation, Action<string, string, string> open) =>`

Attaches a rime-cell open to a navigation as the yunjing area does.

## `internal static void TNavigationStemAttach(this CNavigation navigation, Action<string, string?> open) =>`

Attaches a series open to a navigation as the xiesheng area does.

## `internal static bool TNavigationDiweiOpen(`

Relays the rime-cell jump, which only the display and the editor's sounding ask in production.

## `internal static bool TNavigationStemOpen(this CNavigation navigation, string language, string? key) =>`

Relays the series jump, which only the display asks in production.

## `internal static void TCorpusExampleOpen(this CCorpus corpus, long id) => corpus.LCorpusExampleOpen(id);`

Relays a panel's arrival open, as the navigation runs it.
The arrival relays of the other areas do the same.
