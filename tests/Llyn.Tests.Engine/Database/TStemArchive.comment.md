# TStemArchive.cs
Hash: `90ac6e65c0a258ff`

## `public sealed class TStemArchive`

Covers the series store: the links a character's series makes, their counts and the entries they reach.

## `public void StemApply_TwoSeries_LinksBothAndCountsEntries()`

A character listed under two series links to both, and each series counts the entries it reaches.
Naming two series at once asks for their intersection, as a cell of the rime table does.

## `public void StemApply_SeriesGone_DropsTheRowNothingLinksTo()`

Relinking a character to another series drops the series nothing links to any more.

## `public void StemApply_CharacterWithoutStoredSeries_LinksNothing()`

A character with no stored series row has nothing to link from, so no series is made.

## `public void StemRebuild_StoredRows_BuildsEverySeriesAgain()`

A rebuild cuts every stored series text again and links each character to all of its series.

## `public void ShengfuRow_Deleted_DropsLinksByCascade()`

Dropping a character's series row takes its links with it.

## `public void ShengfuSave_Refetch_DropsStaleLinksAndKeepsTheRest()`

A refetch relinks the row: series it no longer names lose their link, and other characters keep theirs.

## `public void StemKeyScan_StoredText_CutsOneKeyPerSeries(string text, string separator, string[] expected)`

The stored text is cut on the separator, trimmed and deduped, and left whole without one.
