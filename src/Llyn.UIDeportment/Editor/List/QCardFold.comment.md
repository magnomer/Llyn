# QCardFold.cs
Hash: `e15e8aeeda7c7de4`

## `internal static class QCardFold`

Writing and reading cards share one fold painter, preventing their hinge and body shapes from diverging.
The caller supplies both verdicts, so the painter needs no card row or persistence dependency.

## `internal static void QCardFoldRefine(FrameworkElement container, bool folded, bool stored)`

Folded state checks the hinge, collapses the body and selects the folded header resources.
Stored state independently controls hinge visibility.
Unfolding clears local header overrides, allowing the style to supply its normal shape.
Missing named parts do not prevent other parts from being painted.
