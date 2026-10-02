# TEngineXiaoyun.cs
Hash: `f49b68e2fade890c`

## `public sealed class TEngineXiaoyun`

The entry list of one rime table cell, read from the engine as rows ready to show.

## `public void XiaoyunFind_QueryFiltersHeadword()`

An empty query lists every entry anchored at the cell, in id order.
A query keeps only the entries whose headword carries it, and the shown name is the headword.

## `public void DiweiFind_StoredKey_AnswersTheCellAndItsColumn()`

A stored onset or rime key answers the cell's identity and whether it is a rime.
A key never stored answers nothing.

## `internal static void TXiaoyunDiweiPlace(LEngine engine, TWorkspace workspace, string language, string character, string initial)`

Stores one entry for the character, places the character under one onset, and anchors the entry's reflex to it.
The anchor is what the cell scan finds, so a placement alone lists nothing.
The reflex is a Korean reading, so a page under a book language tallies it.
The yunjing panel tests place their cells through it too.
