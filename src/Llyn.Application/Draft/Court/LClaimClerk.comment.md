# LClaimClerk.cs

## `public sealed class LClaimClerk`

The clerk over a held draft: the file, the claim beside it and the set of ids this clerk holds.
A claim is a running program's hold on a draft, written in the workspace.
Every launch can then tell live work from what a crash left behind.
The held set is the in-memory half of that hold, the ids this rig started and still keeps.
Both halves are read together, so no engine can take away a draft another editor is typing into.
The engine keeps what is engine-wide: the gate, the stale marks, the trove and the bulletin.
The engine also keeps the entry commit, because the entry save still lives beside the transcription and reflex syncs.
The dirty check of an entry draft stays with the engine too.
Only the engine loads an entry with its audio resolved.
The per-kind starts and commits live in `LCitationClerk`, over this clerk's primitives.
The court rows live in `LCourtClerk` and the undo history in `LChronicleClerk`.
A new rig gets a new clerk, so a switched workspace starts with nothing held.

## `public LClaimClerk(LRig rig, LIdentity identity, LChronicleClerk chronicle, LCourtClerk courts)`

Reads the draft, claim and author ports, the clock and the process id out of `rig`.
The issuer names a new draft.
The chronicle is dropped with a finished draft and the court is swept with it.

## `public IReadOnlySet<long> LClaimClerkHeld`

The ids of the drafts this clerk started and still holds.
The engine marks every one of them stale when the workspace changes under it.

## `public static LEntryDraft LDraftBlank`

What a draft carrying no entry is measured against.
A form that was never opened on an entry started from nothing.

## `public LDraft LDraftCreate(string origin, long entryId, LEntryDraft content)`

A draft named by the issuer and stamped by the clock, not yet written anywhere.
`origin` records which surface opened the work, so a recovered draft can say where it came from.

## `public LDraft LClaimClerkStart(LDraft draft)`

Writes the first file, writes a claim naming this process beside it and takes the id into the held set.
This process is the one the rig names, so a test may pose as another process without starting one.
Another launch reading the folder then knows the work is live.

## `public LDraft? LDraftRead(long id)`

Reads one held draft, or null when its file is gone or unknown.
The credits come back under the names the database holds now, so a rename elsewhere shows on the next read.

## `public LDraft LDraftLoad(long id)`

Reads a held draft that the caller is about to act on, refusing when it is gone.
Another window may have stored or discarded it already.

## `public LDraft LClaimClerkLoad(long id)`

Reads a draft this clerk holds, refusing one it does not.
A request for a leftover or another copy's draft is refused here.

## `public static void LClaimStaleValidate(IReadOnlySet<long> stale, long id)`

Refuses `id` as stale when a workspace change marked it, before any draft call proceeds.
The engine keeps the set, since a stale mark is a fact of its session.

## `public static string? LDraftRefusalRead(LDraft draft)`

The reason a commit of `draft` would be refused now, or null when none is known ahead.
Only an entry draft has such a reason, and it is the missing headword.
A sentence, situation, source or author draft always commits, so it answers null.

## `public IReadOnlyList<LDraft> LDraftScan()`

Every held draft the folder still carries.
A broken file is skipped rather than thrown, so one bad draft never hides the rest.

## `public void LDraftSave(LDraft draft)`

Writes one draft file over whatever stood there.

## `public bool LClaimClerkCheck(long id)`

Whether this clerk may take a draft away.
It may only when a live claim names this process and its own set holds the id.
Those two together mean this rig started the draft and still has it.
A claim naming another process answers no, and so does a draft with no claim.

## `public bool LClaimForeignCheck(long id)`

Whether a copy of the program other than this one is still working on a draft.
A live claim naming this very process is one this clerk already knows about through its own set.
So only another process's claim answers true, which is the case an in-memory set can say nothing about.
A stale claim is swept by the check itself and answers false.

## `public void LClaimClerkFinish(long id)`

Ends a draft whose work is stored.
The held set, the chronicle, the claim and the file all let it go.
Nothing here touches the court, because the commit settled it before the files went.

## `public void LClaimClerkDelete(long id)`

Removes one held draft with its claim, after the links that draft owns are settled.
The rows pointing at this draft are left, because the caller dropping a chip already took its own row back.

## `public void LClaimClerkCancel(long id)`

Discards held work: the court first, the file second.
The rows this draft owns go with their targets, and the rows pointing at it are settled to nothing.
Each row pointing at it has its tentative id struck from the draft that held it.
A chip left carrying it would be stored as a translation of a record that never existed.

## `public void LClaimClerkSweep()`

Clears what both archives keep forever, before anything counts what is left.
The half-written pending files go, and a draft file of another version goes with its claim and its court rows.
A claim whose draft file is gone goes too, since nothing will ever ask about that draft again.
The engine then walks the drafts left and cancels the ones already saved.

## `private LDraft? LDraftAuthorUpdate(LDraft? draft)`

The draft with each stored credit renamed as the database names it now.
A minted Author is not stored yet, and one deleted meanwhile keeps the name the draft holds.

## `private void LCourtRemove(long id)`

Drops every court row one draft owns, and the tentative target each row named when nothing else wants it.
A target another draft still links to stays, because that draft's chip would otherwise point at nothing.
A target goes only while this clerk holds it, so another window's editor is never emptied.
A target whose claim is gone was left by an earlier launch, and its own recovery will offer it back.
A row whose owner draft is gone is dropped as well, wherever the sweep meets one.
Cancelling and deleting share this, because both end a draft and both leave its links behind otherwise.
