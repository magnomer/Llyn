# PLeaf.cs
Hash: `7777b51a5a8b0889`

## `internal static class PLeaf`

Reading-card fills paint ready Deportment items without resolving lexical state.
Named parts keep template presentation separate from Conduct gates.

## `internal static void PLeafCardRefine(FrameworkElement container, object item, string? _)`

Only ready reading-card items receive presentation.
An untitled open card uses its kind caption, while an untitled folded card uses its peek.
Meaning visibility follows its ready verdict.
`QCardFold` gives writing and reading cards one fold shape.
The body receives the same ready card through `PLeafBodyRefine`.
`PCardHeading` shows Conduct's ready caption and collapses with the homed list when nothing homes on the card.
The card fills `PCardHome` with the collocations homed on it, each drawn by `PLeafRowRefine`.
The `PCardSee` list shows the card's "see" lines, each drawn by `PLeafSeeRefine`, and collapses when there are none.

## `internal static void PLeafRowRefine(FrameworkElement container, object item, string? _)`

Fills one folding collocation row, under its home meaning or in the Other list.
The row takes `QCardFold`'s fold shape too, so its hinge, stored fold and body follow the cards' rule.
The hinge stays the lectern's hinge gate, since the row's hinge carries the item like a card's.
A closed row cuts its meaning at the line end and shows only Conduct's filled markers: flags, examples, "also".
An open row wraps its meaning and hides the markers, since its body then shows the fields they summarise.
The body is the reading card's own body fill, so an open row draws every field like a card.

## `private static void PLeafBodyRefine(FrameworkElement container, QLeafItem card)`

Finds the template's `PCardContents` and hands it the ready card, so a card and a row share one body fill.
A template without that part is left untouched.
Empty chip and example sections collapse rather than leaving blank rows.
Translations gain the chip margin when no situation row precedes them.
Media rows reuse their shared fills without adding separate visibility rules here.

## `private static void PLeafListRefine<PLeafRow>(FrameworkElement body, string name, IReadOnlyList<PLeafRow> rows, Action<FrameworkElement, object, string?> fill)`

The chip lists, homed collocation rows and "see" lines share one empty-row visibility policy and the supplied filler.
Missing named lists are left untouched.

## `private static void PLeafSeeRefine(FrameworkElement container, object item, string? _)`

Paints one "see" line's ready text into its `PCardSeeLabel`.
The line is a button whose item the lectern's see gate reads, so the fill adds no click.

## `private static void PLeafSentenceRefine(FrameworkElement container, object item, string? _)`

Ready sentence identity and mention pieces preserve the click gate's context.
A margin binding follows both frame and sentence fonts, keeping their baseline relationship live.
The byline follows the sentence's margin and typography.
Ready Gloss rows reuse the editor's Gloss filler.

## `private static void PLeafFlagRefine(FrameworkElement container, object item, string? _)`

Paints one ready ensign into a closed row's `PCardEnsign` image.

## `private static void PLeafSituationRefine(FrameworkElement container, object item, string? _)`

Situation text arrives ready, so the fill needs no lexical lookup.

## `private static void PLeafRegisterRefine(FrameworkElement container, object item, string? _)`

Register text follows the same ready-chip boundary.

## `private static void PLeafTagRefine(FrameworkElement container, object item, string? _)`

Tag text follows the same ready-chip boundary.

## `private static void PLeafLinkRefine(FrameworkElement container, object item, string? _)`

Translation flag, headword and language arrive ready together, avoiding a target lookup during rendering.
