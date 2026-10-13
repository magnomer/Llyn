# Language

This folder holds the tests of the language pack loader.
They cover its pack-level, source, Han and inflection sides.

## `TInflectionLoader`

Covers the `inflection` side of the pack loader, on fixture packs and on the real Spanish pack.
A file named under `inflection` loads as a book that predicts a form.
The `null` ending leaves its column out, so the stem keeps one ending.
The stamp is 64 lowercase hex digits, and an edit to the file changes it.
An object written in place loads the same book with a stamp of its own.
A named file that is absent, or no key at all, leaves the pack without a book.
A rule or fold whose pattern does not parse is skipped, and the rest load.
A rule's third element is kept as the kind that limits it.
The Spanish book lays out part 6 in three default collapsed groups and eleven expanded lines under six header keys.
Every expanded line takes the six columns.
The collapsed imperfect subjunctive line joins its values with each form value, codes ascending.
The Spanish book carries both pairs of sheets.
The default pair lists the indicative, the subjunctive, then the imperative.
The custom pair keeps the custom order on the same part and has no custom pair of its own.
A layout without `custom` still carries a custom pair holding the very default sheets.
So either setting shows the same box.
The design's hand checks hold on the real pack, each row stating the prediction and the marked ranges.
The first singular imperative has no ending, so it stays uncovered.

## `TLanguageLoader`

Covers the pack loader reading the pack-level blocks, such as varieties, flags, rewrites, schemes and the spelling list.
The English pack declares two flagged varieties, and a pack without the block declares none.
The Mandarin and Cantonese packs declare tone and the English pack does not.
The Mandarin and Cantonese packs are phonemic, the English pack is not, and all three carry respellings.
The Wu pack is phonemic but stays off the language list.
The English pack loads exactly two respelling groups, both scoped, and an empty cleanup list.
A fixture pack that declares varieties drops every group without a non-empty `varieties` list.
A fixture pack without varieties keeps such groups, so the key stays optional there.
A `cleanup` block loads its rules in order and skips malformed rows.
A group with a broken regex is dropped while its neighbours survive.
A `transcription` entry loads as a scheme whether it is a bare name or an object with sources.
Blank and doubled scheme names are dropped.
A good `spelling` list is stamped on every source kind, and one with a broken regex loads as no rules.
The Classical Latin pack carries its twelve macron rules on its lookup, harvest and frequency sources alike.
A `flag` ending in `.svg` loads as the rooted path of that file in the pack folder.
A flag read whose client times out answers `null`, and one the caller cancels throws.
The source rows a pack declares are covered in `TLanguageLoaderSource`.
A throwaway pack is written beside the shipped ones for the fixture cases and removed again.

## `TLanguageLoaderHan`

Covers the pack loader reading the Han-language sections: script styles, rime books and the hypothesis.
A `script` list loads its styles in written order with their form fields, groups, prefix, rewrite rules and gloss pattern.
A style row missing its match pattern is skipped alone, and the optional keys read as empty or zero.
A `fanqie` list loads its books in written order with their form fields, marker, busy pattern and interval.
A book row missing its match pattern is skipped alone, and the optional keys read as null, empty or zero.
A book row without a source is labelled by its book name.
Two rows may share a book under two sources.
A `hypothesis` object loads its initial and final tables and its tone rules.
A section missing a table loads as null.
A `hypothesis` file name loads the tables from that file in the pack folder.
A missing file, or a name reaching outside the folder, loads as null.
The shipped Classical Chinese pack is read for the real sections, and throwaway packs for the edge cases.

## `TLanguageLoaderSource`

Covers the pack loader reading one declared source's attempts, readings, follow, frequency scale and glyph block.
A readings list loads in written order, each reading on its own variety, with no skip count and every off.
A reading row with `every` on loads that flag, as the Mandarin pack's Wiktionary readings do.
A flat attempt still loads as one untagged reading.
An audio attempt with a readings list loads one non-phonetic reading per variety.
A `follow` object loads as the attempt's follow reading, unnormalized, and an attempt without one carries null.
A `decode` key marks the attempt decoded.
A `frequency` list and a `bands` list load in written order.
Each frequency source keeps its own `once` figures and unit.
A negative or textual `once` figure reads as null.
A band row without a name, without a pattern, or with a broken regex is skipped alone.
A row giving only a numeric limit is skipped the same way.
A pack without the frequency key loads an empty list.
A `glyph` block loads its name, its language and its sources, and an empty sources list when it lists none.
Its `font` block loads as the chip typography, and a block that names none reads as null.
A `glyph` block missing its language reads as null, as does a pack without the block.
The pack-level blocks are covered in `TLanguageLoader`.

## Stamp

Commit: `c2f335c331fe`
- `TInflectionLoader.cs` `fcd1cc2164aeb2e2`
- `TLanguageLoader.cs` `7ba02339d8b50f67`
- `TLanguageLoaderHan.cs` `9e642063ca1a3564`
- `TLanguageLoaderSource.cs` `231a3f5dde13f148`
