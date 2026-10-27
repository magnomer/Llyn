# Vista

This folder holds the tests of the view state of a catalog tab.
They cover loading by row kind, editing mode and the chosen row.

## `TCatalogChosen.cs`

Catalog rows carry the selected identity and disambiguated display names.
Clearing a selection clears every row's chosen flag.
Duplicate and unknown Example labels receive consistent numbers.
A child view's rows depend on its parent's selected Tag and on its own query.
The resulting Entry is chosen only while both filters match.
A child view's rows follow the child's ordering, and equal headwords go by language.

## `TVistaLoading.cs`

Loading and deleting follow the selected row's kind.
Entry views open or delete the chosen Entry.
Stale, missing or cleared choices load nothing and return no draft.
Deleting an Entry view's choice clears the choice and removes the Entry.
Catalog views load their own subject as a draft with identity zero, not an editing session.
Deleting a catalog Example, Situation, Source or Author drops the citations to it.
The citing Entry stays stored.
Tag and Register views delete nothing, and the choice and stored row remain available.
Structural `yunjing` and `yunmu` views never mistake their selection for an Entry.
Their underlying Entry stays stored.
Without a view, the file name is the generic Entry name.
A selected Entry supplies the file name, with unsafe characters replaced.

## `TVistaMode.cs`

Editing mode belongs to a view.
Its saved preference is shared across views and survives reopening the workspace.
Changing a mode raises exactly one notice, for the owning view.
Setting a view to its current value still updates the shared preference.
Later views use that preference, while the first view keeps its mode.

## Stamp

Commit: `9377e657b395`
- `TCatalogChosen.cs` `3eed6e1a5c13e5cb`
- `TVistaLoading.cs` `84deed1ff84cd7ae`
- `TVistaMode.cs` `7c5ff62b6b8c4f21`
