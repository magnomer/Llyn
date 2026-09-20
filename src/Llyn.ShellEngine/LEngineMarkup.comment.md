# LEngineMarkup.cs

## `public sealed partial class LEngine`

The markup facades: the cargo read, the import and the export, over the markup clerk and its intake.
The two `Start` overloads run the same work on a pool thread for the shell.

## `public LMarkupCargo LEngineMarkupRead(string path)`

The file at `path` parsed into a cargo, unknown languages omitted and twin names applied.

## `public Task<LMarkupCargo> LEngineMarkupStart(string path)`

The first stage of an import on a worker thread: the read, so the window stays live while parsing.
The hop lives here because the shell starts no task of its own.

## `public IReadOnlyList<LEntry> LEngineMarkupFind(LMarkupEntry entry)`

The stored entries with the headword and language of one parsed entry.

## `public Task<LMarkupOutcome> LEngineMarkupStart(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

The second stage on a worker thread: the import under the intakes the customs window settled.
One verb names both stages, since together they are the one staged procedure the shell begins.

## `public LMarkupOutcome LEngineMarkupImport(LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

The import of a cargo under the reader's intake choices, run by the intake clerk under the gate.

## `internal void LEngineMarkupExport(IReadOnlyList<long> ids, string path)`

The entries written as one markup file through the markup clerk.

