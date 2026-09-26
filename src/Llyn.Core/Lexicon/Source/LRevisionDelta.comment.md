# LRevisionDelta.cs

## `public sealed record LRevisionDelta(`

One recorded change inside a `LRevision`.
A change has no id of its own.
The store keeps the order it takes within its revision.
So the changes of a revision read back in the sequence they were recorded.

`LRevisionDeltaTarget` and `LRevisionDeltaSubject` name what was touched.
They give the target row's id and the kind of entity it is.
They are plain recorded values rather than a foreign key.
A change routinely describes a row that no longer exists.
That is the point of the history: a deletion is still readable after the deleted row is gone.

**Parameters**

- `LRevisionDeltaTarget` — Id of the row the change touched, recorded and not a reference.
- `LRevisionDeltaSubject` — Entity type of the target, for example `entry` or `sense`.
- `LRevisionDeltaKind` — What was done to the target, for example `create` or `delete`.
- `LRevisionDeltaSummary` — Human-readable description of the change, and `null` when none was given.
