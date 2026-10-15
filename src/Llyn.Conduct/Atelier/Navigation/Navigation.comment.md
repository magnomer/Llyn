# Navigation

This folder tracks where the reader stands.
That is the open tab with the hooks each panel registers, and the back-and-forward trail of visited records.

## `CNavigation`

Decides which tab of the session stands open, naming each tab by its key.
It is the one owner of opening a record in a tab, and it keeps the voyage between records.
Nothing here keeps a copy of the open tab.
The posture's mode is the only truth, so a restart opens the tab the user left.
Every change raises the whole state, which the window paints, so the window keeps only the tabs' buttons and panels.
`LNavigationTabs` gives the fallback station tab, while opening without a stored mode leaves selection untouched.
`LNavigationTabCheck` throws on a key that names no tab, so a typo never saves a mode nobody shows.
`CNavigationArrived` fires once a jump has landed, so the window closes its menus.

`LNavigationTabOpen` runs last in `CAtelierOpen`, after every view has restored.
A tab its panel does not offer now is hidden, and a hidden stored tab falls back to the first.
The tab opened is saved to the posture, so the posture names what shows.
No tab is painted when the posture names none, so the window keeps the tab it shows.
The opening asks no tab to be left, since nothing has been shown yet.

`CNavigationTabSelect` is the gate for a tab click.
It selects once the open tab agrees to be left, and the clicked tab itself is not asked.
It answers false and saves nothing when the open tab refuses.
A jump replaces what the target tab shows, so an arriving jump asks that tab first.

`CNavigationUsageOpen` opens the place a citing click names, and null opens nothing.
A place that quotes an Example opens it in the corpus tab, and any other place opens its entry.
`LNavigationDiweiOpen` and `LNavigationStemOpen` jump to the yunjing and xiesheng tabs through the areas' attached openers.
Nothing opens before the area attached its opener.
`LNavigationRowOpen` takes no jump for a tab without a registered panel.
The station is read before the switch and recorded only once the jump has gone.

`LNavigationStationAdd` records the open tab's station before a panel moves to another row.
The state is raised even when nothing was recorded, so the buttons always read right.
`CNavigationStationUndo` and `CNavigationStationRedo` step the voyage.
`LNavigationVoyageRun` moves the trail only after the jump has gone, and only then raises the landing.

`LNavigationTabAdd` is how each panel area registers its tab when it is built.
A tab without an `allowed` hook is always offered.
A tab with no leave question, station or arrival registers nothing, as the study and scoreboard tabs do.

## `CNavigationPanel`

The hooks one panel area registers with the navigation.
Every hook is a member of the area itself, so no hook comes up from a window.
The hooks are the leave question, the station, the editor restore, the arrival and the offered check.

## `CNavigationState`

What the window paints after a navigation change.
Every change carries the whole state, so the window keeps no copy of its own.
The tab is null when the change leaves the open tab alone, as on a startup with no stored tab.

## `CVoyage`

The trail of records the reader has jumped between, like a browser's back and forward.
A station is one tab key paired with the id of the record it shows.
The trail lives for the session and belongs to the navigation, so the engine never hears of it.
Both trails are capped at fifty stations, the oldest dropped first.
An empty station is ignored, so no trail ever lands on nothing.
A new nonzero station clears the future.
An empty station or the latest past station leaves both trails unchanged.

`LVoyageRun` is the one step that back and forward share.
The station parked on the other trail is the standing one, not a station read back afterwards.
The next station is only peeked, and the trails move only after the jump has gone.
The jump goes through the navigation's `show`, which records nothing.
A refused jump leaves both trails as they were, so the station is still there to try again.

## `CVoyageState`

Whether each trail holds a station, which lights the retreat and advance buttons.

## Stamp

Commit: `2f9a06b8eb95`
- `CNavigation.cs` `8315257f9a86dcbf`
- `CNavigationPanel.cs` `269271317524a145`
- `CNavigationState.cs` `1c11754f58b51511`
- `CVoyage.cs` `a3937490b66ec360`
- `CVoyageState.cs` `83f08d7bc43a5cdf`
