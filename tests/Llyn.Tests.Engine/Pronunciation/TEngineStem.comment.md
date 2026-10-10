# TEngineStem.cs
Hash: `007dc5a5a957da60`

## `public sealed class TEngineStem`

Covers the series the xiesheng panel browses by, from the fetched row to the page and its entries.
The pages are fakes, so nothing here reaches the web.

## `public async Task StemApply_FetchedSeries_OpensAsAPageWithItsEntries()`

Fetching a character's series makes its series a row a chip can open.
The page carries the member character and the entry list reaches the entry written with it.
A blank or missing key finds no series.

## `public void StemFind_PackWithoutSeries_StoresNoSeriesOfItsOwn()`

A pack declaring no series source stores no series, so no chip of its language opens one.
The blank page answers for a panel with nothing chosen.

## `public void StemResolve_StoredOutOfCodePointOrder_ListsCharactersByCodePoint()`

The page lists its characters by code point, whatever order their Shengfu rows were stored in.
U+F900 sits above the surrogate range, so UTF-16 ordinal order would list U+20000 before it.
Code point order lists U+F900 first, so only the code-point sort passes.
U+F900 is a compatibility ideograph that normalisation would fold into U+8C48.
The stored read still returns it unchanged, so nothing on the path normalises.

## `public void KindredFind_StoredOutOfHeadwordOrder_ListsInThePanelOrder()`

The series reaches two entries saved against headword order, so storage order cannot pass for it.
Without a vista they list by headword.
A vista set to reverse lists them by headword descending, as the panel's order setting asks.

## `public void StemResolve_MembersWithAndWithoutEntry_MatchByHeadwordAndCreateNothing()`

A series of two characters, one written as an entry with a reflex.
The other has only a marked fanqie row.
The written member carries its reflex row with one guise.
The other stays bare, with no reflex rows, and still carries its representative reading.
Reading the page makes no entry.

## `public void StemSpread_MemberOpenedThenClosed_KeepsItsStateApartFromTheEntryPage()`

Three member characters: one entry with a folded and a plain reflex, one plain entry, one bare.
Opening the first member marks only it opened, and only its guises fold a row.
Its entry page fold stays closed, and opening the bare member stores nothing.
Opening the second entry's page fold leaves the series closed after the first member closes.
No member mark is left behind.
