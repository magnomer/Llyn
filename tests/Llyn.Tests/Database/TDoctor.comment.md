# TDoctor.cs

## `public sealed class TDoctor`

Covers the recovery of last resort: what happens when the workspace database cannot be opened at all.

The healthy case is here because it is the one that must not fire.
A doctor that reset a working database would lose the user's work on every launch.
That it did nothing is the assertion that matters most.

The two broken cases are the two shapes a real failure takes.
A database written by a newer build is refused by the migration runner.
A file that is not a database at all is refused by SQLite itself.
Both must end with the program holding a database it can use.

The repeat case guards the backup name.
Two rescues inside one second would otherwise land on the same name.
The second would erase the first copy, which this recovery promises never to do.
