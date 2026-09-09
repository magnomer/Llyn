# TMarkupSyntax.cs

## `public sealed class TMarkupSyntax`

Covers the Llyn Markup scanner and the card and source readers built on it.
It pins what one tag becomes, and what a card and a source read off those tags.
The document the scan is part of is covered in `TMarkupCatalog.cs`.
The tests read `docs-work/FormatLlynMarkup.md` as the authority.
So each one pins a promise the format makes to an author.
It is not an implementation detail of the scan.
Text stays literal, and the empty-element form survives as its own flag.
An absent attribute stays apart from an empty one, and order is kept.
Structural damage is reported with a position.

## Inline notes

### `Assert.Null(TInterface.TMarkupTokenRead(tokens[0], "ref"));`

The three citation states of section 6, at the token level.
No `ref` at all is unspecified and comes back `null`.
`ref=""` is unknown and comes back as an empty string.
A named `ref` is specified.
Collapsing the first two would lose a distinction the format keeps deliberately.

### `Assert.Equal("a mark ( & ) joining two clauses, as in a &amp; b < c", token.LMarkupTokenText);`

Nothing inside text is interpreted.
`&amp;` stays the five characters the author typed.
A `<` that opens no tag is ordinary punctuation, not markup.

### `MarkupScan_NestedSense_BoundsTwoBlocks`

A sense may hold a sense, which no block could do before the catalog.
Two blocks are two enter tokens and two leave tokens, and the inner one closes only itself.
A scan that lost the boundary would give the outer sense the inner sense's fields.

### `MarkupScan_UnknownAttribute_DropsIt`

Section 1 makes an unrecognised attribute ignorable rather than an error.
The attributes beside it are still read, so one unknown name costs the tag nothing.

### `MarkupScan_SyllableAttributes_KeepsEveryOne`

A syllable writes six attributes at once, and a token carries all of them.
It is the tag that made a field per attribute untenable.

### `Assert.Contains("7", failure.Message);`

The message names the character position of the damage, not merely that there was some.
Position 7 is where the offending `<` stands in each of those documents.

### `Assert.Equal(LState.LStateUnknown, card.LCardDraftMeaning.LStateValueState);`

The heart of section 6, at the draft level.
`<meaning></meaning>` is *unknown*, because a meaning exists and could not be read.
A `<title>` the card never wrote is *unspecified*.
The same document pins both at once, because the two states are only meaningful against each other.

### `<future>not a field</future>`

Section 1 makes an unrecognised tag ignorable rather than an error.
So a file written by a later version of Llyn still imports on an older one.

### `Assert.Equal(["murray", "bradley"], read.LMarkupReferenceAuthor);`

An `LReference` records only that an author was written.
The author itself is a catalog row that pure reading cannot create.
The credit therefore leaves beside the reference as keys, in the order the block wrote them.
This pins that it is not quietly dropped on the way, and that credit order survives.

### `Assert.Equal(LState.LStateUnknown, reference.LReferenceAuthorState);`

A lone `<author/>` credits somebody unreadable.
No key leaves beside the reference, because there is no row to point at.
The state is what keeps that apart from a source crediting nobody at all.
