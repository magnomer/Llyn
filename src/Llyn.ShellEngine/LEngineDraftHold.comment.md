# LEngineDraftHold.cs

## `public sealed partial class LEngine`

The one gateway to tentative work, so the shell never opens the drafts folder itself.
The shell references `Llyn.Core` and `Llyn.ShellEngine` only, and the archives live below both.
Every call here reaches the workspace root the engine already holds, so a moved workspace moves the drafts with it.
Held work is one file per draft, which is why a crash costs at most the last keystrokes.
It stays apart from `LEngineDraft.cs`, which owns the database write and knows nothing of files.
An id raised here belongs to the workspace that raised it, so a workspace change marks every held id stale.
Every call naming an id is checked against that mark.
A shell can keep an id the engine has already left behind.

A held draft carries an entry, a sentence, a situation or a source.
The kind is a further content field rather than a tag beside one.
`LDraftExample` carries the sentence, `LDraftSituation` the context and `LDraftReference` the source.
Each is null for the kinds that are not its own.
A tag would be a second statement of the same fact, and two statements can disagree after a bad write.
The alternative was one content field of a union type.
That would have rewritten every entry call for a case none of them has.
The source followed this rule rather than inventing a second one.
Each kind gets one nullable content field, null for every other kind.
The sentence calls live in `LEngineDraftExample.cs`, the situation calls in `LEngineDraftSituation.cs` and the source calls in `LEngineDraftReference.cs`.
Only the shared calls belong here.
Start, save and commit differ per kind.
The claim, the sweep, the recovery and the discard are one for all kinds.

## `public LDraft LEngineDraftStart(string origin, string? entryId)`

Mints an id, writes the first file, and returns the held draft.
With no entry the content is blank, which is a new word being typed.
With an entry the content is that entry loaded back into form shape, which is an edit.
An entry that is gone is refused before a file is written, so no draft can point at nothing.
`origin` records which surface opened the work, so a recovered draft can say where it came from.
A claim naming this process is written beside the draft.
Another launch reading the folder then knows the work is live.

## `public LEntryDraft LEngineDraftSave(LDraft draft)`

Overwrites that one file with the content given, and hands back the content it stored.
The write goes through a pending file and a move, so a reader never sees half a draft.
The moment is the caller's, because only the caller knows whether this write was a real edit.
What is stored is not always what was sent, so the caller renders the answer rather than its own copy.
A card arriving without an id is named here, which is what a draft written by an older launch carries.
The content that came in is handed straight back when nothing needed naming.
So the caller can tell a settled write from a corrected one without comparing field by field.

## `public LDraft? LEngineDraftRead(string id)`

Reads one held draft, or null when its file is gone or unreadable.
A missing file is an ordinary answer here, unlike the calls that go on to act on the draft.
An id from a closed workspace is not a missing file and is refused rather than answered null.

## `public IReadOnlyList<LDraft> LEngineDraftScan()`

Every held draft the folder still carries.
This is what a session offers back after a crash.
A broken file is skipped rather than thrown, so one bad draft never hides the rest.

## `public void LEngineLeftoverSweep()`

Clears what the drafts folder keeps forever, before anything counts what is left.
A draft committed just before a kill kept its file, because the commit names the stored entry first.
Its content then matches that entry, so nothing offers it back and nothing deletes it either.
Such a draft is dropped here with its claim and its court rows.
A sentence draft is measured the same way, against the Example it names rather than the entry.
A situation draft is measured against the Situation it names.
A source draft is measured against the Reference it names.
A draft naming no entry, or an entry since deleted, is left for recovery to offer back.
So is one whose content differs from the entry it names, which is work the user would lose.
A draft this engine holds, or another running copy claims, is passed over untouched.
The half-written pending files both archives leave behind go too.
Only files older than an hour, so a save in flight in the other copy is never taken.
Nothing here throws on a folder that is missing or unreadable.

## `public IReadOnlyList<LDraft> LEngineLeftoverRead()`

The held drafts nothing is still working on and that differ from the entry they opened from.
A draft this engine started is claimed by an open window.
Offering it back would fight the window still typing into it.
A draft another running copy of the program claims is passed over too.
The claim file that copy wrote tells it apart.
Two copies may run on one workspace, and an in-memory set cannot see across them.
A claim naming a process that is gone is what a crash leaves behind.
The draft under it is exactly what recovery is for.
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

## `public IReadOnlyList<LCardDraft> LEngineDraftNormalize(string id, bool collocation)`

Renumbers one card list without moving anything, and hands the whole list back.
A removed card leaves a gap in the numbering that nothing else closes.
Asking for a move of nothing said the same thing by accident, and read as a reorder that never happened.

## `public string LEngineCardCreate()`

Mints one card id for a card the form has just added.
The form cannot mint one itself, because identity is the workspace's to give.
An id given at the moment a card appears is what lets a later answer name that card back.

## `private IReadOnlyList<LCardDraft> LEngineCardApply(LDraft draft, bool collocation, List<LCardDraft> cards)`

Writes one card list onto a held draft, numbered from `1` and named throughout.
Positions are rewritten so they stay contiguous whatever the caller did to the list.
This is the only place card order is computed, so the form no longer keeps its own count.
A card still carrying no id is named here too.
The answer is about to be matched by id.
Both the move and the renumber end here, so the two cannot drift apart.

## `public LCourtLink LEngineCourtSave(string ownerId, string targetId, string headword, string language)`

Writes one court row: a link from a held draft to a target that is not an entry yet.
The headword and language travel with it, because the chip is shown long before the target is real.
Returns the row so the caller can drop it again by id.

## `public LCourtLink LEngineCourtStart(string ownerId, string origin, string headword, string language)`

Starts a tentative target and records the row naming it, as one call.
The word and the language are written into the target before the row is made.
A caller doing this in three calls could fail on the last and leave a draft nothing points at.
Such a draft is invisible: no chip names it, and no commit or cancel ever reaches it.
A failure anywhere after the target is started cancels it before the refusal leaves.
So the workspace holds either the target with its row or neither.

## `public void LEngineCourtDelete(string linkId)`

Removes one court row, for a chip the user took back.

## `public LCourtLink? LEngineCourtFind(string ownerId, string targetId)`

The court row one draft holds against one target, or null when there is none.
A chip the user takes back is dropped by finding its row this way.
An ordinary translation to a stored entry answers null, because it never had a row.

## `public bool LEngineDraftCheck(string id)`

Whether held work differs from the record it was started from.
This is the question the shell used to answer by holding a second copy of the form.
A sentence draft is answered by `LEngineExampleCheck`, which measures it against the Example it names.
A situation draft is answered by `LEngineSituationCheck` the same way.
A source draft is answered by `LEngineReferenceCheck`, which measures it against the Reference it names.
A draft carrying no entry id is measured against a blank entry.
So a new word counts as changed the moment anything is typed into it.
A draft whose entry has since gone is measured against blank too.
A missing draft file answers false, because there is nothing to lose.
Language counts against a standing entry, because the tongue it is filed under is part of it.
A new word carries the tongue the shell already chose, so that alone leaves the draft unchanged.
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
Such an id is left by a discarded target or by a draft an earlier launch wrote.
The database has no row for it to point at.
Sending it raises a foreign-key failure from the store rather than a refusal.
That reaches the reader as a crash and not as an answer.
The database write runs first and whole.
A refusal from it therefore leaves the file on disk exactly as it was, so nothing typed is lost.
The draft file is rewritten with the stored entry id the moment the database write returns.
A kill between the two writes would otherwise leave the entry stored and the file blank.
Recommitting that file would store the word twice.
Naming what it already became turns the leftover into an update, and it also stops the draft reporting itself unsaved.
The stored entry is written back into the file as well, not the content that was sent.
The database mints its own card ids, so the content that went in never matches the entry that came out.
A leftover carrying the sent content therefore reads as changed forever.
The sweep passes it over and every launch offers work that was already saved.
Reading the entry back makes the leftover match, which is what lets the sweep collect it.
Only after the entry exists is the court settled and each owner draft rewritten.
Rewriting means the tentative id sitting in a translation list becomes the real entry id.
An owner whose file has since gone is passed over rather than recreated.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
The id leaves the claimed set with the file, and the claim file goes too.
No launch counts the draft again.
The caller asking for this commit owns its own draft, so the walk opens holding it.

## `private LEntry LEngineDraftCommit(string id, HashSet<string> entered, bool held)`

The same commit, carrying the drafts the walk has already entered and whether this frame holds its own draft.
Two drafts naming each other would otherwise recurse until the stack died, which no catch can reach.
A target already in the set is passed over, since the walk is settling it further up.
The id is added before its targets are visited, so the draft cannot reach itself through them.
A target is held only when a live claim names this process and this engine started it.
A target claimed by another process or by an editor this engine never started is entered without being held.
A target claimed by nothing at all is entered the same way.
Nothing at all counts as another's.
A file left with no claim is more likely an earlier launch's work than this frame's.
A frame that does not hold its draft stores the entry and names it in the file.
It settles the court as any other frame does.
It leaves the file, the claim and the claimed set alone.
The editor still typing into that draft keeps it.
The content it leaves is that draft's own words as the database now holds them.
That is what the editor sent a moment earlier.
Taking the file away would leave that editor saving into nothing, and every later keystroke would be dropped in silence.
The draft it keeps names a stored entry.
The editor's own commit updates that entry rather than storing the word twice.

## `public void LEngineDraftCancel(string id)`

Discards held work: the court first, the file second.
It also drops what this draft's lookups and audio searches found, because closing is what frees a trove.
Links go first because a link outliving its target would point at a record that will never arrive.
The rows this draft owns are settled by `LEngineCourtRemove`, and the rows pointing at it are settled here.
Each row pointing at it also has its tentative id struck from the draft that held it.
That id will never become an entry now.
A chip left carrying it would be stored as a translation of a record that never existed.

## `private void LEngineCourtRemove(string id)`

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

## `private bool LEngineHoldCheck(string id)`

Whether this engine may take a draft away.
It may only when a live claim names this process and its own set holds the id.
Those two together mean this engine started the draft and still has it.
A claim naming another process answers no.
So does a draft another editor in this process started, or one with no claim.
Commit and cancel both ask this before deleting a file, so neither can empty an editor it does not own.

## `private bool LEngineClaimCheck(string id)`

Whether a copy of the program other than this one is still working on a draft.
A live claim naming this very process is one this engine already knows about through its own set.
So only another process's claim answers true, which is the case an in-memory set can say nothing about.
A stale claim is swept by the check itself and answers false.

## `private static LEntryDraft LEngineDraftNormalize(LEntryDraft content)`

The same content with every card named.
The content that came in is returned itself when both lists were already named.
That sameness is the answer the form reads to know nothing was corrected.

## `private static IReadOnlyList<LCardDraft> LEngineCardNormalize(IReadOnlyList<LCardDraft> cards)`

The same cards with an id minted for each one carrying none.
The list that came in is returned itself when every card was already named.
A copy is taken only from the first unnamed card, because a settled list is the ordinary case.

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

## `private static bool LEngineSentenceMatch(IReadOnlyList<LSentenceDraft> one, IReadOnlyList<LSentenceDraft> other)`

Whether two row lists say the same thing in the same order.
The frame counts, so writing a marker or a role and nothing else is a change and is saved.
The stored row each names counts too, because a row that changed id is a different row.

## `private static bool LEngineExampleMatch(LExampleDraft? one, LExampleDraft? other)`

Whether two rows quote the same Example on the same terms.
A row quoting none matches only another quoting none.
The citation counts, so retagging a sentence is a change.

## `private static bool LEngineSituationMatch(IReadOnlyList<LSituationDraft> one, IReadOnlyList<LSituationDraft> other)`

Whether two situation lists say the same thing in the same order.
All three stored fields count, not the title the card happens to show.

## `private static bool LEngineValueMatch(IReadOnlyList<LStateValue> one, IReadOnlyList<LStateValue> other)`

Whether two value lists match, keeping an unreadable field apart from an empty one.

## `private static bool LEngineTextMatch(IReadOnlyList<string> one, IReadOnlyList<string> other)`

Whether two text lists match exactly, order included.

## `private void LEngineDraftValidate(string id)`

Refuses an id raised in a workspace this engine no longer holds.
The folder such an id names is gone.
A read would answer null and a write would land in the wrong workspace.
A caller holding one is told so rather than being handed either silence.

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
An id that loads nothing points at a record that was discarded or never arrived.
There is nothing left to store it against.

## `private static IReadOnlyList<LCardDraft> LEngineTranslationUpdate(IReadOnlyList<LCardDraft> cards, string draftId, string realId)`

Swaps the tentative id for the real one wherever a card's translations name it.
An empty real id drops the tentative one instead.
That is how a cancelled target leaves the drafts that pointed at it.
Every other translation is copied through unchanged.
