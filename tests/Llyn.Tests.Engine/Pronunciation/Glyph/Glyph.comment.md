# Glyph

This folder holds the tests of the Han-character facts.
They cover the glyph, script styles and their epochs, and the phonetic series.

## `TEngineScript`

Covers the engine's script fetch over a fixture pack with two styles behind stubbed pages.
A start posts each style's form, reads every picture and caption the pattern names, and fetches the bytes.
The rewrite and prefix shape the picture address before it is fetched, and the gloss is read once per style.
A picture that cannot be fetched is skipped, and the positions count the kept ones.
A start stores the sets and raises the script bulletin.
A character every database answered for yet none drew still raises the bulletin.
It is asked once per session and stores nothing.
A rebuild drops the stored sets and fetches again, asking even sources once silent.
A language whose pack lists no styles reads empty and asks nothing.
A fetch lists its pictures in the pack's declared epoch order, an undated picture last.
The image sort orders by character, then pack style, then declared epoch.
Characters follow the headword's spelling, and one the headword lacks follows by code point.
The stored id breaks the last tie, so input order never decides.
An undeclared style follows the declared ones, and an undeclared epoch precedes an undated one.
A pack pattern that times out answers no pictures and not reached, so the miss is not remembered.

## `TEngineShengfu`

Covers the phonetic series from the pack rule to the block the fanqie box prints.
One start fetches the rime books and the series together and stores one series row.
The series rides on the character's first block, which is where the box prints it.
A character listed under two series keeps both stems, in the order the answer gave them.
A pack declaring no series source stores no series row, and its block carries no stem.
A pack pattern that times out answers no series and not reached, so the miss is not remembered.
The settle helper waits until no fanqie fetch runs, since the series rides on that fetch.

## `TEngineStem`

Covers the series the xiesheng panel browses by, from the fetched row to the page and its entries.
Fetching a character's series makes a row a chip can open.
The page carries the member character, and the entry list reaches the entry written with it.
A blank or missing key finds no series.
A pack declaring no series source stores no series, so no chip of its language opens one.
The page lists its characters by code point, whatever order their Shengfu rows were stored in.
U+F900 sits above the surrogate range, so UTF-16 order would list U+20000 before it.
U+F900 is a compatibility ideograph that normalisation would fold into U+8C48.
The stored read returns it unchanged, so nothing on the path normalises.
The kindred entries list by headword, or descending when the vista is set to reverse.
Entries are saved against headword order, so storage order cannot pass for it.
A member without an entry stays bare and still carries its representative reading.
Reading the page makes no entry.
Opening a member marks only that member opened, and only its guises fold a row.
The entry page fold stays closed, and opening a bare member stores nothing.
No member mark is left behind after a member closes.

## `TEpoch`

Covers the chronology a script source prints in a caption, over the shipped Classical Chinese pack.
A language without script styles lists no labels.
A dated caption gives up its age as a stored code and keeps the vessel's name untouched.
A dynasty written with its period beats the dynasty alone, so the longer label always wins.
A caption that is nothing but an age leaves no name behind.
A label that merely begins a name, as 商 begins 商鞅方升, dates nothing.
An age the pack does not list is left inside the caption.
A later pack can still cut it out.
Every code the pack names has its own text in both shipped languages, so no picture prints a bare code.

## `TGlyph`

Covers the glyph section.
The split keeps Han characters only, each once, in first-seen order, and keeps a beyond-plane ideograph whole.
The divide reads the transcription of the glyph scheme over the headword.
A headword without a Han character and an empty glyph row divide into no cells.
So a native Korean word shows no Hanja row.
Each Han cell carries the glyph language and any other cell carries none.
So only a Han cell opens an entry.
Resolving a character makes its entry in the glyph language once and finds that same entry afterwards.
An entry of the same character in another language is never taken.
The engine hands the section back for a pack that declares one and null for a pack that does not.
The reading view lists only the filled rows outside the glyph scheme, and every filled row without a section.
The character order sorts by code point, and UTF-16 ordinal order would put a beyond-plane ideograph before U+F900.
A text sorts before a longer text it begins.
A blank language answers the empty font, so the theme's own font stands.
The font fact reads the font's members rather than building a font.
So it reaches product code only through the relay.

## Stamp

Commit: `c2f335c331fe`
- `TEngineScript.cs` `6a91e6d779c08fe6`
- `TEngineShengfu.cs` `2c50687e1b2839b6`
- `TEngineStem.cs` `007dc5a5a957da60`
- `TEpoch.cs` `36c2d103a0f377c7`
- `TGlyph.cs` `744fa63fe86f45ed`
