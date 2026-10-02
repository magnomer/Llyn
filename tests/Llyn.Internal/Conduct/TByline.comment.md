# TByline.cs
Hash: `e23f8d40ad1631b7`

## `public sealed class TByline`

Covers the byline the source editor opens under a typed credit.
Typing in a field without the keyboard opens nothing, and typing with it offers the Authors not yet credited.
Each offered name comes split around the word, case-blind, keeping the name's own case.
The word loses its outer spaces, and a blank word closes the byline again.
At most eight Authors are offered.
Down and Up move the lit row and wrap at either end.
Escape closes the byline and drops the lit row.
Enter on a lit row credits its Author through the engine.
Enter with no lit row credits the typed name, and closes the byline.
Escape over an offered byline only closes it, and a second Escape reverts the field.
Picking the Author the field already credits only reverts the typed text, and a press beside a row only closes.

## `private static LAuthor TBylineOpen(LEngine engine, CImprint imprint)`

Stores a Source crediting one Author, and opens the editor over it.
