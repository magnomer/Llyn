# LSentence.cs

## `public sealed record LSentence(`

One Example as a single Meaning holds it.
The Example itself is shared and owned by nothing, so nothing about one Meaning may sit on it.
A Sentence is where those Meaning-only facts live.
Two Meanings citing the same Example each keep their own Sentence over it.
Editing one never reaches the other.

A Sentence carries the frame the Meaning reads the Example under.
`LSentenceParticle` is the grammatical marker the frame uses, and `LSentenceDependence` the role it fills.
Neither is ever shipped with a value: nothing in the program or a language pack names a marker or a role.
A language pack states only which of the two is written first.
So English writes the marker before the role, and a language with postpositions writes the role first.

`LSentenceRevision` is a second Example standing for the rewritten sentence.
It is a full Example row, not text, so it can be cited and translated like any other.
Nothing is revised when it is absent.
A revision belongs to this Meaning alone, so revising here never rewrites what another Meaning shows.

**Parameters**

- `LSentenceId` — Opaque, program-generated stable id of this Meaning's hold on the Example.
- `LSentenceMeaningId` — The Meaning holding the Example.
- `LSentencePosition` — Where the row sits among the Meaning's Sentences, counted from zero.
- `LSentenceExample` — The Example the row shows.
- `LSentenceRevision` — The rewritten Example, and `null` when the sentence stands unrevised.
- `LSentenceParticle` — The frame's grammatical marker, and what is known about it.
- `LSentenceDependence` — The role the frame fills, and what is known about it.
