# LMarkup.cs

## `public static class LMarkup`

Reads Llyn Markup text into entry drafts.
Llyn Markup is the plain-text format Llyn imports and exports whole entries in.
One file carries any number of entries as tagged text.
`docs/Format-LlynMarkup.md` is the authority on its syntax and on the meaning of every tag.
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
The sources an entry declared have already been folded into the citations that name them.
The author names an entry wrote are of use only to the layer that creates author rows.
That layer reads them through `LMarkupEntryRead` instead.

A document that declares no entry reads as an empty list rather than an error.
Text outside every `<entry>` block is not part of any entry and is skipped.
So a file with a comment line or a stray heading above its first entry still imports.

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
`LMarkupTokenSource` is the attribute value the tag carried, and `null` when it carried none.
It is the `src` of a text tag, or the `id` of a block tag.
Which of the two lands here is decided by the tag's kind, not the attribute's name.
The format gives each name to one kind only.
`src` cites a source from an example or a situation.
`id` declares one on a `<source>`.
A text tag written with an `id` therefore carries nothing.
So does a block tag written with an `src`.
That is what an attribute the format does not define there must amount to.

`LMarkupTokenParticle` and `LMarkupTokenDependence` are the `par` and the `dep` of a text tag.
They are the two halves of the frame an example states.
They are scanned on every text tag and read only off an `<example>`.
Keeping them on the token lets the scanner stay ignorant of which tag may state a frame.
A block tag never carries either, because the format writes a frame on an example alone.
`LMarkupTokenText` is the literal inner text with leading and trailing whitespace trimmed.
Everything inside it is untouched.
`&`, `<` and quotes are ordinary characters, as section 6 of the format spec requires.
`LMarkupTokenEmpty` is set for the empty-element form, `<tag></tag>` or `<tag/>`.
That is how the format writes the *unknown* state.
A later job turns that flag into `LStateValueUnknown`.
It turns the absence of the token altogether into `LStateValueUnspecified`.

Distinguishing an absent attribute from an empty one matters for the same reason.
No `src` is unspecified, and `src=""` is unknown.
Only a `null` versus an empty string keeps the two apart.

## `internal static IReadOnlyList<LMarkupToken> LMarkupScan(string text)`

Turns a whole Llyn Markup document into the ordered stream of tag tokens it declares.
The scan is hand-written rather than handed to an XML reader, because Llyn Markup is not XML.
Its text is literal.
A document that says `a & b` inside a `<meaning>` is well formed here and rejected there.
Reading it as XML would force authors to escape ordinary punctuation.
Section 6 of the format spec exists to promise they never have to.

The scan walks the text once.
Outside a tag, anything that is not `<` is skipped.
So whitespace and stray prose between blocks are insignificant rather than an error.
At a `<`, the tag name is read, then its attributes.
Only `src`, `id`, `par` and `dep` are kept.
The rest are dropped.
An unrecognised attribute, like an unrecognised tag, must not break an older file.
A block tag emits an enter token and, at its `</name>`, a leave token.
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

## `public readonly record struct LMarkupReference`

One `<source>` block as read.
`LMarkupReferenceId` is the `id` the block declared.
It is repeated outside the reference so a caller can index the block by it.
The caller need not unpack the value.
An `id` is meaningful only inside its own entry.
So the index a caller builds is per entry and never shared between them.

`LMarkupReferenceAuthor` exists because a source's author is not part of an `LReference`.
An `LReference` records only whether an author was recorded, in `LReferenceAuthorState`.
The names themselves are `LAuthor` rows attached to the reference in order.
They are shared with every other reference that credits the same person.
Reading is pure and cannot create or match those rows.
So the author text is carried out beside the reference.
It is left for the layer that owns author identity to attach.
Dropping it here would silently lose a written author on import.
That is the one thing the format promises not to do.

It is a list rather than one value because section 5 of the format spec allows several.
A source may credit several authors, and the format keeps their order.
The list holds only the names that were written.
An author whose tag was empty contributes no name, since there is no name to attach.
That the source credits someone unreadable is already recorded in the reference's author state.
So the list is empty for a source with no `<author>`.
It is also empty for one whose only `<author>` was empty.
The two stay apart on the reference, which is where that distinction belongs.

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
So `<tag>`, `<example>`, `<situation>`, `<image>` and `<video>` keep the order the document writes them in.
Section 7 makes that order meaningful.

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

The `src` on an example or a situation is kept as it was written.
It is not resolved to a source here.
A card can be read before the `<source>` blocks of its entry have been.
So the citation stays a raw key until entry assembly resolves it against the entry's sources.
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

## `internal static LMarkupReference LMarkupReferenceRead(IReadOnlyList<LMarkupToken> tokens)`

Turns the tokens of one `<source>` block into the reference it declares, keyed by its `id`.
The `id` comes from the block's enter token, which is where the scanner puts the attribute a tag carried.
A block written without one, or with an empty one, raises a `FormatException`.
Section 5 of the format spec declares a source *with* a stable `id`.
A source that has none can be cited by nothing.
It cannot be told apart from the next one that has none either.
Refusing it says so where the file is wrong.
Reading it as an empty key would let two unrelated works collapse into one.
The first would be lost without a word.

Only the block's own direct children are read, through `LMarkupLeafRead`.
So a block nested inside a source contributes none of its fields to it.

Every field runs through the same three-state reading the card fields do.
The author is the exception.
It is the one repeatable field here, and the one whose value does not live on the reference.
Its state is worked out from the tags as a whole.
It is *specified* as soon as one `<author>` names somebody.
It is *unknown* when the only ones written were empty.
It is *unspecified* when none was written.
That state is stored on the reference.
The names leave beside it in the order the block wrote them.

**Parameters**

- `tokens` — Every token of one `<source>` block, in document order.

**Returns** — The reference, its `id`, and the author text the block wrote.

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
