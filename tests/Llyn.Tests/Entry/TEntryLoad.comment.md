# TEntryLoad.cs

## `public sealed class TEntryLoad`

Covers reading entries back.
That is listing and searching them by headword.
It is also loading one whole entry into the draft it was saved from.
The load is the inverse of the save.
So what a test writes through `TEngineEntrySave` is exactly what comes back.
That is headword, language, pronunciation and note.
It is also every card with its definition.
It is also the whole ordered set of Examples, Situations and Tags it references.

## `public void EntryLoad_FrameWithNoExample_KeepsTheFrame()`

An Example row carrying a frame and no sentence survives a save and a load.
The frame is the card's own, so nothing about it may wait on a sentence being written first.
The row comes back naming no Example, because none was ever stored.

## Inline notes

### `Assert.Equal(["sword", "word"], engine.LEngineEntryFind(string.Empty).Select(e => e.LEntryHeadword));`

An empty query lists everything, ordered by headword.

### `Assert.Equal(["sword", "word"], engine.LEngineEntryFind("WORD").Select(e => e.LEntryHeadword));`

A partial headword narrows it, case-insensitively, and matches anywhere in the headword.
"word" is inside "sword" too.
So a query that picks exactly one has to be one of its own.

### `TEntryCardMatch(word.LEntryDraftMeanings, loaded.LEntryDraftMeanings);`

The cards are compared field by field.
A draft holds its cards in lists, and a card holds its Examples, Situations and Tags in lists.
List equality is reference equality.
It is never true across a round trip through the store.

### `private static void TEntryCardMatch(IReadOnlyList<LCardDraft> expected, IReadOnlyList<LCardDraft> actual)`

Asserts that two card lists carry the same cards in the same order, every field included.
The save and the load are inverses.
So a card that was written comes back exactly as it went in.

### `string folder = Path.Combine(workspace.TWorkspaceFolder, "audio", "english");`

The file the downloader would have written: under the workspace, in its audio bucket.

### `LPronunciationArchive pronunciations = new(workspace.TWorkspaceDatabase);`

Stored relative, so a workspace that is moved or copied keeps its audio.
The row is read through the archive rather than as SQL text, so the path separator stays the platform's.

### `LMeaning meaning = Assert.Single(new LMeaningArchive(workspace.TWorkspaceDatabase).LMeaningRead(stored.LEntryId));`

The titles are columns of the cards themselves, so they are on the rows before any load.

### `LMeaning meaning = Assert.Single(new LMeaningArchive(workspace.TWorkspaceDatabase).LMeaningRead(stored.LEntryId));`

The rows are stored at the positions the lists held.
So nothing shares a position.
The order is a fact of the store rather than of the read.

### `LCardDraft card = Assert.Single(loaded.LEntryDraftMeanings);`

Three tags and two examples on the one card, all five back in the order they were written.

### `LCardDraft phrase = Assert.Single(loaded.LEntryDraftCollocations);`

The collocation card references its own sets on the same terms.

### `LEntry stored = engine.TEngineEntrySave(new LEntryDraft(`

A blank value in a list is not a row.
An empty field detaches.
The independent tables stay empty rather than collecting rows with no text.
