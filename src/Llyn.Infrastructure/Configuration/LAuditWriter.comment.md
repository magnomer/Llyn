# LAuditWriter.cs

## `public sealed class LAuditWriter : LAuditVault`

The workspace's record of faults the program did not expect.
It exists so the shell can say something plain to the user and still keep what a developer would need.
It is written beside the database, because a fault belongs to the workspace it happened in.

## `public LAuditWriter(string root)`

Binds the writer to the workspace `root` whose `audit.log` it appends to.

## `public static string LAuditWriterRead(string root)`

Where the log stands for the workspace at `root`.

## `public string? LAuditRecord(Exception exception)`

Appends one fault as a note carrying the whole exception rather than its message alone.
A fault is read later by someone who was not there.
The type and the stack are the part worth keeping.

## `public static string? LAuditWriterRecord(string root, string note)`

Appends one note, stamped in UTC, and answers with the file it was written to.

Not every entry worth keeping is a fault.
A migration that drops data reports what it dropped, and that report has no exception behind it.

It answers `null` when the log cannot be written.
A workspace on a read-only disk is a workspace that still has to report its faults.
Failing to record one must never become a second failure shown over the first.
