# LTally.cs

## `public sealed record LTally(`

The tally of one division on a Diwei page: how the characters placed there sound in each borrowing language.
Each line names one language and the parts its readings take, with the number of characters taking each.
An initial's page tallies onsets, a rime's page vowel and coda, both read off the stored reflex anatomy.
The engine builds it, and a view only prints the lines it is handed.

**Parameters**

- `LTallyDivision` — The division the tally belongs to, as the fanqie rows write it, such as `一`.
- `LTallyLines` — One [LTallyLine](LTallyLine.comment.md) per language and kind, in the pack's reflex-rule order.
  Empty when no character of the division has an entry with reflex rows.
