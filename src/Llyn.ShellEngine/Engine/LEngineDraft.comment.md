# LEngineDraft.cs

## `public sealed partial class LEngine`

The one gateway to tentative work, so the shell never opens the drafts folder itself.
The files, the claims and the held set are `LClaimClerk`'s, and every call here reaches them through it.
An id raised here belongs to the workspace that raised it, so a workspace change marks every held id stale.
Every call naming an id is checked against that mark, which is the one fact the clerk cannot know.
The entry commit stays here, because the entry save still runs the transcription and reflex syncs beside it.
The dirty check of an entry draft stays here too.
Only the engine loads an entry with its audio resolved.
These are primitives, and `LTenure` in `LTenure.cs` is the session object that sequences them for a panel.
The sentence, situation, source and author starts and commits live beside their kind's reads.
Every edit in between is a request, applied in `LEngineRequest.cs` for all kinds alike.

## `private readonly HashSet<long> _lEngineDraftStale = [];`

The ids held when the workspace changed, refused ever after.

## `public IReadOnlyList<LMarkdownBlock> LEngineMarkdownParse(string? text)`

The note text parsed into blocks, through the entry field helpers.

## `internal LDraft LEngineDraftStart(string origin, long? entryId)`

Starts an entry draft through the citation clerk.
A new draft takes the first listed language, an existing entry is loaded with its recordings resolved.
An entry that no longer stands is refused.

## `internal IReadOnlyList<LRequest> LEngineDraftPrepare(long id)`

The requests that give a held entry draft the rows the editor always shows.
One meaning card and one collocation card, one sentence under each card, and the first transcription row.
The glyph section's own row is added too when the language has one.
The tenure applies them in its prepare turn and asks again, since a new card needs its sentence.

## `private static void LEngineSentencePrepare(long id, LCardDraft card, List<LRequest> requests)`

Adds the sentence request for one card that has none.

## `internal LDraft? LEngineDraftRead(long id)`

Reads one held draft, or null when its file is gone or unknown.
An id from a closed workspace is not a missing file and is refused rather than answered null.

## `internal IReadOnlyList<LDraft> LEngineDraftScan()`

Every held draft the folder still carries.
This is what a session offers back after a crash.

## `public void LEngineDraftDelete(long id)`

Removes one held draft with its claim, after the links that draft owns are settled.
A draft bulletin carrying id zero follows, because the held set shrank and the status bar counts that set.
Zero names no draft, so no editor mistakes it for its own.

## `internal bool LEngineDraftCheck(long id)`

Whether held work differs from the record it was started from, with the refusal left unread.

## `internal bool LEngineDraftCheck(long id, out string? refusal)`

Whether held work differs from the record it was started from.
`refusal` names the reason a commit of it would be refused right now, or null when it would go through.
The form enables Save from that answer rather than judging the headword itself.
A missing draft file answers false, because there is nothing to lose.

## `private bool LEngineDraftCheck(LDraft draft)`

Whether the draft differs from its origin, answered by the citation clerk for every kind of draft.

## `internal void LEngineDraftSweep(long id)`

Drops every unreadable value the held draft carries to unspecified and stores the draft again.
A commit refuses a draft holding unreadable values.
The shell asks the user and then calls this before committing again.
A draft bulletin follows so every view re-reads.

## `internal LOutcome LEngineDraftCommit(long id)`

The commit round of one entry draft, run by the outcome clerk under the gate.
The stale mark is checked first and the session trove of the draft cleared after.
Every entry the round wrote gets its bulletin and its inflection fetch outside the gate.

## `internal void LEngineDraftCancel(long id)`

Discards held work: the court first, the file second.
It also drops what this draft's lookups and audio searches found, because closing is what frees a trove.
A draft bulletin carrying id zero follows, as after a delete, so the status bar recounts the held set.

## `public void LEngineLeftoverSweep()`

Cancels every unheld draft that says nothing its origin does not.
A draft with no entry is cancelled when it is blank.
One with an entry is cancelled when the citation clerk finds it equal to the entry.

## `public IReadOnlyList<LDraft> LEngineLeftoverRead()`

The held drafts nothing is still working on and that differ from the entry they opened from.
A draft this engine started is claimed by an open window, and one another copy claims is passed over too.
A claim naming a process that is gone is what a crash leaves behind.
What remains is what a forced shutdown cost, counted at launch.

## `private void LEngineDraftValidate(long id)`

Refuses an id marked stale by a workspace change, through the claim clerk's static check.

## `private LDraft LEngineDraftLoad(long id)`

Reads a held draft that the caller is about to act on, refusing when it is gone.

