# TInterfaceEngine.cs

## `internal static partial class TInterface`

The relays for the engine operations over an entry as a whole.
That is saving, loading, updating, and deleting one, and the drafts and revisions around it.
The frequency fill, its band resolution, and its read and start are relayed here too.
The grasp read and save are relayed here as well.
The script read, its pending check and its find are relayed here too.
The reflex read, its rules, its pending check, its start and its find are relayed here as well.
The markup read, find, import and export and the portrait export are relayed here too.
The entry and page likeness reads, both prints and the press hand-in are relayed here as well.
The workspace open builds a rig for the path through the real factory and applies it, as the bootstrap does.
The status bar read is relayed here too.
The chronicle undo and redo with their two checks are relayed here as well.
The author draft start is relayed here beside the entry draft start.
The clock is set here through the `TClockFake` every test rig carries, so time freezes engine-wide.
The recording sweep is relayed here beside the leftover sweep.
Most relays are transparent and carry no test logic of their own.
A few rebuild a read or a write the engine no longer offers from the clerks it keeps.

## `internal static TMarkupOutcome TEngineMarkupImport(this LEngine engine, LMarkupCargo cargo, IReadOnlyList<LMarkupIntake> intakes)`

The engine reports only what an import left behind, so the stored entries are found again here.
A merge or a replace names its target.
A new entry is the match its headword did not have before the import.

## `private static LEntry TEngineEntryCommit(this LEngine engine, long? id, LEntryDraft draft)`

Writes a whole draft the way the editor does.
The draft is started, its content minted, and then committed.
The commit starts the fetches a real save starts.
A test that must not fetch turns the setting off first.

## `internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine)`

The drafts a leftover sweep leaves for the user.
Each is saveable, held by no one here, and claimed by no other process.
No shell reads them until a recovery dialog exists, so the tests read them through the claim clerk.
