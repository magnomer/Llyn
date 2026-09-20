# LMarkupCardReader.cs

## `internal static class LMarkupCardReader`

Turns a card element and everything under it into records.
A `meaning` and a `collocation` share one card shape and differ only in nesting.
Every parser walks its children in file order and reports what it does not know as an omission.

## `internal static LMarkupCard LMarkupCardParse(LMarkupNode element, bool nested, List<LMarkupOmission> omissions)`

Reads one card with its sentences, chips, media and children in file order.
A nested `meaning` is read as a child only when `nested` allows it.
Under a collocation such a child is an omission, since a collocation never nests.

## `private static LMarkupSentence LMarkupSentenceParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `sentence` element, a frame with or without an example.

## `private static LMarkupExample LMarkupExampleParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `example` element with its glosses, mentions and reference.

## `private static LGlossDraft LMarkupGlossParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `gloss` element as a draft with no id.

## `private static LMarkupMention LMarkupMentionParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `mention` element, its target by headword and language and its sense by position path.
An offset or length that is not a number reads as out of range.
The import then drops the span and reports it.
A mention with no headword is a plain span that stands for no entry.

## `private static LMarkupReference LMarkupReferenceParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `reference` element by value, authors by name in file order.

## `private static LReferenceKind LMarkupKindParse(LMarkupNode element)`

Reads the reference kind, unknown under the state attribute and unspecified when blank or unrecognised.

## `private static LMarkupTranslation LMarkupTranslationParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `translation` element as a headword and language.

## `private static LSituationDraft LMarkupSituationParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `situation` element as a draft with no id.

## `private static LImageDraft LMarkupImageParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `image` element, its location verbatim.

## `private static LVideoDraft LMarkupVideoParse(LMarkupNode element, List<LMarkupOmission> omissions)`

Reads one `video` element, its location and span verbatim.
