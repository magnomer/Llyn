# LDoctor.cs

## `public static class LDoctor`

Gets the program to a database it can run against, whatever state the workspace was left in.
The schema is still moving, so a database written by an older build may no longer be migratable at all.
A failed migration would otherwise stop the program at startup with nothing the user can do about it.
The doctor sets the unusable file aside under a dated name and starts a clean one in its place.

Nothing is deleted.
The old file stays in the workspace beside the new one, so a database is never lost to a launch.
This is the recovery of last resort, not a substitute for migration.
`LSchemaMigration` still carries a database forward whenever it can.

## `public static LDoctorRescue LDoctorDatabaseCreate(LDatabase database)`

Initializes `database`, resetting it when it cannot be initialized.
Returns what had to be done.
The caller can then tell the user their old data is no longer in front of them.

Only a SQLite fault or a refused schema version is treated as an unusable database.
Those are the two ways a broken or unknown file announces itself.
Anything else is a workspace problem that resetting would not fix and would hide.
A missing folder, a locked file and a denied path are left to propagate.

The clean database is created by the same call that failed.
So a fault in the schema itself surfaces rather than being mistaken for a second corruption.

## `private static string LDoctorDatabaseSave(string file)`

Moves the unusable database out of the way and returns where it went.

The connection pool is cleared first.
A pooled connection holds the file open even after the code that used it disposed it.
Windows refuses to move a file that is open.

The write-ahead log and shared-memory files move with the database.
They belong to it.
One left behind would be read as part of the clean database that takes its name.

## `private static string LDoctorBackupResolve(string file)`

Picks a free name for the file being set aside.
The name carries the moment of the rescue, so a workspace rescued more than once keeps every copy in order.
A counter is added when the stamp alone is taken.
Two rescues in the same second would otherwise name the same file.
The second would erase the first.
