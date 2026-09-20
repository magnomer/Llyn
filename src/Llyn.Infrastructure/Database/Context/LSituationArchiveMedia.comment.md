# LSituationArchiveMedia.cs

## `public sealed partial class LSituationArchive`

The media side of the Situation store: the Image and Video links a Situation shows.
A single read fills one Situation through the Image and Video stores.
A list read fills every Situation in two grouped queries instead.
The delete drops a Situation's own links before its row goes.

## Inline notes

### `private static void LSituationMediaDelete(SqliteConnection connection, string table, long situationId)`

Drops the Situation's own media links before the row goes.
The cascade would do it too, but the store never leans on one it can state.

### `private LSituation LSituationMediaRead(LSituation situation)`

Fills the two media lists of one Situation from the Image and Video stores.
Each list keeps the order the Situation holds.
The nested sessions share the open connection, so a read stays one transaction.

### `private IReadOnlyList<LSituation> LSituationMediaRead(List<LSituation> situations)`

Fills the media lists of a whole list in two queries, one per link table, grouped by parent in memory.
A list read serves the catalog, the chip resolver and every card's chips, and those run on each keystroke.
Two queries per situation there would cost hundreds of round trips per key, so the list never asks per row.

### `private static Dictionary<long, List<LImageDraft>> LSituationImageRead(SqliteConnection connection)`

Every situation-to-image link joined to its Image, ordered by parent and position, bucketed by parent.

### `private static Dictionary<long, List<LVideoDraft>> LSituationVideoRead(SqliteConnection connection)`

The same for Videos, carrying the span beside the location.
