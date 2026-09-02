# LEngineMarkup.cs

## `public sealed partial class LEngine`

The import path for Llyn Markup: text in, stored entries out, and nothing in between that the rest of the program can see half-done. It reads with `LMarkup`, which is pure and knows no store, and writes with `LEngineEntrySave`, which is the one write path an entry has; the only work that belongs here is what neither of those can do alone — turning the sources a document declares into the reference rows its citations must name, and holding the whole file inside one transaction.

## `public IReadOnlyList<LEntry> LEngineMarkupImport(string text)`

Imports a whole Llyn Markup document, adding every entry it declares to the workspace, and returns the stored entries in the order the file wrote them.

The document is read before anything is written. A file that is malformed anywhere — a broken tag, an entry with no headword, a citation naming no source — therefore fails with a `FormatException` while the workspace is still untouched, which is the cheapest way for the common failure to end.

Everything the read produced is then written inside one session. Section 9 of the format spec makes an import atomic, and one session is what makes that true: `LEngineEntrySave` opens a nested session that commits nothing of its own, so the entries accumulate in the transaction this method owns, and the single commit at the end is the only moment any of them exist. A failure at the fourth entry rolls back the three before it, and the exception names the entry that failed by its place in the file, counted from one, so a shell can say which entry went wrong without knowing how the file was read. The failure that caused it travels as the inner exception rather than being flattened into the message, because the place alone does not tell an author what to change; a shell that shows only the outer message would replace a usable reason with a number.

It is `LMarkupEntryRead` rather than `LMarkupRead` that is called, because a draft alone is not enough to save: an example carries the key of the source it cites, and the source's title, year and authors travel beside the draft. Saving from the drafts alone would store citations pointing at sources that were never created.

**Parameters**

- `text` — The whole Llyn Markup document, as read from an `.llx` file.

**Returns** — One stored entry per `<entry>` block, in document order.

## `private LEntry LEngineMarkupSave(LMarkup.LMarkupEntry entry, IDictionary<string, string> writers)`

Writes one entry as read: the sources it declared first, then the entry itself with every citation naming the row its key created, then the entry's hold on those sources.

The sources go first because a citation cannot name a row that does not exist yet, and the entry goes before the attachment for the same reason in the other direction. The index from key to row is built per entry, since an `id` is scoped to its entry, so two entries reusing one key get a row each — which is what section 5 of the format spec means when it says the same `id` text collides across entries with nothing. Within one entry the reader has already refused a key declared twice, so the index cannot lose a source.

Every declared source is attached to the entry, in the order the file wrote them, and not only the ones a citation names. A `<source>` block is the entry's statement that it draws on that work; a source no example happens to cite is still declared, and one that were left unattached would be a row no entry owns — invisible in the editor, and left behind when the entry is deleted, since it is the entry's hold on it that cascades.

The draft comes back rebuilt rather than edited, because a draft is immutable; only its cards change, and only in their citations.

**Parameters**

- `entry` — One entry as the reader produced it: the draft, and the sources it declared.
- `writers` — The author rows this import has already created, indexed by name.

**Returns** — The stored entry.

## `private string LEngineReferenceSave(LMarkup.LMarkupReference source, IDictionary<string, string> writers)`

Writes one declared source as a reference row and returns its id.

The reference is created through the engine's own `LEngineReferenceCreate`, and its authors through `LEngineAuthorCreate` and `LEngineAuthorAttach`, so an imported source is written by the same calls the editor writes one with and there is no second copy of that SQL. The reference's author state is carried as the reader parsed it — specified, unknown or unspecified — and the names are attached in the order the block wrote them, because an author's position lives on the reference that credits them.

One name is written once per import. An author row is shared by every reference that credits the person, and a document that cites the same author across several sources is naming one person each time; writing a row per mention would leave the store with duplicates that nothing can tell apart and that no later edit can merge. The index reaches no further than the import, because matching an imported name against a row already in the store is a judgement about who somebody is, and text alone does not make it.

**Parameters**

- `source` — One `<source>` block as read: its `id`, the reference it declares, and the author names it wrote.
- `writers` — The author rows this import has already created, indexed by name.

**Returns** — The id of the stored reference, which is what a citation must now carry.

## `private static IReadOnlyList<LCardDraft> LEngineCardResolve(IReadOnlyList<LCardDraft> cards, IReadOnlyDictionary<string, string> rows)`

Rewrites every example and situation of every card with its citation pointing at the stored reference instead of the key the file wrote.

**Parameters**

- `cards` — The cards as read, their citations still the document's keys.
- `rows` — The id of the reference row created for each declared `id`.

## `private static LStateValue LEngineReferenceResolve(LStateValue citation, IReadOnlyDictionary<string, string> rows)`

Turns one citation from the key the document wrote into the id of the row that key created.

Only a *specified* citation is resolved: no `src` stays *unspecified* and `src=""` stays *unknown*, because neither names a source and section 4 of the format spec keeps the two apart to the end. A key with no row raises a `FormatException` naming it. The reader has already refused a key that no `<source>` declared, so this cannot fire on a document that was read; it stands because a citation silently dropped would store an entry that cites nothing, and losing what the file wrote is the one outcome the import may not have. This is the only place a citation changes: what the reader left on the card is the key the author wrote, and it becomes a row id here, where rows exist.

**Parameters**

- `citation` — The citation as the reader resolved it, in one of the three states.
- `rows` — The id of the reference row created for each declared `id`.
