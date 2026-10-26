# Context

Context holds the Situation and Register fields of a card in the console.
Both are written again rather than edited, so each is a plain `BInlet` with its own rules.

## `BContext`

This builds the two fields and hands out their keys as one list.
A Situation shows its wording in brackets, and a Register shows its name in braces.
Wording that names a localization key is read through the catalog, so the chip follows the language.
Wording with no key is shown as typed.
The Situation takes the first spot of the card and the Register takes the second.
Each field is wired to the caret's own trail and gates, so the console and the window decide alike.
The add gate answers an edit, and the settle gate answers Enter and a close.
A refused settle keeps its text in the entry, as the window does.

## Stamp

Commit: `f3a687d70b47`
- `BContext.cs` `7719c83a213526a9`
