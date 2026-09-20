# TVaultFakeClaim.cs

## `internal sealed class TVaultFakeClaim : LClaimVault`

An in-memory claim folder keyed by draft id, so a clerk test can pose as one process and then another.
Every claim is live, since no process is checked, and a new claim names the process the fake was told.

## `internal TVaultFakeClaim(int process)`

Seats the process a new claim names, the one the test's first rig carries.

## `internal void TClaimProcessSet(int process)`

Changes the process a new claim names, for a second rig posing as another copy of the program.
