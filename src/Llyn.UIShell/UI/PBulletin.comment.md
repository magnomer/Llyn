# PBulletin.cs

## `internal static class PBulletin`

Shared reading of an engine announcement for the panels that list stored entries.

## `internal static bool PBulletinEntryCheck(LSubject subject)`

Whether an announcement of `subject` can change what an entry list shows.
A draft edit or a grasp mark never alters a listed headword.
Neither does a fetched frequency, paradigm, script, fanqie or reflex row.
The display of the open entry answers those subjects on its own.
So a list panel skips them rather than reading every row again.
Every keystroke in the input panel announces a draft change, and every list panel stays alive while hidden.
Re-listing on each of those made a large workspace stall on every keystroke and on every entry switch.
