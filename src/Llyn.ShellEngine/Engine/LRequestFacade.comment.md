# LRequestFacade.cs

## `internal sealed class LRequestFacade`

The engine's facade for requests against held drafts.
A form sends one edit at a time as an `LRequest`, and the engine applies it, saves, and announces.
The form never assembles an `LEntryDraft` from its controls, so the engine's file is the only truth.
The applying itself is `LDraftClerk`'s, over the ports of the rig.
The card order calls, the undo and redo calls and the court calls of a held draft live here too.
The facade shares the engine gate and uses the engine for draft operations and bulletins.

## `public LRequestFacade(LEngine engine)`

Creates the facade for its owning engine and shares the engine gate for request operations.

## `internal LDraft LEngineRequestApply(LRequest request)`

Applies one request to the held draft it names and returns the draft as saved.
The draft must be held by this engine, so a request for a leftover or another copy's draft is refused.
The clerk applies the request, and a headword or language change then drops the recordings that no longer fit.
The content is normalized after the change, so anything the change left unnamed is named before the write.
The draft being replaced is recorded in the chronicle first, so the edit can be undone.
A request that leaves the draft equal to what was held writes nothing and raises nothing.
The bulletin is raised outside the gate, after the file is written.

## `private LDraft LEngineAudioClear(LDraft held, LDraft draft)`

Drops the recordings a headword or language change made wrong, comparing the applied draft with the held one.
A new language makes every recording on the entry audio in the wrong language, so all of them go.
A new spelling drops only a recording fetched this draft, since a stored one is the entry's own.
This stays in the engine because only the engine can load the stored entry with its recordings resolved.

## `private void LEngineChronicleRecord(LDraft held, LDraft saved, LRequest? request)`

Keeps the draft an edit is about to replace, when the edit is worth a step.
Nothing is kept when the two drafts match by the measure the dirty check uses.
Nothing is kept either when neither draft differs from what it was started from.
An editor opening a draft adds blank cards, blank sentences and its language before the user types.
Those requests move nothing the user could see undone, so they leave no step behind.
The chronicle clerk keeps the step once the engine has judged it.

## `internal LDraft? LEngineChronicleUndo(long id)`

Steps the held draft back one snapshot and returns the draft as saved.
Null when nothing is behind it.
A draft bulletin follows, so every open editor of the draft reloads.

## `internal LDraft? LEngineChronicleRedo(long id)`

Steps the held draft forward one snapshot and returns the draft as saved.
Null when nothing is ahead of it.

## `internal bool LEngineUndoCheck(long id)`

Whether an undo would step anywhere, so a button can dim before it is pressed.

## `internal bool LEngineRedoCheck(long id)`

Whether a redo would step anywhere.

## `internal IReadOnlyList<LCardDraft> LEngineDraftMove(long id, bool collocation, int from, int target)`

Reorders one card inside the held content and hands the whole list back renumbered.
Positions are rewritten from `1` so they stay contiguous whatever the drag did.
`collocation` picks which of the two lists is reordered.
Both ends are clamped into range, because a drag can land past the last card.

## `internal IReadOnlyList<LCardDraft> LEngineDraftNormalize(long id, bool collocation)`

Renumbers one card list without moving anything, and hands the whole list back.
A removed card leaves a gap in the numbering that nothing else closes.

## `internal long LEngineCardCreate()`

Mints one card id for a card the form has just added.
The form cannot mint one itself, because identity is the workspace's to give.

## `private IReadOnlyList<LCardDraft> LEngineCardApply(LDraft draft, bool collocation, List<LCardDraft> cards)`

Writes one card list onto a held draft, numbered from `1` and named throughout.
A card still carrying no id is named here too, because the answer is about to be matched by id.
When the list actually changed, the draft is recorded in the chronicle before the write.
So a move can be undone.

## `internal LCourt LEngineCourtSave(long ownerId, long targetId, string headword, string language)`

Writes one court row: a link from a held draft to a target that is not an entry yet.
Returns the row so the caller can drop it again by id.

## `internal LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language)`

Starts a tentative target and records the row naming it, as one call.
The word and the language are written straight into the target's file before the row is made.
A failure anywhere after the target is started cancels it before the refusal leaves.
So the workspace holds either the target with its row or neither.

## `internal void LEngineCourtDelete(long linkId)`

Removes one court row, for a chip the user took back.

## `internal LCourt? LEngineCourtFind(long ownerId, long targetId)`

The court row one draft holds against one target, or null when there is none.


