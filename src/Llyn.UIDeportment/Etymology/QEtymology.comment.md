# QEtymology.cs
Hash: `13e023d1cd7d49bc`

## `public sealed class QEtymology : Decorator`

The etymology field as one control, so the reading view and the editor draw the same thing.
It holds the source chips and the narrative together, since an entry declares its origin in one of the two.
Editable, it adds an entry the user types a source into and writes the narrative in a box.
Read, it prints the narrative with its spans underlined and the chips beside it.
The host decides whether the whole field shows, since the editor's tab and the reading view differ.
Every visibility verdict comes ready from the driver that owns the instance, so the control only draws.

## `public static readonly DependencyProperty QEtymologyEditableProperty`

Which side of the shell draws it, set by the editor that owns the editable instance.

## `public static readonly DependencyProperty QEtymologyTextProperty`

The narrative as the draft holds it.

## `public static readonly DependencyProperty QEtymologyNarratedProperty`

The ready verdict for the narrative's read face, false unless the reading view hands it.

## `public static readonly DependencyProperty QEtymologyCardProperty`

Whether the frame is the editor card with a head strip, set by the editor when it attaches.

## `public QEtymology()`

Builds the chips row, both faces of the narrative and the span chips once.
It also builds the frame and the head strip, and takes their looks from the theme keys.
The templates come from the theme, so the two sides cannot drift apart.

## `public bool QEtymologyCard`

Whether the head strip shows and the frame takes the card look.

## `public bool QEtymologyEditable`

Whether the field takes input.

## `public string QEtymologyText`

The narrative shown or edited.

## `public bool QEtymologyNarrated`

Whether the narrative's read face stands, as the owning driver decided.

## `internal TextBox QEtymologyBox`

The narrative box, which the editor reads a selection from.

## `internal PMentionLine QEtymologyLine`

The chip line under the narrative box, which the editor paints the etymology's ready chips on.

## `internal void QEtymologySourceShow(IReadOnlyList<PEtymon> etymons, bool linked)`

Draws the source link items the driver built from the links the engine already resolved.
`linked` is the driver's ready verdict on whether the row of links shows.
The field holds no Conduct record, so the display driver and the editor each hand it items.
The typing entry always closes the row, collapsed on the read side, so the row has one shape.

## `private void QEtymologyItemApply(FrameworkElement container, object item, string? _)`

Fills one item of `Theme.Etymology.Item`.
The item is the chip for a source word, or the entry for the caret.
The chip takes the flag, the headword, its language, and the entry and removal commands.
The removal button shows only while the field is editable.
The entry takes the typed text both ways and the addition command on Return.
It watches the item's text and shown properties, since an etymon raises no property change.

## `private void QEtymologyItemRefine(object? sender, EventArgs e)`

Fills the field's items again when an etymon's text or shown state changes.

## `private static void QEtymologyBoxRefine(object sender, TextChangedEventArgs e)`

Writes the typed text back to the caret etymon.

## `protected override AutomationPeer OnCreateAutomationPeer()`

A surface peer, since the control is a box and not a button.

## Inline notes

### `private static void QEtymologyStateRefine(DependencyObject sender, DependencyPropertyChangedEventArgs e)`

Every property redraws the whole field, which is small enough to build again.

### `private void QEtymologyStateApply()`

Paints which face of the narrative stands, from the mode and the verdict the owning driver set.
It also picks the frame look and shows the head strip from the card flag.
The prose takes the narrative as one plain piece through `QMentionPieceCreate`, since the mention text takes only pieces.
