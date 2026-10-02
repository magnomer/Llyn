# QDiweiLine.cs
Hash: `7990e2e8bc6f25a4`

## `internal sealed class QDiweiLine`

One row of a section as the template binds it, copied from the Conduct line.

## `internal static void QDiweiLineRefine(FrameworkElement container, object item, string? _)`

Fills a placement line: reading, label, the rounded mark and the character chips.
The chips take their text and command parameter from their own item through `QLook` rows.

## `internal static IReadOnlyList<QDiweiLine> QDiweiLineBuild(IReadOnlyList<CDiweiLine> lines)`

Wraps each line of one section, in the section's order.
