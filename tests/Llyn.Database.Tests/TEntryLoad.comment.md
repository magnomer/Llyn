# TEntryLoad.cs

## `public sealed class TEntryLoad`

Covers reading entries back: listing and searching them by headword, and loading one whole entry into the draft it was saved from. The load is the inverse of the save, so what a test writes through `LEngineEntrySave` is exactly what comes back — headword, language, pronunciation and note, and every card with its definition and the whole ordered set of Examples, Situations and Tags it references.

## Inline notes

### `Assert.Equal(["sword", "word"], engine.LEngineEntryFind(string.Empty).Select(e => e.LEntryHeadword));`

An empty query lists everything, ordered by headword.

### `Assert.Equal(["sword", "word"], engine.LEngineEntryFind("WORD").Select(e => e.LEntryHeadword));`

A partial headword narrows it, case-insensitively, and matches anywhere in the headword — "word" is inside "sword" too, so a query that picks exactly one has to be one of its own.

### `TEntryCardMatch(word.LEntryDraftSenses, loaded.LEntryDraftSenses);`

The cards are compared field by field: a draft holds its cards, and a card its Examples, Situations and Tags, in lists, and list equality is reference equality — never true across a round trip through the store.

### `private static void TEntryCardMatch(IReadOnlyList<LCardDraft> expected, IReadOnlyList<LCardDraft> actual)`

Asserts that two card lists carry the same cards in the same order, every field included: the save and the load are inverses, so a card that was written comes back exactly as it went in.

### `string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");`

The file the downloader would have written: under the workspace, in its audio bucket.

### `LPronunciationArchive pronunciations = new(workspace.TWorkspaceDatabase);`

Stored relative, so a workspace that is moved or copied keeps its audio. The row is read through the archive rather than as SQL text, so the path separator stays the platform's.

### `LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(stored.LEntryId));`

The titles are columns of the cards themselves, so they are on the rows before any load.

### `LSense sense = Assert.Single(new LSenseArchive(workspace.TWorkspaceDatabase).LSenseRead(stored.LEntryId));`

The rows are stored at the positions the lists held, so nothing shares a position and the order is a fact of the store rather than of the read.

### `LCardDraft card = Assert.Single(loaded.LEntryDraftSenses);`

Three tags and two examples on the one card, all five back in the order they were written.

### `LCardDraft phrase = Assert.Single(loaded.LEntryDraftCollocations);`

The collocation card references its own sets on the same terms.

### `LEntry stored = engine.LEngineEntrySave(new LEntryDraft(`

A blank value in a list is not a row: an empty field detaches, and the independent tables stay empty rather than collecting rows with no text.
