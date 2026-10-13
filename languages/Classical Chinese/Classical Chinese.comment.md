# Classical Chinese

The Classical Chinese pack is split over four data files.
`source.json` holds the sources, while the other three hold the user's own reconstruction and tone rules.
The reconstruction is kept apart because Classical Chinese will carry a phonological system of its own.

## `source.json`

This is the language selector pack.
Pronunciation and audio sources are listed separately, and either list can be filled without changing the application.

## `script`

The list names the character styles the reading view shows above the first meaning.
Each style is fetched once per character from the Academia Sinica 小學堂 databases and kept in the workspace database.
The site has no query API, so each style posts the database's search form with the character in `EudcFontChar`.
Every database needs its own hidden fields or answers 500.
Group 1 of the match pattern is the glyph image.
Group 2 is the caption, such as the inscription and its era.
The rewrite lifts the image size so the stored picture is the large original, and the view scales it down.
The small seal style also reads the 說文 gloss the page prints under the results.

## `epoch`

These are the chronology labels the captions date pieces by, each paired with the code Llyn stores.
The table sits beside the script list because all four databases date in the same words.
The code names a localization key under `Epoch`, so the reader meets the age in the interface language.
A label is listed in every spelling the site uses, such as 商晚期 beside 商代晚期.
The longer spelling wins wherever both would match, so a dynasty never stands for one of its periods.
The table runs from the oldest age to the latest, and a plain dynasty precedes its periods.
A style lists its pictures in that order, and a code ranks by its first row.

## `fanqie`

The list names the rime books the reading view shows under the script box, one block per book and source.
Each block is fetched once per character and kept in the workspace database.
Each row names its source, shown as the chip at the right of its lines.

## `fanqie[0]` 廣韻 Kaom

The Kaom rows read the 古音小鏡 tables.
They post the site's form with the character in `word` and the book in `t`.
The site answers with the whole rime table the character sits in.
Only the cells marked by the match pattern are read.
The site refuses a post within a few seconds of the last and prints the busy text instead.
So the interval holds the next post back, and a busy answer is asked again later.
A cell holds one group per 小韻 parted by a line break, which the split pattern separates.
The head, column, rounded and spelling patterns read the rime, division, tone, 合口 underline and 反切 of each group.

## `fanqie[1]` 廣韻 Wiktionary

This row reads the same placements from the English Wiktionary's Middle Chinese data module.
It is fetched with a GET as plain text, one reading per quoted line.
A character without a module answers not found and is left alone.

## `fanqie[2]` 集韻 Kaom

This row posts the same Kaom form as the 廣韻 row with the book changed in `t`.

## `shengfu`

The key names the one source of a character's phonetic series, the character whose sound the graph was borrowed for.
It reads the second field of each row of the Zhengzhang 上古音 module on the English Wiktionary.
A character listed twice under different series has both printed, joined by a middle dot.
A character without a module answers not found and is left alone.

## `hypothesis`, `anatomy` and `tone`

These keys name the sibling files `hypothesis.json`, `anatomy.json` and `anatomy_tone.json`.
One anatomy rule serves each borrowing language, and the Sinitic languages share one rule.
The tone rows tell the anchor dropdown which tone classes of the hypothesis a reflex contour may descend from.

## `reflex`

The list names the readings of a character in the languages that borrowed it, one rule per language.
They are fetched once per entry when the entry has none.
They are then stored on the entry, where the user may correct them.
The rule order is the fetch order only, and the `order` key declares the display order.
The named groups `text`, `kind`, `romanization`, `meaning`, `note` and `main` give the reading and what stands around it.
The main group says whether the reading is the one in common use.
A double-bracketed piece of the format is kept only when its groups were captured.
Every slash and bracket is dropped from the text.
So a bare reading can later be parted into onset, nucleus and coda.
The `split` key cuts a text and a romanization listing several readings into one row per reading, paired by position.
The `region` key names the place every row of the rule is taken from, shown when the language is hovered.
The `gloss` key reads the source note and optional meaning of one reading.
It is looked for after the match as far as the `until` key, here the next language header.
So a Hakka reading never takes the gloss of a Southern Min reading further down.
The `folded` key hides the rule's rows under a fold below the visible rows.
Every row is recast through the respelling of its own language pack while the respelling setting is on.
Under every rule, a character whose readings all share one text has them all marked.
A row repeating the kind, text and romanization of an earlier row is dropped.

## `reflex[0]` Korean

The rule asks the Naver hanja dictionary's search API for the character as a 낱자.
It sends the Referer the API demands.
It reads each 훈 음 pair of the first entry's `expKoreanPron`, parted by a comma.
The last word of a pair is the reading and everything before it the meaning.
The first pair is the representative 훈음, so it is the one marked in common use.
The epithet key names the template printed after the headword.
The clip key cuts the 두음법칙 form out of it.

## `reflex[1]` Mandarin

The rule reads the Sinological IPA of each Standard Chinese block with its Hanyu Pinyin as romanization.
The first block on the page is the primary pronunciation, so it is the one marked in common use.

## `reflex[2]` Cantonese

The rule reads the Sinological IPA of each Standard Cantonese block with its Jyutping line as romanization.
The split parts a block listing two readings into two rows.
The Jyutping romanization allows no tag but sup, so it can never stretch into the Yale line.

## `reflex[3]` Japanese

The rule reads every reading of each on'yomi line, and the kind is found by looking back within the line.
The reading the page lists as Jōyō is marked.
Older spellings follow in sup, and their historical or ancient label is read ahead as the source note.
The look-ahead stops at the sup edge so the current reading never borrows the label of its older shape.
A historical spelling equal to the current one folds into the marked row.

## `reflex[4]` to `reflex[9]` Gan, Hakka, Jin, Southern Min, Wu and Xiang

The six dialect rules are folded away under the rows above and share one shape.
Each anchors on the block whose head names its region.
The region is matched as link text, since an address may carry another place's name.
A scheme spelled with combining marks is matched loosely, so a change of encoding does not lose it.
Southern Min and Wu list one IPA line per place.
So the line whose bracket names Xiamen or Shanghai is the one read.
A block whose IPA lines name neither place gives no row.
Gan, Hakka and Jin have one IPA line per block and read it whatever its bracket says.
Xiang may list an old-style and a new-style IPA line per block, each its own row through a look-behind.
The bracketed style label is the note of that row.
The new-style row is captured as main, so it is the one in common use.
The page order of the two does not matter.
The superscript key raises every tone digit of the romanization.
Wugniu writes the tone before the syllable, as `7oq`, so the Wu recast moves that digit to the end.
The gloss is still looked for under `7oq`, the form the page writes.
Every dialect rule keeps every block, so each pronunciation section's reading is listed.
The first row of each is marked in common use, unless a main group already marked one.

## `order`

The key declares how every view lists the reflex rows of an entry.
Its language list puts Korean first, then Japanese, then the Chinese varieties.
Its kind list orders the Japanese readings as Go-on, Kan-on, Tō-on, Sō-on and Kan'yō-on.
Within one language and kind, the main reading comes first and the rest follow by text.
A language or kind left out of the lists follows the declared ones by name.
The order is applied on every read, so stored rows never need renumbering.

## `frequency[0]` 四庫全書

The 書同文 site counts a character in the 四部叢刊 and the 文淵閣四庫全書 e-text.
Both corpora hold pre-modern text only, so no modern usage leaks into the figure.
The pattern reads the 四库全书 row's raw count, and that count is the stored ground truth.
The site prints simplified names, so the row label is written simplified in the pattern.
The page writes every CJK character as a numeric entity, so the attempt is decoded before the patterns run.
The corpus size of about 698 million characters is derived from the site's own per-ten-thousand coverage.
The ladder rungs are fixed in the engine at one word in 10,000, 100,000 and 1,000,000.
So a count of 69,810 and over is core, 6,981 and over everyday, and 698 and over advanced.
The page title names the character looked up, so the confirm guard fails for an unknown one.
A character the corpora never use has no table, so the attempt yields nothing.
Classical Chinese entries are single characters, and a word of several characters has no table.

## `anatomy.json`

The file declares how a reflex reading is cut into onset, vowel, coda and tone.
Each row names one reflex language or a list of them, and its `ipa` object holds the rules.
`decompose` controls Unicode decomposition before the ordered rewrite pairs run.
`rewrite` pairs are regular-expression substitutions applied in order to normalize transcription.
`match` captures onset, vowel and coda, with tone captured when the language rules include it.

## `anatomy_tone.json`

Each row maps a contour, as `anatomy.json` cuts it, to the `hypothesis.json` tone classes it may descend from.
The anchor dropdown marks every placement whose class the reflex reading's contour allows.
So a Mandarin reading in 55 marks the placements of class 1, and of class 7 for a checked syllable.
The tables follow the tonal split notes kept in the working documents.
A contour is the citation tone as Wiktionary writes it, its superscripts turned into digits.
A sandhi form such as `214-21` is looked up by its citation part alone.
Common variants of one tone are listed beside the standard form, so `34` and `35` both read as Cantonese 3.
The `language` key names one borrowing language or a list.
The `class` key maps each contour to one class or a list.
The letter S marks the sonorant split of 上 and 入, and a checked syllable is an 入 syllable.

## Tone rows by language

Mandarin maps 55 to 1 or 7, 35 to 2 or 8, and 214 to 3 or 4S.
Mandarin maps 51 to 4, 5, 6 or 8S.
Cantonese maps 55 to 1, 21 to 2, 35 to 3 and 13 to 4S or 4.
Cantonese maps 33 to 5 and 22 to 6.
Cantonese checked 5 and 3 are 7, and checked 2 is 8S or 8.
Southern Min maps 44 to 1, 24 to 2 and 53 to 3 or 4S.
Southern Min maps 22 to 4 or 6 and 21 to 5.
Hakka maps 24 to 1, 4S or 4, 11 to 2 and 31 to 3.
Hakka maps 55 to 5, 6, 8S or 8.
Wu maps 53 to 1, 23 to 2, 4S, 4 or 6, and 34 to 3 or 5.
Gan maps 42 to 1, 35 to 2 or 5 and 24 to 2.
Gan maps 213 to 3, 4S or 5, and 2 to 4 or 6.
Jin maps 11 to 1 or 2, 53 to 3 or 4S, and 45 to 4, 5 or 6.
Xiang maps 33 to 1, 13 to 2 and 41 to 3 or 4S.
Xiang maps 21 to 4 or 6 and 45 to 5.
Checked tones follow in each language as listed in `anatomy_tone.json`, with Mandarin 入 spread over every tone.

## `hypothesis.json`

The tables are the user's own hypothesis of how a rime-book placement sounds.
The reading view prints the reading and the class before the placement, as in /ngoʔ/ 4성S 疑 模[模] 一等 上.

## `initial`

The table gives the onset of every initial the rime books name.
Onsets starting p t c s k ʔ h are voiceless.
Onsets starting b d q z g x are voiced obstruents.
Onsets starting m n j w l are sonorants.

## `place`

The list groups the initials by articulatory place, in the order the rime page sections them.
Each place lists its initials in the order its rows should follow.
`name` is printed through the localization key `Yunjing.Place` plus the name capitalized, or as written without one.
An initial no place lists falls into a trailing section of its own.

## `final`

The table is keyed by the rime, a space and the division.
A space and 合 are appended for the rounded medial.
A rime with a 重紐 letter is looked up as written and then without the letter.

## Tone classes

The tone table splits each rime-book tone into reconstruction classes, one row per class in order.
`onset` is a pattern matched against the onset the initial table gave.
The first row whose pattern matches wins, and a row without `onset` matches every onset.
`rewrite` lists rules run over the whole syllable, onset and final joined.
`class` names the tone class printed after the reading, running 1 to 8 with S for the sonorant split.
平 gives 1 to a voiceless onset and 2 to every voiced one.
It also unfolds a voiced obstruent into the aspirated voiced series the pack otherwise folds.
So 並 reads b in the table and bh under 平, while 匣 x has no such series.
上 adds the glottal stop after the coda and gives 3, 4S or 4.
去 adds h and gives 5 or 6.
入 turns a nasal coda into a stop and gives 7, 8S or 8.

## Stamp

Commit: `c2f335c331fe`
- `anatomy.json` `89e0b3ff3c6880e9`
- `anatomy_tone.json` `3fff4d02cbf886af`
- `hypothesis.json` `7d073aef2db89e68`
- `source.json` `602ad2a063e6bf5b`
