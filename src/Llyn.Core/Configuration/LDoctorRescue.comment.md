# LDoctorRescue.cs

## `public sealed record LDoctorRescue(`

What the workspace doctor had to do before the program could open its database.
A launch either found the database usable or found it unusable and started over.
The record carries that answer out of the logic layer so the shell can tell the user.
It is plain data and knows nothing of files, of SQLite, or of the window that shows it.

**Parameters**

- `LDoctorRescueDone` — Whether the database was set aside and a clean one started.
- `LDoctorRescueBackup` — Path the unusable database was moved to, or null when nothing was set aside.
- `LDoctorRescueReason` — Message of the fault that made the database unusable, or null when there was none.

## `public static LDoctorRescue LDoctorRescueHealthy`

The answer for a database that opened on its own.
Nothing was set aside, so there is neither a backup nor a reason to report.
