# LEngineTranscription.cs

## `public sealed partial class LEngine`

The transcription side of the engine.
It covers the ordered rows an Entry keeps, one per scheme, and the schemes a language declares.
A transcription is a spelling of the reading in a named scheme, never IPA.
So it lives beside the pronunciation rather than inside it, and neither points at the other.

## `public IReadOnlyList<string> LEngineSchemeRead(string language)`

The transcription schemes the pack of `language` declares, in the order the form shows them.
An empty list means the language shows no transcription line.

## `internal Task LEngineTranscriptionFind(long session, string word, string language, string scheme, LReceiver receiver, CancellationToken cancellation)`

The transcription counterpart of `LEnginePronunciationFind`, asked for one scheme at a time.
The sources are the ones the pack declares under that scheme, so Pinyin and Bopomofo need not share a page.
What the draft already found for that scheme is replayed instead of searched again.
The lookup runs literal: a transcription keeps its spaces and takes no IPA cleanup, because it is not IPA.
No variety fan-out happens either, since a scheme is already the axis a transcription row stands on.

## `private IReadOnlyList<LSource> LEngineSchemeLoad(string language, string scheme)`

The built sources of one scheme, made once per language and scheme and kept for the engine's life.
A scheme the pack does not declare yields no sources, so its lookup ends at once with nothing found.
The glyph section is asked by its name too, so the glyph row looks its form up through this path.

## `internal IReadOnlyList<LTranscription> LEngineTranscriptionRead(long entryId)`

Reads the transcriptions the Entry identified by `entryId` keeps, in order.
Empty when it keeps none.

## `internal IReadOnlyList<LTranscription> LEngineTranscriptionSet(long entryId, IReadOnlyList<LTranscription> transcriptions)`

Makes `transcriptions` the whole list of the Entry, and returns the rows with their ids filled in.

## Inline notes

### `private void LEngineTranscriptionSync(`

The entry's transcriptions reconciled to the draft, on a create and on an update alike.
A seeded row still blank is left out, because the form offered it and nobody asked for it.
A row the user added is stored even blank, so it stands again the next time the entry is edited.
An unchanged list writes nothing and raises no change.
A positive id naming no stored row of this entry refuses the commit rather than rebinding to a fresh row.
The rows are written as one list, so a swap of two schemes lands in one write.
Each minted id is recorded against the draft id it replaces.

### `private static IReadOnlyList<LTranscription> LEngineTranscriptionRead(`

The draft rows as the rows the archive stores, positions from list order.
A blank scheme, or a scheme another row already carries, is refused here.
One scheme spells one reading one way, so a second row under that name would say nothing new.

### `private static string LEngineTranscriptionFormat(IReadOnlyList<LTranscription> transcriptions)`

The list as words, for the revision change text.
