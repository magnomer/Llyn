# LDraft.cs

## `public sealed record LDraft(`

One tentative record, held in the workspace folder before it reaches the database.
It is plain data and knows nothing of files or the database.
A draft carries its own throwaway id, discarded once the record becomes real.
One draft holds one kind of work, told apart by which content field carries it.
A draft naming no example, no situation and no source holds an entry.
One naming an example holds a sentence and one naming a situation holds a context.
One naming a source holds a citation.
One naming a held author holds that author's name.
The kind is read from the content rather than from a tag beside it, so the two cannot disagree.

**Parameters**

- `LDraftId` — Throwaway id of this draft, valid only while the draft is unsaved.
- `LDraftOrigin` — Panel that started the draft.
  The input panel and the library can each hold one at the same time.
- `LDraftEntryId` — Id of the real record being edited, or empty for a new one.
  It names an entry, an example, a situation or a source, whichever kind the draft holds.
- `LDraftContent` — The entry form contents as one immutable value.
  A draft of another kind carries a blank one here, because its work lives in its own field.
- `LDraftMoment` — Instant the draft was last written.
- `LDraftExample` — The sentence being written, or null when this draft holds no sentence.
- `LDraftSituation` — The context being written, or null when this draft holds no context.
- `LDraftReference` — The source being written, or null when this draft holds no source.
- `LDraftVersion` — Shape of the file this draft was read from, stamped by the archive on every write.
  A file the current build did not write is not a draft.
  A reader that finds another number skips it.
  There is no compatibility read, because a field renamed underneath an old file would commit as a new entry.
- `LDraftAuthor` — The authors credited on the source being written, in order.
  Empty for a draft holding no source.
  A credit with a minted id names an author commit creates before attaching.
- `LDraftAuthorHeld` — The author being named, or null when this draft holds no author.
  An author draft carries no source and no credits, because an author is a shared row edited alone.

## `public IReadOnlyList<LAuthor> LDraftAuthor { get; init; }`

Never null, so a reader walks it without a check.

## `public IReadOnlyList<LAuthorRow> LDraftCreditRead()`

The credits as editor rows, each knowing its place and whether it can move.
The source editor copies them and splices its own blank row in, so it never counts the credits itself.

## `public LDraft LDraftNormalize()`

The same draft, whichever kind of record it holds with every unreadable value dropped to unspecified.
Called only after the user agreed to lose what the store could not read.

## `public string LDraftAuthorName`

The name of the held Author, or empty while the draft holds none, so a name field writes without branching.
