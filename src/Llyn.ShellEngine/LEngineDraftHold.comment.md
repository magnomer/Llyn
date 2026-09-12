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
Start and commit differ per kind.
Every edit in between is a request, applied in `LEngineRequest.cs` for all kinds alike.
The claim, the sweep, the recovery and the discard are one for all kinds.
The sweep and the recovery live in `LEngineDraftLeftover.cs`.
The card order calls live in `LEngineDraftCard.cs`.
The one-edit requests the form sends live in `LEngineRequest.cs`.
The court rows live in `LEngineCourt.cs`.
The field by field comparisons live in `LEngineDraftMatch.cs`.

## `public LDraft LEngineDraftStart(string origin, long? entryId)`

Mints an id, writes the first file, and returns the held draft.
With no entry the content is blank, which is a new word being typed.
With an entry the content is that entry loaded back into form shape, which is an edit.
An entry that is gone is refused before a file is written, so no draft can point at nothing.
`origin` records which surface opened the work, so a recovered draft can say where it came from.
A claim naming this process is written beside the draft.
Another launch reading the folder then knows the work is live.

## `public LDraft? LEngineDraftRead(long id)`

Reads one held draft, or null when its file is gone or unreadable.
A missing file is an ordinary answer here, unlike the calls that go on to act on the draft.
An id from a closed workspace is not a missing file and is refused rather than answered null.

## `public IReadOnlyList<LDraft> LEngineDraftScan()`

Every held draft the folder still carries.
This is what a session offers back after a crash.
A broken file is skipped rather than thrown, so one bad draft never hides the rest.

## `public void LEngineDraftDelete(long id)`

Removes one held draft with its claim, after the links that draft owns are settled.
A dropped draft that kept its rows would leave links no call can reach again.
`LCourtArchiveSettle` matches on target alone, so a row whose owner is gone answers no settlement.
The rows pointing at this draft are left, because the caller dropping a chip already took its own row back.
Abandoning work is `LEngineDraftCancel`, which settles those rows too.

## `public bool LEngineDraftCheck(long id)`

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
The whole round, every target and the draft itself, runs inside one database session.
A refusal anywhere in it rolls every entry of the round back, so a half-committed round never stands.
The file side is settled only after that session commits.
A held draft file and its claim go then, never before, so a rolled-back round leaves every file for a retry.
A refusal from the store therefore leaves the file on disk exactly as it was, so nothing typed is lost.
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
An owner whose file has since gone is passed over rather than recreated.
Two drafts naming each other cannot both settle through the map, because one commits before the other has an id.
Such a link is held back and written once the round is through, when both ids are known.
Neither direction of the pair is lost, and nothing is silently dropped.
The id leaves the claimed set with the file, and the claim file goes too.
No launch counts the draft again.
Every entry the round stored is announced, not only the one the caller asked for.
The caller asking for this commit owns its own draft, so the walk opens holding it.

## `private LOutcome LEngineDraftCommit(long id, bool held, Dictionary<long, LDraft> loaded, Dictionary<long, LOutcome> settled, List<LCourt> deferred, List<long> finished)`

The same commit, carrying what the walk has already loaded and settled, whether this frame holds its own draft, the links held back and the drafts to finish.
Each frame builds its own map, so a frame reports only the ids its own draft held.
Two drafts naming each other would otherwise recurse until the stack died, which no catch can reach.
A target already loaded is not entered again, since the walk is settling it further up.
When that target has already settled, its entry id goes into this frame's map like any other.
When it has not, the link is held back for the pass that runs once the round is through.
The draft is loaded before its targets are visited, so it cannot reach itself through them.
A frame that holds its draft names it as finished rather than deleting its file here.
The files go only after the whole round's session has committed.
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

## `public void LEngineDraftCancel(long id)`

Discards held work: the court first, the file second.
It also drops what this draft's lookups and audio searches found, because closing is what frees a trove.
Links go first because a link outliving its target would point at a record that will never arrive.
The rows this draft owns are settled by `LEngineCourtRemove`, and the rows pointing at it are settled here.
Each row pointing at it also has its tentative id struck from the draft that held it.
That id will never become an entry now.
A chip left carrying it would be stored as a translation of a record that never existed.

## `private bool LEngineHoldCheck(long id)`

Whether this engine may take a draft away.
It may only when a live claim names this process and its own set holds the id.
Those two together mean this engine started the draft and still has it.
A claim naming another process answers no.
So does a draft another editor in this process started, or one with no claim.
Commit and cancel both ask this before deleting a file, so neither can empty an editor it does not own.

## `private bool LEngineClaimCheck(long id)`

Whether a copy of the program other than this one is still working on a draft.
A live claim naming this very process is one this engine already knows about through its own set.
So only another process's claim answers true, which is the case an in-memory set can say nothing about.
A stale claim is swept by the check itself and answers false.

## `private static LEntryDraft LEngineDraftBlank`

What a draft carrying no entry is measured against.
A form that was never opened on an entry started from nothing.

## `private void LEngineDraftValidate(long id)`

Refuses an id raised in a workspace this engine no longer holds.
The folder such an id names is gone.
A read would answer null and a write would land in the wrong workspace.
A caller holding one is told so rather than being handed either silence.

## `private LDraft LEngineDraftLoad(long id)`

Reads a held draft that the caller is about to act on, refusing when it is gone.
Another window may have stored or discarded it already.

## `private void LEngineCourtApply(IReadOnlyDictionary<long, LDraft> loaded, IReadOnlyDictionary<long, LOutcome> settled, IReadOnlyList<LCourt> deferred)`

Writes every link the walk held back, now that every draft of the round has an entry id.
The owner's content as it was loaded says which cards carried the target's draft id.
The owner's map says which stored card each of those became.
A link whose owner or target did not settle is passed over, because there is nothing to write it against.

## `private void LEngineCourtApply(IReadOnlyList<LCardDraft> cards, LOutcome made, long draftId, long entryId, bool collocation)`

One card list of the owner walked for the target's draft id, children included.
A card that carried it has the target's entry id appended to its stored translations.

## `private static bool LEngineTranslationCheck(IReadOnlyList<long> translations, long id)`

Whether one card's translation list carries `id`.

## `private void LEngineTranslationAppend(long ownerId, long entryId, bool collocation)`

Adds `entryId` to the end of one stored card's translations, unless the card already links it.

## `private LEntryDraft LEngineTranslationSettle(LEntryDraft content)`

The same content with every translation that names no stored entry removed.
Both card lists are settled, because a chip can sit on a meaning or on a collocation.

## `private IReadOnlyList<LCardDraft> LEngineTranslationSettle(IReadOnlyList<LCardDraft> cards)`

Keeps only the translations that still read as a stored entry.
An id that reads nothing points at a record that was discarded, never arrived, or is still being committed in this round.
There is nothing to store it against yet, and the round's last pass writes the ones that become real.
The check is a single row read, not a load of the whole entry tree.
