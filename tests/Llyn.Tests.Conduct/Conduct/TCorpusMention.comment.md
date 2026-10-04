# TCorpusMention.cs
Hash: `cb4d2c7383c9ded1`

## `public sealed class TCorpusMention`

Covers the excerpt's word click and the shared Mention reads, on a real workspace.
The shared reads prepare a held transcript through `TTranscriptMention.TMentionPrepare`.
A failed chip line or Meaning read reports `Mention.FindFailed` once and answers nothing.
A failed Meaning read hands the envoy the `Notice.Unexpected` ledger notice.
The Meaning read leads with the whole Entry, maps each row, and asks the engine with the unknown key.
The excerpt click stays a corpus gate, since it asks the leave question.
An excerpt click with no chosen Example finds nothing, and a kept leave asks once and finds nothing.
A failed word find reports `Mention.FindFailed` and offers nothing.
One found Entry opens at once and leaves an empty menu, and several stay offered under the found word.

## `private static List<long> TMentionLibraryAdd(CAtelier atelier)`

Registers a Library tab whose arrivals the test reads, so an Entry open is observable.
