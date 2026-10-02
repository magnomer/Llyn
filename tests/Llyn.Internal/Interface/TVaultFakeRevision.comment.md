# TVaultFakeRevision.cs
Hash: `6f60bed6da1e60c5`

## `internal sealed class TVaultFakeRevision : LRevisionVault`

An in-memory revision log keyed from one, so a clerk test can read back what a delete recorded.
The change lists are kept as handed in, in the order recorded.
