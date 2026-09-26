# TSchemaRealm.cs

## `public sealed class TSchemaRealm`

Covers the realm row, the origin stamp and the id counters of a fresh workspace.
The realm row is single, minted once, and kept when the schema runs again.
A row made here carries the realm and its own id as its stamp.
A row from elsewhere keeps the stamp it arrived with, and a half-written stamp is refused.
Tables that carry history never reuse an id, and a temporary id is never handed out twice across restarts.

## Inline notes

### `private static long TSchemaAutoincrementRead(TWorkspace workspace, string table)`

Whether the table was declared with AUTOINCREMENT, which is what stops SQLite reusing a deleted id.
