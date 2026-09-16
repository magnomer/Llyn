# LEngineTally.cs

## `public sealed partial class LEngine`

The tally side of the engine: the reflex parts counted per division for one Diwei page.
It joins the fanqie rows of the category to the reflex rows anchored to each of them.
The counting itself is [LTallyLine](../Llyn.Core/Pronunciation/LTallyLine.comment.md)'s, so a view only prints.

## `public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)`

One tally per division the category's fanqie rows name, in the order the rows first name them.
Each division lists its distinct characters, and a character placed twice in one division counts once.
The languages rank in the order the pack's reflex fetch rules name them, each once.
An initial category tallies onsets, a rime category vowel and coda, from the anchored reflex rows alone.
A reflex row anchored to a placement in one division counts in that division and no other.
A language with kinds of reading, as Japanese, gets one line per kind.
A placement no reflex row is anchored to is not read at all, so its division is not listed either.

## `private static void LEngineTallyLoad(`

Adds the reflex rows anchored to one fanqie row to the character's list of the row's division.
A reflex anchored to two placements of the same character in one division is added once.
