# LCardKind.cs
Hash: `a3b69b9ff90b4039`

## `public enum LCardKind`

Which of the two card lists a request means.
A Meaning card and a Collocation card share one shape.
So a request that adds one has to say which list.
An existing card is named by id instead, because the engine knows which list holds it.

## `LCardKindMeaning,`

A card in the meanings list, which may nest cards under it.

## `LCardKindCollocation,`

A card in the collocations list, which holds no cards inside it.
