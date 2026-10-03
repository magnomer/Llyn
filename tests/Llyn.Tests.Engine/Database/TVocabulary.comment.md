# TVocabulary.cs
Hash: `ec13137bc408eab1`

## `public sealed class TVocabulary`

Covers the integer identity of the vocabulary rows and the links that point at them.

A pack seeded twice must hand back the same row ids.
Otherwise every link would go stale on the second launch.
A renamed pack row keeps its id, so an entry that linked it shows the new name without being touched.
A user-added preset takes a negative code, so no pack can ever collide with it.
An entry whose part of speech no preset names stores the text and no link.
A draft that already links a value stores that link, and its display name is not consulted.
An inflection feature row hangs off the inflection's own id and links a `morphology_value` row.
A link to a value row that does not exist is refused by the clerk's link check.
The write then leaves nothing behind.
