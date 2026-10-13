# Fanqie

This folder holds the rime-book placements of Han characters and the Diwei category pages built from them.
A placement belongs to the character and not to the entry, so every entry holding that character shares it.
A Diwei category is derived from stored fanqie rows rather than typed by the user.
The engine groups and sorts here, so the shell only draws what it is handed.

## `LFanqieBook`

One rime book a language pack lists under `fanqie`, such as 廣韻 or 集韻.
The book names the web database that places a character in its rime tables and how its answer is read.
The engine knows nothing of the site, only that a form is posted and a table is read.
Every URL, field and pattern is pack data, so a new book needs no code.
The form fields are posted in pack order with `{word}` replaced by the character.
A book with no field is fetched with a GET instead, the character escaped into the address.
The cell pattern marks the queried character inside a table cell, and only cells carrying it are read.
The busy pattern matches an answer that means the site refused the request.
A refusal is a throttle, not a miss, so the character is asked again later.
The interval is the seconds the engine waits after one post before the next to any book.
The site refuses a request that follows another within a few seconds.
The split pattern cuts one cell into groups, each with its own head.
A cell of the 廣韻 tables holds one group per 小韻.
The head pattern reads the named parts `rime`, `heading` and `division`.
Without it the line keeps the group's head as text and the parts stay empty.
The column pattern reads `division` and `tone` from a column heading.
The column's division wins over the group's, since the group's follows the 韻鏡 placement.
The rounded pattern matches a group whose marked character is rounded, since the 廣韻 tables underline it.
The source is the site shown as the chip at the right of its lines.
Two rows may name the same book from two sites, and each keeps its own lines under its own chip.
A row without a source is labelled by its book name.
The line pattern reads one placement per match over the whole answer, for a site that answers in plain text.
Its named parts are `initial`, `rime`, `heading`, `division`, `tone`, `rounded` and `spelling`, any of them optional.
The spelling pattern reads the 反切 spelling, which the 廣韻 tables print as the link that opens the 小韻.

## `LFanqieSource`

The port that asks one rime book's site for one character's placements.
It is a fetch over the network, not storage, so it is not a Vault.
`LFanqieSourceHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine holds the port from the rig and never names a client.
It answers the placements and whether the site answered at all.
An unreached site answers an empty list and false, so the caller can retry later.

## `LFanqieRow`

One placement of one character in one rime book, as fetched and as stored.
The row belongs to the character, not to the entry.
The text is one line, such as `疑 模[模] 一等平`.
It reads initial, rime with its rime heading, then division and tone.
The parts the line was built from are kept beside it.
The reading the hypothesis derives from them is stored beside them too, so the view prints it as stored.
The position is web hit order, so it keys the upsert but never the order a view shows.
The trailing capital of a rime such as 真B is the 重紐 letter.
It is kept as fetched and read out of the key the view links to.
The stored id is zero before the archive has kept it.
The save upserts on the natural key, so the id survives a refetch and an Anchor on it stands.
The label and summary are filled by the format pass for the view.
The representative rank is the row's place among the character's representative readings, one first, zero when unmarked.
The ranks of one character run from one without a gap, so the number is the order itself.

## `LFanqieRankLast`

The rank that asks for a row to be placed last among its character's representative readings.
The archive clamps any rank past the end, so the largest value always means last.

## Derived members of `LFanqieRow`

A row is marked when it is a representative reading at all, and primary when it is the first.
A row is parted when the line was read into parts, by an initial or a rime.
An unparted row prints its raw text and shows no medial.
The medial is 開 or 合 as the source marked the placement.
The cell is the key of the rime category the row belongs to, as `LDiwei.LDiweiRimeFormat` writes it.
The view links the cell to that category by this key.
The remainder is the raw line when nothing was read into parts.
So a book without a head pattern still shows what it answered.

## `LFanqieRankResolve`

The rank a mark request asks the archive for, given the row's current rank.
Raising moves the row up one place, and raising the first wraps it to last.
Without raising, the request toggles the mark.
An unmarked row is marked last and a marked one is unmarked.

## `LFanqieRowMatch`

Whether the row places the character in the book as read from that book's own site.
Two sites may serve one book, so the source is part of the match.

## `LFanqieCharacterScan`

The distinct characters the rows place, in first-seen order.
The grouping follows that order, so the blocks stand as the headword spells them.

## `LFanqieRowScan`

The rows placing the character in the book, or null when there are none.
Null rather than empty lets the grouper skip a missing block in one pattern test.

## `LFanqieRowSort`

The one declared order of fanqie rows, which every view reaches through the clerk or the grouper.
A ranked row comes before an unranked one, and lower ranks come first.
Then the rows follow the order the pack lists its books.
A book the pack no longer lists goes last.
Then the reading sorts by ordinal, and the stored id breaks the last tie only to keep the result stable.
Fetches store rows in arrival order, so the stored order never decides what the user sees.

## `LFanqieRowFormat`

The row with its label and summary filled for the view.
The pattern is the localized format around the tone class, and an empty one prints the class bare.
The summary prints the rime key, or the raw line when no key could be made.

## `LFanqieGroup`

One block of the fanqie box: the rows of one character from one book and source.
The heading is the character, printed once at its first block when the entry has several.
The label is the book name, printed once and blank while the book stays the same.
The stems are the character's phonetic series as separate keys, carried only by its first block.

## `LFanqieGroupScan`

Groups the rows by character and then by book, with characters in the order the rows bring them.
The rows are sorted by `LFanqieRow.LFanqieRowSort` first, whatever order the caller hands them in.
A book's block stands where its first sorted row stands.
So a book holding a ranked row comes first, and unranked books keep the pack's order.
The series of a character is put on its first block alone, so the line is printed once.
The stored series text is cut on the separator, so each series stands as its own chip.
Handing no series over leaves every block without one, as a pack declaring none does.

## `LFanqieReadingFormat`

The group overload gives the headword's representative readings, drawn under the headword itself, such as `/bhiaeng, bhien 'an/`.
Each character gives its marked readings through `LFanqieMarkedScan`, in the order the headword writes the characters.
The character-list overload lays the line out, so the headword and a series member print alike.
Each character's readings are separated by commas, the characters by a space, and the whole stands between slashes.
A character with no marked reading is left out.
Nothing ranked at all gives an empty line, and the view then shows none.

## `LFanqieMarkedScan`

The readings of the character's marked rows in rank order, rows of equal rank kept in the order given.
A row the user never marked as representative, or that stored no reading, is left out.
The mark is the user's per-row choice, stored as the `representative` rank of the row.

## `LDiwei`

One 音韻地位 category of a language.
It is an initial, a rime or a tone class that placements point to.
It is a shared node, like a Tag, but derived from stored fanqie rows rather than typed by the user.
Every character whose placement carries the part links to the same row, so a category can be browsed.
The kind is `initial`, `rime` or `tone`.
The key is the part as the category names it: 來, `寒W I` or the class `6`.
The count is how many entries of the language carry a character placed in the category.
The chosen flag is set when the rows are listed, if the open view has the category selected.

## Kinds in `LDiwei`

The initial kind is made from a placement's initial, such as 來.
The rime kind is made from a placement's rime, division and 開合, such as `寒W I`.
The tone kind is made from the tone class the hypothesis gives a placement, such as `6`.
`LDiweiKindRead` returns the rime kind when the final flag is set and the initial kind otherwise.
`LDiweiFinal` reads the same pairing back from a row.

## `LDiweiRounded`

The mark `W` ending a rime key of a 合口 placement, so 開 and 合 name two rows.
It rides on the rime itself, since it names the rime rather than the division.

## `LDiweiDivisions`

The division words 一 to 四 the rime books print.
`LDiweiRomans` holds the Roman numeral each becomes inside a rime key.

## `LDiweiChongniuRead`

The 重紐 letter the source hung on a rime, such as the `A` of 寒A, or empty.
The letter tells the two halves of a 重紐 pair apart, and `X` marks a row outside the pair.

## `LDiweiRimeNormalize`

The rime as a category keys it, with the trailing 重紐 letter dropped.
So 寒A and 寒 name one row.

## `LDiweiRimeFormat`

The key of a rime category.
It is the rime without its 重紐 letter, `W` when 合口, then the division as a Roman numeral.
So 模 一 開 keys `模 I` and 寒 一 合 keys `寒W I`.
Every division and 開合 gets its own row.
A division outside 一 to 四 stays as printed.
Empty with an empty rime.

## `LDiweiDivisionFormat`

The Roman numeral of a division 一 to 四, or the division as printed.
The rime key and the division label both print it, so it is written once.

## `LDiweiRankRead`

The sorting rank of a category page section: its table or pack position, then unknown ones, then blank.
An initial's page sections by division, ranked in table order.
A rime's page sections by place, ranked as the hypothesis lists the places.

## `LDiweiRankNormalize`

An unlisted rank turned into the last position, so it sorts after every listed one.

## `LDiweiPage`

The page of one Diwei category as the engine composes it.
It holds the language, the key, and the sorted sections.
The blank page stands for no category, so the veneer never branches on null.

## `LDiweiSection`

One heading of a Diwei page.
It is a division on an initial page and a place of articulation on a rime page.
It carries its label already formatted, the tally rows of the shown set, and its sorted lines.
The switched flag says whether a respelling exists for the language.
The respelled flag says which set the page shows.
The label is the localized heading, or the raw heading when no text is found.
A section is data once built, so the shells carry it without a projection.

## `LDiweiSectionScan`

Groups the fanqie rows of one category under their headings and lines, sorted as the page prints them.
Headings follow the rank `LDiwei.LDiweiRankRead` gives them, the lowest first.
Unknown divisions and unplaced rimes share one rank, so the raw heading breaks the tie.
It compares by code point through `LGlyphOrder`, then ordinally as a final stable key.
The sort is stable, so the order the rows were stored in never reaches the page.
The localize seam answers null for a missing key, so the raw heading stands in.
Rows are walked by character in code point order through `LGlyphOrder`, the stored id last.
So each line lists its characters by that declared rule, never by fetch order.
Characters gather in a list the line record already holds, so the scan mutates nothing after it returns.

## `LDiweiCharacterAdd`

Adds a placed character once, skipping blanks.
The scan feeds it rows in code point order, so appending keeps the line in that order.

## `LDiweiLineSort`

Orders the lines of a section by Hypothesis rank, unranked last, then by label in code point order.
Labels are rime or initial characters, so `LGlyphOrder` compares them, never UTF-16 order.
An unrounded line precedes a rounded line of the same rime, as rhyme tables print open before closed mouth.
The first character of the line is the last key, and the sort is stable.
Rank, label and rounding already identify a line, so no tie is left to storage order.

## `LDiweiTallyScan`

The tally lines under one heading that carry marks in the shown set, each holding only those marks.

## `LDiweiLabelFormat`

The heading of a division.
It is the localized pattern over the numeral and its roman form, or the blank label.

## `LDiweiPlaceFormat`

The heading of a place of articulation.
It is its localized name, or the unplaced label.

## `LDiweiLine`

One line of a Diwei page section.
It holds a label, its reconstructed reading, and the characters placed there.
An initial page lines by rime and roundedness, a rime page by initial ranked under the Hypothesis.
`LDiweiSection.LDiweiSectionScan` builds the lines and fills their characters before the page is handed out.
The reading is between slashes, or empty when no Hypothesis gives one.
The rounded flag is true for the rounded line of an initial page.
The rank is the Hypothesis rank of the initial on a rime page, minus one when unranked.
The characters are in the order they were met, blanks and repeats dropped.

## Stamp

Commit: `c2f335c331fe`
- `LDiwei.cs` `a4d9ffba3bed0f78`
- `LDiweiLine.cs` `a7890669235899b6`
- `LDiweiPage.cs` `db55061a6b4e8db3`
- `LDiweiSection.cs` `2ed919a6c81057e0`
- `LFanqieBook.cs` `fd87cb371d75c44d`
- `LFanqieGroup.cs` `70925acd222e5431`
- `LFanqieRow.cs` `98d612b9e92f73e1`
- `LFanqieSource.cs` `74ef99396d60bbbb`
