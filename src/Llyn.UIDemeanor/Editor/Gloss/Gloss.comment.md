# Gloss

The console's language pill of the editor, as the GUI's `QSpeaker` is.
The pill shows the draft's language and opens a list of the loaded languages.

## `BSpeaker`

It holds the list and not the language, so the language stays with Conduct's entry.
A pick hands the language name to a callback, and the draft changes through the entry's own gate.
So the pill shows the new language only once the entry announces it.
The list is loaded when the pill is opened, through a loader the owner passes.
The languages are the workspace's, so a list built earlier could be out of date.
The load is slow, so the user can leave the pill before it ends.
A counter of turns marks each open and each close.
An answer that comes back after a close finds the counter moved and opens nothing.
A second open during a load does nothing, so one press loads once.
The list starts with the draft's language lit, and a page holds eight rows.
Selecting with no list open is a misuse and throws, since only a lit list can give a language.
Letters typed while the list is open go to the drop's letter seek.
The pill takes the key only when a row matched.
The draw is the language and a down arrow in the text role, and the frame's tone overrides the role.
The arrow glyph is the one the order chooser uses, so the pill reads as a chooser.

## Stamp

Commit: `f3a687d70b47`
- `BSpeaker.cs` `b018920d30940624`
