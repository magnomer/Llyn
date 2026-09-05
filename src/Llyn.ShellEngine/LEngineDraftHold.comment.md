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
A claim naming this process is written beside the draft, so another launch reading the folder knows the work is live.

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

## `public void LEngineLeftoverSweep()`

Clears what the drafts folder keeps forever, before anything counts what is left.
A draft committed just before a kill kept its file, because the commit names the stored entry first.
Its content then matches that entry, so nothing offers it back and nothing deletes it either.
Such a draft is dropped here with its claim and its court rows.
A draft naming no entry, or an entry since deleted, is left for recovery to offer back.
So is one whose content differs from the entry it names, which is work the user would lose.
A draft this engine holds, or another running copy claims, is passed over untouched.
The half-written pending files both archives leave behind go too.
Only files older than an hour, so a save in flight in the other copy is never taken.
Nothing here throws on a folder that is missing or unreadable.

## `public IReadOnlyList<LDraft> LEngineLeftoverRead()`

The held drafts nothing is still working on and that differ from the entry they opened from.
A draft this engine started is claimed by an open window, so offering it back would fight the window still typing into it.
A draft another running copy of the program claims is passed over too, told apart by the claim file that copy wrote.
Two copies may run on one workspace, and an in-memory set cannot see across them.
A claim naming a process that is gone is what a crash leaves behind, and the draft under it is exactly what recovery is for.
A draft matching its origin carries nothing worth recovering, which is what an untouched panel leaves behind.
What remains is what a forced shutdown cost, counted at launch.

## `public void LEngineDraftDelete(string id)`

Removes one held draft with its claim, after the links that draft owns are settled.
A dropped draft that kept its rows would leave links no call can reach again.
`LCourtArchiveSettle` matches on target alone, so a row whose owner is gone answers no settlement.
The rows pointing at this draft are left, because the caller dropping a chip already took its own row back.
Abandoning work is `LEngineDraftCancel`, which settles those rows too.

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
An entry id naming a record since deleted is a create as well, because there is nothing left to update.
Such a draft is otherwise trapped: it reports itself unsaved forever and every commit is refused.
A translation naming no stored entry is struck from the content before it is sent.
Such an id is left by a target that was discarded, or by a draft an earlier launch wrote, and the database has no row for it to point at.
Sending it raises a foreign-key failure from the store rather than a refusal, which reaches the reader as a crash and not as an answer.
The database write runs first and whole.
A refusal from it therefore leaves the file on disk exactly as it was, so nothing typed is lost.
The draft file is rewritten with the stored entry id the moment the database write returns.
A kill between the two writes would otherwise leave the entry stored and the file still blank, and recommitting that file would store the word twice.
Naming what it already became turns the leftover into an update, and it also stops the draft reporting itself unsaved.
The stored entry is written back into the file as well, not the content that was sent.
The database mints its own card ids, so the content that went in never matches the entry that came out.
A leftover carrying the sent content therefore reads as changed forever: the sweep passes it over and every launch offers work that was already saved.
Reading the entry back makes the leftover match, which is what lets the sweep collect it.
Only after the entry exists is the court settled and each owner draft rewritten.
Rewriting means the tentative id sitting in a translation list becomes the real entry id.
An owner whose file has since gone is passed over rather than recreated.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
The id leaves the claimed set with the file and the claim file goes with it, so no launch counts the draft again.
The caller asking for this commit owns its own draft, so the walk opens holding it.

## `private LEntry LEngineDraftCommit(string id, HashSet<string> entered, bool held)`

The same commit, carrying the drafts the walk has already entered and whether this frame holds its own draft.
Two drafts naming each other would otherwise recurse until the stack died, which no catch can reach.
A target already in the set is passed over, since the walk is settling it further up.
The id is added before its targets are visited, so the draft cannot reach itself through them.
A target is held only when a live claim names this process and this engine started it.
A target claimed by another process, by an editor this engine never started, or by nothing at all is entered without being held.
Nothing at all counts as another's, because a file left with no claim is more likely an earlier launch's work than this frame's to delete.
A frame that does not hold its draft stores the entry, names it in the file, and settles the court as any other does.
It leaves the file, the claim, and the claimed set alone, so the editor still typing into that draft keeps it.
The content it leaves is that draft's own words as the database now holds them, which is what the editor sent a moment earlier.
Taking the file away would leave that editor saving into nothing, and every later keystroke would be dropped in silence.
The draft it keeps names a stored entry, so the editor's own commit updates that entry rather than storing the word twice.

## `public void LEngineDraftCancel(string id)`

Discards held work: the court first, the file second.
Links go first because a link outliving its target would point at a record that will never arrive.
The rows this draft owns are settled by `LEngineCourtRemove`, and the rows pointing at it are settled here.
Each row pointing at it also has its tentative id struck from the draft that held it.
That id will never become an entry now, so a chip left carrying it would be stored as a translation of a record that never existed.

## `private void LEngineCourtRemove(string id)`

Drops every court row one draft owns, and the tentative target each row named when nothing else wants it.
A tentative target goes only when this draft is the last thing holding it.
A target another draft still links to stays, because that draft's chip would otherwise point at nothing.
A target goes only while this engine owns it: a live claim naming this process, for a draft this engine started.
A target another window or another copy of the program is editing therefore stays, because taking it would empty an open editor.
A target whose claim is gone was left by an earlier launch, and sweeping it here would delete work its own recovery is about to offer back.
The panel a draft names cannot answer that: it is a kind of surface, not a window, and both copies of the program name the same ones.
The target's claim is dropped with its file, so nothing outlives the draft it named.
A row whose owner draft is gone is dropped as well, wherever the sweep meets one.
Such a row can no longer be reached by owner or by target, so nothing else would ever collect it.
Cancelling and deleting share this, because both end a draft and both leave its links behind otherwise.

## `private bool LEngineHoldCheck(string id)`

Whether this engine may take a draft away.
It may only when a live claim names this very process and its own set holds the id, which together mean this engine started the draft and still has it.
A claim naming another process, a draft another editor in this process started, and a draft with no claim at all each answer no.
Commit and cancel both ask this before deleting a file, so neither can empty an editor it does not own.

## `private bool LEngineClaimCheck(string id)`

Whether a copy of the program other than this one is still working on a draft.
A live claim naming this very process is one this engine already knows about through its own set.
So only another process's claim answers true, which is the case an in-memory set can say nothing about.
A stale claim is swept by the check itself and answers false.

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

## `private LEntryDraft LEngineTranslationSettle(LEntryDraft content)`

The same content with every translation that names no stored entry removed.
Both card lists are settled, because a chip can sit on a meaning or on a collocation.

## `private IReadOnlyList<LCardDraft> LEngineTranslationSettle(IReadOnlyList<LCardDraft> cards)`

Keeps only the translations that still load as an entry.
An id that loads nothing points at a record that was discarded or never arrived, so there is nothing left to store it against.

## `private static IReadOnlyList<LCardDraft> LEngineTranslationUpdate(IReadOnlyList<LCardDraft> cards, string draftId, string realId)`

Swaps the tentative id for the real one wherever a card's translations name it.
An empty real id drops the tentative one instead, which is how a cancelled target leaves the drafts that pointed at it.
Every other translation is copied through unchanged.
