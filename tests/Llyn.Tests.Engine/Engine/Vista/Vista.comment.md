# Vista

This folder holds the tests of the vista promises, bulletins, panel finds and relays.
They also cover the tenures it starts and the rows it lists.

## `TEngineFellow.cs`

It covers the co-author tally the vista shows, read from the engine in one call.
It also covers the two names the union question reads.
An Author credited beside the read one on two Sources is listed once and counted two.
The worded shared count follows the count the clerk raised on a copy.
The higher shared count leads, and equal counts sort by name.
With no held draft and no stored kept Author, the union question reads two empty names.
The kept name reaches the union question without its outer spaces.

## `TEngineTenureVista.cs`

It covers the tenures a vista starts from one given row, on a fresh workspace.
The occurrence, quotation, footnote, membership and cohort starts each open a fresh entry that carries the row.
A start without its row opens a blank entry that reads unchanged.
An occurrence start links the Situation to the first card before it returns.
The draft is unstored, so the link is the only change.
An occurrence is storable only once its headword is written.
The storable verdict never writes the queue, so a waiting headword counts only after the change check writes it.
A quotation or footnote start cites the Example or Source in the first sentence of the first card.
A membership start puts the Tag on the first card, and a cohort start the Register.
Only the storable fact holds the queue, through `TEngineTenure.TTenureHold`.

## `TEngineVista.cs`

It holds the vista's promises, one fact per thing a browse panel relies on.
Each fact starts a vista on a fresh workspace and changes it through the vista itself.
A fact that must find an ordering or filter again starts its vistas through the posture, which stores them.
This class covers starting a vista and listing the library's entries through it.
The other panels' finds live in `TEngineVistaList`, the bulletins in `TEngineVistaBulletin`, and the relays in `TEngineVistaRelay`.
A listed entry with no epithet carries an empty one, so no reader of the row supplies a fallback.
An ordering or filter set on the vista is found again by a vista started on the reopened workspace.
An ordering set on the left tab leaves the right tab on its fallback.
A tab with nothing stored lists by the fallback, hides nothing, queries nothing and chooses nothing.
A hidden language's entries are left out, and only headwords matching the query are listed.
A blank vista with nothing typed lists no rows while a plain one lists every entry.
Typing a query makes the blank vista list its matches like any other.
Two entries sharing a headword are numbered by entry id, so the older is `(1)` even when listed second.
An unshared headword is listed unnumbered.
Twins number only within one language, so one headword in Mandarin and Classical Chinese lists bare twice.
The row of the selected entry is the one row marked chosen.

## `TVistaEntryCreate`

It stores one entry with one meaning, the least an entry needs to be listed.
A phonology test passes a `sound`, since only the pending ordering lists an entry without one.

## `TEngineVistaBulletin.cs`

It covers which vista changes raise the vista bulletin, on a real workspace.
A changed ordering raises one vista bulletin carrying the vista's own id.
Setting the order, filter, query or chosen row to what it already is raises no bulletin.
A repeated click therefore re-lists nothing.

## `TEngineVistaList.cs`

It covers the vista finds of the favorite, phonology, yunjing, yunmu and taxonomy panels.
Its favorite and phonology facts build their entries through `TEngineVista.TVistaEntryCreate`.
Each find answers rows the panel shows as they come, with twins numbered and no fallback left to the panel.
Pronunciation rows come in the vista's ordering.
Under the usage ordering the onset anchored by more entries lists first, and under the name ordering second.
Only tags whose text matches the vista's query are listed.
The favorites and phonology finds number twins by entry id as the library does.
A phonology row with no epithet carries an empty one, so the Conduct map needs no fallback.

## `TVistaDiweiPlace`

It stores one entry, places the character under one onset, and anchors the entry's reflex to it.
The anchor is what the onset's entry count is tallied over, so a placement alone counts nothing.

## `TEngineVistaRelay.cs`

It covers the relays a panel reads its vista through, first with no vista and then with one.
A panel with no vista yet reads the headword order and the shared empty filter.
No row and the zero row name no stored record, and a positive id does.
Hidden languages set on the vista are kept as given.
Hiding nothing sets the engine's shared empty filter.
An Author reaches the Sources that credit it, and nothing while no row is chosen.
A chosen entry that no other record refers to counts zero.

## `TEngineXiaoyun.cs`

It reads the entry list of one rime table cell as rows ready to show.
An empty query lists every entry anchored at the cell, in headword order.
A query keeps only entries whose headword carries it, and the shown name is the headword.
The cell's entries are stored against headword order, so storage order cannot pass for it.
A vista set to reverse lists them by headword descending, as the panel's order setting asks.
A stored onset or rime key answers the cell's identity and whether it is a rime.
A key never stored answers nothing.

## Stamp

Commit: `9377e657b395`
- `TEngineFellow.cs` `2735c6fb7b6b0a4b`
- `TEngineTenureVista.cs` `dfb4b4b7348cf02d`
- `TEngineVista.cs` `de1fc64e14ba441c`
- `TEngineVistaBulletin.cs` `0a1ba9717c4eacd6`
- `TEngineVistaList.cs` `1421427398e7e300`
- `TEngineVistaRelay.cs` `573b5f858d89ec51`
- `TEngineXiaoyun.cs` `c7580df10b93bc98`
