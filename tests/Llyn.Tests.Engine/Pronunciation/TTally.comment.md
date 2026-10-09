# TTally.cs
Hash: `a2a1ba18f1db07b4`

## `public sealed class TTally`

Covers the tally the engine builds for a Diwei page from anchored reflex rows and their anatomy.
A reflex row nobody anchored counts nothing, the page lists no placement, and its entry count is nought.
A row anchored to one of two placements of its character counts on that placement's page alone.
An initial's page counts onsets per division, one count per distinct character, both sets carried.
A character with two Mandarin readings counts once under each onset they take, and each mark names its characters.
A mark names its characters in code point order, not in the order they were placed.
Languages follow the Classical Chinese pack's declared order and marks sort by count.
Kinds of one language follow the declared kind order, whatever order their rows were stored in.
A language spelled with other letter case than the declared one ranks as undeclared, as in the reflex list.
So the tally and the entry's reflex rows order one pack alike.
A character nobody anchored is not listed, and a division of such characters is not listed either.
A rime's page counts vowel and coda joined, and Japanese Go-on and Kan-on stand on separate lines.
A rime page sections its tallies by the articulatory place of the initial, so each section counts its own characters.
