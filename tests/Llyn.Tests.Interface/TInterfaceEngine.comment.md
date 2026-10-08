# TInterfaceEngine.cs
Hash: `42f55fbcd7b1b7e6`

## `internal static partial class TInterface`

The relays for the engine operations over an entry as a whole.
That is saving, loading, updating, and deleting one, and the drafts and revisions around it.
The court relays and the chronicle undo and redo with their two checks are relayed here too.
The author draft start is relayed here beside the entry draft start.
The grasp, the favorite and the mention find relays sit here as well.
The leftover read and sweep and the request apply are relayed here too.
The engine start over a hand-built rig sits here as well.
Most relays are transparent and carry no test logic of their own.
A few rebuild a read or a write the engine no longer offers from the clerks it keeps.

## `private static LEntry TEngineEntryCommit(this LEngine engine, long? id, LEntryDraft draft)`

Writes a whole draft the way the editor does.
The draft is started, its content normalized and saved, and then committed.
The commit starts the fetches a real save starts.
A test that must not fetch turns the setting off first.

## `internal static IReadOnlyList<LDraft> TEngineLeftoverRead(this LEngine engine)`

The drafts a leftover sweep leaves for the user.
Each is saveable, held by no one here, and claimed by no other process.
No shell reads them until a recovery dialog exists, so the tests read them through the claim clerk.

## `internal static (string, IReadOnlyList<LMentionPiece>, string) TEngineLineRead(LSentenceDraft sentence, LSentenceOrder order, string mark, IReadOnlyDictionary<long, string> citations)`

Relays the example line read.
It answers the frame, the sentence pieces and the Source line.

## `internal static LEngine TEngineCreate(LRig rig)`

Starts an engine over `rig` that answers the same rig for any workspace and keeps no workspace pointer.
A fault fact thus runs the engine over a rig whose vaults it has replaced.
