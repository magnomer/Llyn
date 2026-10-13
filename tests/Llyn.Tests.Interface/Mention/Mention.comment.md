# Mention

This folder holds the relays for mention reads.
It also holds the relays for the text spans mentions cover.

## `TInterfaceMention`

The relays that run the mention-related reads over failing or scripted ports.
They build sentence gates whose chip line read fails, or answers any lines a fact hands, and one chip.
They run the shared sense menu read over a scripted draft port.
They run the corpus, favorites, quotation and occurrence rows reads over failing ports.
They run the chip read over a failing draft port.
They run the display word find over a failing or recording mention port.
They map a click result and add a Mention to an Example, so a test builds no engine record itself.
They run the unit read of a piece and the shared open of a find answer.
Each relay is transparent.

## `TSentenceLineCreate`

Builds sentence gates whose chip line read answers the given lines as they stand, over a real desk.
A fact hands rows the draft lacks, rows it misses or null, so the read meets the engine's worst answer.
Its repaint memory is fresh, and a failed read reaches the envoy through a real settings outlet.
`TSentenceFailCreate` instead makes the chip line read throw.

## `TMentionLabelCreate`

Builds one chip of a word with no sense, so a fact builds no engine record itself.
It is linked whenever the entry is not zero.

## `TAnthologyMentionFind`

Finds the word clicked at unit 3 of `a cat sat`, with the chosen Example selected on a real vista.
The fake word find answers the word at offset 2 with the given Entries, and throws when they are null.
When the find answers one Entry, the atelier's navigation opens it.

## `TMentionResultOpen`

Runs the shared open of a find answer over the atelier's mention area.
The whole shown text places the offer.

## `TMentionUnitRead`

Runs the atelier's unit read of one piece, which only the result open calls in the product.

## `TDisplayOffsetRead`

Clicks a word through the display find gate and answers the offset the engine find was handed.
The fake word find records that offset and then throws, so nothing opens.
Null means the gate never reached the engine.

## `TDisplayCreate`

Builds a display over the scripted mentions, a media stub and the engine's other facades, for both display finds.
The display is attached to the atelier's navigation and mention area, as the composition does.
It holds a fresh repaint memory, since the lookup is a user act that never goes through it.
It shows the stored entry, since a display showing nothing answers before it asks the engine.

## `TMentionSenseRead`

Runs the shared sense menu read on a desk with a live draft.
The draft port's sense read answers through the given function.
A test hands rows to check the map and the key, or a throw to check the failure policy.

## `TInterfaceMentionSpan`

The relays for mentions and the text spans they cover.
They build mentions and mention drafts, and resolve, divide and read the spans a test checks.
The span finds on an etymology or example draft are relayed here too.
The example read off a whole draft is relayed here too.
Each relay is transparent.

## Stamp

Commit: `c2f335c331fe`
- `TInterfaceMention.cs` `7a9bb306c8c6edcc`
- `TInterfaceMentionSpan.cs` `3e7ce9b2f62e1583`
