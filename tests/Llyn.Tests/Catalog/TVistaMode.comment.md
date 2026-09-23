# TVistaMode.cs

## `public sealed class TVistaMode`

Editing mode belongs to a view.
Its saved preference is shared across views and survives reopening the workspace.
Changing a mode raises one notice for the owning view.
Repeating a setting can update the preference used by newly opened views.

## `EditingSet_ChangedMode_RaisesOnlyOwningVistaNotice()`

Setting the same new value twice updates one view and leaves another unchanged.
Exactly one notice is published for the owner.

## `EditingSet_WorkspaceReopened_AllVistasStartFromSavedMode()`

A mode saved before closing the engine is restored by views created after reopening the workspace.

## `EditingSet_UnchangedVistaMode_StillUpdatesSharedPreference()`

Setting a second view to its current value still changes the posture preference.
Later views use that preference, while the first view keeps its mode.
