# TBylineStep.cs

## `public sealed class TBylineStep`

Covers the byline the source editor opens under a typed credit.
Typing in a field without the keyboard opens nothing, and typing with it offers the Authors not yet credited.
A blank word closes the byline again.
Down and Up move the lit row and wrap at either end.
Escape closes the byline and drops the lit row.
Enter on a lit row credits its Author through the engine.
Picking the Author the field already credits only reverts the typed text, and a click beside a row only closes.
