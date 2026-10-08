# CEntryBundle.cs
Hash: `86acdf64ecc028df`

## `public sealed class CEntryBundle`

The entry ports Host hands to Conduct in one piece, one port per narrow slice of the engine.
Each facade implements its own port, so no forwarding outlet stands between Conduct and the engine.
An area builder reads only the ports its constructors call and hands each one down.

## `internal CEntryBundle(LEntryPort entries, LGraspPort grasps, LFavoritePort favorites, LVistaPort vistas, LCardPort cards, LTagPort tags, LRegisterPort registers, LMentionPort mentions, LGlyphPort glyphs, LSituationPort situations, LExamplePort examples, LReferencePort references, LAuthorPort authors, LMarkdownPort markdown, LPronunciationPort pronunciations)`

Rejects a missing port and keeps each one.
It is internal, so only Host builds a bundle.

## `public LEntryPort CEntryBundleEntry { get; }`

The entry port, which loads an entry, reads its stamps, tally and reflexes, resolves a glyph and finds a card.

## `public LGraspPort CEntryBundleGrasp { get; }`

The grasp port, which reads, words and saves the grasp an entry carries.

## `public LFavoritePort CEntryBundleFavorite { get; }`

The favourite port, which lists, checks, saves and deletes favourites.

## `public LVistaPort CEntryBundleVista { get; }`

The vista port, which finds entry rows and makes twin names distinct.

## `public LCardPort CEntryBundleCard { get; }`

The card port, which reads incoming links, translation targets and etymology of a shown entry.

## `public LTagPort CEntryBundleTag { get; }`

The tag port, which lists and creates Tags.

## `public LRegisterPort CEntryBundleRegister { get; }`

The register port, which lists and creates registers.

## `public LMentionPort CEntryBundleMention { get; }`

The mention port, which finds a Mention under an offset and reads a clicked link.

## `public LGlyphPort CEntryBundleGlyph { get; }`

The glyph port, which reads the glyph section, divides the glyph row and lists transcriptions.

## `public LSituationPort CEntryBundleSituation { get; }`

The situation port, which lists situations.

## `public LExamplePort CEntryBundleExample { get; }`

The example port, which lists Examples, matches a field text and composes a sentence line.

## `public LReferencePort CEntryBundleReference { get; }`

The reference port, which lists Sources, reads citations, the read and edit sheets and the kind menu.

## `public LAuthorPort CEntryBundleAuthor { get; }`

The author port, which reads the roll, union, vita, oeuvre and credits, and folds two Authors.

## `public LMarkdownPort CEntryBundleMarkdown { get; }`

The markdown port, which parses a note into blocks.

## `public LPronunciationPort CEntryBundlePronunciation { get; }`

The pronunciation port, which lists pronunciations, resolves an entry's frequency and reads the consonant and vowel articulations.
