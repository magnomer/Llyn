# CNavigation.cs

## `public sealed class CNavigation`

Decides which tab of the session stands open, naming each tab by its key.
It is the one owner of opening a record in a tab, and it keeps the voyage between records.
Nothing here keeps a copy of the open tab.
The posture's mode is the only truth, so a restart opens the tab the user left.
Every change raises the whole state, which the window paints.
The window keeps only the tabs' buttons and panels.

## `private static readonly string[] LNavigationTabs`

The tab keys in navigation order.
The first tab stands open when the posture names none of them.

## `internal CNavigation(CAtelier atelier)`

Only the atelier builds its navigation, once, over its posture.

## `public event Action<CNavigationState>? CNavigationChanged;`

Raised after every change the window paints, such as a tab switch, a restore or a voyage step.

## `public event Action? CNavigationArrived;`

Raised once a jump has landed and the tab's area has opened the record, so the window closes its menus.

## `internal void LNavigationTabOpen()`

Opens the tab the posture names when the atelier opens a workspace.
Only `CAtelierOpen` calls it, last, after every view has restored.
A tab its panel does not offer now is hidden.
A hidden stored tab falls back to the first tab.
The tab opened is saved to the posture, so the posture names what shows.
No tab is painted when the posture names none, so the window keeps the tab it shows.
The restored tab's panel then turns its editor on or off as the session left it.
The opening asks no tab to be left, since nothing has been shown yet.

## `public bool CNavigationTabSelect(string tab)`

The gate for a tab click.
It selects `tab` once the open tab agrees to be left, then saves its mode to the posture.
The click keeps what `tab` shows, so `tab` itself is not asked.
Answers false, saving nothing, when the open tab refuses.

## `public bool CNavigationEntryOpen(long id)`

Opens an entry in the library tab, the jump every entry link takes.

## `public bool CNavigationUsageOpen(CUsage? usage)`

The gate for a citing place the user clicked, which opens the place it names.
A click on no place hands null and opens nothing.
A place that quotes an Example opens it in the corpus tab, and any other place opens its entry.

## `internal bool LNavigationDiweiOpen(string language, string kind, string key)`

Opens a rime cell in the yunjing tab once every asked tab agrees to be left.
Only the reading view's and the editor's sound areas ask it, so it is internal.
The yunjing area opens the cell, and nothing opens before it attached its opener.
The jump replaces what the tab shows, so the yunjing tab is asked first.

## `internal void LNavigationStationAdd()`

Records the open tab's station on the voyage, before a panel moves to another row.
The areas call it from their row gates once the leave is settled.
The state is raised even when nothing was recorded, so the buttons always read right.

## `public bool CNavigationStationUndo()`

Steps back one station, parking the standing station on the future.
Each panel's retreat button calls it, as do the keyboard and mouse shortcuts.

## `public bool CNavigationStationRedo()`

Steps forward one station, the mirror of the step back.

## `internal bool LNavigationStemOpen(string language, string? key)`

Opens a phonetic series in the xiesheng tab through the xiesheng area, the rime-cell jump's twin.
Only the display asks it, so it is no driver's gate.

## `internal bool LNavigationRowOpen(string tab, long id)`

Opens a record in the given tab, recording the station the jump leaves.
A tab without a registered panel takes no jump.
The jump replaces what `tab` shows, so `tab` itself is asked first, then the open tab.
The station is read before the switch and recorded only once the jump has gone.
The tab's area then opens the record itself.

## `internal void LNavigationTabAdd(`

Each panel area registers its tab here when it is built, as one `CNavigationPanel`.
`leave` is the panel's own leave question, asked through its envoy.
`station` reads the record the panel stands on, and `scribe` restores its editor.
`arrival` opens a landed record in the area, and its change events repaint the panel.
`allowed` tells whether the tab is offered, and a tab without it is always offered.

## `internal void LNavigationDiweiAttach(Action<string, string, string> open)`

The yunjing area hands in its rime-cell open.

## `internal void LNavigationStemAttach(Action<string, string?> open)`

The xiesheng area hands in its series open.

## `private bool LNavigationVoyageRun(Func<Func<string, long, bool>, bool> step)`

Runs one voyage step through the jump that records nothing.
The trail moves only after the jump has gone, and only then is the landing raised.

## `private void LNavigationRowShow(string tab, long id)`

Paints the tab, has its area open the record, then says the jump landed.

## `private bool LNavigationRowSelect(string tab)`

Selects a tab for a jump, which only a tab with a registered panel takes.

## `private bool LNavigationTabSelect(string tab, bool arriving)`

Selects `tab` once every other open tab agrees to be left, then saves its mode to the posture.
An `arriving` jump replaces what `tab` shows, so `tab` itself is asked first.

## `private (string LNavigationTab, long LNavigationId) LNavigationStationRead()`

The open tab and the record its panel stands on, zero for a tab without a panel.

## `private string? LNavigationFind()`

The tab whose mode the posture holds, or null.

## `private void LNavigationStateRaise(string? tab)`

Raises the whole state, with `tab` null when the open tab is left as it is.

## `private static void LNavigationTabCheck(string tab)`

Throws on a key that names no tab, so a typo never saves a mode nobody shows.
