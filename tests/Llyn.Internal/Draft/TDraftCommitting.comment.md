# TDraftCommitting.cs

## `public sealed class TDraftCommitting`

Covers what a refused commit round leaves behind.
It also covers what a round that goes through strips or skips.
It also covers the requests a browsing panel sends without knowing which card the form made.

## `public void DraftCommit_OwnerRefused_LeavesTargetFileUntouched()`

An owner with no headword is refused after its target has already been written inside the round.
The rollback takes the target's entry away, so the target's file must still name no entry.
A file naming a rolled-back id would leave its editor reading favorites and grasp off a ghost.
The court row survives too, so the retry settles the same link.

## `public void DraftCommit_PaddedHeadword_StoresItTrimmed()`

Spaces typed around a word are not part of it, and the stored row must not carry them.

## `public void DraftCheck_HeadwordPaddingOnly_ReportsUnchanged()`

A trailing space on the headword and Windows line endings in the note are what the form produces while typing.
The commit strips both, so the check must not call them a change.

## `public void DraftCheck_BlankHeadword_NamesTheRefusal()`

A draft with a note and no word is changed, yet cannot be saved.
The check names the refusal so the form can enable Discard and not Save.

## `public void EntryUpdate_NothingChanged_RecordsNoRevision()`

Saving an entry back exactly as loaded leaves the history alone.
An empty revision would move the workspace pointer and wake every panel for nothing.

## `public void RequestApply_TagPickOnCardZero_LandsOnFirstMeaning()`

Card zero means the first Meaning card, made by the engine when the draft has none.

## `public void RequestApply_ExampleOnSentenceZero_LandsOnFirstRow()`

Sentence zero means the first sentence row of that card, made the same way.

## `public void ExampleCommit_FreshSentence_RecordsRevision()`

A stored sentence is history like a stored entry, so the round records a revision naming it.

## `private static LEntryDraft TDraftContentCreate(string headword)`

One entry with a single meaning card, enough to commit under the given headword.
