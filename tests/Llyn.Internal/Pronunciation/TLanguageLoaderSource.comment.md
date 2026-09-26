# TLanguageLoaderSource.cs

## `public sealed class TLanguageLoaderSource`

Covers the language pack loader reading one declared source: its attempts, readings, follow, frequency scale and glyph block.
A readings list loads in written order, each reading on its own variety, with no skip count and every off.
A reading row with `every` on loads that flag, as the Chinese packs' per-etymology readings do.
A flat attempt still loads as one untagged reading.
An audio attempt with a readings list loads one non-phonetic reading per variety.
A `follow` object loads as the attempt's second reading, unnormalized, and an attempt without one carries null.
A `decode` key marks the attempt decoded.
A `frequency` list and a `bands` list load in written order, limit rows and pattern rows alike.
Each frequency source carries its own `once` figures and unit.
A band row without a name, without a limit or pattern, or with a broken regex is skipped alone.
A pack without the frequency key loads an empty list.
A `glyph` block loads its name, its language and its sources, and an empty sources list when it lists none.
Its `font` block loads as the chip typography, and a block that names none reads as null.
A `glyph` block missing its language reads as null, as does a pack without the block.
The pack-level blocks are covered in `TLanguageLoader.cs`.

## `private static LSourceSpec TLanguageSourceFind(string language, string name)`

The one lookup source of a shipped pack under that name.
