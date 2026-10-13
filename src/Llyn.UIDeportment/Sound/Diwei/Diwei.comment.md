# Diwei

The category page of the Yunjing panel and its row items.
Each item is a plain copy of a Conduct record, so nothing is computed here.
Its label, lines, tally lines and switch state are all decided below in Conduct.

## `QDiwei`

The driver of the category page, over the Veneer's `PDiwei` surface the yunjing panel places.
Its list attaches `QDiweiItem.QDiweiItemRefine`, which fills each section and the lists inside it.
Named parts are read through `QContract.QContractFind`.
It keeps the yunjing session for its gates.
The headword line takes the page's ready headword font, and the list takes its ready glyph font.
The kind chip reads the resource under the key the session names.
An empty page shows the empty notice instead.
A click on either half of any section's switch goes to the tally gate, the parameter saying which half.
A character chip goes to the glyph gate.

## `QDiweiCommand`

The routed commands the chips and switches of a page raise, bound by `QDiwei`.
They carry their value as the parameter, because the template has no code of its own.
The entry command opens the entry a chip stands for, making it first when none exists.
The switch command picks the set the tally lines print.
Either half of the switch every division heading repeats raises it.
Its parameter is `true` for the respelling half and `false` for the IPA half.

## `QDiweiItem`

One section of a page as the template binds it.
The fill covers the heading, the IPA and respelling switch, the tally lines and the placement lines.
The switch shows only while respelling is on.
Each half wears its chosen style while its set shows.
The chosen styles `Theme.Diwei.IpaChosen` and `Theme.Diwei.RespellingChosen` carry the accent ink.
The tally and placement lists are attached to their own fills.

## `QDiweiLine`

One placement row of a section, copied from the Conduct line.
The fill covers reading, label, the rounded mark and the character chips.
The chips take text and command parameter from their own item through `QLook` rows.

## `QTally`

One printed tally line of a section: language, kind and the marks of the chosen set.
Conduct's tally already holds only the set the section shows.
Each mark draws as its part text with the count raised after it.

## `QTallyMark`

One tally mark as the surface draws it: its text, its count and its characters.
It is copied from the Conduct mark, so no engine type reaches a fill.
The count is written as invariant text once.
The mark is such as an initial or a rime, and the characters are listed in its dropdown.
The toggle opens the dropdown, and a dropdown closed from outside, such as by a click, clears the toggle.

## Stamp

Commit: `c2f335c331fe`
- `QDiwei.cs` `3d0d49f83fa39ee9`
- `QDiweiCommand.cs` `97c3bc8ee506d7e0`
- `QDiweiItem.cs` `1e5db46e6a6657f1`
- `QDiweiLine.cs` `7990e2e8bc6f25a4`
- `QTally.cs` `2e711a7ede1b5e58`
- `QTallyMark.cs` `97ffc197d08bc1fc`
