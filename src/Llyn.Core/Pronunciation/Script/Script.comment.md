# Script

This folder holds what a Han-character language pack declares about the written form of its words.
It covers glyph pictures in historical hands, chronology labels and typography.
It stands apart because every site, pattern and label is pack data.
The engine knows only that a form is posted and a pattern is matched.
So a new database needs no code.

## `LEpoch`

One chronology label a pack lists under a `script` style, paired with the code Llyn stores for it.
A source names the age of an inscription in its own language, such as 西周早期 under a bronze.
The label is the source's word and the code is Llyn's, so the interface language decides what the reader sees.
The code names a localization key under `Epoch`.

## `LEpochResolve`

Cuts a known chronology out of a caption, returning its code and the caption that remains.
A caption carries its chronology wherever the source likes, so every word of it is weighed.
A word counts as a chronology only when a label spells the whole word or runs up to 或.
That mark is the source's own way of dating a piece between two ages.
The longest label wins, so a dynasty with a period is never mistaken for the dynasty alone.
A vessel named 商鞅方升 therefore keeps its name, because 商 there is the start of a longer word.
The cut word leaves single spaces behind it, so a name and its collection number read as one line.
Nothing matches when the pack lists no label for the age printed, and the caption is returned whole.
An unmatched chronology stays inside the caption, so a later pack can still cut it out.

## `LFont`

The typography a language pack declares for showing its own words.
One record carries one role, named by `LFontRole`.
The editor and the reading view are given the same record, so they never drift apart.
A family or size the pack leaves out is null or zero, and the theme's own value then stands.
The slant is lowered, so `Italic` arrives as `italic`.
Any other word arrives null, so every surface maps only the two slants.
English glosses are set in italic, as a translation under a sentence is by convention.

## `LFontFace`

The declared family, or null when the pack declares none or a blank one.
No surface then builds a font family from nothing.

## `LFontSized`

The declared size, or null unless it is finite and positive.
Every surface then sets only a usable size.

## `LFontRole`

Which of a pack's declared typographies is being asked for.
The headword and the example are separate because a headword is read as a specimen and an example as prose.
A Gloss is asked of the pack the sentence is in, whatever language the Gloss itself is written in.
The glyph role styles the character chips of the glyph row.
A traditional character is read in a serif face, so the pack names one apart from the headword face.
A pack that declares none falls back to its example typography, which the Han packs keep serif.

## `LScriptGroup`

One block of the script box, the images of one character in one style.
The engine groups the images, so the shell only draws the blocks it is handed.
The heading is printed once at its first block when the entry has several characters.
The gloss is the first one any image of the block carries.

## `LScriptGroupScan`

Groups the images by character and then by style, in the order the pack lists the styles.

## `LScriptImage`

One glyph picture of one character in one style, as fetched and as stored.
The picture belongs to the character, not to the entry, so every entry holding that character shares it.
The bytes are the original the source drew, a PNG with the glyph in black on a clear ground.
The view scales them down itself.
The position records arrival order only, so no view orders by it.
The caption is the one the source printed, without its chronology.
A chronology the pack does not list stays inside the caption, in the source's own language.
The gloss is repeated on every picture of the style.
The epoch code is stored once and read back, so the age is never matched again and never fetched again.
The view turns it into the reader's language, so changing that language changes what is printed.
The id is zero for a picture just fetched and not yet stored.
It only breaks the last tie of the sort.

## `LScriptCharacterScan`

The distinct characters the pictures draw, in first-seen order.
The grouping follows that order, so the blocks stand as the headword spells them.

## `LScriptImageScan`

The pictures of a character in a style, or null when there are none.
Null rather than empty lets the grouper skip a missing block in one pattern test.

## `LScriptImageSort`

The pictures in the one declared order every view shows them in.
Characters follow the headword's spelling order.
A character the headword lacks follows, by code point through `LGlyphOrder`.
So the order of the input never decides where a character's block stands.
Within a character, styles follow the pack's declared list, and an undeclared style follows by name.
Within a style, epochs follow the first place their code holds in the style's epoch table.
The pack lists that table from the oldest age to the latest, so the pictures read as a chronology.
An undeclared epoch code follows the declared ones by code, and a picture without an epoch comes last.
The stored position is web hit order, so it is never a key.
The stored row id is the last key, only to make the result stable.

## `LScriptSource`

The port that asks one script style's site for one character's glyph images.
It is a fetch over the network, not storage, so it is not a Vault.
`LScriptSourceHttp` in Infrastructure is its adapter over the shared `HttpClient`.
The engine holds the port from the rig and never names a client.
It answers the images the page links, each with its bytes, and whether the site answered at all.
An unreached site answers an empty list and false, so the caller can retry later.

## `LScriptStyle`

One character style a pack lists under `script`, such as 金文, 小篆 or 隸書.
The style names the web database that draws the character in that hand and how its answer is read.
The name is shown as the row's chip and stored beside every picture of it.
The form fields are posted in pack order, with `{word}` replaced by the character.
A database that needs a hidden field answers an error without it, so the pack lists every field it wants.
The pattern is matched over the whole answer, once per glyph picture.
Its image group holds the picture's address.
The caption group is zero for none.
Tags inside the caption are dropped and its lines joined by a space.
The prefix is prepended to a relative picture address.
The rewrite rules change the picture address before it is fetched.
A site that draws the glyph at a requested size is asked for a large original this way.
The gloss regex reads what the page prints beside the pictures.
An example is the 說文 entry under the small seal results.
The epoch labels are the pack's shared table unless the style listed labels of its own.
A style whose captions carry no age holds none, and its captions are stored whole.

## Stamp

Commit: `c2f335c331fe`
- `LEpoch.cs` `df9425872b3db824`
- `LFont.cs` `13c17918045f5df8`
- `LFontRole.cs` `1b0ccdc1ec7a4613`
- `LScriptGroup.cs` `2a71ba602270b84b`
- `LScriptImage.cs` `553c5fd7d2eb218a`
- `LScriptSource.cs` `b60d0cd4c64708d7`
- `LScriptStyle.cs` `6a49289ad2a1fd2e`
