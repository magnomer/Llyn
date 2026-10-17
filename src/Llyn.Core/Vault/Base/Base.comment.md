# Base

This folder holds the root ports of storage and the ambient ports the engine runs on.
It stands apart because the ports here are not tied to one lexicon base.
They cover the session, the store fault, the clock, paths, the fault log, recovery and the workspace lease.
The engine never learns which adapter stands behind any of them.

## `LVault`

The persistence port through which the engine opens one unit of work over storage.
A vault is what the engine may ask of storage for one lexicon base, with no knowledge of SQLite.
Its adapter is the `{Base}Archive` in Infrastructure, and `LDatabase` is the adapter of this root port.
The engine holds ports as fields handed in by the rig and never builds an archive itself.

## `LVaultSessionStart`

Opens a session that spans every vault call made until it is disposed.
A session opened inside another on the same thread joins it rather than starting a second one.

## `LVaultMigrated`

Whether opening the storage rebuilt it from an older schema.
The engine refills its derived strings once after such an open.

## `LVaultSession`

One unit of work over storage.
Disposing a session that was never committed rolls its work back.
A joined session neither commits nor rolls back, because the outer one owns the transaction.
Committing makes every write durable, and calling it twice is harmless.

## `LVaultFault`

The one exception a store adapter raises when the store or the source behind it fails.
Examples are a file locked, a folder denied or a download refused.
The engine catches it where it would carry on without the store, records it and keeps running.
A pure ring never names the file system, so it cannot catch the file system's own exceptions.
The adapter wraps them here and the cause travels as the inner exception.
The message of the cause is kept as this one's.

## `LAuditVault`

The port for the fault log, where the engine records an exception it could not act on.
`LAuditWriter` in Infrastructure is its adapter over `audit.log` inside the workspace.
Recording appends the exception with the instant it was recorded.
It returns where the log lives, or null when it could not be written.

## `LClock`

The clock the engine stamps a moment with.
`LClockSystem` in Infrastructure answers with the system clock in UTC, and a test answers with a frozen moment.
Every draft, claim and fetch interval reads the time through here, so a test freezes the whole engine at once.
Every fetch interval and the draft debounce also wait through here, so a harness can make those waits virtual.
A pause ends early when the token is cancelled.

## `LDoctorVault`

The port for the last-resort recovery of the workspace store.
`LDoctor` in Infrastructure is its adapter over the workspace database.
The engine asks once per workspace and learns only whether a rescue happened.

## `LDoctorDatabaseCreate`

Opens or creates the workspace store, migrating it to the current shape.
A store this build can no longer read is moved aside and a clean one started in its place.
It returns the healthy rescue, or one naming the file moved aside and why.

## `LLease`

The port that lets one running program hold a workspace at a time.
`LLeaseFile` in Infrastructure is its adapter over a file inside the workspace.
`LLeaseOpen` takes the lease and raises `LLeaseFault` when another holder has it.
`LLeaseClose` gives it up.
The engine opens the lease only when it is built as leased, and a test may answer with a fake.

## `LLeaseFault`

The one exception a lease adapter raises when another holder has the workspace.
It is no `LVaultFault`, because the store is healthy and only the lease is taken.
The cause travels as the inner exception, and its message is kept as this one's.
`LDoctor.LDoctorBusyCheck` counts it as a busy workspace, so the launch names the case to the user.

## `LTrail`

A path as the operating system reads it, and the port that resolves, relates and names one.
The engine keeps every path as text.
It asks here whenever the text must be read the way the disk reads it.
`LTrailSystem` in Infrastructure answers with `System.IO.Path`, and a test may answer the same or with its own rule.
Nothing here touches the disk, so no answer depends on what exists.

## `LTrailResolve`

The full path, taken under the root when it is not already fully qualified.
A relative path that climbs out of the root resolves to nothing.
So does a path the system cannot read.

## `LTrailRelativeResolve`

The path relative to the root, or nothing when it lies outside the root or on another root.

## `LTrailNameRead`

The last segment of the path, a trailing separator ignored.

## `LTrailRootCheck`

Whether the path starts from a root rather than from wherever the program stands.

## `LTrailNameNormalize`

The name with every character a file name cannot carry replaced by an underscore.

## Stamp

Commit: `52c21d68b4c4`
- `LAuditVault.cs` `051abf6a534e5663`
- `LClock.cs` `7cec97284660ee50`
- `LDoctorVault.cs` `e354cbea24a852f6`
- `LLease.cs` `cd302c1bc8f6caf3`
- `LLeaseFault.cs` `f0b5b3b1273de6bf`
- `LTrail.cs` `fc4a23c952f61d61`
- `LVault.cs` `d0de38b0fce48b77`
- `LVaultFault.cs` `030adfbeea8a02fa`
- `LVaultSession.cs` `d05acc00ce5ec68f`
