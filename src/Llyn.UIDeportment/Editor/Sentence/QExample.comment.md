# QExample.cs
Hash: `5f90d6603b8d3066`

## `internal sealed class QExample`

Fills the sentence row's template and shows its controls on hover.
It hooks the row's parts to the observers of [QSentence](QSentence.comment.md), [QGloss](../Gloss/QGloss.comment.md) and [QCitation](QCitation.comment.md).
It calls no Conduct gate, since every gate stays with the drivers whose handlers it hooks.

## `internal QExample(QSentence sentence, QGloss gloss, QCitation citation)`

Holds the three drivers whose handlers a row's wiring takes.
`QEditorCard` builds it from the drivers it already holds.

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

## `private static void QExampleRowRefine(FrameworkElement container, PSentence row)`

Writes every part of a sentence row from the row, where bindings and data triggers stood.
The frame switch, its sign, the frame and its gap follow whether the frame is open or written.
Each frame field takes the column twice its slot in the frame's order.
The gap between the two fields stands in column 1, so the fields take columns 0 and 2.
The sentence and citation texts go through the same lookups the converters made.
The three icons are set here, since an icon is drawn by code.

## `private static void QExampleChoiceRefine(FrameworkElement container, string name, CStateWording value, ObservableCollection<string> catalog)`

Fills one frame field.
It sets the dropdown's offers, text and hint, the ghost copy and the hint shown when empty.

## `private static void QExampleLayoutRefine(FrameworkElement container, TextBox text)`

Binds the row's layout once, where element bindings stood in the markup.
The sentence column and the citation drop to the frame's baseline through the font converter.
The citation takes the sentence's font, and each dropdown the size of its field.
A row already bound is left alone, so a refill binds nothing twice.
