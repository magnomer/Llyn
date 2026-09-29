# LMarkupFacade.cs

## `internal sealed class LMarkupFacade`

The engine's facade for markup cargo reads, imports, exports and entry matching.
`LEngineMarkupStart` runs a whole import in its order, reading and storing on a pool thread.

## `public LMarkupFacade(LEngine engine)`

Stores the engine and its gate.

## `public LMarkupCargo LEngineMarkupRead(string path)`

The file at `path` parsed into a cargo, unknown languages omitted and twin names applied.

## `public IReadOnlyList<IReadOnlyList<LMarkupTarget>> LEngineMarkupFind(IReadOnlyList<LMarkupEntry> entries)`

The stored entries each parsed entry may join, found by the intake clerk under the gate.

## `public async Task<IReadOnlyList<LMarkupOmission>?> LEngineMarkupStart(`

The whole import in its order: read the file, find the targets, ask for the intakes, then store the cargo.
The question gets each parsed entry beside its ready targets, so no caller finds them again.
The read, the find and the store run on a worker thread, so the window stays live while parsing.
The hops live here because the shell starts no task of its own.
The question runs back on the caller's thread, since it may put up a window.
A declaration answered null stores nothing and answers null.
Otherwise it answers what the read and the import left behind.
The cargo never leaves the engine, so no caller holds it between the two steps.

## `public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

The import of a cargo under the reader's intake choices, run by the intake clerk under the gate.
