# TVaultFakeWorkspace.cs
Hash: `4823df3b3751b6f0`

## `internal sealed class TVaultFakeWorkspace : LWorkspaceVault`

An in-memory workspace row, so an identity issuer and a revision pointer work without a database.
The floor descends by one on every adjust, as the archive's does, so a clerk test sees negative ids.
The size is always zero, since nothing here is on disk.
The realm is always the empty `Guid`, since no schema ever wrote one here.
