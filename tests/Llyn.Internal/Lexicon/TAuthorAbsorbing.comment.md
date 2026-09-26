# TAuthorAbsorbing.cs

## `public sealed class TAuthorAbsorbing`

Covers folding one Author into another.
It covers the credits of the dropped Author moving to the kept one, at the place the dropped one stood.
It covers a Source crediting both, which keeps the kept credit where it was and loses the duplicate.
It covers the refusals: an Author into itself, and an id naming no Author.

## `private static LReference TAuthorWorkCreate(LEngine engine, string title)`

Makes one titled Source through the engine, with every other field unspecified.
