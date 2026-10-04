# CByline.cs
Hash: `e0137a6efea61338`

## `public sealed class CByline`

The popup of Authors a typed credit may already name, owned by the source editor.
It keeps the typed word, how many rows it offers, and which offered row is lit.
The rows are read from the engine on every notice and never kept.

## `internal CByline(CImprint imprint, LDraftPort drafts, CLedgerNoticed noticed)`

Builds the byline over its editor, whose held draft it searches beside.
It takes the atelier's repaint memory, since its rows are read on every repaint.

## `public event Action? CBylineChanged;`

The word, the lit row or the open state moved, so the driver reads the byline again.

## `public bool CBylineShown`

Whether the last read offered any row.

## `public int CBylineIndex`

The lit row's place among the offered rows, or `-1` while none is lit.
A new word and a close both put it back to `-1`.

## `public void CBylineWordSet(string? text, bool? focused)`

Typing in a credit field searches again from no lit row.
A field the driver wrote itself has no keyboard, and it opens nothing.

## `public IReadOnlyList<CAuthor> CBylineRowsRead()`

The Authors the word may name and the draft does not credit yet.
Each name arrives split around the word, so the driver only paints it.

## `public bool CBylineMove(int? position, long? id, int step)`

Down or Up in a credit field lights the next or former row through `CLanternMove`.
It takes the key only while rows are offered.

## `public void CBylineSelect(long? id, int? position, long? held)`

A press on an offered row credits its Author in the focused field's place.
A press with no row or no focused field only closes.

## `public void CBylineClose()`

Closes the popup and forgets the word and the lit row.

## `internal void LBylineCommit(int at, long author, long picked)`

Closes the popup, then hands the picked Author to the editor.

## `internal void LBylineReset()`

Forgets the word, the offered count and the lit row without announcing it.

## `private IReadOnlyList<CAuthor> LBylineFind()`

A failed search offers nothing, so the popup stays shut rather than failing the field.
The failure is still shown, under `Source.FindFailed`, through the imprint desk's `LDeskRepaintShow`.
The rows are read on every repaint, so a lasting failure shows once until the user acts.
