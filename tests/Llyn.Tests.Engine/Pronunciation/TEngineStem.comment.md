# TEngineStem.cs
Hash: `f70fd3c87969a477`

## `public sealed class TEngineStem`

Covers the series the xiesheng panel browses by, from the fetched row to the page and its entries.
The pages are fakes, so nothing here reaches the web.

## `public async Task StemApply_FetchedSeries_OpensAsAPageWithItsEntries()`

Fetching a character's series makes its series a row a chip can open.
The page carries the member character and the entry list reaches the entry written with it.
A blank key finds no series.

## `public void StemFind_PackWithoutSeries_StoresNoSeriesOfItsOwn()`

A pack declaring no series source stores no series, so no chip of its language opens one.
The blank page answers for a panel with nothing chosen.

## `public void StemResolve_StoredOutOfCodePointOrder_ListsCharactersByCodePoint()`

The page lists its characters by code point, whatever order their Shengfu rows were stored in.
An ideograph beyond the basic plane sorts after the compatibility block, as its code point says.
UTF-16 ordinal order would have put it first.

## `public void KindredFind_StoredOutOfHeadwordOrder_ListsInThePanelOrder()`

The series reaches two entries saved against headword order, so storage order cannot pass for it.
Without a vista they list by headword.
A vista set to reverse lists them by headword descending, as the panel's order setting asks.
