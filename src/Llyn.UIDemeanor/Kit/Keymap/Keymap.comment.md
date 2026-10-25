# Keymap

The console's table of key bindings, which the window, the legend and help all read.

## `BKeymap`

A list of rows, each binding one key at one focus to a label, an action and a live state.
Each control builds its own table: the window, a box and the pager.
Dispatch, the legend and help read the same rows, so they cannot disagree about a key.
The constructor refuses a null list or a null row.
It throws `ArgumentException` for a row on a key the roof or the terminal owns.
Alt with a letter belongs to the roof, so only a row at the roof focus may bind it.
F11, Alt+F4, Alt+Enter, Ctrl+V and any Ctrl+Tab belong to the terminal.
Ctrl+Shift with a letter also belongs to the terminal, except Ctrl+Shift+Z.

## `BKeymapFind`

Returns the first row of the focus whose key and modifiers both match exactly, or null.
A row whose live state is hidden is skipped, so a hidden key finds nothing and falls through.
So Shift+F10 does not find the F10 row.
The character U+0003 counts as Ctrl+C, since a terminal may send the interrupt as a character.

## `BKeymapRead`

Returns the rows of one focus in table order.
The window and help read them to collect the keys of a page.

## `BKeymapRow`

One binding: key, modifiers, focus, label key, action and live function.
The label is a catalog key, resolved when the legend or help draws.
The action is never null, and it runs when the key is found.
The live function is asked at each draw, so a state follows the program without a rebuild.

## `BKeymapFocus`

Where the keys go.
That is the window, a box, the small-console notice, the help box, the roof or the reading view's lectern.

## `BKeymapLive`

Shown draws a key in the legend and help.
Dim draws it in the `disabled` role, for a key that does nothing at the moment.
Hidden leaves it out of both, and `BKeymapFind` does not find it.

## Stamp

Commit: `cb8ab7e8feb4`
- `BKeymap.cs` `f4521bf6490e80ae`
- `BKeymapFocus.cs` `77e5600e95171633`
- `BKeymapLive.cs` `c4bed21c76f50c73`
- `BKeymapRow.cs` `3a59402801d27b87`
