# LNavigation.cs

## `public sealed class LNavigation`

Decides which panel of the main window shows.
Nothing here keeps a copy of the open tab.
The posture's mode is the only truth, asked through the window deportment.

## `public LNavigation(Window window, LWindow deportment, DependencyProperty chosen, IReadOnlyList<LTab> tabs)`

Takes the window, its deportment and the tabs in navigation order.
The first tab opens the window when the posture names none.
The `chosen` property marks the open button, since the deportment cannot name `PTab`.
The voyage is built last over this navigation.

## `public LTab LNavigationShownRead()`

The tab whose mode the posture holds.
Falls back to the first tab when the posture matches none.

## `public bool LNavigationSelect(object? button)`

Opens the tab of the given button.
Answers false for an unknown button or when the open tab refuses to be left.

## `public void LNavigationRestore()`

Shows every tab button unless its tab is not allowed.
Then it restores the tab the posture names.
A tab that is not allowed falls back to the first tab, so the posture names what shows.
No tab is restored when the posture matches none.
The restore skips the leave guard and hands the split state to the tab's scribe.

## `public bool LNavigationShow(object button, long id)`

Jumps to a record in the tab of the given button.
The station is read before anything moves.
It is recorded only when the jump went, so a refused jump leaves no station.

## `public void LNavigationShow(object button, Action arrival)`

Jumps to the tab of the given button and records no station.
The arrival runs only when the switch succeeds.

## `public bool LNavigationTabShow(LTab target, long id)`

Opens the target tab and hands it the record id.
Answers false for a tab without arrival or a refused switch.
It records no station, so the voyage walks through it freely.

## `private bool LNavigationTargetSelect(LTab target)`

Asks the target tab first whether it may be left.
Then it asks the open panel through the tab switch.

## `private bool LNavigationTabSelect(LTab chosen)`

Asks the open tab whether it may be left before switching.
The posture, never the screen, names the open panel.
A tab without a leave hook has nothing to lose.

## `private void LNavigationApply(LTab chosen)`

Marks the chosen button and shows only its panel.
Then it saves the chosen mode to the posture.

## `private LTab? LNavigationFind(object? button)`

The tab owning the given button, or null.
