# LSentence.cs

## `public sealed record LSentence(`

One Example as a single Meaning or Collocation holds it.
The Example itself is shared and owned by nothing, so nothing about one owner may sit on it.
A Sentence is where those owner-only facts live.
Two owners citing the same Example each keep their own Sentence over it.
Editing one never reaches the other.

A Sentence carries the frame the owner reads the Example under.
`LSentenceParticle` is the grammatical marker the frame uses, and `LSentenceDependence` the role it fills.
Neither is ever shipped with a value.
Nothing in the program or a language pack names a marker or a role.
A language pack states only which of the two is written first.
So English writes the marker before the role, and a language with postpositions writes the role first.

A Sentence may state a frame and no Example at all.
The frame belongs to the owner, not to the Example.
A marker written before any sentence is not the Example's to lose.

The owner is a Meaning or a Collocation, and the two hold an Example on identical terms.
The row itself does not say which kind it is, because the store it was read from already does.

**Parameters**

- `LSentenceId` — Opaque, program-generated stable id of this owner's hold on the Example.
- `LSentenceExample` — The Example the row shows, and `null` when the row states a frame and no sentence.
- `LSentenceParticle` — The frame's grammatical marker, and what is known about it.
- `LSentenceDependence` — The role the frame fills, and what is known about it.
