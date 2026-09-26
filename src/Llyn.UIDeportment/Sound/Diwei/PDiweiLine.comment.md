# PDiweiLine.cs

## `internal sealed class PDiweiLine`

One row of a section as the template binds it, copied from the engine's line.

## `internal static void PDiweiLineApply(FrameworkElement container, object item, string? _)`

Fills a placement line: reading, label, the rounded mark and the character chips.
The chips take their text and command parameter from their own item through `PLook` rows.

## `internal static IReadOnlyList<PDiweiLine> PDiweiLineBuild(IReadOnlyList<LDiweiLine> lines)`

A plain copy loop over the sorted lines of one section.
