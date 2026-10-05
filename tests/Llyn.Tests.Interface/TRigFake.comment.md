# TRigFake.cs
Hash: `4360138ca3b18025`

## `internal static class TRigFake`

Builds a rig from fakes, so an engine test runs without a workspace folder or a database.
The ports the engine touches at start are answered by small in-memory fakes below.
Every other port is a stub that throws on any call.
A test reaching storage it did not seat therefore fails loudly.

## `internal static LEngine TRigFakeStart(LRig rig)`

Binds an engine to `rig`, whose workspace change builds a fresh fake rig.
The engine's pointer callback does nothing, so no pointer is recorded.

## `internal static LRig TRigFakeBuild()`

The fake rig over a `TVaultFake` entry port, on the bare `fake` root.

## `internal static LRig TRigFakeBuild(LEntryVault entries)`

The fake rig over `entries` as its entry port, on the bare `fake` root.

## `internal static LRig TRigFakeBuild(LEntryVault entries, string workspace)`

The rig over `entries` as its entry port, standing on the root named `workspace`.
The root is a bare label, since no fake here touches disk.
Two rigs built with two labels let a test prove a rig apply moved the engine.
The language port is a `TRigFakeLanguages` and the usher is a throwing stub.

## `internal static LRig TRigFakeBuild(LLanguageVault languages, LUsher usher)`

The fake rig over the language packs and the usher a test hands in, on the bare `fake` root.
A flag fill then runs through the real engine, while the case decides every fetch and file.

## `private static LRig TRigFakeBuild(LEntryVault entries, string workspace, LLanguageVault languages, LUsher usher)`

The one rig build the public forms reach, so each fake stays in one place.
The trail is the real system adapter, since path rules touch no disk, and the clock is a `TClockFake`.
The press is a `TPress`, which records what it is handed and touches no printer.
The warrant is a `TWarrantFake`, so no test touches the operating system store.
The process id is one.
Every port not named above is a throwing stub, the livery and manifest ports among them.

## `internal static LSourceFactory TRigSourceCreate()`

A source factory that throws on every build, for a search that must fail before its sources exist.

## `internal static string TRigFaultRead(string? method)`

The message a stub throws when `method` is called.
A test asserting a recorded stub fault reads it here instead of copying the text.

## `private static TRigFakePort TRigStubCreate<TRigFakePort>() where TRigFakePort : class`

One throwing stub for the port `TRigFakePort`, generated over the proxy below.
A generated stub per port spares the suite a hand-written class for every one of the forty-odd ports.

## `public class TRigFakeProxy : DispatchProxy`

The proxy base every stub derives from.
Any call throws `NotSupportedException` with `TRigFaultRead` of the member that was asked.
It is public because the runtime derives a type from it in a dynamic assembly.

## `private sealed class TRigFakeVault : LVault`

The root port: a session that commits nothing and reports no migration.

## `private sealed class TRigFakeSession : LVaultSession, IDisposable`

The session the root port hands out, whose commit and disposal do nothing.

## `private sealed class TRigFakeAudit : LAuditVault`

An audit shelf that records nothing and answers no path.

## `private sealed class TRigFakeDoctor : LDoctorVault`

A doctor port whose database creation always answers healthy.

## `private sealed class TRigFakeSettings : LSettingsVault`

Settings kept in memory, reading an English default until the engine saves once.
Existence follows the save.

## `private sealed class TRigFakeLanguages : LLanguageVault`

A language port listing no packs and validating no name.
Its flag read answers none, and reading a pack throws.
