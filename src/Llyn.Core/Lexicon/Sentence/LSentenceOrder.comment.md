# LSentenceOrder.cs

## `public sealed record LSentenceOrder(`

The one fact a language pack states about a Sentence's frame.
It says which of the two written fields the form writes first.
It says nothing about what either field may hold.
No marker and no role is ever shipped, by a pack or by the program.

Each field is stated as the place it is written in, counted from zero.
English writes its markers before what they mark, so the marker stands at nothing.
A language whose markers follow the word they attach to writes the role there instead.
A pack that states nothing leaves the marker first.
That is a layout choice and not a claim about the language.

**Parameters**

- `LSentenceOrderParticle` — Where the marker is written, counted from zero.
- `LSentenceOrderDependence` — Where the role is written, counted from zero.

## `public static LSentenceOrder LSentenceOrderDefault { get; }`

The order used when a pack states none, with the marker written first.
