# LEngineTally.cs

## `public sealed partial class LEngine`

The tally side of the engine: the reflex parts counted per section for one Diwei page.
It joins the fanqie rows of the category to the reflex rows anchored to each of them.
The counting itself is [LTallyLine](../Llyn.Core/Pronunciation/LTallyLine.comment.md)'s, so a view only prints.

## `public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)`

One tally per section the category's fanqie rows fall into, in the order the rows first name them.
An initial category sections by division, a rime category by the articulatory place of the row's initial.
Each section lists its distinct characters, and a character placed twice in one section counts once.
The languages rank in the order the pack's reflex fetch rules name them, each once.
An initial category tallies onsets, a rime category vowel and coda, from the anchored reflex rows alone.
A reflex row anchored to a placement in one section counts in that section and no other.
A language with kinds of reading, as Japanese, gets one line per kind.
A placement no reflex row is anchored to is not read at all, so its section is not listed either.

## `private static string LEngineHeadingRead(string kind, LFanqieRow row, LHypothesis? hypothesis)`

The section heading of one row: its division on an initial page, its initial's place name on a rime page.
An initial no place lists, or a language without a hypothesis, heads the blank section.

## `private static void LEngineTallyLoad(`

Adds the reflex rows anchored to one fanqie row to the character's list of the row's section.
A reflex anchored to two placements of the same character in one section is added once.
