# TDisplayArea.cs
Hash: `15c98e3dfce5cd7c`

## `public sealed class TDisplayArea`

Covers the reading view's area, its gates, reads and change events, driven with no window.
The wing opens and chooses entries, so most cases stand on its display.

## `public void DisplayEntryOpen_WingLoadsAnEntry_OpensItsHeader()`

A loaded entry opens once, with its headword, language, speech names, note and worded stamp ready to draw.
The note arrives parsed, so its italic span is already a block's span.
An entry without a unit carries an empty unit key.

## `public void DisplayEntryOpen_GoneEntry_ClosesTheView()`

An entry gone from the store chooses nothing, so the view closes and drops its draft.

## `public void DisplayEntryClose_ShownEntry_DropsTheDraftAndRaisesClosed()`

Closing drops the shown draft and blanks the header.

## `public void DisplayPanelAttach_PanelLoadsAndCloses_OpensAndClosesTheView()`

A followed panel's load opens its entry here, and its clearing closes the view.

## `public void DisplayEntryResonate_ChosenEntry_ReopensItsDraft()`

A notice about the chosen entry reloads its draft and opens it again.

## `public void DisplayEntryResonate_NothingChosen_OpensAndClosesNothing()`

With nothing chosen the notice changes nothing.

## `public void DisplayWorkspaceResonate_ShownEntry_ClosesTheView()`

A workspace swap closes the view.

## `public void DisplayVistaAttach_ChosenEntryMarked_RaisesOnlyTheFavoriteChange()`

The plan subscribed on the vista routes a favourite bulletin about the chosen entry to its own event alone.

## `public void DisplayFavoriteToggle_ChosenEntry_AnswersTheStoredMark()`

The heart's gate stores the mark and answers what the store now holds.

## `public void DisplayFavoriteToggle_NoEntryChosen_StoresNothing()`

With no entry chosen the heart stays unmarked.

## `public void DisplayGraspSet_StandingStepPressedAgain_ClearsTheGrasp()`

A new step is stored and answered with its wording, and the standing step pressed again clears it.

## `public void DisplayGraspSet_NoEntryChosen_StoresNothing()`

With no entry chosen the gate stores nothing and words nothing.

## `public void DisplayFrequencyRead_EntryWithoutFrequency_LooksUpTheOnceKeyAndAnswersNone()`

Conduct chooses the one-off key for the driver's lookup, and an unranked entry shows no chip.

## `private static CWing TDisplayWingPrepare(CAtelier atelier)`

A wing on a fresh vista, standing on no entry.

## `private static LEntry TDisplayEntrySave(LEngine engine, string headword)`

Stores one English entry with a single meaning.

## `private static CAtelier TDisplayAtelierCreate(LEngine engine)`

An atelier over the real engine whose media port only answers a stop, since closing the view stops its play.
