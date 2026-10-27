# Court

The clerks that hold a draft over time, link it to targets not yet stored, and commit it.
This folder stands apart from Clerk because it owns the draft files, claims, history and commit round.
Clerk only edits draft content in memory.
Court rows link a held draft to a target that is not an entry yet.

## `LChronicleClerk`

It gives editorial undo and redo over one held draft, kept in memory for the session only.
A chronicle is the ordered snapshots a held draft passed through, walked by undo and redo.
It is not `Revision`, which is the stored commit history, and not `Voyage`, which is the reader's trail.
`LDraft` is an immutable record and every edit already ends in one draft save.
So a step is one whole `LDraft`, and no request needs an inverse of its own.
Each draft id keeps a past list and a future list, the newest snapshot at the end of each.
They are lists rather than stacks, because trimming the oldest snapshot is a removal at the front.
The engine records the draft an edit replaces, once it has judged the edit worth a step.
Ending a draft drops its chronicle, because the file it walked is gone.
A settled court link drops the owner's chronicle too.
A snapshot from before the id rewrite would resurrect a draft translation id.
Steps that continue the same edit within a short window merge into one.

The clerk also keeps the keystrokes a tenure defers, and times the quiet before they flush.
That is a timing rule about draft edits, holding with no user present, so it sits beside the merge window.
The quiet waits on the rig's clock, so a replay on a virtual clock drives it.
The deferred requests have their own lock, since every tenure defers from its own thread.
That lock is never held while the clerk calls out, so it is always the innermost one.

## `LChronicleClerk.LChronicleClerkCap`

It is the most snapshots one draft keeps behind it.
Beyond it the oldest snapshot is forgotten, so a long session cannot grow without bound.

## `LChronicleClerk.LChronicleClerkWindow`

It is the milliseconds within which a request of the same type on the same draft continues the last step.

## `LChronicleClerk._lChronicleClerkDeferred`

It holds the requests waiting to flush, per draft id, in arrival order and one per `LRequestKey`.
It is also the lock guarding itself and the pending waits.

## `LChronicleClerk._lChronicleClerkPending`

It holds the wait in progress per draft id, cancelled by every new deferral with a delay.

## `LChronicleClerk.LChronicleClerkRecord`

It keeps the draft an edit is about to replace, and forgets the future.
The engine has already judged that the two drafts differ and that one of them is real work.
A new edit after an undo starts a new branch.
A request of the same type on the same draft within the window continues the last step.
Then nothing is pushed and only the moment is refreshed.
A null request always pushes and leaves no step to continue.
The moment is read from the rig's clock, so a test holds or advances it through the rig.

## `LChronicleClerk.LChronicleClerkClear`

It forgets both lists of one draft, and the last step with them.

## `LChronicleClerk.LChronicleClerkUndo`

Undo steps the held draft back one snapshot.
It returns the draft as saved, or null when nothing is behind it.
The draft being replaced is kept for redo.
Redo mirrors it forward and keeps the replaced draft for undo.
`LChronicleUndoCheck` and `LChronicleRedoCheck` tell whether a step would go anywhere, so a button can dim before it is pressed.

## `LChronicleClerk.LChronicleClerkDefer`

It files the request for the draft and waits out the quiet before its flush.
A later request with the same `LRequestKey` replaces the earlier one and keeps its place.
So the order of first arrival holds, and the last value per field wins.
The filing runs before the first await, so the request is queued when the call returns.
A positive delay cancels the wait in progress and starts a new one on the rig's clock.
A delay of zero files the request only and answers null, leaving the flush to the caller.
The replaced wait is cancelled outside the lock, so its ending runs with no clerk lock held.
The pause always yields before resuming, so an instant clock never flushes under the caller's locks.
It answers the wait that ran out, which the caller hands back to `LChronicleClerkDispatch`.
A cancelled wait answers null, since a newer deferral or a flush has replaced it.

## `LChronicleClerk.LChronicleClerkDispatch`

It takes every request waiting for the draft and stops its wait.
Given the wait it resumes from, it takes nothing when a newer wait has replaced that one.
Given null, it takes regardless, which is how a flush, a close and a halt empty the set.
The check and the taking share one hold of the lock, so no deferral slips in between them.
The stopped wait is cancelled outside the lock.

## `LChronicleClerk.LChronicleClerkRestore`

It is the one step undo and redo share, with the two lists swapped between them.
The newest snapshot of the source list is written as the held draft.
The draft it replaced is kept at the end of the target list.
It answers null when the source holds nothing for the draft.
A draft whose file is gone is refused, since there is nothing to restore over.

## `LClaimClerk`

The clerk over a held draft: the file, the claim beside it and the set of ids this clerk holds.
A claim is a running program's hold on a draft, written in the workspace.
Every launch can then tell live work from what a crash left behind.
The held set is the in-memory half of that hold, the ids this rig started and still keeps.
Both halves are read together, so no engine can take away a draft another editor is typing into.

The engine keeps what is engine-wide: the gate, the stale marks, the trove and the bulletin.
The engine also keeps the entry commit, because the entry save still lives beside the transcription and reflex syncs.
The dirty check of an entry draft stays with the engine too.
Only the engine loads an entry with its audio resolved.
The per-kind starts and commits live in the kind owners `LCitationClerk` holds, over this clerk's primitives.
The court rows live in `LCourtClerk` and the undo history in `LChronicleClerk`.
A new rig gets a new clerk, so a switched workspace starts with nothing held.
The chronicle is dropped with a finished draft and the court is swept with it.

## `LClaimClerk.LClaimClerkHeld`

It holds the ids of the drafts this clerk started and still holds.
The engine marks every one of them stale when the workspace changes under it.

## `LClaimClerk.LDraftBlank`

It is what a draft carrying no entry is measured against.
A form that was never opened on an entry started from nothing.

## `LClaimClerk.LDraftCreate`

It makes a draft named by the issuer and stamped by the clock, not yet written anywhere.
`origin` records which surface opened the work, so a recovered draft can say where it came from.

## `LClaimClerk.LClaimClerkStart`

It asks the draft port to write the first file and a claim naming this process beside it.
Then it takes the id into the held set.
The port names the process, so a test fake may pose as another process without starting one.
Another launch reading the folder then knows the work is live.

## `LClaimClerk.LDraftRead`

It reads one held draft, or null when its file is gone or unknown.
The credits come back under the names the database holds now, so a rename elsewhere shows on the next read.
`LDraftAuthorUpdate` renames each stored credit.
A minted Author is not stored yet, and one deleted meanwhile keeps the name the draft holds.

## `LClaimClerk.LDraftLoad`

It reads a held draft that the caller is about to act on, refusing when it is gone.
Another window may have stored or discarded it already.
`LClaimClerkLoad` also refuses a draft this clerk does not hold, so a leftover or another copy's draft is refused.

## `LClaimClerk.LClaimStaleValidate`

It refuses `id` as stale when a workspace change marked it, before any draft call proceeds.
The engine keeps the set, since a stale mark is a fact of its session.

## `LClaimClerk.LDraftRefusalRead`

It gives the reason a commit of the draft would be refused now, or null when none is known ahead.
An entry draft is refused for a missing headword, and an author draft for a blank name.
A sentence, situation or source draft always commits, so it answers null.

## `LClaimClerk.LDraftScan`

It lists every held draft the folder still carries.
A broken file is skipped rather than thrown, so one bad draft never hides the rest.

## `LClaimClerk.LDraftSweep`

It loads a draft, drops its unreadable values to unspecified, and stores it again.
The facade calls it so Core's normalizing never runs in the engine.
`LDraftSave` writes one draft file over whatever stood there.

## `LClaimClerk.LClaimClerkCheck`

It answers whether this clerk may take a draft away.
It may only when a live claim names this process and its own set holds the id.
Those two together mean this rig started the draft and still has it.
A claim naming another process answers no, and so does a draft with no claim.

## `LClaimClerk.LClaimForeignCheck`

It answers whether a copy of the program other than this one is still working on a draft.
A live claim naming this very process is one this clerk already knows about through its own set.
So only another process's claim answers true, which is the case an in-memory set can say nothing about.
A stale claim is swept by the check itself and answers false.

## `LClaimClerk.LClaimClerkFinish`

It ends a draft whose work is stored.
The held set and the chronicle let it go here, and the port drops the claim and the file.
Nothing here touches the court, because the commit settled it before the files went.

## `LClaimClerk.LClaimClerkDelete`

It removes one held draft with its claim, after the links that draft owns are settled.
The rows pointing at this draft are left, because the caller dropping a chip already took its own row back.
The cascade runs inside the draft port, and the clerk only applies the release it answers.

## `LClaimClerk.LClaimClerkCancel`

It discards held work, the court first and the file second.
The rows this draft owns go with their targets, and the rows pointing at it are settled to nothing.
Each row pointing at it has its tentative id struck from the draft that held it.
A chip left carrying it would be stored as a translation of a record that never existed.
The cascade runs inside the draft port, and the clerk only applies the release it answers.

## `LClaimClerk.LClaimClerkSweep`

It asks the draft port to clear what the stores keep forever, before anything counts what is left.
The half-written pending files go, and a draft file of another version goes with its claim and its court rows.
A claim whose draft file is gone goes too, since nothing will ever ask about that draft again.
The engine then walks the drafts left and cancels the ones holding nothing unsaved.

## `LClaimClerk.LClaimClerkSync`

It runs when a delete, a cancel or a sweep of the port throws midway.
The port may have dropped some claims before it failed.
So each held id the port no longer claims leaves the held set and the chronicle.
The original failure is thrown on.
The clerk never keeps a draft held that the store no longer backs.

## `LClaimClerk.LDraftReleaseApply`

It applies one release the draft port answered.
Each released draft leaves the held set and the chronicle.
Each rewritten owner leaves the chronicle, because a snapshot from before the rewrite would bring the draft id back.
The held set goes to the port as it stands, so a target goes only while this clerk holds it.

## `LCourtClerk`

It owns the rows linking a held draft to a target that is not an entry yet.
A chip naming a word the user has not stored is such a target.
The row outlives neither end, since committing settles it and cancelling drops it.
The translation side of a commit lives here too, since it is the court that decides what a chip becomes.
The settle pass maps each tentative id through the round's identity map and strikes what still names nothing.
The court pass runs once the round is through and writes the links the walk had to hold back.
The draft calls that open and end that work live in `LClaimClerk`.
The chronicle is dropped when an owner is rewritten, and the translation clerk appends the settled links.

## `LCourtClerk.LCourtClerkSave`

It writes one court row linking a held draft to a target that is not an entry yet.
The headword and language travel with it, because the chip is shown long before the target is real.
It returns the row so the caller can drop it again by id.

## `LCourtClerk.LCourtClerkScan`

It lists every row one draft holds, whatever it points at.
Committing and cancelling both act on the whole set.

## `LCourtClerk.LCourtClerkFind`

It finds the court row one draft holds against one target, or null when there is none.
An ordinary translation to a stored entry answers null, because it never had a row.

## `LCourtClerk.LCourtClerkDelete`

It removes one court row, for a chip the user took back.

## `LCourtClerk.LCourtClerkSettle`

It settles every row pointing at `draftId` and rewrites the draft that held each one.
A real id of zero strikes the tentative id instead, which is how a cancelled target leaves its owners.
The draft port does the rewrite through Core's `LEntryDraft.LEntryDraftSettle` and answers the owners it rewrote.
Each owner's chronicle is dropped, because a snapshot from before the rewrite would bring the draft id back.

## `LCourtClerk.LCourtClerkApply`

It writes every link the walk held back, now that every draft of the round has an entry id.
The owner's content as it was loaded says which cards carried the target's draft id.
The owner's map says which stored card each of those became.
A link whose owner or target did not settle is passed over, because there is nothing to write it against.
The private form walks one card list of the owner for the target's draft id, children included.
A card that carried it has the target's entry id appended to its stored translations.
`LTranslationAppend` adds it unless the card already links it.

## `LCourtClerk.LTranslationSettle`

It gives the same content with every translation that names no stored entry removed.
Each id is first mapped through `identity` to the entry it became.
Both card lists are settled, because a chip can sit on a meaning or on a collocation.
The card list form keeps only the translations that still read as a stored entry.
An id that reads nothing points at a record that was discarded or never arrived.
It may also be one still being committed in this round.
There is nothing to store it against yet, and the round's last pass writes the ones that become real.
The check is a single row read, not a load of the whole entry tree.

## `LIdentity`

It issues the temporary ids a draft row carries before the database has given it a real one.
A row that has never been stored has no id, yet the draft holding it still needs to name it.
Something has to say which sentence a chip stands for while the user is still editing.
Every link inside the draft is made through that id exactly as it would be through a real one.
Temporary ids are negative and real ids are positive, because SQLite counts rows from one.
That makes the two impossible to confuse and removes the need for a second kind of key beside the id.
A row whose id is still negative has never been written, and one whose id is positive has.

The issuer lives here and not in the UI, because the UI holds no ground truth.
The draft is held below the UI, and ids are issued below the UI.
Saving sends that held draft down to the database rather than reading it back off the widgets.
The issuer is an instance the engine owns, one per open workspace.
It counts from the workspace row rather than from a process-wide static.
Drafts, court links, and claims are files named by the id they were issued.
A counter that restarted at zero with the process would name a new draft after a file still on disk.
So the floor lives in the workspace database and every issue lowers it there before the id is used.

## `LIdentity.LIdentity`

It binds the issuer to the workspace vault whose floor it lowers.
The retired ids are ones a tenure still holds from a workspace the engine has left.
Each workspace counts from its own floor, so the next one would otherwise issue those ids again.

## `LIdentity.LIdentityCreate`

It lowers the floor in the database and returns the new floor, in one statement.
The write commits before the id is handed out.
A crash after the call therefore cannot bring the id round again.
Two engines open on one workspace lower the same row, so neither can issue what the other already holds.
A retired id is skipped, so a draft of this workspace never shares an id with a stale tenure.

## `LIdentity.LIdentityRecord`

It records under `draftId` the real `rowId` a store just gave a draft row.
A `draftId` that is not negative names a stored row already and is not recorded.
The map is what the commit hands back so the held draft can be renumbered onto real ids.

## `LOutcomeClerk`

It runs the commit round of an entry draft, producing the outcome the tenure reports.
It composes the claim, court and entry clerks, which is why it is not a part of any of them.
The single entry save and update the engine offers live here too, since the round is made of them.

## `LOutcomeClerk.LOutcomeEntrySave`

It makes a fresh entry from a draft, its ids minted, its respellings filled and its headword trimmed.
The frequency fetch starts once the entry stands.

## `LOutcomeClerk.LOutcomeEntryUpdate`

It rewrites an entry from a draft, its pending inflection fetch cancelled first.
A headword or language that changed starts the frequency fetch again.

## `LOutcomeClerk.LOutcomeClerkCommit`

The round covers one draft and every draft it links, in one session.
The court is applied once the round settled.
Then the queued draft and court writes run and the held drafts finish.
`raised` receives every entry the round wrote, for the bulletins.

The private frame handles one draft of the round.
A linked draft already loaded but not settled is deferred to the court.
A linked draft not yet loaded is committed first, so its entry id can settle the link.
The draft is saved and its court settled after the session, so a failed round leaves both untouched.

## Stamp

Commit: `1f3467bbe5c6`
- `LChronicleClerk.cs` `af0a4478272c0830`
- `LClaimClerk.cs` `d1ae3bcdb451a317`
- `LCourtClerk.cs` `06e2ab7d81fc26cd`
- `LIdentity.cs` `5e1bafe1d4235f90`
- `LOutcomeClerk.cs` `1844015f999d9a62`
