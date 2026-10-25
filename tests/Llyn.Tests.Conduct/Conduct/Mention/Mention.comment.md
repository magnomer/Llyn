# Mention

This folder holds the mention gates tying a word of an Example to an Entry.
They run from the atelier, the card sentence area and the corpus transcript.

## `TAtelierMention`

Covers the mention gates, mostly over fake ports that answer with the real span rules.
A linked Mention divides the text into three pieces, and only the linked one is marked.
A code-point offset converts to a UTF-16 unit and back through the display find gate.
Through that gate, two units past the astral character reach the engine two code points on.
A piece answers the unit of an offset counted from its own start, and nothing for an offset outside it.
A click result carries the stored Mention's entry and sense, and a click on nothing stored carries no mark.
A click on a stored Mention opens its entry and then raises its sense, if it names one.
An unlinked Mention or a declined leave opens nothing and raises no sense.
A click on one candidate opens it.
A click on several candidates or none opens nothing and offers the list.
Every open places the offer on the found word, as its UTF-16 unit in the whole text.
A result with one target names its id, and a result with several names none.

## `TCardMention`

Covers the mention gates and reads of the sentence area over an entry desk on a real workspace.
A linked Mention offers its Entry's Meanings, and the sense gate narrows it to one, which its chip names.
A silent Mention may be unlinked but offers no sense.
A Mention drops by the selection over it and by its chip's id.
The chip line names a linked headword, and keys a silent chip with no name or sense.
The chip lines are keyed by line key, which the test reads from the held draft.
A collocation sentence sharing the meaning sentence's row id keeps its own labels under its own line key.
A sentence inserted before a labelled one leaves that label on its own line key, read from the shown entry.
A failed chip line read reports `Mention.FindFailed` once.
It still keys every sentence of the held draft, each with an empty chip line.
Lines for rows the draft lacks drop, and a held row the engine skips reads an empty line.
A null answer reports the find failure and keys every sentence empty.
With no draft held, the read answers no entries and reports nothing.

## `TMentionEntryCreate`

Stores an Entry with one Meaning, and answers the Entry and the Meaning ids.

## `TCorpusMention`

Covers the excerpt's word click and the shared Mention reads, on a real workspace.
The shared reads prepare a held transcript through `TTranscriptMention.TMentionPrepare`.
A failed chip line or Meaning read reports `Mention.FindFailed` once and answers nothing.
A failed Meaning read hands the envoy the `Notice.Unexpected` ledger notice.
The Meaning read leads with the whole Entry, maps each row, and asks the engine with the unknown key.
The excerpt click stays a corpus gate, since it asks the leave question.
An excerpt click with no chosen Example finds nothing, and a kept leave asks once and finds nothing.
A failed word find reports `Mention.FindFailed` and offers nothing.
One found Entry opens at once and leaves an empty menu, and several stay offered under the found word.

## `TTranscriptMention`

Covers `CTranscript`'s Mention gates and reads, reached through `CCorpusTranscript`, over a held Example.
A linked Mention offers its Entry's Meanings, and the sense gate narrows it to one.
A silent Mention may be unlinked but offers no sense.
A Mention drops by the selection over it and by its chip's id.
The chip line names a linked headword and keys a silent chip.
A corpus holding no Example answers no chips and no Meanings.
A selected word is searched in the Example's language, and the pick links it.
A link over an Entry keeps the selection's span and carries no sense.
`TMentionPrepare` sets the edit delay to zero, stores an Example, opens it and turns the scribe on.
`TCorpusMention` calls it for its shared Mention reads.

## Stamp

Commit: `44d0e95bcfcc`
- `TAtelierMention.cs` `3af14e590ada7db8`
- `TCardMention.cs` `36fd51f386eb3377`
- `TCorpusMention.cs` `8de0ba8b6aaf7922`
- `TTranscriptMention.cs` `2fef1d74f0e5c94c`
