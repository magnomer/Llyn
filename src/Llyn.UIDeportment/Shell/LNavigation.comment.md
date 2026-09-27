# LNavigation.cs

## `public sealed class LNavigation`

Decides which panel of the main window shows, naming each tab by its key.
Nothing here keeps a copy of the open tab.
The posture's mode is the only truth, asked through the window deportment.
The window view holds the tabs, their buttons and their hooks, and hands the hooks in as seams.

## `public LNavigation(LWindow window, IReadOnlyList<string> tabs)`

Takes the window deportment and the tab keys in navigation order.
The first tab opens the window when the posture names none.

## `public LVoyage LNavigationVoyage`

The trail of stations the reader has jumped between, built with the navigation.

## `public string LNavigationShownRead()`

The tab whose mode the posture holds.
Falls back to the first tab when the posture matches none.

## `public string? LNavigationRestore(Func<string, bool> allowed)`

Restores the tab the posture names and answers it, for the window view to show.
A tab that is not allowed falls back to the first tab, so the posture names what shows.
No tab is answered when the posture matches none.
The restore skips the leave guard, since nothing has been shown yet to leave.

## `private string LNavigationAllowRead(string tab, Func<string, bool> allowed)`

The tab itself when it is allowed, else the first tab, saved to the posture either way.

## `public bool LNavigationShow(string tab, Func<string, bool> leave)`

Asks the target tab first whether it may be left, then selects it.
Answers false when either the target or the open tab refuses.

## `public bool LNavigationSelect(string tab, Func<string, bool> leave)`

Asks the open tab whether it may be left, then saves the chosen mode to the posture.
The posture, never the screen, names the open panel.
The `leave` seam answers for a tab, and a tab without a leave hook has nothing to lose.

## `private string? LNavigationFind()`

The tab whose mode the posture holds, or null.
