# LYunjing.cs

## `public sealed class LYunjing`

The deportment of the yunjing panel: the two category columns, the entry list's panel state, and the cell page.
It holds no language of its own.
The language of the page is the language of the chosen cell, else the first pack that carries rime books.
The page opens on the column last clicked, kept as a side flag the engine is never told.
The page is shown while that cell is chosen and the entry list has nothing chosen and is not writing.

## `public event Action? LYunjingChanged;`

The columns or the page need re-reading, raised after any request that moves a cell or the tally set.

## `public event Action<string, string>? LYunjingGlyphChosen;`

A character on the page was chosen, with the page's language, for the window to show as a glyph.

## `public LEditor LYunjingEditor { get; }`

The entry editor's deportment, whose desk answers whether the panel may leave and opens the row that is edited.

## `public bool LYunjingAllowed`

Whether the tab may be shown at all: some loaded pack carries rime books.

## `public string LYunjingDiweiKey`

The key of the page's kind chip, by the side the shown cell sits on.

## `private LVista? LYunjingSideVista`

The vista of the column whose cell the page opens.

## `private string LYunjingLanguage`

The language the columns and the entry list are read for, derived as the class describes.

## `private IReadOnlyList<LDiwei> LYunjingDiweiFind(LVista? vista, string kind)`

One column's rows, empty while the tab is not allowed, since a language without books has no table.

## `public LDiweiPage LYunjingDiweiRead()`

The page of the shown cell as the engine composes it, or the blank page while none is shown.

## `public void LYunjingReset()`

The workspace changed: both columns stand on nothing and the entry list starts afresh.

## `public void LYunjingRowsUpdate()`

Something the table is built from changed, so the columns are read before the entry list.
The engine drops a chosen cell the columns no longer hold, so the columns must go first.

## `public void LYunjingEntryHandle(LBulletin bulletin)`

A stored entry changes the cell counts as well as the entry list, so both are read again.

## `public void LYunjingDiweiSelect(long? id, bool? final)`

A click on a cell: toggles it in its column, clears the entry list, and opens the page on it.
The side arrives with the id because the two columns share one row template.

## `public void LYunjingDiweiShow(string language, string kind, string key)`

A glyph link from elsewhere: finds the cell, clears both columns, and opens the page on it.

## `private void LYunjingDiweiOpen(LDiwei? found)`

The found cell crosses as a parameter, so no local carries an engine answer into the select.

## `public void LYunjingTallySet(bool? respelled)`

The tally switch on the page: saves the chosen set and reads the page again.

## `public void LYunjingGlyphSelect(string? character)`

A character on the page: asks about an unsaved entry first, then announces the glyph with the page's language.
