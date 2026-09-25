# LRevisionVault.cs

## `public interface LRevisionVault`

The persistence port for revisions, the recorded history of what changed in the workspace.
`LRevisionArchive` in Infrastructure is its adapter over the workspace database.

## `LRevision LRevisionRecord(IReadOnlyList<LRevisionChange> changes);`

Records `changes` as one new revision stamped with the current time.
Returns the stored revision with its id filled in.
