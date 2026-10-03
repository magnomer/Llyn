# TEngineXiaoyun.cs
Hash: `7fd765c6e5c54a59`

## `public sealed class TEngineXiaoyun`

The entry list of one rime table cell, read from the engine as rows ready to show.

## `public void XiaoyunFind_QueryFiltersHeadword()`

An empty query lists every entry anchored at the cell, in id order.
A query keeps only the entries whose headword carries it, and the shown name is the headword.

## `public void DiweiFind_StoredKey_AnswersTheCellAndItsColumn()`

A stored onset or rime key answers the cell's identity and whether it is a rime.
A key never stored answers nothing.
