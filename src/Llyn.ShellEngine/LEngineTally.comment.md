# LEngineTally.cs

## `public sealed partial class LEngine`

The tally side of the engine: the reflex parts counted per division for one Diwei page.
It joins the fanqie rows of the category to the entries written with each character and their reflex rows.
The counting itself is [LTallyLine](../Llyn.Core/Pronunciation/LTallyLine.comment.md)'s, so a view only prints.

## `public IReadOnlyList<LTally> LEngineTallyRead(LDiwei diwei)`

One tally per division the category's fanqie rows name, in the order the rows first name them.
Each division lists its distinct characters, and a character placed twice in one division counts once.
The languages rank in the order the pack's reflex fetch rules name them, each once.
An initial category tallies onsets, a rime category vowel and coda, from every entry of the category's language.
A language with kinds of reading, as Japanese, gets one line per kind.
A character without an entry, or with an entry without reflex rows, adds no line.

## `private void LEngineTallyLoad(`

Reads the reflex rows of every entry of `language` whose headword is the character into `readings`, once per character.
One character sits in several divisions and is read once for the page.
