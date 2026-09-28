# CNavigation.cs

## `public sealed class CNavigation`

Decides which tab of the session stands open, naming each tab by its key.
Nothing here keeps a copy of the open tab.
The posture's mode is the only truth, so a restart opens the tab the user left.
The driver owns the tabs' buttons and panels, and hands the tab keys in navigation order.

## `internal CNavigation(LPosture posture, IReadOnlyList<string> tabs)`

Takes the session's posture and the tab keys.
The first tab stands open when the posture names none of them.

## `public static CNavigation CNavigationCreate(CAtelier atelier, IReadOnlyList<string> tabs)`

Builds the navigation over the atelier's posture.
Building it is no user action, so it is no gate on the atelier.

## `public string CNavigationTabRead()`

The tab whose mode the posture holds, or the first tab when it holds none of them.

## `public string? CNavigationTabOpen(Func<string, bool> allowed)`

Opens the tab the posture names at startup, and answers it for the driver to show.
A tab that is not allowed falls back to the first tab.
The tab answered is saved to the posture, so the posture names what shows.
No tab is answered when the posture names none.
The opening asks no tab to be left, since nothing has been shown yet.

## `public bool CNavigationTabSelect(string tab, bool arriving, Func<string, bool> leave)`

Selects `tab` once every other open tab agrees to be left, then saves its mode to the posture.
An `arriving` jump replaces what `tab` shows, so `tab` itself is asked first.
A plain tab click keeps what `tab` shows, so it is not asked.
The `leave` seam answers for one tab, and asks its panel's own leave question.
Answers false, saving nothing, when any asked tab refuses.

## `private string LNavigationAllowRead(string tab, Func<string, bool> allowed)`

The tab itself when it is allowed, else the first tab, saved to the posture either way.

## `private string? LNavigationFind()`

The tab whose mode the posture holds, or null.
