# LWorkspaceVault.cs

## `public interface LWorkspaceVault`

The persistence port for the single workspace state row.
`LWorkspaceArchive` in Infrastructure is its adapter over the workspace database.

## `LWorkspaceState LWorkspaceStateRead();`

Reads the workspace state, creating a blank one when none is stored yet.

## `void LWorkspaceStateSave(LWorkspaceState state);`

Stores `state` as the workspace state.

## `long LWorkspaceFloorAdjust();`

Issues the next opaque id above the identity floor and raises the floor past it.

## `long LWorkspaceSizeRead();`

The bytes the workspace store occupies, or zero when nothing is stored yet.
The establishment panel shows it, and only the adapter knows what file that is.
