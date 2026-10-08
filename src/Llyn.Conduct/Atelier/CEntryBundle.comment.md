# CEntryBundle.cs
Hash: `bf4bf14472f5898b`

## `internal sealed class CEntryBundle`

The entry ports Host hands to Conduct in one piece, one port per narrow slice of the engine.
Each facade implements its own port, so no forwarding outlet stands between Conduct and the engine.
An area builder reads only the ports its constructors call and hands each one down.
It is internal, so Host and Conduct reach it and no Deportment driver can name it.

## `internal CEntryBundle(LEntryPort entries, LGraspPort grasps, LFavoritePort favorites, LVistaPort vistas, LCardPort cards, LTagPort tags, LRegisterPort registers, LMentionPort mentions, LGlyphPort glyphs, LSituationPort situations, LExamplePort examples, LReferencePort references, LAuthorPort authors, LMarkdownPort markdown, LPronunciationPort pronunciations)`

Rejects a missing port and keeps each one.
It is internal, so only Host builds a bundle.

## `internal LEntryPort CEntryBundleEntry { get; }`

The entry port, which loads an entry, reads its stamps, tally and reflexes, resolves a glyph and finds a card.

## `internal LGraspPort CEntryBundleGrasp { get; }`

The grasp port, which reads, words and saves the grasp an entry carries.

## `internal LFavoritePort CEntryBundleFavorite { get; }`

The favourite port, which lists, checks, saves and deletes favourites.

## `internal LVistaPort CEntryBundleVista { get; }`

The vista port, which finds entry rows and makes twin names distinct.

## `internal LCardPort CEntryBundleCard { get; }`

The card port, which reads incoming links, translation targets and etymology of a shown entry.

## `internal LTagPort CEntryBundleTag { get; }`

The tag port, which lists and creates Tags.

## `internal LRegisterPort CEntryBundleRegister { get; }`

The register port, which lists and creates registers.

## `internal LMentionPort CEntryBundleMention { get; }`

The mention port, which finds a Mention under an offset and reads a clicked link.

## `internal LGlyphPort CEntryBundleGlyph { get; }`

The glyph port, which reads the glyph section, divides the glyph row and lists transcriptions.

## `internal LSituationPort CEntryBundleSituation { get; }`

The situation port, which lists situations.

## `internal LExamplePort CEntryBundleExample { get; }`

The example port, which lists Examples, matches a field text and composes a sentence line.

## `internal LReferencePort CEntryBundleReference { get; }`

The reference port, which lists Sources, reads citations, the read and edit sheets and the kind menu.

## `internal LAuthorPort CEntryBundleAuthor { get; }`

The author port, which reads the roll, union, vita, oeuvre and credits, and folds two Authors.

## `internal LMarkdownPort CEntryBundleMarkdown { get; }`

The markdown port, which parses a note into blocks.

## `internal LPronunciationPort CEntryBundlePronunciation { get; }`

The pronunciation port, which lists pronunciations, resolves an entry's frequency and reads the consonant and vowel articulations.
