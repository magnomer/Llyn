# LCourtClerk.cs

## `public sealed class LCourtClerk`

The rows linking a held draft to a target that is not an entry yet.
A chip naming a word the user has not stored is such a target.
The row outlives neither end: committing settles it and cancelling drops it.
The translation side of a commit lives here too, since it is the court that decides what a chip becomes.
The settle pass maps each tentative id through the round's identity map and strikes what still names nothing.
The court pass runs once the round is through and writes the links the walk had to hold back.
The draft calls that open and end that work live in `LClaimClerk`.

## `public LCourtClerk(LRig rig, LIdentity identity, LChronicleClerk chronicle, LTranslationClerk translations)`

Reads the court, draft and entry ports out of `rig`.
The issuer names a new row and the chronicle is dropped when an owner is rewritten.
The translation clerk appends the settled links.

## `public LCourt LCourtClerkSave(long ownerId, long targetId, string headword, string language)`

Writes one court row: a link from a held draft to a target that is not an entry yet.
The headword and language travel with it, because the chip is shown long before the target is real.
Returns the row so the caller can drop it again by id.

## `public IReadOnlyList<LCourt> LCourtClerkScan()`

Every court row the workspace holds.

## `public IReadOnlyList<LCourt> LCourtClerkScan(long ownerId)`

Every court row one draft holds, whatever it points at.
Committing and cancelling both act on the whole set.

## `public LCourt? LCourtClerkFind(long ownerId, long targetId)`

The court row one draft holds against one target, or null when there is none.
An ordinary translation to a stored entry answers null, because it never had a row.

## `public void LCourtClerkDelete(long linkId)`

Removes one court row, for a chip the user took back.

## `public void LCourtClerkSettle(long draftId, long realId)`

Settles every row pointing at `draftId` and rewrites the draft that held each one.
A real id of zero strikes the tentative id instead, which is how a cancelled target leaves its owners.

## `public void LCourtClerkSweep()`

Drops the rows whose file is of another version or half written.

## `public void LCourtClerkApply(IReadOnlyDictionary<long, LDraft> loaded, IReadOnlyDictionary<long, LOutcome> settled, IReadOnlyList<LCourt> deferred)`

Writes every link the walk held back, now that every draft of the round has an entry id.
The owner's content as it was loaded says which cards carried the target's draft id.
The owner's map says which stored card each of those became.
A link whose owner or target did not settle is passed over, because there is nothing to write it against.

## `public LEntryDraft LTranslationSettle(LEntryDraft content, IReadOnlyDictionary<long, long> identity)`

The same content with every translation that names no stored entry removed.
Both card lists are settled, because a chip can sit on a meaning or on a collocation.

## `private IReadOnlyList<LCardDraft> LTranslationSettle(IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<long, long> identity)`

Keeps only the translations that still read as a stored entry.
An id that reads nothing points at a record that was discarded or never arrived.
It may also be one still being committed in this round.
There is nothing to store it against yet, and the round's last pass writes the ones that become real.
The check is a single row read, not a load of the whole entry tree.

## `private void LCourtClerkApply(IReadOnlyList<LCardDraft> cards, LOutcome made, long draftId, long entryId, bool collocation)`

One card list of the owner walked for the target's draft id, children included.
A card that carried it has the target's entry id appended to its stored translations.

## `private static bool LTranslationCheck(IReadOnlyList<long> translations, long id)`

Whether one card's translation list carries `id`.

## `private void LTranslationAppend(long ownerId, long entryId, bool collocation)`

Adds `entryId` to the end of one stored card's translations, unless the card already links it.

## `private void LCourtClerkUpdate(LCourt link, long realId)`

Rewrites the one draft that held a settled link.
The owner's chronicle is dropped first, because a snapshot from before the rewrite would bring the draft id back.

## `private static IReadOnlyList<LCardDraft> LTranslationUpdate(IReadOnlyList<LCardDraft> cards, long draftId, long realId)`

Swaps the tentative id for the real one wherever a card's translations name it.
An empty real id drops the tentative one instead.
Every other translation is copied through unchanged.
