# TEngineFake.cs

## `internal static class TEngineFake`

Builds a port from a table of answers, so a deportment test runs without an engine or a database.
A member the table names is answered by its function over the call's arguments.
Every other member throws, so a deportment reaching the engine where the test expected none fails loudly.
One generated proxy per port spares the suite a hand-written class for every port member.

## `internal static TEngineKind TEngineCreate<TEngineKind>(Dictionary<string, Func<object?[]?, object?>> answers)`

The port `TEngineKind` answering the members `answers` names by method name.

## `internal static TEngineKind TEngineStubCreate<TEngineKind>() where TEngineKind : class`

A port that answers nothing, for the ports a deportment holds but the test never drives.

## `public class TEngineProxy : DispatchProxy`

The proxy the runtime generates the port over, public and constructible as `DispatchProxy` requires.
