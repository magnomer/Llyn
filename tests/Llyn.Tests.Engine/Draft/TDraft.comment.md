# TDraft.cs
Hash: `8490b5283bb57a47`

## `public sealed class TDraft`

Covers what the engine does with a held draft: committing and checking for change.
The court beneath the drafts folder is exercised through those calls, never on its own.

## `public void DraftCommit_HeldDraft_StoresEntryAndDeletesFile()`

Held work that reaches the database leaves the drafts folder empty and the entry stored.
A commit that wrote the entry but kept the file would offer the same work back.
The next session would read it as unfinished.

## `public void DraftStart_Fresh_TakesFirstLanguage()`

A draft started on no entry already carries the first language the engine lists.
Carrying it counts as no change, so a form opened and closed untouched still stores nothing.

## `public void DraftCommit_WriteRefused_KeepsDraftFile()`

A commit the database refuses leaves the file exactly where it was, and writes no entry.
A blank headword is refused, and that is the moment the user most needs what they typed to survive.

## `public void DraftCheck_OnlyVideoAdded_ReportsChanged()`

A card given nothing but a video reads as changed against the entry it was opened from.
The comparison decides whether the editor offers to store at all.
A field it skips is a field the user cannot save.

## `public void DraftCheck_OnlyLanguageChanged_ReportsChanged()`

Changing nothing but the language of an opened entry reads as changed.
A skipped language field would leave the editor unable to offer a store for that edit.

## `public void DraftCheck_NewDraftCarryingLanguage_ReportsUnchanged()`

A fresh draft holding nothing but the tongue the shell chose is not changed work.
The editor writes that tongue on its own, so counting it would warn about edits the user never made.

## `public void DraftCommit_DraftsPointingAtEachOther_NoRecursion()`

Two drafts that each name the other as a court target commit without looping.
The entry and its target are each stored once, and no draft or court file is left behind.

## `public void DraftCommit_NamesStoredEntry_UpdatesIt()`

A draft naming an entry that already exists commits as an update, and the word is stored once.
That is the file a kill between the database write and the delete leaves behind.
Committing it again would store a second copy if the file still named nothing.

## `public void DraftCommit_NamesDeletedEntry_StoresNewEntry()`

A draft opened on an entry that is deleted while it is open commits as a new entry.
Taking the update branch would refuse it forever, and the work would sit unsaved with no way out.

## `public void DraftCommit_DraftsTranslatingEachOther_StoresBothLinks()`

Two drafts each naming the other as a translation must both keep their link once committed.
The second draft commits inside the first, before the first has an id.
Its link therefore cannot settle in the ordinary pass.
The engine holds that link back and writes it once every entry in the round has its id.
Losing it silently was the old behaviour, and nothing told the user.

## `public void DraftCommit_TargetAnotherEditorHolds_KeepsTargetFile()`

A commit that reaches a target another editor is holding stores that target and leaves its file where it is.
Deleting it would pull the file out from under the panel typing into it.
Every later keystroke would be lost.
The file it leaves names the entry it became.
The holder's own commit updates that entry instead of storing the word twice.
The chip still lands on a real id.
The court row is settled exactly as a held target's would be.

## `public void DraftCommit_TargetHeldByAnotherEngine_LeavesItNamingTheStoredEntry()`

A target held by a second engine is stored, and its draft file stays and names the new entry.
The holder can then update that entry instead of storing the word twice.
The leftover list ends empty, so nothing is offered back as unfinished.

## `public void DraftCommit_TranslationNamingNoEntry_DropsItInsteadOfFailing()`

A translation naming an entry that does not exist is dropped when the draft commits.
Failing instead would leave the whole commit refused over one stale link.
