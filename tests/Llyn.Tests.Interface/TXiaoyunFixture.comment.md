# TXiaoyunFixture.cs
Hash: `e87fb087f6871a3e`

## `internal static class TXiaoyunFixture`

Places characters in a rime table cell for tests in Engine and Conduct.

## `internal static void TXiaoyunDiweiPlace(LEngine engine, TWorkspace workspace, string language, string character, string initial)`

Stores one entry for the character, places the character under one onset, and anchors the entry's reflex to it.
The anchor is what the cell scan finds, so a placement alone lists nothing.
The reflex is a Korean reading, so a page under a book language tallies it.
The Engine rime table tests and the Conduct yunjing panel tests place their cells through it.
