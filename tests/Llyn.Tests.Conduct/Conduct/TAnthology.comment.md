# TAnthology.cs
Hash: `c754dfc872dc49ea`

## `public sealed class TAnthology`

Covers the corpus panel's example list end to end on a real workspace.
Before a vista arrives the list reads nothing, and an unknown text shows the localized unknown word.
A failing find shows `Example.LoadFailed` and answers nothing.
A tally the engine refuses shows the same failure once and answers an empty tally.
The ordering menu offers text, language, Source and usage.
A typed query narrows the rows, no order keeps the ordering, and a hidden language marks the list filtered.
The held transcript's citation lives in `TAnthologyCitation`, its glosses in `TAnthologyGloss`, and its text and Example map in `TAnthologyExample`.

## `internal static CAnthology TAnthologyPrepare(LEngine engine, CAtelier atelier, out CDesk desk)`

Builds the list over the atelier and a corpus desk, through the relay of its internal factory.
Its seams answer that the tab is in front and that every finish succeeds.
Every `TAnthology` sibling class builds its list through it.

## `private static LExample TAnthologyExampleSave(LEngine engine, LStateValue text, long? source)`

Stores one English Example with the given text and cited Source.
