# TDoctor.cs

## `public sealed class TDoctor`

Covers the recovery of last resort: what happens when the workspace database cannot be opened at all.

The healthy case is here because it is the one that must not fire.
A doctor that reset a working database would lose the user's work on every launch.
That it did nothing is the assertion that matters most.

A database written at another version is the migration runner's to rebuild, so the doctor stays out of it.
A file that is not a database at all is refused by SQLite itself, and that is the doctor's case.
Both must end with the program holding a database it can use.

The repeat case guards the backup name.
Two rescues inside one second would otherwise land on the same name.
The second would erase the first copy, which this recovery promises never to do.

A file that cannot be opened at all, here a folder standing in its place, is not the doctor's case.
The fault passes through and nothing is set aside, since the file was never shown to be broken.
The classification cases spell out the line: corrupt and not-a-database rescue, busy, full and read errors do not.
A busy or locked file is reported as such, so the launch can tell the user which program to close.
