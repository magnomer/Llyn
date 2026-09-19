# PDiweiItem.cs

## `internal sealed class PDiweiItem`

One section of a category page as the template binds it, copied from the engine's section.
Its label, lines, tally lines and switch state are all decided below, so nothing is computed here.

## `internal static IReadOnlyList<PDiweiItem> PDiweiItemBuild(IReadOnlyList<LDiweiSection> sections)`

A plain copy loop over the composed sections.
