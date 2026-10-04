# TDisplay.cs
Hash: `d5bca173eb524b33`

## `public sealed class TDisplay`

Covers the reading view rules that moved into the conduct, driven with no window.

## `public void DisplayFoldSet_CurrentValue_KeepsFold()`

Setting the fold to the value it already holds leaves it there, so a toggle echo changes nothing.

## `public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()`

Clearing a shown draft drops both the draft and its entry.

## `public void DisplayReflexRead_LoadedReflex_PrefersIt()`

The stored reflexes loaded for the shown entry win over the shown draft's empty ones.

## `public void DisplayFrequencyRead_NoEntry_ReturnsNone()`

With no entry given, no frequency chip shows.

## `public void DisplayFrequencyRead_EntryWithoutFrequency_ReturnsNone()`

A stored entry no source has ranked shows no chip.

## `public void DisplayCompassRead_ShownCards_NamesEachPartAndCardAndNumbersTwins()`

The lookup echoes each key but the collocation one, so every other wording key shows in the names.
A titled card keeps its title, an untitled one its kind, and an uncertain one the unknown mark.
The collocation kind is looked up to the meaning wording, so the two twins carry one name and are numbered.
Each card carries its number, its depth and its place in its section's list.

## `public void DisplayCompassRead_NothingShown_NamesOnlyTheParts()`

With no draft shown, the parts are named through the lookup and carry no cards.

## `private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes)`

One English draft with a single meaning, carrying `reflexes`.
