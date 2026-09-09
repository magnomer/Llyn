# LMarkupEntry.cs

## `public static partial class LMarkup`

The document half of the markup reader.
It turns the flat token stream `LMarkup.cs` scans into a whole document.
A document is the catalog it declares and the entries that cite it.
It lives in its own file because scanning text and assembling a document are two responsibilities.
The class stays one class.
Callers should see one door into the format and not two.

## `public readonly record struct LMarkupEntry`

One `<entry>` block as read: the draft it describes, and the key it declared.

`LMarkupEntryDraft` is the whole entry in the shape the editor produces.
So an imported entry and a typed one reach the save path as the same value.
`LMarkupEntryKey` is kept beside it because a draft has nowhere to put a key.
A relation, a translation or a synonym in another entry names this entry by that key.
An entry that declares none carries the empty string, which no citation can name.

## `public readonly record struct LMarkupDocument`

One `.llx` file as read.

`LMarkupDocumentCatalog` is every row the file declared and the key each was declared under.
`LMarkupDocumentEntry` is every entry, in the order the file wrote them.
The two travel together because neither is the whole file.
An entry cites rows it does not own, and a row is kept whether or not an entry cites it.
Returning drafts alone would drop the catalog, which is where sharing is written down.

## `public static LMarkupDocument LMarkupEntryRead(string text)`

Scans the whole document once and reads the catalog and every `<entry>` it contains.

The first element must be `<llyn>`, and a file that opens otherwise raises a `FormatException`.
Section 2 of the format spec makes it the document element.
A file in the retired format opens with `<entry>` and is refused here rather than half read.
Only the root's own children are read, so an entry written outside it is not an entry.

The catalog is read whole before any citation is resolved.
Section 10 of the format spec declares an author after the source that credits him.
A reader resolving as it went would refuse that file.
Entry and sense keys join the catalog's keys in one namespace, which is where a repeat is caught.
Every citation in the file is then held against that namespace before an entry is read.
So a dangling key is reported once, from the document, rather than by each reader that meets it.

**Parameters**

- `text` — The whole Llyn Markup document.

**Returns** — The catalog and every entry the document declares.

## `private static LMarkupCatalog LMarkupCatalogFind(IReadOnlyList<LMarkupToken> tokens, int root)`

Finds the `<catalog>` among the root's children and reads it.
A document with no shared rows may omit it, and an absent catalog reads as an empty one.
Section 2 of the format spec allows the omission outright.

**Parameters**

- `tokens` — The whole document's tokens.
- `root` — Index of the leave token that closes `<llyn>`.

## `private static LMarkupEntry LMarkupEntryCreate(IReadOnlyList<LMarkupToken> tokens, int first, int last, int place)`

Reads the tokens of one `<entry>` block into its draft.

The block is read in one pass.
A nested block is handed whole to the reader that owns it.
Those blocks are `<sense>` and `<collocation>`.
The walk resumes after its leave token.
So a `<title>` inside a meaning is never mistaken for a field of the entry.
Everything else is a leaf tag read by name.
An unrecognised one is ignored, as section 1 of the format spec requires.
Meanings and collocations keep the order they were written in, which section 8 makes meaningful.

Citations are not checked here.
The document has already held every key in the file against the one namespace.
Checking again per entry would report the same fault twice and scope it wrongly.

The entry's own fields are plain text rather than three-state values.
`LEntryDraft` holds them as strings.
An absent tag and an empty one both come out as an empty string.
Nothing is lost that the draft could have carried.
The headword is the exception the format makes required.
It is required here in the same breath.
Absent and empty are both no headword, and either raises a `FormatException`.
The message names the entry's place in the file, so an import can say which entry went wrong.
The two are told apart in the message, no headword against an unreadable one.
The author's next move differs, and the reader knows which it saw.
The place is the entry's position in the file counted from one.
The only reader of the message is a person looking at the file.
The first entry of a file is its first, not its zeroth.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the entry's enter token.
- `last` — Index of its matching leave token.
- `place` — The entry's position in the file, counted from one, used only to name it in an error.

## `private static IReadOnlyList<string> LMarkupSpeechRead(LMarkupToken? token)`

Splits `<pos>` into the entry's parts of speech.
The old format wrote them as one comma-separated tag, and the draft holds them as an ordered list.
So the tag is split on commas and each name trimmed.
A part that is blank between two commas is dropped rather than kept as an empty speech.
An absent or empty tag reads as no speeches at all.

**Parameters**

- `token` — The `<pos>` tag as scanned, or `null` when the entry never wrote one.

## `private static int LMarkupBlockFind(IReadOnlyList<LMarkupToken> tokens, int first)`

Finds the leave token that closes the block opening at `first`.
It counts depth so that a block inside a block does not end it.
A `<sense>` inside a `<sense>` is the case that depends on it.
The scanner has already refused any document whose blocks are unbalanced.
So the search always finds its match.
The last token is returned as a fallback only because the language requires the method to end.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the block's enter token.

## `private static IReadOnlyList<LMarkupToken> LMarkupBlockRead(IReadOnlyList<LMarkupToken> tokens, int first, int last)`

Copies out the tokens of one block, its enter and leave tokens included.
That is the shape `LMarkupCardRead` and `LMarkupReferenceRead` both take.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the block's enter token.
- `last` — Index of its matching leave token.
