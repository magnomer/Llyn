# hypothesis.json

## file

Reconstruction tables for Classical Chinese.
They are the user's own hypothesis of how a rime-book placement sounds.
This file is kept apart from `source.json` so the system can grow on its own.
The reading view prints the reading and the class before the placement.
One placement reads /ngoʔ/ 4성S 疑 模[模] 一等 上.

## `initial`

The initial table gives the onset of every initial the rime books name.
Onsets starting p t c s k ʔ h are voiceless.
Onsets starting b d q z g x are voiced obstruents.
Onsets starting m n j w l are sonorants.

## `place`

The place list groups the initials by articulatory place, in the order the rime page sections them.
Each place names itself and lists its initials in the order its rows should follow.
`name` is printed through the localization key `Yunjing.Place` plus the name capitalized, or as written without one.
An initial no place lists falls into a trailing section of its own.

## `final`

The final table gives the final of every rime.
It is keyed by the rime, a space and the division.
A space and 合 are appended for the rounded medial.
A rime with a 重紐 letter is looked up as written and then without the letter.

## `tone`

The tone table splits each rime-book tone into the classes of the reconstruction, one row per class in order.
`onset` is a pattern matched against the onset the initial table gave.
The first row whose pattern matches wins, and a row without `onset` matches every onset.
`rewrite` lists rules run over the whole syllable, onset and final joined.
`class` names the tone class the reading view prints after the reading.
It runs 1 to 8, with S for the sonorant split of 上 and 入.
平 gives 1 to a voiceless onset and 2 to every voiced one.
It also unfolds a voiced obstruent into the aspirated voiced series the pack otherwise folds.
So 並 reads b in the table and bh under 平.
匣 x has no such series.
上 adds the glottal stop after the coda and gives 3, 4S or 4.
去 adds h and gives 5 or 6.
入 turns a nasal coda into a stop and gives 7, 8S or 8.
