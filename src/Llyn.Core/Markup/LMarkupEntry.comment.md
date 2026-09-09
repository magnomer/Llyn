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

## `private static LMarkupEntry LMarkupEntryCreate(LMarkupCatalog catalog, IReadOnlyList<LMarkupToken> tokens, int first, int last, int place)`

Reads the tokens of one `<entry>` block into its draft.

The block is read in one pass.
A nested block is handed whole to the reader that owns it.
Those blocks are `<sense>`, `<collocation>`, `<inflection>` and `<pronunciation>`.
The catalog is handed down to the card reader, because a card cites rows by key.
The walk resumes after its leave token.
So a `<title>` inside a meaning is never mistaken for a field of the entry.
Everything else is a leaf tag read by name.
An unrecognised one is ignored, as section 1 of the format spec requires.
Meanings and collocations keep the order they were written in, which section 8 makes meaningful.

Citations are not checked here.
The document has already held every key in the file against the one namespace.
Checking again per entry would report the same fault twice and scope it wrongly.

`<form>` and `<pos>` are leaf tags of the entry and are read here in the order they stand.
The entry's own text fields are plain text rather than three-state values.
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

- `catalog` — The rows the document declared, so a card can resolve what it cites.
- `tokens` — The whole document's tokens.
- `first` — Index of the entry's enter token.
- `last` — Index of its matching leave token.
- `place` — The entry's position in the file, counted from one, used only to name it in an error.

## `private static LPronunciationDraft? LMarkupSoundRead(LMarkupToken token, IReadOnlyList<LMarkupToken> block, int place)`

Reads a whole `<pronunciation>` block into the draft the entry carries.
Section 4 of the format spec puts the reading, the syllables, the representations and the audio inside it.
The block's `level` is an attribute of the opening tag, so the tag itself is handed in.
A block holding nothing at all gives back null, because an empty pronunciation is no pronunciation.
The recording's `source` attribute records where the audio came from and travels on the draft.
The audio's own added time is bookkeeping and is never read from a file.

**Parameters**

- `token` — The `<pronunciation>` enter token, which carries `level`.
- `block` — The block's tokens, from its enter token to its leave token.
- `place` — The entry's position in the file, used only to name it in an error.

## `private static LSyllable LMarkupSyllableRead(LMarkupToken token, int position, int place)`

Reads one `<syllable>` and keeps its place among the syllables before it.
A nucleus is the one part a syllable cannot be written without.
Section 9 refuses the file over a syllable that names none, and the message names the element.
Every other segment is optional and comes back null when the tag never wrote it.

**Parameters**

- `token` — The `<syllable>` tag as scanned.
- `position` — Its place among the syllables read so far.
- `place` — The entry's position in the file, used only to name it in an error.

## `private static int? LMarkupToneRead(string? tone)`

Reads the tone number a syllable carries, or null when it carries none.
The store keeps a number, so a value that is not one records nothing rather than refusing the file.

**Parameters**

- `tone` — The `tone` attribute as written, or null when the tag omitted it.

## `private static LRepresentation LMarkupRepresentationRead(LMarkupToken token, int position)`

Reads one `<representation>`, which is the word written in another system.
`system` and `role` are what the representation is in and what it is for.
The local tone marking is optional and rides on `tone`.
The text is the representation itself.

**Parameters**

- `token` — The `<representation>` tag as scanned.
- `position` — Its place among the representations read so far.

## `private static void LMarkupInflectionRead(List<LInflection> inflections, LMarkupToken token, IReadOnlyList<LMarkupToken> block)`

Reads one `<inflection>` block and the features that produce it.
`local` and `pos` are attributes of the block and `<text>` is the form itself.
Features keep their order inside the inflection, and inflections keep theirs inside the entry.
An inflection with no form records nothing, because the form is the whole of it.

**Parameters**

- `inflections` — The inflections read so far, in document order.
- `token` — The `<inflection>` enter token, which carries `local` and `pos`.
- `block` — The block's tokens, from its enter token to its leave token.

## `private static void LMarkupFormRead(List<LForm> forms, LMarkupToken token)`

Reads one `<form>`, which is a variant form of the headword.
`role` names what the form is and `local` is its label in the interface language.
A form is plain text, so an empty tag records nothing at all.

**Parameters**

- `forms` — The forms read so far, in document order.
- `token` — The `<form>` tag as scanned.

## `private static void LMarkupSpeechRead(List<LSpeechDraft> speeches, LMarkupToken token, int place)`

Adds the part of speech one `<pos>` names, keeping the order the entry writes.
The format writes one tag per part rather than one comma-separated tag.
`<pos id="verb"/>` names a value a language pack defines.
`<pos>서술어</pos>` is the custom name a user typed.
The draft keeps the two apart, which is the whole reason the tag has two forms.
A tag carrying both, or neither, refuses the file as section 9 requires.
The message names the element, because that is what an author needs to find the line.

**Parameters**

- `speeches` — The parts read so far, in document order.
- `token` — The `<pos>` tag as scanned.
- `place` — The entry's position in the file, used only to name it in an error.

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
That is the shape `LMarkupReferenceRead` and the catalog's row readers take.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the block's enter token.
- `last` — Index of its matching leave token.
