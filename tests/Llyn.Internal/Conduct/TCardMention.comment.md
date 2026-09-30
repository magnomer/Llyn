# TCardMention.cs

## `public sealed class TCardMention`

Covers the mention gates and reads of the sentence area over an entry desk on a real workspace.
A linked Mention offers its Entry's Meanings, and the sense gate narrows it to one, which its chip names.
A silent Mention may be unlinked but offers no sense.
A Mention drops by the selection over it and by its chip's id.
The chip line names a linked headword, and keys a silent chip with no name or sense.
A failed chip line read reports `Mention.FindFailed` once and answers no rows.

## `private static (CDesk TMentionDesk, CSentence TMentionGate, long TMentionSheet, long TMentionRow) TMentionPrepare(LEngine engine)`

Starts an entry desk with one meaning card holding one typed sentence row, and answers the ids.

## `private static IReadOnlyList<LMentionDraft> TMentionRead(CDesk desk, long sheet)`

The Mentions the card's first row holds in the held draft.

## `internal static (long TMentionEntry, long TMentionSense) TMentionEntryCreate(LEngine engine)`

Stores an Entry with one Meaning, and answers the Entry and the Meaning ids.
