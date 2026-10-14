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
Ancestor boxes stand in the order the entry lists them, and equality compares them in sequence.
Unit and freedom are unchosen when the element is absent or unknown.
`LMarkupEntryLine` is the line the entry opened on, and zero for an entry built in memory.

## `LMarkupAncestor`

One box of the etymology map as a markup file carries it.
A box names itself by a key and its child by the child's key, so no database id travels.
A box with no child feeds the headword itself.
The position orders co-sources under one child.
A linked box is named by headword and language, and a text box by its text and language.
The relation travels as its name, so a user relation needs no id to survive a move.
Boxes carry keys, because the map is a tree.
List order alone cannot name a child.

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

Commit: `c6e60141a172`
- `LMarkupAncestor.cs` `68890876e68e5602`
- `LMarkupCard.cs` `4f245dcabda5b7bc`
- `LMarkupCardReader.cs` `773e779971f1dbc9`
- `LMarkupCardWriter.cs` `a95a3eaaa5a1915e`
- `LMarkupEntry.cs` `37de61a6aa336535`
- `LMarkupInflection.cs` `801a88cc6598dcb6`
- `LMarkupTranslation.cs` `38e82c598b03d683`
