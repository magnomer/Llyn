# Card

This folder holds the records and the reader and writer for one entry's content as a markup file carries it.
Nothing here carries an id, so every link names its target by headword and language.
It stands apart from `Node`, which holds the generic element tree, text format and import plumbing.
Every row stands in file order, and position is the order in the list holding it.

## `LMarkupEntry`

One entry as a markup file carries it, with no id anywhere in it.
Metadata such as grasp, frequency, favorite and timestamps is not content and never travels.
The headword and language are required for the entry to enter a workspace.
Meaning cards may nest, and collocation cards never nest.
Unit and freedom are unchosen when the element is absent or unknown.
`LMarkupEntryLine` is the line the entry opened on, and zero for an entry built in memory.

## `LMarkupEntryName`

The headword as the import list names the entry, until the clerk tells twin headwords apart.
It is display text, so equality leaves it out.

## `Equals` in `LMarkupEntry`

Equality is row by row in order, so a round trip can be checked.
The line is left out, since it says where the entry was read and not what it holds.

## `LMarkupPronunciationMatch`

Pronunciation drafts compare by content, since the draft itself compares its syllable list by reference.

## `LMarkupCard`

One meaning or collocation as a markup file carries it.
The shape is `LCardDraft` with every id and position gone.
Translations name their target by headword and language.
Nested sub-senses are always empty on a collocation.
Sense paths are the dotted paths of the meanings a collocation links, in link order, and empty on a meaning.
Equality compares rows in order, children and sense paths included, and paths ordinally.

## `LMarkupInflection`

An inflected form with its speech and morphologies as names, since the file stores no ids.
The local spelling is null when none is given.

## `LMarkupTranslation`

A translation link naming its target entry by natural key.
On import the target is looked up among stored entries and entries the same file creates.

## `LMarkupEtymology`

The narrative etymology of an entry.
Each span inside the text names a source entry by headword and language.
The text is kept as written, untrimmed.

## `LMarkupEtymon`

One direct link from an entry to the entry it comes from.
The order of these links in the file is the order they are stored in.

## `LMarkupCardReader`

Turns a card element and everything under it into records.
A `meaning` and a `collocation` share one card shape and differ only in nesting and sense links.
Every parser walks its children in file order and reports what it does not know as an omission.

## `LMarkupCardParse`

A nested `meaning` is read as a child only when `nested` allows it.
Under a collocation such a child is an omission, since a collocation never nests.
A `sense` child is read as a link path only when `nested` is false, as for a collocation.
Under a meaning it is an omission.

## `LMarkupMentionParse`

The target is read by headword and language, and the sense by position path.
A `sense` child is read only when `sensed` allows it, and is an omission everywhere else.
An offset or length that is not a number reads as out of range.
The import then drops the span and reports it.
A mention with no headword is a plain span that stands for no entry.

## `LMarkupKindParse`

The reference kind is unknown under the state attribute.
It is unspecified when blank or unrecognised.

## Other parsers in `LMarkupCardReader`

The sentence, example, gloss, reference, translation, situation, image and video parsers each read one element.
Gloss and situation drafts carry the id zero, since the file stores none.
Image and video locations and spans are kept verbatim.

## `LMarkupCardWriter`

Turns a card record and everything under it into markup elements.
Stated values follow the state rule, so unspecified writes nothing and unknown writes a marked empty element.

## `LMarkupCardFormat`

Writes one card under the element name `meaning` or `collocation`.
Children are always written as `meaning`, since only a meaning nests.
Each link path follows them as a `sense` element, in link order.

## Other writers in `LMarkupCardWriter`

A sentence writes its frame first and the example after it.
A mention writes offset and length in invariant digits.
A reference writes the kind by its format name and each author on its own element.

## Stamp

Commit: `c2f335c331fe`
- `LMarkupCard.cs` `4f245dcabda5b7bc`
- `LMarkupCardReader.cs` `773e779971f1dbc9`
- `LMarkupCardWriter.cs` `a95a3eaaa5a1915e`
- `LMarkupEntry.cs` `390bb6350a78abd4`
- `LMarkupEtymology.cs` `c81d407310e96677`
- `LMarkupEtymon.cs` `6f1dbcfb517f40f7`
- `LMarkupInflection.cs` `801a88cc6598dcb6`
- `LMarkupTranslation.cs` `38e82c598b03d683`
