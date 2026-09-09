# TMarkupRound.cs

## `public sealed class TMarkupRound`

Covers the round trip: an entry exported and re-imported is the same entry.

The reader and the writer are one contract, and this is where the contract is held.
A field the writer drops and a tag the reader ignores both show up here as a difference.
Neither shows up in a test of the reader alone or the writer alone.

Every test drives the engine rather than the writer, because only the engine knows a workspace.
The comparison is over the loaded drafts, not over the file, so formatting is free to change.

## Inline notes

### `private const string TMarkupSample`

One document carrying everything the round trip has to keep.
A shared example, a frame with no example, a sub-sense, every kind of citation, a translation between entries.
A second entry exists so that the translation has an entry in the file to name.

### `MarkupExport_EveryFieldOfAnEntry_ComesBackTheSame`

The whole promise in one test: two workspaces, and the same entry in both.
The comparison is field by field through a rendering, so a failure names the field that differs.
Row ids differ between workspaces, so what a citation names is compared by its content.

### `MarkupExport_CustomSpeech_StaysCustom`

A language-pack value and a typed name are written differently and stored differently.
A custom name that came back as a lookup would resolve to nothing in the workspace that read it.

### `MarkupExport_TheSameWorkspaceTwice_WritesTheSameBytes`

Keys are derived from row content and rows are written in a fixed order.
Without both, two exports of one unchanged workspace would differ and every diff would be noise.

### `MarkupExport_ExampleQuotedTwice_WritesOneCatalogRow`

Five uses over two examples, and one source under them.
A row is written once however many citations name it, which is why examples live in the catalog.

### `MarkupExport_ExampleQuotedTwice_ImportsAsOneRow`

The other half of sharing: the file that declares one row must store one row.
Situations, images and videos are counted too, because each is cited by one card and each is one row.

### `MarkupExport_SubSenseTree_SurvivesASecondTrip`

Exporting an export gives the same file back.
A tree that lost a level, or gained one, would not survive the second trip.

### `MarkupExport_UncitedSourceAndUncreditedAuthor_ComeBack`

Section 3 keeps a source nothing quotes and an author credited on nothing.
A row reachable from no card is the row that would be lost if reach decided what to export.

### `MarkupExport_TwoAuthorsOfOneName_StayTwoRows`

Two keys are what keep two namesakes apart, in the file and in the store after it.

### `MarkupExport_GlossPresentAndAbsent_KeepsBothAndNoThirdCase`

A gloss is present or absent and has no unreadable form.
An empty `<gloss></gloss>` would be that third case, so the test says it is never written.

### `MarkupExport_FrameWithNoExample_ComesBackAsAFrame`

A use may state a frame and quote nothing.
Writing an example for it would invent a sentence the card never had.

### `MarkupExport_TranslationBetweenEntries_KeepsThePointer`

A translation names another entry of the same file by its key.
Following it back to the headword proves the key survived both the write and the read.

### `private static string TMarkupRoundExport(string text, out IReadOnlyList<LEntry> imported)`

Imports a document into a fresh workspace and exports it straight back out.
Every test that is about the written file rather than the store starts here.

### `private static int TMarkupRoundFind(string text, string named)`

How many times a tag opens in the written file.
Counting is enough to say a row was written once, and does not pin how it was formatted.

### `private static void TMarkupRoundFormat(StringBuilder text, LEntryDraft draft)`

Renders the entry-level detail: parts of speech, forms, inflections, syllables and representations.
Each part of speech is rendered as its stored value and its custom name, so the two never read alike.

### `private static string TMarkupRoundFormat(LEngine engine, string entryId)`

Renders one stored entry as text, field by field, so that two workspaces can be compared.
Row ids are workspace-local, so a citation is rendered as the content of the row it names.
The recording is rendered by file name, because a workspace resolves it against its own folder.
