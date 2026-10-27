# Lookup

This folder holds the tests of pronunciation lookup.
They cover the readings fan-out, respelling with its receiver wrapper and the trove of candidates.

## `TLookup`

Covers the lookup fanning one source's readings out into candidates.
Every reading of a source becomes one candidate sharing the source's order and keeping its variety.
An untagged reading in a language with varieties becomes one candidate per declared variety, in pack order.
Without varieties the untagged reading stays untagged, so a pack like Spanish is unchanged.
A variety the answer already tags is not fanned out again.
An empty answer stays one untagged null candidate even when the pack declares varieties.
That candidate is reached and handed to the receiver, so the shell can say why.
A dotted source form reaches the candidate normalized, with no switch involved.
A literal lookup keeps the spaces and skips the cleanup, trimming the ends alone, because a transcription is not IPA.
A pack cleanup group applies to every candidate the same way, and a scoped one only to its fanned-out variety.
The returned set saved into a trove holds cleaned but un-respelled text.
Replaying that set through the respelling wrapper recasts what the receiver sees while the trove is unchanged.
The receiver hears the finish exactly once, and never when the lookup was cancelled.

## `TReading`

Covers the static fan-out a source's readings pass through before they become candidates or recordings.
A tag the pack does not declare passes through first, then the untagged reading fans out per declared variety.
Without varieties the readings come back as they were.
A variety the answer already tags is not fanned out again.

## `TReceiverRespelling`

Covers the wrapper that recasts candidates on their way to a receiver.
A candidate with text reaches the inner receiver with its respelling filled and its phonetic unchanged.
A candidate without text is forwarded as the same object.
A group scoped to one variety leaves a candidate of another variety to the unscoped groups alone.
The source start and the finish each reach the inner receiver exactly once.

## `TRespelling`

Covers the built-in reading cleanup and the pack-declared respelling groups.
A reflex guise shows the respelling only while respelled and filled, and the phonetic otherwise.
The cleanup drops syllable dots and every whitespace while stress marks and tie bars survive.
Rules run in written order, so a later rule sees the earlier rule's output.
A scoped group skips an untagged reading and one of another variety, and an unscoped group takes both.
Precomposed and decomposed input reach the same output through NFC.
Scanning a pack chains its groups, and an empty pack returns the input.
The shipped English pack is idempotent over the listed forms, run per variety.
Both the British and the American group recast the glide and open-mid rows, and an untagged reading is left alone.
The shipped Mandarin pack drops tie bars and plains the retroflex stop, the fricatives and the vowels.
It is idempotent over its own output.

## `TTrove`

Covers the per-session trove holding the candidates a lookup found.
Several candidates sharing one order survive a save and a read unchanged.
A hold is read back only for the word it was saved under.
A find with no phonetic at all is not held.
A recording hold keeps every variety a source gave.
A recording find with no address at all is not held.
A transcription hold is kept once per scheme, so two schemes of one word read back separately.
Clearing a session drops every scheme it held and leaves another session's holds standing.

## Stamp

Commit: `9377e657b395`
- `TLookup.cs` `b42f986cb9780f9b`
- `TReading.cs` `90dbf2aa8f082f59`
- `TReceiverRespelling.cs` `bfa1fef6048ff512`
- `TRespelling.cs` `3912212efffbceb1`
- `TTrove.cs` `3e97d43994b785d0`
