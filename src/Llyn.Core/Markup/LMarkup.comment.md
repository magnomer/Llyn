# LMarkup.cs

## `public static class LMarkup`

Reads Llyn Markup text into entry drafts.
Llyn Markup is the plain-text format Llyn imports and exports whole entries in.
One file carries any number of entries as tagged text.
`docs-work/FormatLlynMarkup.md` is the authority on its syntax and on the meaning of every tag.
This class implements that document and decides nothing on its own.
Where the two disagree the document is right and the code is wrong.

The result is a list of `LEntryDraft`, the same shape the editor produces.
So imported text and typed text reach the rest of the program through one door.
Reading stops at drafts: nothing here touches the database, the editor, or any store.

## `public static IReadOnlyList<LEntryDraft> LMarkupRead(string text)`

Takes the whole text of a Llyn Markup document.
Returns one draft per entry it declares, in the order the file declares them.
It is the projection of `LMarkupEntryRead` that keeps only the drafts.
A draft is all the rest of the program has a shape for.
The catalog the document declared is of use only to the layer that creates rows.
That layer reads it through `LMarkupEntryRead` instead.

A document that declares no entry reads as an empty list rather than an error.
Text between elements is insignificant and is skipped.
A file whose first element is not `<llyn>` raises a `FormatException` and reads as nothing.

**Parameters**

- `text` — The whole Llyn Markup document, as read from an `.llx` file or held in memory.

## `internal enum LMarkupTokenKind`

What one scanned tag is, as far as the scanner can tell without knowing the format's meaning.
`LMarkupTokenText` is a leaf tag that carries text, `LMarkupTokenEnter` opens a block, and `LMarkupTokenLeave` closes the block it names.
Blocks are bounded rather than nested in the token stream.
The scan stays flat.
A caller that wants the contents of one `<sense>` reads from its enter token to its leave token.
That keeps the scanner from having to know which tags belong inside which block.
That is the format spec's business and not the scanner's.

## `internal readonly record struct LMarkupToken`

One tag, as scanned.
`LMarkupTokenName` is the tag name exactly as written.
`LMarkupTokenMark` holds the attributes the tag carried, each under its own name.
A map rather than a field per attribute, because the format names twenty-six of them.
A positional record of that width would be read by counting commas.
The scanner keeps only the attributes the format defines and drops the rest.
Section 1 of the format spec makes an unrecognised attribute ignorable rather than an error.
An attribute the tag never carried is absent from the map, and `ref=""` is present and empty.
Only that difference keeps *unspecified* apart from *unknown* on an attribute.

`LMarkupTokenText` is the literal inner text with leading and trailing whitespace trimmed.
Everything inside it is untouched.
`&`, `<` and quotes are ordinary characters, as section 7 of the format spec requires.
`LMarkupTokenEmpty` is set for the empty-element form, `<tag></tag>` or `<tag/>`.
That is how the format writes the *unknown* state.
A later reader turns that flag into `LStateValueUnknown`.
It turns the absence of the token altogether into `LStateValueUnspecified`.

## `internal string? LMarkupTokenRead(string mark)`

Reads one attribute off the token by name.
It returns `null` when the tag carried no such attribute.
It returns the empty string when the tag carried it written empty.
So every caller reads the three states through one door instead of probing the map.

**Parameters**

- `mark` — The attribute name as the format writes it, such as `ref` or `tone-local`.

## `internal static IReadOnlyList<LMarkupToken> LMarkupScan(string text)`

Turns a whole Llyn Markup document into the ordered stream of tag tokens it declares.
The scan is hand-written rather than handed to an XML reader, because Llyn Markup is not XML.
Its text is literal.
A document that says `a & b` inside a `<meaning>` is well formed here and rejected there.
Reading it as XML would force authors to escape ordinary punctuation.
Section 7 of the format spec exists to promise they never have to.

The scan walks the text once.
Outside a tag, anything that is not `<` is skipped.
So whitespace and stray prose between blocks are insignificant rather than an error.
At a `<`, the tag name is read, then its attributes.
Only the attributes the format defines are kept.
The rest are dropped.
An unrecognised attribute, like an unrecognised tag, must not break an older file.
A block tag emits an enter token and, at its `</name>`, a leave token.
The block list holds every tag the format writes children inside.
`<catalog>`, `<example>`, `<situation>`, `<register>`, `<video>`, `<inflection>`, `<pronunciation>` and `<relation>` joined it with the catalog.
`<llyn>` is the document element and is a block like any other.
`<sense>` may hold a `<sense>`, and depth counting is what keeps the inner one from closing the outer.
Any other tag is a text tag.
Its content is taken literally up to the first `</name>`.
So the one sequence an author cannot write inside text is that tag's own closing tag.
Nesting therefore goes one level for text tags — a tag inside a text tag is part of the text.

Structural damage is an error.
The message names the character position so an import can report where the file went wrong.
Unclosed tags and a `<` that no name follows raise a `FormatException`.
So do a closing tag matching no open block and a block still open at the end.

**Parameters**

- `text` — The whole Llyn Markup document.

**Returns** — Every tag in the order the document writes it.
Block tags are bounded by an enter and a leave token.

## `internal static LCardDraft LMarkupCardRead(IReadOnlyList<LMarkupToken> tokens, int position)`

Turns the tokens of one `<sense>` or `<collocation>` block into the card draft it describes.
Both card kinds carry the same tags and differ only in the list they join.
So one reader serves both, and the caller decides which list the result belongs to.

The tokens handed in are the whole block, its enter and leave tokens included.
Only the block's own direct children are read, through `LMarkupLeafRead`.
Any tag the format does not name is skipped.
Section 1 of the format spec makes an unrecognised tag ignorable rather than an error.
A block written where the format does not allow one is skipped whole for the same reason.
A `<source>` inside a `<sense>` is such a block.
So its `<title>` can never be mistaken for the card's.
Everything else is read by tag name in one pass.
So `<tag>` and `<image>` keep the order the document writes them in.
Section 8 makes that order meaningful.
A card's uses, situations, registers and videos are blocks the format now writes children inside.
This reader takes none of them, and reading them is the card reader's own next change.

The card comes back holding no Translation, because the format writes none.
A link is an id of a stored Entry, and a document has no id to give.

An empty `<tag></tag>` contributes nothing.
A Tag is its own name.
So a Tag with no name is not an unreadable Tag but no Tag at all.
It is the one field of a card that does not carry the three states.
It has no place to carry them.

A tag that may appear only once is not an error when it appears twice.
Those tags are `<title>`, `<expression>`, `<meaning>` and `<synonym>`.
The last one written wins.
A document that repeats a singular tag is malformed in a way the format does not describe.
Refusing the whole import over it would be harsher than the format's leniency elsewhere warrants.

The `src` on an example is kept as it was written.
It is not resolved to a source here.
A card is read before the catalog it cites has been.
So the citation stays a raw key until the document has resolved it.
It is stored as an `LStateValue` because the raw key already carries the three states.
No `src` is unspecified, `src=""` is unknown, and a named `src` is specified.
The state survives resolution unchanged when the key does not.

The `par` and the `dep` of an example are read the same way and kept as they were written.
Nothing here knows what a marker or a role may hold, because nothing ships either.
The format states where the two are written and never what they may say.
A language pack orders the two fields in the editor and has no say over a document.
Markup is read in tag order, so no pack can reorder what a file wrote.

**Parameters**

- `tokens` — Every token of one card block, in document order.
- `position` — The number this card is shown by, counted from one.
  A document gives no number, so the caller counts the card blocks it has read.
  Cards of one entry therefore number one to n in the order the document writes them.

**Returns** — The card as a draft, with no id, because an imported card has none until it is saved.

## `private static IEnumerable<LMarkupToken> LMarkupLeafRead(IReadOnlyList<LMarkupToken> tokens)`

Yields the text tokens that belong to a block directly, and no deeper.
The token stream is flat.
A block's tokens and the tokens of a block nested inside it arrive in one run.
Walking the run with a depth counter and reporting only depth one tells the two apart.
Without it a reader taking every text token in the run would read a nested block's fields as its own.
The `<title>` of a misplaced `<source>` would become the title of the `<sense>` that holds it.
That is a silent wrong answer where an ignored tag was the promise.

**Parameters**

- `tokens` — One block's tokens, its enter and leave tokens included.

**Returns** — Its direct text tags, in document order.

## `private static LStateValue LMarkupStateRead(LMarkupToken? token)`

The one place the format's three states become an `LStateValue`.
So no field decides for itself what an empty tag means.
A token that was never seen is *unspecified*.
A token carrying the empty flag is *unknown*, and a token with text is *specified*.
The distinction is the format's most important rule.
It is the reason a missing tag and an empty one cannot be treated alike.

The value form of the helper is deliberately not `LStateValueRead`, which folds blank text into *unspecified*.
Here an empty tag has already been distinguished from an absent one by the scanner.
Folding the two back together would throw away what the document took the trouble to write.

**Parameters**

- `token` — The tag as scanned, or `null` when the block never wrote it.

## `private static LStateValue LMarkupStateRead(string? source)`

The same three states read off an attribute rather than a tag.
An attribute the tag never carried is *unspecified*.
One written empty is *unknown*, and one with text is *specified*.
A citation follows the same rule as a field.
So it is read by the same helper under the same name.

**Parameters**

- `source` — The attribute value as scanned, or `null` when the tag carried none.
