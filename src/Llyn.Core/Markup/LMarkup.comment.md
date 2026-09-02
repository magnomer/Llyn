# LMarkup.cs

## `public static class LMarkup`

Reads Llyn Markup text into entry drafts. Llyn Markup is the plain-text format Llyn imports and exports whole entries in — one file carrying any number of entries as tagged text — and `docs/Format-LlynMarkup.md` is the authority on its syntax and on the meaning of every tag. This class implements that document and decides nothing on its own; where the two disagree the document is right and the code is wrong.

The result is a list of `LEntryDraft`, the same shape the editor produces, so imported text and typed text reach the rest of the program through one door. Reading stops at drafts: nothing here touches the database, the editor, or any store.

## `public static IReadOnlyList<LEntryDraft> LMarkupRead(string text)`

Takes the whole text of a Llyn Markup document and returns one draft per entry it declares, in the order the file declares them. The tag reading itself is not written yet, so the list comes back empty.

**Parameters**

- `text` — The whole Llyn Markup document, as read from an `.llx` file or held in memory.

## `internal enum LMarkupTokenKind`

What one scanned tag is, as far as the scanner can tell without knowing the format's meaning. `LMarkupTokenText` is a leaf tag that carries text, `LMarkupTokenEnter` opens a block, and `LMarkupTokenLeave` closes the block it names. Blocks are bounded rather than nested in the token stream: the scan stays flat and a caller that wants the contents of one `<sense>` reads forward from its enter token to its leave token. That keeps the scanner from having to know which tags belong inside which block, which is the format spec's business and not the scanner's.

## `internal readonly record struct LMarkupToken`

One tag, as scanned. `LMarkupTokenName` is the tag name exactly as written. `LMarkupTokenSource` is the attribute value the tag carried, `null` when it carried none — an `src` on an example or situation, and the `id` on a `<source>` block, both land here, because the two are the same thing to a scanner that reads an attribute and forms no opinion about it. `LMarkupTokenText` is the literal inner text with leading and trailing whitespace trimmed and everything inside it untouched: `&`, `<` and quotes are ordinary characters, as section 6 of the format spec requires. `LMarkupTokenEmpty` is set for the empty-element form, `<tag></tag>` or `<tag/>`, which is how the format writes the *unknown* state; a later job turns that flag into `LStateValueUnknown`, and the absence of the token altogether into `LStateValueUnspecified`.

Distinguishing an absent attribute from an empty one matters for the same reason: no `src` is unspecified, `src=""` is unknown, and only a `null` versus an empty string keeps the two apart.

## `internal static IReadOnlyList<LMarkupToken> LMarkupScan(string text)`

Turns a whole Llyn Markup document into the ordered stream of tag tokens it declares. The scan is hand-written rather than handed to an XML reader, because Llyn Markup is not XML: its text is literal, so a document that says `a & b` inside a `<meaning>` is well formed here and rejected there. Reading it as XML would force authors to escape ordinary punctuation, which section 6 of the format spec exists to promise they never have to.

The scan walks the text once. Outside a tag, anything that is not `<` is skipped, so whitespace and stray prose between blocks are insignificant rather than an error. At a `<`, the tag name is read, then its attributes, of which only `src` and `id` are kept and the rest are dropped — an unrecognised attribute, like an unrecognised tag, must not break an older file. A block tag emits an enter token and, at its `</name>`, a leave token. Any other tag is a text tag: its content is taken literally up to the first `</name>`, which is why the one sequence an author cannot write inside text is that tag's own closing tag. Nesting therefore goes one level for text tags — a tag inside a text tag is part of the text.

Structural damage is an error, and the message names the character position so an import can report where the file went wrong. Unclosed tags, a `<` that no name follows, a closing tag matching no open block, and a block still open at the end of the text all raise a `FormatException`.

**Parameters**

- `text` — The whole Llyn Markup document.

**Returns** — Every tag in the order the document writes it, with block tags bounded by an enter and a leave token.

## `internal readonly record struct LMarkupReference`

One `<source>` block as read. `LMarkupReferenceId` is the `id` the block declared, repeated outside the reference so a caller can index the block by it without unpacking the value; an `id` is meaningful only inside its own entry, so the index a caller builds is per entry and never shared between them.

`LMarkupReferenceAuthor` exists because a source's author is not part of an `LReference`. An `LReference` records only whether an author was recorded — `LReferenceAuthorState` — while the names themselves are `LAuthor` rows attached to the reference in order, shared with every other reference that credits the same person. Reading is pure and cannot create or match those rows, so the author text is carried out beside the reference and left for the layer that owns author identity to attach. Dropping it here would silently lose a written author on import, which is the one thing the format promises not to do.

## `internal static LCardDraft LMarkupCardRead(IReadOnlyList<LMarkupToken> tokens)`

Turns the tokens of one `<sense>` or `<collocation>` block into the card draft it describes. Both card kinds carry the same tags and differ only in the list they join, so one reader serves both and the caller decides which list the result belongs to.

The tokens handed in are the whole block, its enter and leave tokens included; those two are skipped, as is any tag the format does not name, because section 1 of the format spec makes an unrecognised tag ignorable rather than an error. Everything else is read by tag name in one pass, so `<tag>`, `<example>`, `<situation>` and `<image>` keep the order the document writes them in, which section 7 makes meaningful.

A tag that may appear only once — `<title>`, `<expression>`, `<meaning>`, `<synonym>` — is not an error when it appears twice; the last one written wins. A document that repeats a singular tag is malformed in a way the format does not describe, and refusing the whole import over it would be harsher than the format's leniency elsewhere warrants.

The `src` on an example or a situation is kept as it was written and is not resolved to a source here: a card can be read before the `<source>` blocks of its entry have been, so the citation stays a raw key until entry assembly resolves it against the entry's sources. It is stored as an `LStateValue` because the raw key already carries the three states — no `src` is unspecified, `src=""` is unknown, and a named `src` is specified — and the state survives resolution unchanged when the key does not.

**Parameters**

- `tokens` — Every token of one card block, in document order.

**Returns** — The card as a draft, with no id, because an imported card has none until it is saved.

## `internal static LMarkupReference LMarkupReferenceRead(IReadOnlyList<LMarkupToken> tokens)`

Turns the tokens of one `<source>` block into the reference it declares, keyed by its `id`. The `id` comes from the block's enter token, which is where the scanner puts the attribute a tag carried; a block written without one reads as an empty key, which no `src` can cite, so an unkeyed source simply goes uncited rather than breaking the read.

Every field runs through the same three-state reading the card fields do, and the author's state is stored on the reference while the author's text leaves beside it.

**Parameters**

- `tokens` — Every token of one `<source>` block, in document order.

**Returns** — The reference, its `id`, and the author text the block wrote.

## `private static LStateValue LMarkupStateRead(LMarkupToken? token)`

The one place the format's three states become an `LStateValue`, so that no field decides for itself what an empty tag means. A token that was never seen is *unspecified*, a token carrying the empty flag is *unknown*, and a token with text is *specified*. The distinction is the format's most important rule and the reason a missing tag and an empty one cannot be treated alike.

The value form of the helper is deliberately not `LStateValueRead`, which folds blank text into *unspecified*. Here an empty tag has already been distinguished from an absent one by the scanner, and folding the two back together would throw away exactly what the document went to the trouble of writing.

**Parameters**

- `token` — The tag as scanned, or `null` when the block never wrote it.

## `private static LStateValue LMarkupStateRead(string? source)`

The same three states read off an attribute rather than a tag: an attribute the tag never carried is *unspecified*, one written empty is *unknown*, and one with text is *specified*. A citation follows the same rule as a field, so it is read by the same helper under the same name.

**Parameters**

- `source` — The attribute value as scanned, or `null` when the tag carried none.
