# LMarkupCardWriter.cs

## `internal static class LMarkupCardWriter`

Turns a card record and everything under it into markup elements.
Stated values follow the state rule, so unspecified writes nothing and unknown writes a marked empty element.

## `internal static XElement LMarkupCardFormat(LMarkupCard card, string name)`

Writes one card under the element `name`, either `meaning` or `collocation`.
Children are always written as `meaning`, since only a meaning nests.

## `private static XElement LMarkupSentenceFormat(LMarkupSentence sentence)`

Writes one `sentence` element, the frame first and the example after it.

## `private static XElement LMarkupExampleFormat(LMarkupExample example)`

Writes one `example` element with its glosses, mentions and reference.

## `private static XElement LMarkupMentionFormat(LMarkupMention mention)`

Writes one `mention` element, offset and length in invariant digits.

## `private static XElement LMarkupReferenceFormat(LMarkupReference reference)`

Writes one `reference` element, the kind by its format name and each author on its own element.

## `private static XElement LMarkupTranslationFormat(LMarkupTranslation translation)`

Writes one `translation` element as a headword and language.

## `private static XElement LMarkupSituationFormat(LSituationDraft situation)`

Writes one `situation` element with its title, description and kind.
