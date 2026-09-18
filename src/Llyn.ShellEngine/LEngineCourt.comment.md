# LEngineCourt.cs

## `public sealed partial class LEngine`

The rows linking a held draft to a target that is not an entry yet.
A chip naming a word the user has not stored is such a target.
The row outlives neither end: committing settles it and cancelling drops it.
The draft calls that open and end that work live in `LEngineDraftHold.cs`.

## `internal LCourt LEngineCourtSave(long ownerId, long targetId, string headword, string language)`

Writes one court row: a link from a held draft to a target that is not an entry yet.
The headword and language travel with it, because the chip is shown long before the target is real.
Returns the row so the caller can drop it again by id.

## `public LCourt LEngineCourtStart(long ownerId, string origin, string headword, string language)`

Starts a tentative target and records the row naming it, as one call.
The word and the language are written straight into the target's file before the row is made.
The target is not yet anybody's to request against, so the archive is written rather than a request applied.
A caller doing this in three calls could fail on the last and leave a draft nothing points at.
Such a draft is invisible: no chip names it, and no commit or cancel ever reaches it.
A failure anywhere after the target is started cancels it before the refusal leaves.
So the workspace holds either the target with its row or neither.

## `public void LEngineCourtDelete(long linkId)`

Removes one court row, for a chip the user took back.

## `public LCourt? LEngineCourtFind(long ownerId, long targetId)`

The court row one draft holds against one target, or null when there is none.
A chip the user takes back is dropped by finding its row this way.
An ordinary translation to a stored entry answers null, because it never had a row.

## `private void LEngineCourtRemove(long id)`

Drops every court row one draft owns, and the tentative target each row named when nothing else wants it.
A tentative target goes only when this draft is the last thing holding it.
A target another draft still links to stays, because that draft's chip would otherwise point at nothing.
A target goes only while this engine owns it.
Ownership means a live claim naming this process for a draft this engine started.
A target another window or another copy of the program is editing therefore stays.
Taking it would empty an open editor.
A target whose claim is gone was left by an earlier launch.
Sweeping it here would delete work its own recovery is about to offer back.
The panel a draft names cannot answer that.
A panel is a kind of surface, not a window.
Both copies of the program name the same ones.
The target's claim is dropped with its file, so nothing outlives the draft it named.
A row whose owner draft is gone is dropped as well, wherever the sweep meets one.
Such a row can no longer be reached by owner or by target, so nothing else would ever collect it.
Cancelling and deleting share this, because both end a draft and both leave its links behind otherwise.

## `private IReadOnlyList<LCourt> LEngineCourtScan(long ownerId)`

Every court row one draft holds, whatever it points at.
Committing and cancelling both act on the whole set.

## `private void LEngineCourtUpdate(LCourt link, long realId)`

Rewrites the one draft that held a settled link.
The owner's chronicle is dropped first, because a snapshot from before the rewrite would bring the draft id back.

## `private static IReadOnlyList<LCardDraft> LEngineTranslationUpdate(IReadOnlyList<LCardDraft> cards, long draftId, long realId)`

Swaps the tentative id for the real one wherever a card's translations name it.
An empty real id drops the tentative one instead.
That is how a cancelled target leaves the drafts that pointed at it.
Every other translation is copied through unchanged.
