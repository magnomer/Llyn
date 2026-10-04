# QExample.cs
Hash: `a9d2d0bb0ba25e35`

## `internal sealed class QExample`

Fills the sentence row's template and shows its controls on hover.
It hooks the row's parts to the observers of [QSentence](QSentence.comment.md), [QGloss](../Gloss/QGloss.comment.md) and [QCitation](QCitation.comment.md).
It names no Conduct type, since every gate stays with the drivers whose handlers it hooks.

## `internal QExample(QSentence sentence, QGloss gloss, QCitation citation)`

Holds the three drivers whose handlers a row's wiring takes.
`QSentenceIntroduce` builds it once those drivers are introduced.

## `internal void QExampleRefine(FrameworkElement container, object item)`

Paints a row's template from its sentence, then shows or hides the row controls of the list that holds it.
The card's row fill calls it before `QExampleIntroduce` on every fill and refill.

## `internal void QExampleIntroduce(FrameworkElement container, object item)`

Wires a row's template.
That covers the command bindings, the buttons, the citation field, the chip lines and the Gloss lines.
It hooks each typed field to its own observer, the two choices through their inner box's text change.
Each event is hooked once, so a refill leaves one handler.

## `internal static void QExampleRevealIntroduce(ItemsControl list)`

Hooks a list's hover and keyboard focus.
Its row controls then show while the pointer or focus is over the list.
The card then calls `QExampleRevealRefine` once for the list's first look.

## `internal static void QExampleRevealRefine(ItemsControl list)`

Shows or hides each row's controls.
It dims an uncited row's citation field while the list is neither hovered nor focused.
Whether a row is cited is the ready verdict `PSentenceCited`, and the driver adds only its own hover state.
The two event overloads only hand their sender to it.

## `private void QExampleOpeningRefine(object sender, RoutedEventArgs e)`

Opens or closes the row's frame.
