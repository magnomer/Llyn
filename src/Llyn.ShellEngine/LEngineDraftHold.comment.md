# LEngineDraftHold.cs

## `public sealed partial class LEngine`

The one gateway to tentative work, so the shell never opens the drafts folder itself.
The shell references `Llyn.Core` and `Llyn.ShellEngine` only, and the archives live below both.
Every call here reaches the workspace root the engine already holds, so a moved workspace moves the drafts with it.
Held work is one file per draft, which is why a crash costs at most the last keystrokes.
It stays apart from `LEngineDraft.cs`, which owns the database write and knows nothing of files.

## `public LDraft LEngineDraftStart(string origin, string? entryId)`

Mints an id, writes the first file, and returns the held draft.
With no entry the content is blank, which is a new word being typed.
With an entry the content is that entry loaded back into form shape, which is an edit.
An entry that is gone is refused before a file is written, so no draft can point at nothing.
`origin` records which surface opened the work, so a recovered draft can say where it came from.

## `public void LEngineDraftSave(LDraft draft)`

Overwrites that one file with the content given.
The write goes through a pending file and a move, so a reader never sees half a draft.
The moment is the caller's, because only the caller knows whether this write was a real edit.

## `public LDraft? LEngineDraftRead(string id)`

Reads one held draft, or null when its file is gone or unreadable.
A missing file is an ordinary answer here, unlike the calls that go on to act on the draft.

## `public IReadOnlyList<LDraft> LEngineDraftScan()`

Every held draft the folder still carries.
This is what a session offers back after a crash.
A broken file is skipped rather than thrown, so one bad draft never hides the rest.

## `public IReadOnlyList<LDraft> LEngineLeftoverRead()`

The held drafts this engine did not start and that still differ from the entry they opened from.
A draft this session started is claimed by an open window, so offering it back would fight the window still typing into it.
Two copies of the program may run on one workspace, so the claim is per engine and never guesses at the other copy.
A draft matching its origin carries nothing worth recovering, which is what an untouched panel leaves behind.
What remains is what a forced shutdown cost, counted at launch.

## `public void LEngineDraftDelete(string id)`

Removes one held draft and leaves the court alone.
Abandoning work is `LEngineDraftCancel`, which also settles the links.

## `public IReadOnlyList<LCardDraft> LEngineDraftMove(string id, bool collocation, int from, int target)`

Reorders one card inside the held content and hands the whole list back renumbered.
Positions are rewritten from `1` so they stay contiguous whatever the drag did.
This is the only place card order is computed, so the form no longer keeps its own count.
`collocation` picks which of the two lists is reordered.
Both ends are clamped into range, because a drag can land past the last card.
An empty list is written back untouched.

## `public LCourtLink LEngineCourtSave(string ownerId, string targetId, string headword, string language)`

Writes one court row: a link from a held draft to a target that is not an entry yet.
The headword and language travel with it, because the chip is shown long before the target is real.
Returns the row so the caller can drop it again by id.

## `public void LEngineCourtDelete(string linkId)`

Removes one court row, for a chip the user took back.

## `public LCourtLink? LEngineCourtFind(string ownerId, string targetId)`

The court row one draft holds against one target, or null when there is none.
A chip the user takes back is dropped by finding its row this way.
An ordinary translation to a stored entry answers null, because it never had a row.

## `public bool LEngineDraftCheck(string id)`

Whether held work differs from the entry it was started from.
This is the question the shell used to answer by holding a second copy of the form.
A draft carrying no entry id is measured against a blank entry.
So a new word counts as changed the moment anything is typed into it.
A draft whose entry has since gone is measured against blank too.
A missing draft file answers false, because there is nothing to lose.
Language counts, because the tongue an entry is filed under is part of the entry.
Cards holding nothing at all are left out too.
The form always offers one empty card, and an empty card is not work.

## `public LEntry LEngineDraftCommit(string id)`

Turns held work into a stored entry and returns it.
Every tentative target this draft links to is committed first.
It opens the walk with nothing entered yet.
A chip naming a word that does not exist yet becomes a real entry that way.
Settling those targets rewrites this draft's file, so it is read back afterwards.
A draft carrying no entry id is a create, one carrying an entry id is an update.
The database write runs first and whole.
A refusal from it therefore leaves the file on disk exactly as it was, so nothing typed is lost.
The draft file is rewritten with the stored entry id the moment the database write returns.
A kill between the two writes would otherwise leave the entry stored and the file still blank, and recommitting that file would store the word twice.
Naming what it already became turns the leftover into an update, and it also stops the draft reporting itself unsaved.
Only after the entry exists is the court settled and each owner draft rewritten.
Rewriting means the tentative id sitting in a translation list becomes the real entry id.
An owner whose file has since gone is passed over rather than recreated.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
The id leaves the claimed set with the file, so a later scan no longer counts it.

## `private LEntry LEngineDraftCommit(string id, HashSet<string> entered)`

The same commit, carrying the drafts the walk has already entered.
Two drafts naming each other would otherwise recurse until the stack died, which no catch can reach.
A target already in the set is passed over, since the walk is settling it further up.
The id is added before its targets are visited, so the draft cannot reach itself through them.

## `public void LEngineDraftCancel(string id)`

Discards held work: the court first, the file second.
Links go first because a link outliving its target would point at a record that will never arrive.
A tentative target goes too, but only when this draft is the last thing holding it.
A target another draft still links to stays, because that draft's chip would otherwise point at nothing.
A target another window opened stays as well, told apart by the origin its draft carries.
So discarding here never reaches work being edited elsewhere.

## `private static LEntryDraft LEngineDraftBlank`

What a draft carrying no entry is measured against.
A form that was never opened on an entry started from nothing.

## `private IReadOnlyList<LCourtLink> LEngineCourtScan(string ownerId)`

Every court row one draft holds, whatever it points at.
Committing and cancelling both act on the whole set.

## `private static bool LEngineDraftMatch(LEntryDraft one, LEntryDraft other)`

Field by field, whether two forms of an entry say the same thing.
Language sits beside the headword, because changing only the tongue is still an edit.
The lists inside are compared by their contents, since records compare them by reference.

## `private static bool LEngineCardMatch(IReadOnlyList<LCardDraft> one, IReadOnlyList<LCardDraft> other)`

Whether two card lists carry the same cards in the same order.
Order counts, because the order cards are in is the order they are stored in.
Position is compared with it, so a reorder is caught by what it changed rather than by chance.

## `private static IReadOnlyList<LCardDraft> LEngineCardScan(IReadOnlyList<LCardDraft> cards)`

The cards that carry something, in their original order.

## `private static bool LEngineCardCheck(LCardDraft card)`

Whether a card holds nothing at all.
An open form always shows one such card, and offering it is not an edit.

## `private static bool LEngineExampleMatch(IReadOnlyList<LExampleDraft> one, IReadOnlyList<LExampleDraft> other)`

Whether two example lists say the same thing in the same order.
The citation each names counts, so retagging a sentence is a change.

## `private static bool LEngineSituationMatch(IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)`

Whether two situation lists say the same thing in the same order.

## `private static bool LEngineValueMatch(IReadOnlyList<LStateValue> one, IReadOnlyList<LStateValue> other)`

Whether two value lists match, keeping an unreadable field apart from an empty one.

## `private static bool LEngineTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)`

Whether two text lists match exactly, order included.

## `private LDraft LEngineDraftLoad(string id)`

Reads a held draft that the caller is about to act on, refusing when it is gone.
Another window may have stored or discarded it already.

## `private void LEngineCourtUpdate(LCourtLink link, string realId)`

Rewrites the one draft that held a settled link.

## `private static IReadOnlyList<LCardDraft> LEngineTranslationUpdate(IReadOnlyList<LCardDraft> cards, string draftId, string realId)`

Swaps the tentative id for the real one wherever a card's translations name it.
Every other translation is copied through unchanged.
