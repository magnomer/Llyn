# anatomy_tone.json

## file

Tone correspondence rows for Classical Chinese, one per borrowing language.
Each row maps a contour, as `anatomy.json` cuts it, to the `hypothesis.json` tone classes it may descend from.
The anchor dropdown of the editor marks every placement whose class the reflex reading's contour allows.
So a Mandarin reading in 55 marks the placements of class 1, and of class 7 for a checked syllable.
The tables follow the tonal split notes kept in the working documents.

## `tone`

`language` names the borrowing language the row serves, one name or a list.
`class` maps each contour to its classes, one string or a list.
A contour is the citation tone as Wiktionary writes it, its superscripts turned into digits.
A sandhi form such as `214-21` is looked up by its citation part alone.
Common variants of one tone are listed beside the standard form, so `34` and `35` both read as Cantonese 3.

## Mandarin

55 is 1, or 7 for a checked syllable, since 入 spread over every tone.
35 is 2, or 8.
214 is 3 or 4S, the 上 of a sonorant onset having stayed 上.
51 is 4, 5 or 6, or 8S.

## Cantonese

55 is 1, 21 is 2, 35 is 3, 13 is 4S or 4, 33 is 5, 22 is 6.
Checked 5 and 3 are 7, checked 2 is 8S or 8.

## Southern Min

44 is 1, 24 is 2, 53 is 3 or 4S, 22 is 4 or 6, 21 is 5.
Checked 32 is 7, checked 4 is 8S or 8.

## Hakka

24 is 1, 4S or 4, 11 is 2, 31 is 3, 55 is 5, 6, 8S or 8.
Checked 2 is 7, checked 5 is 8S or 8.

## Wu

53 is 1, 23 is 2, 4S, 4 or 6, 34 is 3 or 5.
Checked 55 is 7, checked 12 is 8S or 8.
