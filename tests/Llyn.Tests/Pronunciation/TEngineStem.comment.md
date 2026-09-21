# TEngineStem.cs

## `public sealed class TEngineStem`

Covers the series the xiesheng panel browses by, from the fetched row to the page and its entries.
The pages are fakes, so nothing here reaches the web.

## `public async Task StemApply_FetchedSeries_OpensAsAPageWithItsEntries()`

Fetching a character's series makes its series a row a chip can open.
The page carries the member character and the entry list reaches the entry written with it.

## `public void StemFind_PackWithoutSeries_StoresNoSeriesOfItsOwn()`

A pack declaring no series source stores no series, so no chip of its language opens one.
The blank page answers for a panel with nothing chosen.
