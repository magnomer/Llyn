# LTag.cs

## `public sealed record LTag(`

One Tag.
A Tag has no identity beyond the text it reads, because the name *is* the tag.
So two Tags reading alike are the same Tag.
Renaming one produces a different Tag rather than the same Tag under a new name.
A Tag carries no state of its own.
A Tag that was never written is simply absent, and a nameless Tag does not exist.
The order a Tag appears in lives on the card that carries it rather than here.
So the same Tag can sit first under one Meaning and third under a Collocation.

**Parameters**

- `LTagText` — The tag text, which is also its identity.
  Text may hold spaces and ordinary punctuation.
  Only a comma separates one Tag from the next where Tags are written as a line.
