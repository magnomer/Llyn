# TEngineXiaoyun.cs
Hash: `ab8517408741cd07`

## `public sealed class TEngineXiaoyun`

The entry list of one rime table cell, read from the engine as rows ready to show.

## `public void XiaoyunFind_QueryFiltersHeadword()`

An empty query lists every entry anchored at the cell, in headword order.
A query keeps only the entries whose headword carries it, and the shown name is the headword.

## `public void XiaoyunFind_StoredOutOfHeadwordOrder_ListsInThePanelOrder()`

The cell's entries are stored against headword order, so storage order cannot pass for it.
Without a vista they list by headword.
A vista set to reverse lists them by headword descending, as the panel's order setting asks.

## `public void DiweiFind_StoredKey_AnswersTheCellAndItsColumn()`

A stored onset or rime key answers the cell's identity and whether it is a rime.
A key never stored answers nothing.
