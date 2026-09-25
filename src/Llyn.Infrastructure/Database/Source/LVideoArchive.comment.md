# LVideoArchive.cs

## `public sealed class LVideoArchive`

Stores the independent Video and the references that reach it from a Meaning, a Collocation, or a Situation.
It mirrors `LImageArchive` row for row, because a Video is kept exactly as an Image is.
It keeps one thing an Image has no use for, the span of the film worth watching.
The span is held as written and read as written.
A Video is owned by nothing, so a reference is a link and never containment.
The order a Video appears in lives on each reference, and every write renumbers that referrer's set.
