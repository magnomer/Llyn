# Desk

This folder holds the desk with its session, chronicle and draft.
It also holds the entry gate and the easel gates over a desk.

## `TDesk`

Covers the Conduct seat at one tenure over an Author draft on a real workspace.
A store after a deferred name commits the Author, announces its id and lets the tenure go.
A cancel after a deferred name stores nothing and leaves the desk empty.
A start over a stored Author announces its name at once and reads as unchanged.
A start without a vista holds nothing, and a start by origin and subject holds a fresh draft.
A finish with nothing held reads done and stores nothing.
A tenure the engine refuses to start shows the scope's load failure through the envoy once.
A start over a held tenure, a refused start and a cancel each announce the change of state once.
The desk is built on the guild vista, with an overload whose envoy records each shown key.

## `TDeskChronicle`

Covers the chronicle of a Conduct desk over an Author draft on a real workspace.
Undo and redo step the chronicle.
An undo or redo after a workspace change shows the scope's `HoldFailed` key once.
A state bulletin without a halt shows no notice through the envoy and still announces the state.
Each test builds its desk inline, since the desk tests keep their builder private.

## `TDeskDraft`

Covers the draft holder of a Conduct desk over an Author draft on a real workspace.
The draft announcement runs with the filling flag raised and lowers it afterwards.

## `TEaselDesk`

Covers the image and video gates over a desk, on the playwright's scenario and on the editor's cards.
A pressed add appends a blank row after the rows held, and the driver hands no card.
A removed row leaves its own list, and the owning card is found below Conduct.
Add and remove gates take a flag for a collocation card.
The playwright's calls pass false with no owning card.
A chosen file lands at once, even under a long delay.
A cancelled file dialog hands null, and the row stays blank.
A chosen file that exists carries its ready address or screen, and a missing one carries none.

## `TEaselDeskHold`

A wait no test outlives, so only a request sent at once can show before the fact ends.

## `TEntry`

Covers the editor's head field gates end to end on a real workspace.
Field gates write the headword, the trimmed note and the reading, and an empty language changes nothing.
The reading answers empty on an empty desk.
It builds its editors through `TEditor`, so both files open the same stored entry.

## `TSession`

Covers one side panel's draft session and the quit decision across editors.
A refused ready check keeps the draft, and a close stores without asking it.
While the editor shows, finish and close go to the editor, and an unchanged save stores nothing.
A failed undo of the own desk shows the scope's `HoldFailed` key once, not twice.
With the editor hidden, finish ends the own desk.
The quit decision asks only over unsaved work, and every editor hears the one answer.

## Stamp

Commit: `8c403d1df768`
- `TDesk.cs` `76919744adc4f5dc`
- `TDeskChronicle.cs` `78a5a7c1f4c0db7e`
- `TDeskDraft.cs` `10fa2f8f82605c92`
- `TEaselDesk.cs` `ff56c313e73f77f3`
- `TEntry.cs` `c7407ea7a0b1c9d5`
- `TSession.cs` `b4e7142879e4bb5a`
