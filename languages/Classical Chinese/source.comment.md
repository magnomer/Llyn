# source.json

## pack

Language selector pack for Classical Chinese.
Pronunciation and audio sources are listed separately.
Either list can be filled without changing the application.

## `script`

The script list names the character styles the reading view shows above the first meaning, one row per style.
Each style is fetched once per character from the Academia Sinica 小學堂 databases and kept in the workspace database.
Each style posts the database's own search form with the character in `EudcFontChar`.
The site has no query API, and every database needs its own hidden fields or answers 500.
The match pattern reads every result cell.
Group 1 is the glyph image the site renders from its private fonts.
Group 2 is the caption under it, such as the inscription and its era.
The rewrite lifts the image size so the stored picture is the large original, and the view scales it down.
The small seal style also reads the 說文 gloss the page prints under the results.

## `fanqie`

The fanqie list names the rime books the reading view shows under the script box.
It shows one block per book and source.
Each block is fetched once per character and kept in the workspace database.
Each row names its source, shown as the chip at the right of its lines.

## `fanqie[0]` 廣韻 Kaom

The Kaom rows read the 古音小鏡 廣韻·集韻 tables.
Each book posts the site's search form with the character in `word` and the book in `t`.
The site answers with the whole rime table the character sits in, marking the character with the match pattern.
Only the marked cells are read.
They give the row's initial, the cell's rime with its rime heading, and the column's division and tone.
One placement reads 疑 模[模] 一等平.
The site refuses a post within a few seconds of the last and prints the busy text instead.
So the interval holds the next post back, and a busy answer is asked again later.
A cell of the table holds one group per 小韻 parted by a line break.
The split pattern parts them.
The head pattern reads each group's rime, rime heading and 韻鏡 division.
The column pattern reads the column's division and tone.
The rounded pattern reads the underline the site draws under a 合口 character.
The spelling pattern reads the 反切 printed as the link of each group.

## `fanqie[1]` 廣韻 Wiktionary

The Wiktionary row reads the same 廣韻 placements from the English Wiktionary's Middle Chinese data module.
It is fetched with a GET as plain text, one reading per quoted line such as 疑模一開 平五乎.
The line pattern reads it apart into initial, rime, division, 開合, tone and the 反切 spelling.
A character without a module answers not found and is left alone.

## `fanqie[2]` 集韻 Kaom

The 集韻 row posts the same Kaom form as the 廣韻 row with the book changed in `t`.

## `hypothesis`

The hypothesis key names the file beside this one holding the user's own reconstruction tables, `hypothesis.json`.
It is kept apart because Classical Chinese will carry a system of its own.

## `reflex`

The reflex list names the readings of a character in the languages that borrowed it, one rule per language.
They are fetched once per entry from the web when the entry has none.
They are stored on the entry, where the user may correct them.
Each rule fetches the page with a GET and reads one row per match of its pattern.
The named groups `text`, `kind`, `note` and `main` give the reading and what stands around it.
The kind is printed before the reading and the note after it.
The main group says whether it is the one in common use.
The format joins the groups into the stored text.
A double-bracketed piece is kept only when its groups were captured.
Every reflex row is recast through the respelling of its own language pack while the respelling setting is on.
So the Cantonese rows read as a Cantonese entry's own IPA does.
Under every rule, a character whose readings all share one text has them all marked.
Such a character is said one way only.
Every mark is a star the user may move in the editor.

## `reflex[0]` Korean

The Korean rule asks the Naver hanja dictionary's search API for the character as a 낱자.
It sends the Referer the API demands.
It reads each 훈 음 pair of the first entry's `expKoreanPron`, parted by a comma.
The last word of a pair is the 음 and everything before it the 훈.
So 강 이름 호 gives 호 under 강 이름.
The first pair is the dictionary's representative 훈음 and so the one marked in common use.
The epithet key names the template every list prints after the headword.
`{note} {text}` reads 희롱할 롱(농).
The clip key names the pattern cut out of it, here the 두음법칙 form in brackets.
So the list reads 弄 [희롱할 롱].

## `reflex[1]` Mandarin

The Mandarin rule reads the Sinological IPA of each Standard Chinese pronunciation block with its Hanyu Pinyin as the note.
The first block on the page is the primary pronunciation and so the one marked in common use.

## `reflex[2]` Cantonese

The Cantonese rule reads the Sinological IPA of each Standard Cantonese block whole with its Jyutping line as the note.
Its rewrite turns every slashed reading into a bracketed one.
So a block listing two readings stores [lʊŋ²²], [nʊŋ²²] under lung6 / nung6.
The first block is again marked in common use.
The Jyutping note allows no tag but sup.
So it can never stretch into the Yale line when the IPA lies far below.

## `reflex[3]` Japanese

The Japanese rule reads every reading of each on'yomi line, あく and お both under Kan-on.
The kind is found by looking back within the line.
The reading the page lists as Jōyō is marked.
