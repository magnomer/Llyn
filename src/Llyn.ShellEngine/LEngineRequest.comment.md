# LEngineRequest.cs

## `public sealed partial class LEngine`

The engine owns the draft.
A form sends one edit at a time as an `LRequest`, and the engine applies it, saves, and announces.
The form never assembles an `LEntryDraft` from its controls, so the engine's file is the only truth.
The applying itself is `LDraftClerk`'s, over the ports of the rig.
The engine keeps what is engine-wide: the gate, the held set, the chronicle, the file and the bulletin.

## `internal LDraft LEngineRequestApply(LRequest request)`

Applies one request to the held draft it names and returns the draft as saved.
The draft must be held by this engine, so a request for a leftover or another copy's draft is refused.
The clerk applies the request, and a headword or language change then drops the recordings that no longer fit.
The content is normalized after the change, so anything the change left unnamed is named before the write.
The draft being replaced is recorded in the chronicle first, so the edit can be undone.
A request that changes nothing records nothing, since there is nothing to step back from.
A request that leaves the draft equal to what was held writes nothing and raises nothing.
So a form may send the whole body of a panel on every pause.
Only a real change is announced.
The bulletin is raised outside the gate, after the file is written.
So a subscriber that re-reads on the bulletin reads what was announced, and never deadlocks on the gate.
The saved draft is returned as well, so a caller can read a minted id without waiting for the bulletin.

## `private LDraft LEngineAudioClear(LDraft held, LDraft draft)`

Drops the recordings a headword or language change made wrong, comparing the applied draft with the held one.
A recording is audio of one word in one language.
A new language makes every recording on the entry audio in the wrong language, so all of them go.
A new spelling drops only a recording fetched this draft, since a stored one is the entry's own.
So a typo corrected in a stored headword keeps the entry's audio.
A request that leaves the field as it was drops nothing, so a form may send it on every pause.
The stored entry is read through the draft's entry id, and a draft started on nothing has none.
This stays in the engine because only the engine can load the stored entry with its recordings resolved.
