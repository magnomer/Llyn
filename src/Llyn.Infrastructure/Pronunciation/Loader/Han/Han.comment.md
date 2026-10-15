# Han

This folder holds the loaders of the pack sections only a Han-character language declares.
They cover fanqie, hypothesis, script, shengfu, glyph, reflex, anatomy and descent.

## `LAnatomyLoader`

It reads the rules that cut a reflex reading into onset, vowel, coda and tone.
The rules become a list, empty without them.
They live in their own file beside `source.json`, since only the Classical Chinese pack declares them.
`LAnatomyPackRead` reads the `anatomy` key.
A file name opens that file in the pack folder through `LPackFile`, and an array is read in place.
The file holds either the array itself or an object wrapping it under `anatomy`.
Any other value, no key, or a missing or unreadable file yields no rules.
Every reflex row then keeps a blank anatomy.
A rule row serves the `language` names.
It has an `ipa` pattern over the reading and a `respelling` pattern over the respelling.
A row without a language or without an `ipa` pattern reads `null`.
A row without a `respelling` pattern cuts the respelling with the `ipa` pattern.
A pattern block has a `match` regex, `rewrite` rules in respelling form, and a `decompose` normalization flag.
A missing block, a blank regex or a broken one reads `null`.

## `LDescentLoader`

It reads the rows that say which tone classes a reflex reading's contour may descend from.
The rows become a list, empty without them.
They live in their own file beside `anatomy.json`, since only the Classical Chinese pack declares them.
`LDescentPackRead` reads the `tone` key the same way the anatomy loader reads its own.
A file holds either the array itself or an object wrapping it under `tone`.
A missing or unreadable file yields no rows, and no placement is marked as estimated.
A row serves the `language` names, and its `class` table maps each contour to the classes it may descend from.
A row without a language or with an empty table reads `null`.
Each contour is trimmed, and its classes are one string or an array with blanks and repeats dropped.
A contour left with no class is dropped.

## `LFanqieLoader`

It reads the rime books a Han language pack lists under `fanqie`.
Each row becomes a book, in written order.
A pack without the array yields an empty list, and the reading view shows no fanqie box.
A row without a name, a URL or a match pattern is refused, since nothing could be posted or read.
The form fields are read by the envelope reader, since a book posts a search form like other fetch rules.
The busy pattern, the interval and the split, head, column, rounded, line and spelling patterns are optional.
They read as `null` or zero.
The source label is optional and falls back to the book name.

## `LGlyphLoader`

It reads the traditional form a simplified language shows beside its word.
The `glyph` section holds a `name`, a `language`, and an optional `sources` list and `font` block.
A section missing either the name or the language reads as null, and the glyph row stays off.
The sources list has the shape of a scheme's, so the traditional form is looked up as a transcription is.
A blank font block reads as none, so the chips fall back to the example typography.

## `LHypothesisLoader`

It reads the reconstruction tables a Han language pack declares.
The tables become one hypothesis, or `null` without them.
They live in their own file beside `source.json`, since a language may grow a system of its own.
`LHypothesisPackRead` reads the `hypothesis` key, holding a file name or the tables themselves.
A file name opens that file through `LPackFile`, and an object is read in place.
The file's root is the tables object itself, never wrapped under a key.
A missing or unreadable file yields `null`, and the fanqie box prints the placements alone.
An object missing the `initial` or `final` table yields `null`, since nothing could be resolved from it.
A table maps string to string with keys trimmed, and rows with a non-string value are skipped.
A missing table reads as empty.
The `tone` object keys are rime-book tone characters, and each value holds the class rows of that tone in order.
A tone with no readable row is left out.
A class row has an `onset` pattern, `rewrite` rules in respelling form, and a `class` label key.
A missing onset matches every onset, and a missing rewrite leaves the syllable as joined.
A row that is not an object, or whose onset pattern is broken, reads `null`.
The `place` array holds the articulatory loci in file order.
A locus has a `name` key and an `initial` list with blanks and repeats dropped.
A row without a name or any initial reads `null`, and a missing or malformed array reads as no loci.

## `LReflexLoader`

It reads the `reflex` list naming one fetch rule per borrowing language.
It also reads the `order` the rows are shown in, apart from the rules.
`LReflexPackScan` returns every well-formed rule in written order, or an empty list.
A `clip` that does not compile loads as no clip.
So the rule stays, and the epithet never meets a bad pattern.
`LReflexOrderRead` returns the order under `order`, or the empty order when the key is absent.
Its `language` list names the borrowing languages in their order, and its `kind` object maps a language to its kinds.
The rule list is never read for it, so reordering the rules leaves the screen alone.
A rule lacking a language, an address or a match pattern yields `null`.
Every other key is optional, read as `null`, zero, false or no rules when absent.
`recast` is read like `rewrite` but over the romanization, and `superscript` raises the romanization's digits.
A `form` object turns the fetch into a post.

## `LScriptLoader`

It reads the character styles a Han language pack lists under `script`.
Each row becomes a style, in written order.
A pack without the array yields an empty list, and the reading view shows no script box.
The pack's own `epoch` table is read once and handed to every row.
One source dates its bronzes, seals and clerical hands alike, so one table serves them all.
A row needs a name, the address the form is posted to, and the pattern reading the answer.
A row missing any of the three is dropped, because none of them has a default worth guessing.
The group numbers, prefix, rewrite rules, gloss pattern and chronology labels are optional.
A row listing its own labels keeps them whole, and any other row takes the pack's.
A style is a page of one site, so it overrides the table only where that page dates differently.
The `epoch` array reads as ordered pairs of a label and its code.
The pack and a style row read it the same way.
A pair missing either half is skipped.
A label with no code dates nothing, and a code with no label is never met.
A pack whose captions carry no age lists none, and its captions are stored whole.

## `LShengfuLoader`

It reads the one phonetic-series lookup a Han language pack may declare.
`LShengfuPackRead` reads the `shengfu` block into one rule, or null when the pack declares none.
A block without an address or a pattern is no rule, because neither has a sensible default.
The separator defaults to a middle dot, which joins the series of a character with two.

## Stamp

Commit: `2f9a06b8eb95`
- `LAnatomyLoader.cs` `2815dc5b88354b1c`
- `LDescentLoader.cs` `6074ddb4512f23f8`
- `LFanqieLoader.cs` `eaf86e4c2be229a4`
- `LGlyphLoader.cs` `f03c01566a0369e3`
- `LHypothesisLoader.cs` `506d5de38b34cd71`
- `LReflexLoader.cs` `0ae0f57830d217cd`
- `LScriptLoader.cs` `9059c851cfb800eb`
- `LShengfuLoader.cs` `68404391d41adc01`
