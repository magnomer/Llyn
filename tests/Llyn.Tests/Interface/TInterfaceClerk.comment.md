# TInterfaceClerk.cs

## `internal static partial class TInterface`

The relays for the clerks of the application ring, built over a rig of fakes.
Each relay is transparent and carries no test logic of its own.

## `internal static LRig TRigClerkCreate(LEntryVault entries)`

The fake rig with the workspace, revision and tombstone ports seated by in-memory fakes.
Those are the ports a clerk test touches beyond the entries it is handed.

## `internal static LDraftClerk TDraftClerkCreate(LRig rig)`

The draft clerk over `rig`, with its issuer and its pack cache built over the same rig.

## `internal static LRequest TRequestStrayCreate(long draftId)`

A request of a kind no switch knows, so a test can prove the default arm throws.

## `private sealed record TRequestStray(long LRequestDraftId) : LRequest(LRequestDraftId);`

The one request kind the production code never declares.

## `internal static LEntryClerk TEntryClerkCreate(LRig rig)`

The entry clerk over `rig`.

## `internal static LEntry TEntryClerkAdd(this LEntryClerk clerk, LEntry entry)`

Creates `entry` through the clerk with no forms and no speeches.
