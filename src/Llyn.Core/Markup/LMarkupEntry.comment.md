# LMarkupEntry.cs

## `public static partial class LMarkup`

The entry half of the markup reader: it turns the flat token stream `LMarkup.cs` scans into whole entries, checking each entry's citations against the sources that entry declares. It lives in its own file because scanning text and assembling entries are two responsibilities, and the one file that held both had outgrown the size the project's audit advises; the class stays one class, because callers should see one door into the format and not two.

## `public readonly record struct LMarkupEntry`

One `<entry>` block as read: the draft it describes, and the sources it declared.

`LMarkupEntryDraft` is the whole entry in the shape the editor produces, so an imported entry and a typed one reach the save path as the same value. `LMarkupEntrySource` is kept beside it because a draft has nowhere to put a reference: `LEntryDraftSource` is the label of the source a *recording* came from and is a different thing entirely, and an example or situation carries only the key of the source it cites, never the source's title, year or authors. Those fields would be lost between reading and saving if the block did not travel out whole, and the format's promise is that nothing written is silently dropped.

The sources travel as a list rather than a map keyed by `id` because the list is what the format wrote — order included — and because the map is a working index that only assembly needs. A map would also decide, silently and by position, what a document that declares one `id` twice means; the list holds every block as written and leaves that question to be answered out loud, which assembly does by refusing the file.

## `public static IReadOnlyList<LMarkupEntry> LMarkupEntryRead(string text)`

Scans the whole document once and reads every `<entry>` block it contains, in the order the file writes them.

Only a top-level `<entry>` starts an entry: the walk jumps from an entry's enter token straight past its leave token, so nothing inside one entry can be mistaken for the start of another, and anything between entries is skipped rather than refused, as section 2 of the format spec requires of the whitespace and stray text between blocks.

**Parameters**

- `text` — The whole Llyn Markup document.

**Returns** — Every entry the document declares, each with the sources it wrote.

## `private static LMarkupEntry LMarkupEntryCreate(IReadOnlyList<LMarkupToken> tokens, int first, int last, int place)`

Reads the tokens of one `<entry>` block into its draft.

The block is read in one pass. A nested block — `<sense>`, `<collocation>`, `<source>` — is handed whole to the reader that owns it and the walk resumes after its leave token, so a `<title>` inside a sense is never mistaken for a field of the entry. Everything else is a leaf tag read by name, and an unrecognised one is ignored, as section 1 of the format spec requires. Senses, collocations and sources each keep the order they were written in, which section 7 makes meaningful.

Citations are checked only after the whole block has been read, because `<source>` may be written after the cards that cite it — the sample in section 8 does exactly that — and a reader that checked as it went would reject a perfectly ordinary file. The check is all this layer does with a citation: the key the file wrote is left on the card untouched, because reading is pure and the id a stored citation must carry does not exist until a source has been written to a store. Rewriting the key as itself here would read as resolution and be none, and the layer that does resolve would then be resolving something already claimed to be resolved.

Two `<source>` blocks sharing one `id` raise a `FormatException` naming the key. The entry is the scope an `id` is unique in — section 5 says so, and says two entries may reuse one freely — so a repeat inside one entry is a broken file: every `src` that names the key would have to point at one of the two works, chosen by nothing the author wrote, and the other would vanish. Refusing names the file's mistake while both are still there to name.

The entry's own fields are plain text rather than three-state values, because `LEntryDraft` holds them as strings: an absent tag and an empty one both come out as an empty string. Nothing is lost that the draft could have carried. The headword is the exception the format makes required, and it is required here in the same breath: absent and empty are both no headword, and either raises a `FormatException` naming the entry's place in the file so an import can say which entry went wrong. The two are told apart in the message — no headword against an unreadable one — because the author's next move differs, and the reader knows which it saw. The place is the entry's position in the file counted from one, because the only reader of the message is a person looking at the file, and the first entry of a file is its first, not its zeroth.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the entry's enter token.
- `last` — Index of its matching leave token.
- `place` — The entry's position in the file, counted from one, used only to name it in an error.

## `private static void LMarkupCitationCheck(IReadOnlyList<LCardDraft> cards, IReadOnlySet<string> registry)`

Walks every example and situation of every card and holds each citation against the sources the entry declared. Nothing is rewritten: the cards are already the values the draft will carry, and the only question left about them is whether the keys they hold were ever declared.

**Parameters**

- `cards` — The entry's cards, their citations the keys the file wrote.
- `registry` — The `id` every `<source>` in the entry declared.

## `private static void LMarkupSourceCheck(LStateValue citation, IReadOnlySet<string> registry)`

Refuses a citation that names a source the entry never declared.

Only a *specified* citation is checked. No `src` stays *unspecified* and `src=""` stays *unknown*: neither names a source, so neither can be held against one, and section 4 of the format spec keeps the two apart all the way through. A named key that no `<source>` in the same entry declares is a `FormatException` that quotes the key, because a citation pointing at nothing is a broken file, not a field to be read leniently — leniency is for tags the format may one day add, not for a reference the author meant to make.

**Parameters**

- `citation` — The `src` as read, in one of the three states.
- `registry` — The `id` every `<source>` in the entry declared.

## `private static IReadOnlyList<string> LMarkupSpeechRead(LMarkupToken? token)`

Splits `<pos>` into the entry's parts of speech. The format writes them as one comma-separated tag, and the draft holds them as an ordered list, so the tag is split on commas and each name trimmed. A part that is blank between two commas is dropped rather than kept as an empty speech, and an absent or empty tag reads as no speeches at all.

**Parameters**

- `token` — The `<pos>` tag as scanned, or `null` when the entry never wrote one.

## `private static int LMarkupBlockFind(IReadOnlyList<LMarkupToken> tokens, int first)`

Finds the leave token that closes the block opening at `first`, counting depth so that a block inside a block does not end it. The scanner has already refused any document whose blocks are unbalanced, so the search always finds its match; the last token is returned as a fallback only because the language requires the method to end.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the block's enter token.

## `private static IReadOnlyList<LMarkupToken> LMarkupBlockRead(IReadOnlyList<LMarkupToken> tokens, int first, int last)`

Copies out the tokens of one block, its enter and leave tokens included, which is the shape `LMarkupCardRead` and `LMarkupReferenceRead` both take.

**Parameters**

- `tokens` — The whole document's tokens.
- `first` — Index of the block's enter token.
- `last` — Index of its matching leave token.
