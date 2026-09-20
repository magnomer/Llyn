# LMarkupCardWriter.cs

## `internal static class LMarkupCardWriter`

Turns a card record and everything under it into markup elements.
Stated values follow the state rule, so unspecified writes nothing and unknown writes a marked empty element.

## `internal static LMarkupNode LMarkupCardFormat(LMarkupCard card, string name)`

Writes one card under the element `name`, either `meaning` or `collocation`.
Children are always written as `meaning`, since only a meaning nests.

## `private static LMarkupNode LMarkupSentenceFormat(LMarkupSentence sentence)`

Writes one `sentence` element, the frame first and the example after it.

## `private static LMarkupNode LMarkupExampleFormat(LMarkupExample example)`

Writes one `example` element with its glosses, mentions and reference.

## `private static LMarkupNode LMarkupMentionFormat(LMarkupMention mention)`

Writes one `mention` element, offset and length in invariant digits.

## `private static LMarkupNode LMarkupReferenceFormat(LMarkupReference reference)`

Writes one `reference` element, the kind by its format name and each author on its own element.

## `private static LMarkupNode LMarkupTranslationFormat(LMarkupTranslation translation)`

Writes one `translation` element as a headword and language.

## `private static LMarkupNode LMarkupSituationFormat(LSituationDraft situation)`

Writes one `situation` element with its title, description and kind.
