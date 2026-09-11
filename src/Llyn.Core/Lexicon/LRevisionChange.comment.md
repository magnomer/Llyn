# LRevisionChange.cs

## `public sealed record LRevisionChange(`

One recorded change inside a `LRevision`.
A change has no id of its own.
Its identity is its revision plus `LRevisionChangePosition`, the order it takes within that revision.
So the changes of a revision read back in the sequence they were recorded.

`LRevisionChangeTarget` and `LRevisionChangeSubject` name what was touched.
They give the target row's id and the kind of entity it is.
They are plain recorded values rather than a foreign key.
A change routinely describes a row that no longer exists.
That is the point of the history: a deletion is still readable after the deleted row is gone.

**Parameters**

- `LRevisionChangePosition` — Zero-based order of this change within its revision.
- `LRevisionChangeTarget` — Id of the row the change touched, recorded and not a reference.
- `LRevisionChangeSubject` — Entity type of the target, for example `entry` or `sense`.
- `LRevisionChangeKind` — What was done to the target, for example `create` or `delete`.
- `LRevisionChangeSummary` — Human-readable description of the change, and `null` when none was given.
