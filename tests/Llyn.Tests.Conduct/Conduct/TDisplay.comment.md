# TDisplay.cs
Hash: `3b053518f99182d9`

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

## `public void DisplayFrequencyRead_HostileBand_KeepsAFullStarRow(int band, int stars, CFrequencyTier rank, bool ranked)`

A band at or below zero reads zero stars, the unknown tier and no rank.
Its spare stars then fill the whole row.
A band on the scale keeps its stars and tier, and its spare stars fill the row.
An empty source stays empty, since the chip only shows it as a tooltip.

## `public void DisplayFrequencyRead_BandPastScale_ReadsUnknownTier(int band)`

A band past the scale throws nothing and reads the unknown tier with its key and no spare stars.
The band stays non-negative and the source passes through.

## `public void DisplayFrequencyRead_NoGauge_ReadsNone()`

An engine that answers no gauge reads no frequency, so the chip hides.

## `private static LPronunciationPort TFrequencyPortCreate(LFrequencyGauge? gauge)`

A fake pronunciation port answering only the frequency read, with `gauge` for every entry.

## `private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes)`

One English draft with a single meaning, carrying `reflexes`.
