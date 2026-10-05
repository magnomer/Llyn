# LWorkspaceVault.cs
Hash: `ef9ab7fcfdfee9d2`

## `public interface LWorkspaceVault`

The persistence port for the single workspace state row.
`LWorkspaceArchive` in Infrastructure is its adapter over the workspace database.

## `LWorkspaceState LWorkspaceStateRead();`

Reads the workspace state, creating a blank one when none is stored yet.

## `void LWorkspaceStateSave(LWorkspaceState state);`

Stores `state` as the workspace state.

## `long LWorkspaceFloorAdjust();`

Issues the next provisional id by lowering the identity floor by one and answering the new floor.

## `long LWorkspaceSizeRead();`

The bytes the workspace store occupies, or zero when nothing is stored yet.
The establishment panel shows it, and only the adapter knows what file that is.

## `Guid LWorkspaceRealmRead();`

The realm the workspace was created under, fixed for the life of its store.
The realm separates independently created workspaces.
A folder copied outside Llyn keeps it and pushes onto the same notes.
