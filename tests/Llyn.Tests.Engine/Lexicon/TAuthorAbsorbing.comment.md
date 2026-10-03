# TAuthorAbsorbing.cs
Hash: `2d18dd4c46364a19`

## `public sealed class TAuthorAbsorbing`

Covers folding one Author into another.
It covers the credits of the dropped Author moving to the kept one, at the place the dropped one stood.
It covers a Source crediting both, which keeps one credit at the earlier place and loses the other.
It covers the refusals of an Author into itself and of an id naming no Author.

## `private static LReference TAuthorWorkCreate(LEngine engine, string title)`

Makes one titled Source through the engine, with every other field unspecified.
