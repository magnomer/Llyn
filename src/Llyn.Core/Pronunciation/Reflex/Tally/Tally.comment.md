# Tally

This folder holds the per-division tally of reflex parts on a Diwei page.

## `LTally`

The tally of one section on a Diwei page.
It shows how the characters placed there sound in each borrowing language.
An initial's page sections by division, and a rime's page by the articulatory place of the initial.
An initial's page tallies onsets, and a rime's page vowel and coda, both read off the stored reflex anatomy.
The engine builds it, and a view only prints the lines it is handed.
`LTallyHeading` is empty for neither kind.
It is the division as the fanqie rows write it, such as `一`.
Or it is the place name the hypothesis gives the initial, such as `labial`.
`LTallyLines` has one line per language and kind, in the language order the pack declares under `order`.
It is empty when no character of the section has an entry with reflex rows.

## `LTallyLine`

One line of a tally, a language and a kind.
A language sorting its readings by kind, as Japanese by Go-on and Kan-on, gets one line per kind.
Both the IPA and the respelling sets are always carried, so the view's switch only picks which is printed.
The IPA marks are cut from the readings as fetched, most characters first, then by part text.
The respelling marks are cut from the respellings, in the same order.
`LTallyLineKind` is empty for a language without kinds.

## `LTallyLineRead`

The marks of one set, the respelling marks when asked for, else the IPA marks.

## `LTallyLineScan`

The lines of one division.
The reflex parts of every character are gathered per language and kind, both sets apart.
`kind` picks the part off each row's stored anatomy, onset for an initial and vowel with coda for a rime.
A character absent from `readings` adds nothing.
An empty part or blank language records nothing.
Languages sort by their place in the pack's declared `order`, undeclared ones after by name.
Within a language, kinds sort by the declared kind order, undeclared ones after by name.
Places come from `LReflexOrderFind`, so the tally and the entry's reflex list order a pack alike.
Names match exactly, never with case folded.
So the order the anchored rows were stored in never reaches the page.
A line whose two sets are both empty is left out, and each line lists its marks by count.

## `LTallyLineAdd`

Records one character under one language, kind and part, once however many readings take it.

## `LTallyMark`

One part and the characters whose readings take it, printed as the part with a raised count.
A character with two readings taking the same part is listed once, and one taking two parts under each.
The characters are kept so the view can open a list of them from the part.
`LTallyMarkText` is the part as cut, such as `ㄹ`, `l` or `an`.

## `LTallyMarkCount`

How many characters take the part, the printed number.

## `LTallyMarkScan`

The marks of one language from its parts and the characters under each, count descending, then part text.
A missing or empty table gives no marks.
Each mark lists its characters in code point order through `LGlyphOrder`, whatever order they were gathered in.

## `LTallyRow`

One tally line of a Diwei section with its marks already taken from the shown set.
The page picks the set once, so no reader chooses between the phonemic and the respelled marks.
`LTallyRowMarks` is never empty.

## Stamp

Commit: `c2f335c331fe`
- `LTally.cs` `8c6dfaeeed37515d`
- `LTallyLine.cs` `4beb0ccc1b3239f5`
- `LTallyMark.cs` `834ee2f5348a4b4f`
- `LTallyRow.cs` `c92108897254182f`
