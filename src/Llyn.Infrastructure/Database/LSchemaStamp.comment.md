# LSchemaStamp.cs

## `public static class LSchemaStamp`

Makes stand-alone rows record which realm first created them.

Only a row that can exist on its own carries the mark.
An owned child knows its realm by following the reference to its parent.
Marking it again would store the same fact twice.

## `private static readonly string[] LSchemaStampTable`

The tables whose rows stand alone: Entry and the shared pool every card points at.

## `public static void LSchemaStampCreate(SqliteConnection connection)`

Creates, for each of those tables, the trigger that fills the mark and the index that keeps it unique.

The trigger runs only when the inserted row left `origin_id` at zero, which is what a row made here does.
It then writes this workspace's realm value and the id the row was just given.
A row arriving from a merge already carries both.
The trigger passes it over, and the mark it was born with survives the trip.

Filling the mark in the database rather than in each store keeps every insert path honest.
A store cannot forget to stamp a row, because it never stamps one.

Each stamped table also checks that a row naming an origin id carries a sixteen-byte realm.
A row with the id and no realm would be a stamp with half its meaning gone.
So an import that forgets the realm is refused instead of quietly admitted.

The unique index is what makes importing the same file twice harmless.
The second pass finds the mark already present and updates that row instead of adding another.
