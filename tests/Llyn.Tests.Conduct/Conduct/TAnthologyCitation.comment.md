# TAnthologyCitation.cs
Hash: `9e6422ccd79e62ec`

## `public sealed class TAnthologyCitation`

Covers the citation gates of the held transcript over a corpus desk on a real workspace.
The held transcript carries the ready line of the Source it cites, and none before it cites one.
A typed title cites the Source it resolves to, and a failing resolve shows `Reference.CreateFailed` and cites nothing.
The citation drawer offers a Source split around the trimmed word, with no count while nothing cites it.
It offers nothing for a blank word, with no held transcript, or for the byline of the Source already cited.
Each test but the failing resolve builds its list through `TAnthology.TAnthologyPrepare`.
