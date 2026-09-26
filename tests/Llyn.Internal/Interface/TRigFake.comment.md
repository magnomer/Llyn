# TRigFake.cs

## `internal static class TRigFake`

Builds a rig from fakes, so an engine test runs without a workspace folder or a database.
The ports the engine touches at start are answered by small in-memory fakes below.
Every other port is a stub that throws on any call.
A test reaching storage it did not seat therefore fails loudly.

## `internal static LRig TRigFakeBuild(LEntryVault entries, string workspace)`

The rig over `entries` as its entry port, standing on the root named `workspace`.
The root is a bare label, since no fake here touches disk.
Two rigs built with two labels let a test prove a rig apply moved the engine.
The trail is the real system adapter, since path rules touch no disk, and the clock is a `TClockFake`.
The press is a `TPress`, which records what it is handed and touches no printer.
The process id is one, so a claim from another process is any claim not naming one.

## `private static TRigFakePort TRigStubCreate<TRigFakePort>() where TRigFakePort : class =>`

One throwing stub for the port `TRigFakePort`, generated over the proxy below.
A generated stub per port spares the suite a hand-written class for every one of the forty ports.

## `public class TRigFakeProxy : DispatchProxy`

The proxy base every stub derives from: any call throws and names the member that was asked.
It is public because the runtime derives a type from it in a dynamic assembly.

## `private sealed class TRigFakeVault : LVault`

The root port: a session that commits nothing and reports no migration.

## `private sealed class TRigFakeSettings : LSettingsVault`

Settings kept in memory, reading as the defaults until the engine saves once.
Existence follows the save, so a rig apply inherits the current settings the way a fresh folder does.

## `private sealed class TRigFakeLanguages : LLanguageVault`

A language port listing no packs, so the vocabulary import and the diwei rebuild have nothing to write.
