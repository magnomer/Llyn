# Fold

This folder holds the remembered open or closed state of an entry's cards and boxes.
The editor and the reading view store and toggle it.

## `TCardFold`

Covers the card fold reads and gates of the reading view and the editor on a real workspace.
Each entry holds two meanings and one collocation, so a fold names one card among several.
A stored fold is a pair of card kind and id.
A meaning and a collocation may share an id.
Folds made through the engine relay arrange the store, and the gates are read back through it.

## `DisplayCardRead_FoldedCards_CarryEachCardsStoredFold`

Each meaning leaf carries its card id in draft order.
Every leaf carries its stored fold and reads as able to fold.
A folded meaning and a folded unlinked collocation read folded, and the other meaning reads open.
The collocation is read from the lectern card's other-collocations list, since it links to no meaning.

## `DisplayFoldToggle_StoredCard_FoldsThenUnfolds`

The reading view gate folds a card, the next card read shows it folded, and the store holds the fold.
The same gate unfolds it, and the next card read shows it open.
Each toggle raises the view's own fold event for the shown entry.
Both toggles answer true.

## `EntryDraftChanged_FoldedCard_CarriesStoredFold`

The opened draft is handed once and carries each card's stored fold.
Its first meaning reads folded, while the second meaning and the collocation read open.
Each meaning reads stored.

## `CardFoldToggle_StoredCard_FoldsCardById`

The editor list gate folds a collocation by its id and the store holds exactly that card.
The editor hears the fold and hands the card folded, while the draft stays clean.
The same gate unfolds it, and the store and the held draft both read open.
Both toggles answer true.

## `CardFoldToggle_UnsavedCard_RefusesAndStoresNothing`

A meaning added in the editor is unstored.
Its fold gate stores nothing, and the card still reads open in the next draft.

## `DisplayFoldToggle_SaveRefused_RaisesNoDisplayFold`

The fold table is made unavailable, so the store refuses the write and the gate answers false.
The reading view rereads its cards only on the display fold event, so a refused fold must raise none.

## `DisplayFoldToggle_OneEntry_LeavesOtherEntryUnfolded`

A fold on one entry leaves every card of another entry open, in the view and in the store.

## `DisplayFoldToggle_SameEntryEditor_RefreshesEditorCards`

A fold in the reading view reaches an editor on the same entry.
The editor hands its draft again with the folded card marked.

## `CardFoldToggle_SameEntryReadingView_RaisesDisplayFold`

A fold in the editor raises the reading view's fold event for the shown entry.
A fold the editor made on another entry first raises nothing in that view.
The reading view's next card read shows the card folded.

## `DisplayFoldToggle_NoEntryChosen_RefusesAndStoresNothing`

The reading view gate with no entry chosen answers false and writes no fold.

## `CardFoldToggle_NoStoredEntry_RefusesAndStoresNothing`

The editor list gate with no stored entry held answers false and writes no fold.

## `TFold`

Covers the editor's box folds end to end, driven with no window.
Editors share one engine over a throwaway workspace.
Stored changes refresh open editors holding the same entry.
A fault bundle lets a test refuse each box read or write at the reflex port.
It also covers the reading view's box reads and gates through the display's own fold area.
Both modes share one stored state per entry, so each mode reads back what the other wrote.
Each box reads the state the engine stores for the held or shown entry.
Opening another entry reads that entry's own states, not the former entry's.
Each gate stores its box for the held entry and answers true.
Each stored editor write refreshes the editor once through the entry's fold bulletin.
Each stored view write raises the display's fold notice once for that entry.
Closing the rime-book box leaves the script box open.
A fresh draft or an empty view makes both gates answer false.
Both boxes then read folded, and no stored entry gains a box state.
A box write that throws shows `Box.SpreadFailed` through the envoy and answers false, once per gate.
Both boxes keep their stored states, and a second editor on the entry hears nothing.
A box read that throws answers folded for both boxes, though the store holds them open.
Both failed reads share `Box.SpreadReadFailed`, so this fold's repaint memory shows the notice only once.
A box stored for one entry leaves the other box and the other entry as they were.
States stored through one editor read back open in a fresh editor holding the same entry.
A second editor on the same entry hears each stored write once and reads the new states.
A second editor that has closed hears no box stored in the first.
It holds no entry, so it reads folded.
A fold made in the reading view refreshes an editor holding the same entry, once per write.
A fold made in the editor raises the display's fold notice for the shown entry, once per write.
The reading view then reads the rime-book box closed and the script box open.

## Stamp

Commit: `9377e657b395`
- `TCardFold.cs` `92590f10386fae33`
- `TFold.cs` `8451256db6a1c93f`
