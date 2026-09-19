# LDoctor.cs

## `public sealed class LDoctor : LDoctorVault`

Gets the program to a database it can run against, whatever state the workspace was left in.
The schema is still moving, and a rebuild from another version can still fail on a file it cannot read.
A failed rebuild would otherwise stop the program at startup with nothing the user can do about it.
The doctor sets the unusable file aside under a dated name and starts a clean one in its place.

Nothing is deleted.
The old file stays in the workspace beside the new one, so a database is never lost to a launch.
This is the recovery of last resort, not a substitute for migration.
`LSchemaMigration` still carries a database forward whenever it can.

## `public LDoctor(LDatabase database)`

Binds the doctor to the workspace `database` it stands ready to rescue.

## `public LDoctorRescue LDoctorDatabaseCreate()`

Initializes the database, resetting it when it cannot be initialized.
Returns what had to be done.
The caller can then tell the user their old data is no longer in front of them.

Only a file SQLite itself calls corrupt, or not a database, is treated as unusable.
Those are the two ways a broken or unknown file announces itself.
Anything else is a passing condition that resetting would not fix and would hide.
A busy file, a full disk, a read error and a failed rebuild are left to propagate.
Resetting on one of those would set aside a healthy database on a bad morning.

The clean database is created by the same call that failed.
So a fault in the schema itself surfaces rather than being mistaken for a second corruption.

## `public static bool LDoctorRescueCheck(Exception fault)`

Whether `fault` names a database the doctor may set aside.
Only the corrupt and not-a-database result codes qualify.
Every other fault, SQLite or not, is left for the caller to report as it stands.

## `public static bool LDoctorBusyCheck(Exception fault)`

Whether `fault` says another connection holds the database.
The launch names that case to the user, since closing the other program is the cure.

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
