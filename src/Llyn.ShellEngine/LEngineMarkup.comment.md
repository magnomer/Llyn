# LEngineMarkup.cs

## `public sealed partial class LEngine`

The import path for Llyn Markup.
Text goes in and stored entries come out.
Nothing in between is visible half-done to the rest of the program.
It reads with `LMarkup`, which is pure and knows no store.
It writes with `LEngineEntrySave`, which is the one write path an entry has.
The only work that belongs here is what neither of those can do alone.
That is turning the catalog a document declares into the rows its citations must name.
It is also holding the whole file inside one transaction.

## `public IReadOnlyList<LEntry> LEngineMarkupImport(string path)`

Imports a whole Llyn Markup document, adding its catalog and every entry it declares to the workspace.
Takes the path of the file, which it opens itself.
Returns the stored entries in the order the file wrote them.

File access belongs to the engine, and a caller hands over a path rather than a string it read.
A path naming no file fails with a `FileNotFoundException` before anything is read or written.

The document is read before anything is written.
A file that is malformed anywhere therefore fails with a `FormatException`.
A broken tag, an entry with no headword, or a citation naming no row is such a fault.
The workspace is still untouched, which is the cheapest way for the common failure to end.

Everything the read produced is then written inside one session.
Section 9 of the format spec makes an import atomic, and one session is what makes that true.
`LEngineEntrySave` opens a nested session that commits nothing of its own.
So the catalog and the entries accumulate in the transaction this method owns.
The single commit at the end is the only moment any of them exist.
A failure at the fourth entry rolls back the three before it and the catalog with them.
The exception names the entry that failed by its place in the file, counted from one.
So a shell can say which entry went wrong without knowing how the file was read.
The failure that caused it travels as the inner exception rather than flattened into the message.
The place alone does not tell an author what to change.
A shell that shows only the outer message would replace a usable reason with a number.

It is `LMarkupEntryRead` rather than `LMarkupRead` that is called.
A draft alone is not enough to save.
The rows a card cites live in the catalog, not on the draft.
Saving from the drafts alone would store citations pointing at rows that were never created.

**Parameters**

- `path` — The path of the `.llx` file to import.

**Returns** — One stored entry per `<entry>` block, in document order.

## `private IReadOnlyDictionary<string, string> LEngineCatalogSave(LMarkup.LMarkupCatalog catalog, IReadOnlyList<LMarkup.LMarkupEntry> entries)`

Writes the catalog once for the whole document, and returns the row id each key created.

Once per document is the point of the catalog.
A row declared once and cited by four cards is one row, and the file says so by citing a key.
The old format declared a source inside each entry, so the same work imported once per entry.

The order the kinds are written in is the order they point at each other.
Authors come first, because a source credits them.
Sources come next, because an example cites one.
Nothing else in the catalog points at anything, so the rest follow in any order.

Every declared row is written, whether or not a citation names it.
Section 3 of the format spec keeps a source nothing quotes and an author credited on nothing.
Declaring a row is the author's statement that the workspace holds it.

The keys themselves are not written anywhere.
A row is created with an empty id and takes the identifier the store hands out.
Section 2 of the format spec says a key is meaningful only inside its own file.
Writing the key as the row's id would let two files disagree about which row `oed` is.

**Parameters**

- `catalog` — Every row the document declared, under its key.
- `entries` — The document's entries, read only for the language an example may not state.

**Returns** — The id of the stored row for every key the catalog declared.

## `private static string LEngineLanguageRead(string key, LExample example, IReadOnlyList<LMarkup.LMarkupEntry> entries)`

Decides which language a catalog example is stored under.

An example states its own `lang`, and that is used whenever it is written.
An example that states none falls back to the language of the first entry that declares one.
A stored example carries a language, and a sentence in a file about English is English.
A document with no language anywhere raises a `FormatException` naming the example.
Guessing there would file the sentence under a language the author never wrote.

**Parameters**

- `key` — The example's key, used only to name it in the message.
- `example` — The example as read.
- `entries` — The document's entries, in file order.

**Returns** — The language the example is stored under.

## `private string LEngineRegisterSave(LRegisterArchive registers, LRegister written)`

Writes one declared register and returns its id.

A register the file marks `builtin="yes"` is matched to the workspace's own register of that name and language.
The language pack's registers are created first, so the match is against what the pack ships.
A built-in register is the pack's, and importing one must not make a second copy of it.
A register with no match, and every register the file does not mark, is created as a user register.
Section 3 of the format spec asks for exactly that fallback.

**Parameters**

- `registers` — The register store this import writes through.
- `written` — The register as the file declared it.

**Returns** — The id of the register the file's key now names.

## `private LEntry LEngineMarkupSave(LMarkup.LMarkupEntry entry, IReadOnlyDictionary<string, string> rows)`

Writes one entry as read, with every citation naming the row its key created.
The catalog is already written, so a citation always finds its row.
The draft comes back rebuilt rather than edited, because a draft is immutable.
Only its cards change, and only in their citations.

**Parameters**

- `entry` — One entry as the reader produced it.
- `rows` — The id of the stored row for every key the catalog declared.

**Returns** — The stored entry.

## `private string LEngineReferenceSave(LMarkup.LMarkupReference source, IReadOnlyDictionary<string, string> rows)`

Writes one declared source as a reference row and returns its id.

The reference is created through the engine's own `LEngineReferenceCreate`.
Its authors are attached through `LEngineAuthorAttach`.
So an imported source is written by the same calls the editor writes one with.
There is no second copy of that SQL.
The reference's author state is carried as the reader parsed it.
It is specified, unknown or unspecified.
The credits are attached in the order the block wrote them.
An author's position lives on the reference that credits them.

Author identity comes from the key, not from the name.
Two `<author>` rows with the same name and different keys are two people and two rows.
The old format deduplicated by name, so a file could not tell one namesake from another.

**Parameters**

- `source` — One `<source>` block as read: its key, the reference it declares, and the author keys it credited.
- `rows` — The id of the stored row for every key the catalog declared.

**Returns** — The id of the stored reference, which is what a citation must now carry.

## `private static IReadOnlyList<LCardDraft> LEngineCardResolve(IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<string, string> rows)`

Rewrites every example of every card.
Its citation points at the stored row instead of the key the file wrote.

**Parameters**

- `cards` — The cards as read, their citations still the document's keys.
- `rows` — The id of the stored row for every key the catalog declared.

## `private static LStateValue LEngineReferenceResolve(LStateValue citation, IReadOnlyDictionary<string, string> rows)`

Turns one citation from the key the document wrote into the id of the row that key created.

Only a *specified* citation is resolved.
No `src` stays *unspecified* and `src=""` stays *unknown*.
Neither names a row, and section 6 of the format spec keeps the two apart to the end.
A key with no row raises a `FormatException` naming it.
The reader has already refused a key the catalog never declared.
So this cannot fire on a document that was read.
It stands because a citation silently dropped would store an entry that cites nothing.
Losing what the file wrote is the one outcome the import may not have.
This is the only place a citation changes.
What the reader left is the key the author wrote.
It becomes a row id here, where rows exist.

**Parameters**

- `citation` — The citation as read, in one of the three states.
- `rows` — The id of the stored row for every key the catalog declared.
