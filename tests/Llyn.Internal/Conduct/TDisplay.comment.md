# TDisplay.cs

## `public sealed class TDisplay`

Covers the reading view rules that moved into the conduct, driven with no window.

## `public void DisplayStampFormat_UnreadableText_ReturnsEmpty()`

A stamp that does not parse, or none at all, shows as nothing.

## `public void DisplayFoldSet_CurrentValue_KeepsFold()`

Setting the fold to the value it already holds leaves it there, so a toggle echo changes nothing.

## `public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()`

Clearing a shown draft drops both the draft and its entry.

## `public void DisplayReflexRead_LoadedReflex_PrefersIt()`

The stored reflexes loaded for the shown entry win over the shown draft's empty ones.
A refused draft load stays untested, since no seam fails the entry load.

## `public void DisplayFrequencyRead_NoEntry_ReturnsNone()`

A fresh draft has no entry, so no frequency chip shows.

## `public void DisplayFrequencyRead_EntryWithoutFrequency_ReturnsNone()`

A stored entry no source has ranked shows no chip, and the read asks the engine to fetch one.

## `public void DisplayCompassRead_ShownCards_NamesEachPartAndCardAndNumbersTwins()`

The lookup echoes each key, so every wording key Conduct chose shows in the names.
A titled card keeps its title, an untitled one its kind, and an uncertain one the unknown mark.
The collocation kind is looked up to the meaning wording, so the engine numbers the two twins.
Each card carries its number, its depth and its place in its section's list.

## `public void DisplayCompassRead_NothingShown_NamesOnlyTheParts()`

With no draft shown, the parts are named through the lookup and carry no cards.

## `public void DisplayGraspSet_StandingStepPressedAgain_ClearsTheGrasp()`

A new step is stored, and the step that stands pressed again clears the grasp to zero.

## `public void DisplayGraspSet_NoEntryChosen_StoresNothing()`

With no entry chosen the gate stores nothing.

## `private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes)`

One English draft with a single meaning, carrying `reflexes`.
