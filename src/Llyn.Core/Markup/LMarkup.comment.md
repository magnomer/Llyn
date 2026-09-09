# LMarkup.cs

## `public static partial class LMarkup`

Reads Llyn Markup text into entry drafts.
Llyn Markup is the plain-text format Llyn imports and exports whole entries in.
One file carries any number of entries as tagged text.
`docs-work/FormatLlynMarkup.md` is the authority on its syntax and on the meaning of every tag.
This class implements that document and decides nothing on its own.
Where the two disagree the document is right and the code is wrong.

The result is a list of `LEntryDraft`, the same shape the editor produces.
So imported text and typed text reach the rest of the program through one door.
Reading stops at drafts: nothing here touches the database, the editor, or any store.

This file owns the card reader.
The scanner stands in `LMarkupToken.cs` and the document reader in `LMarkupEntry.cs`.

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

## `internal static LCardDraft LMarkupCardRead(LMarkupCatalog catalog, IReadOnlyList<LMarkupToken> tokens, int first, int last, int place, bool collocation)`

Turns one `<sense>` or `<collocation>` block into the card draft it describes.
Both card kinds carry the same children and differ in how they name their meaning.
So one reader serves both, and the flag says which the block is.

The block is named by its bounds inside the whole token stream rather than copied out.
A sense holds senses, and the reader calls itself on each child block it meets.
Copying every nesting level would cost a list per sub-sense for nothing.

Only the block's own direct children are read.
Each child block is skipped whole once it has been handled.
So the fields of a nested `<sense>` can never be mistaken for its parent's.
Any tag the format does not name is skipped.
Section 1 of the format spec makes an unrecognised tag ignorable rather than an error.
A `<sense>` inside a `<collocation>` is ignored, because a collocation does not nest.

Everything is read in one pass in document order.
So uses, tags, translations and citations keep the order section 8 makes meaningful.
A tag that may appear only once is not an error when it appears twice, and the last one wins.
Refusing the whole import over it would be harsher than the format's leniency elsewhere warrants.

`<gloss>` and `<labels>` are plain text and not three-state.
`sense.gloss` is a nullable column with no state column beside it, so an empty tag records nothing.
An absent gloss and an empty one are one case, and the draft carries `null` for both.

`<meaning lang="English">` names the language the definition is written in.
Only a sense records it, so a collocation's meaning carries no `lang`.
The reader takes the attribute wherever it stands and lets the writer decide where it belongs.

## `private static LSentenceDraft LMarkupUseRead(LMarkupCatalog catalog, LMarkupToken token, int card, int place)`

Reads one `<use>`: a position in the card that may quote an example and may state a frame.
A card holds uses rather than examples, because an example is a row many cards may quote.

A use that names neither a reference nor a frame records nothing and raises a `FormatException`.
Section 9 of the format spec refuses the file over it.
`LSentenceDraftEmpty` already knows what carrying nothing means, so the check asks it.
The message names the use and the card so the author can find the line.

**Parameters**

- `token` — The `<use>` tag as scanned.
- `card` — The number of the card holding it, counted from one.
- `place` — The number of this use within that card, counted from one.

## `private static LExampleDraft? LMarkupQuoteRead(LMarkupCatalog catalog, string? cited)`

Turns a use's `ref` into the example it quotes.

No `ref` quotes nothing, and the use is a frame alone.
`ref=""` quotes an example that exists and cannot be read, so the draft carries unknown text.
A named `ref` is resolved against the catalog and the row's own fields fill the draft.
The key is kept on the draft, so two uses of one key stay two uses of one row.
The layer that saves rows turns that key into the id of the row it created.

## `private static void LMarkupSituationAdd(LMarkupCatalog catalog, List<LSituationDraft> situations, LMarkupToken token)`

Adds the situation one `<situation ref="...">` cites.
The draft is filled from the catalog row and keeps the key, exactly as a use does.
A citation naming no key records nothing, because a card cites a row or cites nothing.

## `private static void LMarkupRegisterAdd(LMarkupCatalog catalog, List<LRegisterDraft> registers, LMarkupToken token)`

Adds the register one `<register ref="...">` cites, by the same rule as a situation.
The row's language and its built-in mark travel with it, because both decide what import creates.

## `private static void LMarkupImageAdd(LMarkupCatalog catalog, List<LImageDraft> images, LMarkupToken token)`

Adds the image one `<image ref="...">` cites, by the same rule as a situation.

## `private static void LMarkupVideoAdd(LMarkupCatalog catalog, List<LVideoDraft> videos, LMarkupToken token)`

Adds the video one `<video ref="...">` cites, by the same rule as a situation.

## `private static void LMarkupTagRead(List<string> tags, LMarkupToken token)`

Adds one `<tag>` to the card, keeping the order the document writes.
A tag is its own name, so an empty tag names nothing and records nothing.
Leading and trailing spaces are dropped and two tags of one text are one tag.
Section 5 of the format spec states all three rules.

## `private static void LMarkupTranslationRead(List<string> translations, LMarkupToken token)`

Adds the entry one `<translation entry="...">` names.
The key is kept as written, because the entry it names may be declared later in the file.
A translation naming no entry records nothing.

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

## `private static string? LMarkupPlainRead(LMarkupToken? token)`

Reads a field that is plain text rather than three-state.
An absent tag and an empty one both record nothing, so both come back `null`.
`<gloss>` and `<labels>` are the two fields a card carries this way.

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
