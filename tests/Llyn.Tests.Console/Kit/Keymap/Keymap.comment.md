# Keymap

Tests of the console key-binding table, mirroring `src/Llyn.UIDemeanor/Kit/Keymap/`.

## `TKeymap`

A row on a key the roof or the terminal owns makes the table throw.
No focus finds Alt with a letter, F11, Alt+F4, Alt+Enter, Ctrl+V, Ctrl+Tab or Ctrl+Shift with a letter but Z.
The character U+0003 finds the Ctrl+C row.
A key found in the window, notice or roof focus keeps one label.
Every label the window table finds, and `Legend.Import`, exists in the English and Korean catalogs.
The table comes from a window built with no atelier, and `BKeymapWindow` builds it.

Two rows on one key are told apart by live state.
The key finds the row that is not hidden.
A hidden row passes its key on, and the table finds no row for it.
A dim row still takes its key, but running its action does nothing.

## Stamp

Commit: `c05429890cec`
- `TKeymap.cs` `2a2d8d45a6dbe8af`
