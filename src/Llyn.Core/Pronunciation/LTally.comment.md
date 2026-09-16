# LTally.cs

## `public sealed record LTally(`

The tally of one section on a Diwei page: how the characters placed there sound in each borrowing language.
An initial's page sections by division, a rime's page by the articulatory place of the initial.
Each line names one language and the parts its readings take, with the number of characters taking each.
An initial's page tallies onsets, a rime's page vowel and coda, both read off the stored reflex anatomy.
The engine builds it, and a view only prints the lines it is handed.

**Parameters**

- `LTallyHeading` — The section the tally belongs to: the division as the fanqie rows write it, such as `一`,
  or the place name the hypothesis gives the initial, such as `labial`, or empty for neither.
- `LTallyLines` — One [LTallyLine](LTallyLine.comment.md) per language and kind, in the pack's reflex-rule order.
  Empty when no character of the section has an entry with reflex rows.
