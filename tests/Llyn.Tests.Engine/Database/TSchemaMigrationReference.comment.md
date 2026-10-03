# TSchemaMigrationReference.cs
Hash: `720f13444005e50b`

## `public sealed class TSchemaMigrationReference`

Covers the Unknown Reference the schema seeds and the old wording that points at it.
A new workspace holds exactly one Reference, titled Unknown.
An example that wrote `unknown` as its source state is pointed at that row after the rebuild.
The row is minted again only for old wording, since the user may delete it.
It builds each workspace through `TWorkspace.TWorkspacePrepare`.

## `public void DatabaseCreate_UnknownSourceWording_LinksTheUnknownSource()`

A version-48 workspace wrote `unknown` as an example's source state.
The rebuild points such an example at the Unknown Reference the workspace already holds, and mints no second one.

## `public void DatabaseCreate_UnknownSourceDeleted_MintsItForOldWording()`

A workspace whose Unknown Reference was deleted, yet whose old rows still carry the wording, gets the row back.
Only the old wording brings it back, since the row is ordinary data the user may remove.

## `public void DatabaseCreate_NewWorkspace_SeedsTheUnknownSource()`

A new workspace starts with exactly one Reference, titled Unknown.

## `public void DatabaseCreate_UnknownSourceDeleted_SeedsNothingAgain()`

Opening a workspace again never re-seeds the Unknown Reference, so deleting it holds.
