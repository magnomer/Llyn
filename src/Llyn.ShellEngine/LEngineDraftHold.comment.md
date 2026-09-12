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
The sweep and the recovery live in `LEngineDraftLeftover.cs`.
The card order calls live in `LEngineDraftCard.cs`.
The court rows live in `LEngineCourt.cs`.
The field by field comparisons live in `LEngineDraftMatch.cs`.

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

## `public void LEngineDraftDelete(string id)`

Removes one held draft with its claim, after the links that draft owns are settled.
A dropped draft that kept its rows would leave links no call can reach again.
`LCourtArchiveSettle` matches on target alone, so a row whose owner is gone answers no settlement.
The rows pointing at this draft are left, because the caller dropping a chip already took its own row back.
Abandoning work is `LEngineDraftCancel`, which settles those rows too.

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

## `public LOutcome LEngineDraftCommit(long id)`

Turns held work into a stored entry and returns it with the identity map.
The map pairs every negative id the draft held with the row id the database gave it.
It is an output the caller may read, never something the engine reads back in.
Every tentative target this draft links to is committed first.
It opens the walk with nothing entered yet.
A chip naming a word that does not exist yet becomes a real entry that way.
The entry id such a target became is recorded in the map under the target's draft id.
The translation list is settled through the map, so the draft file is not read back for it.
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
An owner this same walk entered settles its own list through the map and is rewritten all the same.
A store that fails after the targets committed then leaves a file naming real entries, so a retry keeps the links.
An owner whose file has since gone is passed over rather than recreated.
The held file is deleted last, so a failure anywhere above leaves the work recoverable.
The id leaves the claimed set with the file, and the claim file goes too.
No launch counts the draft again.
The caller asking for this commit owns its own draft, so the walk opens holding it.

## `private LOutcome LEngineDraftCommit(long id, HashSet<long> entered, bool held)`

The same commit, carrying the drafts the walk has already entered and whether this frame holds its own draft.
Each frame builds its own map, so a frame reports only the ids its own draft held.
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

## `private static LEntryDraft LEngineDraftBlank`

What a draft carrying no entry is measured against.
A form that was never opened on an entry started from nothing.

## `private void LEngineDraftValidate(string id)`

Refuses an id raised in a workspace this engine no longer holds.
The folder such an id names is gone.
A read would answer null and a write would land in the wrong workspace.
A caller holding one is told so rather than being handed either silence.

## `private LDraft LEngineDraftLoad(string id)`

Reads a held draft that the caller is about to act on, refusing when it is gone.
Another window may have stored or discarded it already.

## `private LEntryDraft LEngineTranslationSettle(LEntryDraft content)`

The same content with every translation that names no stored entry removed.
Both card lists are settled, because a chip can sit on a meaning or on a collocation.

## `private IReadOnlyList<LCardDraft> LEngineTranslationSettle(IReadOnlyList<LCardDraft> cards)`

Keeps only the translations that still load as an entry.
An id that loads nothing points at a record that was discarded or never arrived.
There is nothing left to store it against.
