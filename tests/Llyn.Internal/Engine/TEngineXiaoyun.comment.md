# TEngineXiaoyun.cs

## `public sealed class TEngineXiaoyun`

The entry list of one rime table cell, read from the engine as rows ready to show.

## `public void XiaoyunFind_QueryFiltersHeadword()`

An empty query lists every entry anchored at the cell, in id order.
A query keeps only the entries whose headword carries it, and the shown name is the headword.

## `private static void TXiaoyunDiweiPlace(LEngine engine, TWorkspace workspace, string language, string character, string initial)`

Stores one entry for the character, places the character under one onset, and anchors the entry's reflex to it.
The anchor is what the cell scan finds, so a placement alone lists nothing.
