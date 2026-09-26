# TDisplay.cs

## `public sealed class TDisplay`

Covers the reading view rules that moved into the conduct, driven with no window.

## `public void DisplayStampFormat_UnreadableText_ReturnsEmpty()`

A stamp that does not parse, or none at all, shows as nothing.

## `public void DisplayFoldScan_NoRule_ReturnsEmpty()`

No reflex rule folds no language.

## `public void DisplayFoldSet_CurrentValue_KeepsFold()`

Setting the fold to the value it already holds leaves it there, so a toggle echo changes nothing.

## `public void DisplayBandResolve_UnbandedRows_ReturnsZero()`

Rows that carry no band resolve to the unknown band.

## `public void DisplayBandResolve_BandedRow_ReturnsItsRank()`

The first row that carries a band resolves to that band's rank.

## `public void DisplayBandResolve_UnknownBandFirst_ReturnsNextRank()`

A first row whose band is not a ladder name is passed over for the next row's band.

## `public void DisplayBandRead_TopRank_ReturnsCore()`

The highest rank reads as the core band.

## `public void DisplayFoldScan_FoldedRule_ReturnsItsLanguage()`

Only the language of a folded rule is folded.

## `public void DisplaySoundClear_ShownDraft_DropsDraftAndEntry()`

Clearing a shown draft drops both the draft and its entry.

## `public void DisplayReflexRead_LoadedReflex_PrefersIt()`

The stored reflexes loaded for the shown entry win over the shown draft's empty ones.
A refused draft load stays untested, since no seam fails the entry load.

## `public void DisplaySourceFormat_OnceInterval_FormatsInterval()`

A row with a once interval shows the interval in the given pattern, and a plain row shows its figure.
Each source takes its own line.

## `private static LEntryDraft TDisplayDraftCreate(IReadOnlyList<LReflexDraft> reflexes)`

One English draft with a single meaning, carrying `reflexes`.
