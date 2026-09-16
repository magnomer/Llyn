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
Every slash and bracket is dropped from the text, so a pronunciation is stored bare.
A bare reading can later be parted into its onset, nucleus and coda.
The `split` key cuts a text and a note listing several readings into one row per reading.
`\s*[,/]\s*` parts `ghah4 / gheh4` and `/ɣaʔ²/, /ɣəʔ²/` into two rows paired by position.
The `region` key names the place every row of the rule is taken from, shown when the language is hovered.
The `remark` key reads what the page says of one reading, such as `literary`, shown when the reading is hovered.
It is looked for after the match, as far as the `until` key, here the next language header.
So a Hakka reading never takes the note of a Southern Min reading further down.
The `folded` key hides the rule's rows under a fold below the visible rows.
Every reflex row is recast through the respelling of its own language pack while the respelling setting is on.
So the Cantonese rows read as a Cantonese entry's own IPA does.
Under every rule, a character whose readings all share one text has them all marked.
Such a character is said one way only.
Every mark is a star the user may move in the editor.
A row repeating the kind, text and note of an earlier row is dropped.
So a character with two Mandarin readings and one Wu reading lists that Wu reading once.

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
Its region is Beijing.
The first block on the page is the primary pronunciation and so the one marked in common use.

## `reflex[2]` Cantonese

The Cantonese rule reads the Sinological IPA of each Standard Cantonese block whole with its Jyutping line as the note.
Its region is Guangzhou.
The split parts a block listing two readings, so `lʊŋ²²` under `lung⁶` and `nʊŋ²²` under `nung⁶` are two rows.
The superscript key raises the tone digit of the Jyutping note.
The first row is again marked in common use.
The Jyutping note allows no tag but sup.
So it can never stretch into the Yale line when the IPA lies far below.

## `reflex[3]` Japanese

The Japanese rule reads every reading of each on'yomi line, あく and お both under Kan-on.
The kind is found by looking back within the line.
The reading the page lists as Jōyō is marked.
Older spellings follow in sup, and their historical or ancient label is read ahead as the remark.
The look-ahead stops at the sup edge so the current reading never borrows the label of its older shape.
A historical spelling equal to the current one repeats kind and text and so folds into the marked row.

## `reflex[4]` to `reflex[9]` Gan, Hakka, Jin, Southern Min, Wu and Xiang

The six dialect rules are folded away under the rows above and share one shape.
Each anchors on the block whose head names its region: Nanchang, Sixian, Taiyuan, Xiamen, Shanghai and Changsha.
The region is matched as link text, since a link address may carry another place's name.
Each reads the romanization line the pack names as the note: Wiktionary, Pha̍k-fa-sṳ, Wiktionary, Pe̍h-ōe-jī, Wugniu and Wiktionary.
A scheme spelled with combining marks is matched loosely, so a change of encoding does not lose it.
Each then reads the Sinological IPA line of its block.
Southern Min and Wu list one IPA line per place.
So the line whose bracket names Xiamen or Shanghai is the one read.
A block whose IPA lines name neither place gives no row, since only that place is wanted.
Gan, Hakka and Jin have one IPA line per block and read it whatever its bracket says.
Xiang may list two IPA lines per block, one bracketed old-style and one new-style.
Its block context sits in a look-behind, so each IPA line of the block is its own row.
The bracketed style label is captured as the remark of that row.
The new-style label is also captured as the main group, so the new-style row is the one in common use.
The old-style row is never marked, whatever order the page lists the two in.
A block with one unlabelled IPA line gives one row with no remark, marked as the first.
A block listing two readings is parted by the split into two rows.
The superscript key raises every tone digit of the note, so a Nanchang `lung5` is stored as `lung⁵`.
Wugniu writes the tone before the syllable, as `7oq`.
The Wu recast moves that leading digit to the end before it is raised, so the note becomes `oq⁷`.
The remark is still looked for under `7oq`, the form the page writes.
The remark of each reading is read from the note box under the block, such as `literary` or `vernacular (“difficult”)`.
Every dialect rule keeps every block, so a character with several pronunciation sections lists each section's reading.
The first row of each is marked in common use, unless a main group already marked one.

