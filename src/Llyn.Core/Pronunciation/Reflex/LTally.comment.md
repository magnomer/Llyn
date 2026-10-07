# LTally.cs
Hash: `8c6dfaeeed37515d`

## `public sealed record LTally(string LTallyHeading, IReadOnlyList<LTallyLine> LTallyLines)`

The tally of one section on a Diwei page.
It shows how the characters placed there sound in each borrowing language.
An initial's page sections by division, a rime's page by the articulatory place of the initial.
Each line names one language and the parts its readings take, with the number of characters taking each.
An initial's page tallies onsets, a rime's page vowel and coda, both read off the stored reflex anatomy.
The engine builds it, and a view only prints the lines it is handed.

**Parameters**

- `LTallyHeading` — The section the tally belongs to, or empty for neither kind.
  It is the division as the fanqie rows write it, such as `一`.
  Or it is the place name the hypothesis gives the initial, such as `labial`.
- `LTallyLines` — One [LTallyLine](LTallyLine.comment.md) per language and kind, in the language order the pack declares under `order`.
  Empty when no character of the section has an entry with reflex rows.
