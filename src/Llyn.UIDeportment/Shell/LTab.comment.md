# LTab.cs

## `public sealed record LTab(`

One tab of the main window as the deportment sees it.
One record per tab replaces the parallel tables the window kept before.

**Parameters**

- `LTabMode` — Name the posture stores the open tab under.
- `LTabButton` — Navigation button that opens the tab.
- `LTabPanel` — Panel the tab shows.

## `public Func<bool>? LTabAllowed { get; init; }`

Tells whether the tab may be offered now.
A missing hook means the tab is never offered hidden.

## `public Func<bool>? LTabLeave { get; init; }`

Asks the tab whether it may be left, saving or dropping unsaved work.
A missing hook means the tab has nothing to lose.

## `public Action<bool>? LTabScribe { get; init; }`

Turns the tab's editor on or off.
A missing hook means the tab has no editor.

## `public Func<long>? LTabStation { get; init; }`

Reads the id the tab shows now, for the voyage history.
A missing hook means the tab has no station.

## `public Action<long>? LTabArrival { get; init; }`

Shows the given id when a jump lands on the tab.
A missing hook means no jump lands on the tab.

## `public Action<bool, bool>? LTabVoyage { get; init; }`

Tells the tab whether it can go back and forward.
A missing hook means the tab shows no voyage buttons.
