# Tenure

This folder holds the tests of a held draft from start to commit or cancel.
They cover commit, cancel, refused rounds, chronicle, language, observers, the request queue, stale drafts and the status bar count.

## `TChronicle.cs`

Covers the chronicle of a held draft, the snapshots undo steps back to and redo steps forward to.
What matters is which draft is on disk after each step, and when the chronicle is empty.
Undo puts the earlier draft on disk and moves the edit to the future.
Redo puts it back, and an edit made after an undo forgets what the undo left ahead.
A request that changes nothing leaves no snapshot behind.
Choosing a language and adding empty cards or sentences are scaffolding, so they leave no undo step.
Two edits of the same type within the merge window are one step, and the test holds the clock.
A different type, or an edit right after an undo and redo, starts its own step.
A card move is an edit, so undo restores the order and the held cards keep their ids.
Committing a draft drops its chronicle.
Both steps answer null on a draft nothing has edited.
Past the cap of a hundred snapshots the oldest is forgotten, so a hundred undos stop at the first edit.

## `TDraft.cs`

Covers what the engine does with a held draft when committing and checking for change.
The court is exercised through those calls, never on its own.
A commit that reaches the database leaves the drafts folder empty and the entry stored.
Keeping the file would offer the same work back as unfinished.
A commit the database refuses leaves the file where it was and writes no entry.
A blank headword is refused, and that is the moment the user most needs the typed work to survive.
A fresh draft already carries the first language the engine lists, and that counts as no change.
So a form opened and closed untouched stores nothing.
A fresh draft holding only the tongue the shell chose is unchanged, because the editor writes it on its own.
The change check must compare every field, since a skipped field is one the user cannot save.
A video added to a card, or a changed language on an opened entry, reads as changed.
Two drafts naming each other as court targets commit without looping, each stored once with no file left.
A draft naming an existing entry commits as an update.
So a kill between write and delete cannot store a copy.
A draft whose entry was deleted meanwhile commits as a new entry, since refusing would trap the work.
Two drafts translating each other both keep their link.
The second commits inside the first, before the first has an id.
So the engine holds that link back and writes it once every entry in the round has its id.
Losing it silently was the old behaviour.
A target another editor or engine holds is stored, but its file stays and names the stored entry.
Deleting it would pull the file from under the typing panel and lose every later keystroke.
The holder's own commit then updates that entry, and the chip lands on a real id.
The leftover list ends empty, so nothing is offered back as unfinished.
A translation naming no entry is dropped at commit, since failing would refuse the whole commit over one stale link.

## `TDraftCancelling.cs`

Covers ending a held draft without storing it, by cancel or by delete.
What matters is what the court and the other drafts are left holding.
Cancelling a draft another draft still names as a target leaves that target in place.
The link is the other draft's, so only that draft's end may settle it.
Cancelling a target strikes its throwaway id from the draft that named it and drops the court row.
An id left behind would commit as a link to nothing.
Cancelling the owner of a link leaves a target another engine holds, since that is someone else's open work.
Deleting a held draft takes its court rows with it, and any tentative target nothing else wants.
A row left behind names an owner no call can reach, so nothing would collect it.
A row whose owner is already gone is collected by the next draft that ends.
An earlier launch stranded such rows, and neither resolving nor cancelling finds them by target alone.

## `TDraftCommitting.cs`

Covers what a refused commit round leaves behind, and what a round that goes through strips or skips.
It also covers requests a browsing panel sends without knowing which card the form made.
An owner with no headword is refused after its target was written inside the round.
The rollback takes the target's entry away, so the target's file must still name no entry.
A file naming a rolled-back id would leave its editor reading favorites and grasp off a ghost.
The court row survives too, so the retry settles the same link.
Padding around a headword is stripped at commit, so the stored row never carries it.
A trailing headword space and Windows line endings in the note are what typing produces.
So the change check must not call them a change.
A draft with a note and no word is changed yet cannot be saved.
The check names the refusal so the form enables Discard and not Save.
An author draft with a blank or space-only name names the blank-name refusal until a name is typed.
Saving an entry back exactly as loaded records no revision.
An empty revision would move the workspace pointer and wake every panel for nothing.
Card zero means the first Meaning card, and sentence zero the first sentence row of that card.
The engine makes them when the draft has none.
A situation pick naming card zero by the collocation kind still lands on the first Meaning card.
It adds no collocation, since the kind cannot send a pick to a card the draft lacks.
A stored sentence is history like a stored entry, so the round records a revision naming it.

## `TEstablishment.cs`

Covers the one read the status bar makes and the bulletin that keeps it current.
A fresh workspace counts no unsaved draft and no entry.
Its database file has some size, because the schema alone occupies pages.
A draft with a headword counts as unsaved, and a blank draft does not, though it is still held.
Committing moves the draft from the unsaved count to the entry count.
The size counts in megabytes from one, and in kilobytes rounded up below that.
So a file of one byte counts as one kilobyte, never zero.
Cancelling raises one draft bulletin carrying id zero.
The bar listens for it, so a discard without it would leave the bar saying unsaved.

## `TTenureLanguage.cs`

Language checks use the tenure's held draft.
After cancellation, the tenure returns empty defaults instead of retaining the prior language state.
Checking an independently empty draft also remains false.
The scheme read marks a scheme another row holds, and an ended tenure reads no schemes.
A second row of the same scheme would be refused, so the picker must show it as unavailable.

## `TTenureObserver.cs`

Tenure subscriptions filter bulletins by subject and the relevant identity.
Preparation suppresses notices during nested or failing changes.
Cancellation detaches subscriptions.
Applying during preparation returns the updated draft and suppresses its immediate notice.
After preparation, a matching bulletin is delivered.
Subject subscriptions accept the configured global settings notice.
Draft subscriptions require the tenure's draft ID.
Unrelated subjects and IDs are ignored.
An Entry subscription matches the stored Entry ID rather than the separate tenure draft ID.
A stored draft answers its Entry ID, and a deferred headword stays unapplied, so the read persists nothing.
A workspace switch leaves the held draft stale, so its stored-id read answers null instead of throwing.
A halted tenure whose discard fails still raises its state.
The failure here is a file standing where the court folder belongs.
It reaches the caller as exactly an `IOException` naming the court folder.
The tenure then answers the ended state, no longer halted.
The court folder is restored right after the cancel, even when the assertion fails.
So the engine and workspace dispose over a sound tree.
Nested preparation suppresses matching notices through the outermost scope, even when it throws.
Later notices still arrive.

## `TTenureQueue.cs`

Covers the queue of deferred requests when one of them is refused.
The refused request raises one draft bulletin carrying the card refusal and the tenure id.
The next deferred request still applies, so the headword lands.
The tenure is not halted by the refusal.

## `TTenureStale.cs`

Covers a held draft whose workspace was switched away.
Most facts first check that reading the tenure throws the refusal.
The first fact switches the workspace through the engine's change relay and the others through its open call.
The stored-id read answers null instead of throwing, and answers null for a live blank entry.
Cancelling a stale tenure ends it without a refusal, and its read then answers null.
Starting a recording or a transcription errand answers null.
The speech read answers no names and empty typed text.
The kept read answers null.

## Stamp

Commit: `36e31eb71c98`
- `TChronicle.cs` `47fd8579113e846b`
- `TDraft.cs` `8490b5283bb57a47`
- `TDraftCancelling.cs` `8e5113e3e3f1289b`
- `TDraftCommitting.cs` `6cd5dee712aff104`
- `TEstablishment.cs` `7c95814d06051070`
- `TTenureLanguage.cs` `e4c5c6b4324fd3df`
- `TTenureObserver.cs` `48226af69615022a`
- `TTenureQueue.cs` `902b2ee7b18c788a`
- `TTenureStale.cs` `c52bd3cb1c5726cc`
