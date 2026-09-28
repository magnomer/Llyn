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

## `private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes)`

One English draft with a single meaning, carrying `reflexes`.
