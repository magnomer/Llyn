# TCardFold.cs
Hash: `ff8856dfc5773655`

## `public sealed class TCardFold`

Covers the card fold reads and gates of the reading view and the editor on a real workspace.
Each entry holds two meanings and one collocation, so a fold names one card among several.
Folds made through the engine relay arrange the store, and the gates are read back through it.

## `public void DisplayCardRead_FoldedCards_CarryEachCardsStoredFold()`

Each leaf carries its card id, its stored fold and that it can fold.
A folded meaning and a folded collocation read folded, and the other meaning reads open.

## `public void DisplayFoldToggle_StoredCard_FoldsThenUnfoldsAndRaisesTheDisplayFold()`

The reading view gate folds a card, the next card read shows it folded, and the store holds the fold.
The same gate unfolds it, and the next card read shows it open.
Each toggle raises the view's own fold event for the shown entry, which drives the re-read.
Both toggles answer true, since the port took each write.

## `public void EntryDraftChanged_FoldedCard_CarriesTheStoredFold()`

The opened draft carries each card's stored fold.
Its meaning cards also assert the stored marker.

## `public void CardFoldToggle_StoredCard_FoldsTheCardById()`

The editor list gate folds a collocation by its id and the store holds exactly that card.
The editor hears the fold and hands the card folded, while the draft stays clean.
The same gate unfolds it, and the store and the held draft both read open.
Both toggles answer true, since the port took each write.

## `public void CardFoldToggle_UnsavedCard_AnswersItCannotFoldAndStoresNothing()`

A meaning added in the editor is unstored, so it reads as unable to fold.
Its fold gate stores nothing, and the card still reads open.

## `public void DisplayFoldToggle_OneEntry_LeavesAnotherEntrysCardsUnfolded()`

A fold on one entry leaves every card of another entry open, in the view and in the store.

## `public void DisplayFoldToggle_EditorOnTheSameEntry_RefreshesTheEditorCards()`

A fold in the reading view reaches an editor on the same entry through the fold bulletin.
The editor hands its draft again with the folded card marked.

## `public void CardFoldToggle_ReadingViewOnTheSameEntry_RaisesTheDisplayFold()`

A fold in the editor raises the reading view's fold event for the shown entry.
A fold the editor made on another entry first raises nothing in that view.
The reading view's next card read shows the card folded.

## `public void DisplayFoldToggle_NoEntryChosen_AnswersFalseAndStoresNothing()`

The reading view gate with no entry chosen answers false and writes no fold.

## `public void CardFoldToggle_NoStoredEntry_AnswersFalseAndStoresNothing()`

The editor list gate with no stored entry held answers false and writes no fold.

## `private static LEntry TCardFoldPrepare(LEngine engine, string headword)`

Stores an English entry with two meanings and one collocation and answers it.
