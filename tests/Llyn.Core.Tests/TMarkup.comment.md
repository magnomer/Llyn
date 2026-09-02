# TMarkup.cs

## `public sealed class TMarkup`

Covers the Llyn Markup scanner and the card and source readers built on it: what one tag becomes, what a card and a source read off those tags, and what a broken document does. The tests read `docs/Format-LlynMarkup.md` as the authority, so each one pins a promise the format makes to an author rather than an implementation detail of the scan — text stays literal, the empty-element form survives as its own flag, an absent attribute stays apart from an empty one, order is kept, and structural damage is reported with a position.

## Inline notes

### `Assert.Null(tokens[0].LMarkupTokenSource); Assert.Equal(string.Empty, tokens[1].LMarkupTokenSource);`

The three citation states of section 4, at the token level: no `src` at all is unspecified and comes back `null`, `src=""` is unknown and comes back as an empty string, and a named `src` is specified. Collapsing the first two would lose a distinction the format keeps deliberately.

### `Assert.Equal("a mark ( & ) joining two clauses, as in a &amp; b < c", token.LMarkupTokenText);`

Nothing inside text is interpreted. `&amp;` stays the five characters the author typed, and a `<` that opens no tag is ordinary punctuation, not markup.

### `Assert.Contains("7", failure.Message);`

The message names the character position of the damage, not merely that there was some. Position 7 is where the offending `<` stands in each of those documents.

### `Assert.Equal(LState.LStateUnknown, card.LCardDraftMeaning.LStateValueState);`

The heart of section 4, at the draft level. `<meaning></meaning>` is *unknown* — a meaning exists and could not be read — while a `<title>` the card never wrote is *unspecified*. The same document pins both at once, because the two states are only meaningful against each other.

### `Assert.Equal(LState.LStateUnknown, situation.LSituationDraftReference.LStateValueState);`

`src=""` cites an unreadable source, which is not the same as citing none. The reader keeps the state before any source exists to resolve against, so the distinction cannot be lost between reading a card and reading the entry's sources.

### `<future>not a field</future>`

Section 1 makes an unrecognised tag ignorable rather than an error, so a file written by a later version of Llyn still imports on an older one.

### `Assert.Equal("Murray, James", read.LMarkupReferenceAuthor.LStateValueShow());`

An `LReference` records only that an author was written; the name itself belongs to an `LAuthor` row that pure reading cannot create. The name therefore leaves beside the reference, and this pins that it is not quietly dropped on the way.
