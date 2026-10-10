# PLeaf.cs
Hash: `6369d68a22e3d506`

## `internal static class PLeaf`

Reading-card fills paint ready Deportment items without resolving lexical state.
Named parts keep template presentation separate from Conduct gates.

## `internal static void PLeafCardRefine(FrameworkElement container, object item, string? _)`

Only ready reading-card items receive presentation.
An untitled open card uses its kind caption, while an untitled folded card uses its peek.
Expression and meaning visibility follow their ready verdicts.
`QCardFold` gives writing and reading cards one fold shape.
The body receives the same ready card as its rows.

## `private static void PLeafBodyRefine(FrameworkElement body, QLeafItem card)`

Empty chip and example sections collapse rather than leaving blank rows.
Translations gain the chip margin when no situation row precedes them.
Media rows reuse their shared fills without adding separate visibility rules here.

## `private static void PLeafListRefine<PLeafRow>(FrameworkElement body, string name, IReadOnlyList<PLeafRow> rows, Action<FrameworkElement, object, string?> fill)`

Chip collections share one empty-row visibility policy and the supplied item filler.
Missing named lists are left untouched.

## `private static void PLeafSentenceRefine(FrameworkElement container, object item, string? _)`

Ready sentence identity and mention pieces preserve the click gate's context.
A margin binding follows both frame and sentence fonts, keeping their baseline relationship live.
The byline follows the sentence's margin and typography.
Ready Gloss rows reuse the editor's Gloss filler.

## `private static void PLeafSituationRefine(FrameworkElement container, object item, string? _)`

Situation text arrives ready, so the fill needs no lexical lookup.

## `private static void PLeafRegisterRefine(FrameworkElement container, object item, string? _)`

Register text follows the same ready-chip boundary.

## `private static void PLeafTagRefine(FrameworkElement container, object item, string? _)`

Tag text follows the same ready-chip boundary.

## `private static void PLeafLinkRefine(FrameworkElement container, object item, string? _)`

Translation flag, headword and language arrive ready together, avoiding a target lookup during rendering.
