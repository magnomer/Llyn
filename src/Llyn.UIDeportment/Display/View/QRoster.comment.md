# QRoster.cs
Hash: `13a81324c9024430`

## `internal sealed class QRoster`

The item fills of the shared read-only entry view's lists.
[QDisplay](QDisplay.comment.md) hands it the lists, and it paints each row from a ready item.
The card fills live in [PLeaf](../Template/PLeaf.comment.md), which paints the ready cards the lectern hands the lists.

## `internal void QRosterIntroduce(QLectern lectern, ItemsControl speech, ItemsControl meaning, ItemsControl collocation, ItemsControl incoming, ItemsControl compass)`

Attaches the item fills of the speech, card, incoming and contents lists.
It keeps the lectern, since a contents row's click goes to the lectern's compass.

## `private static void QRosterSpeechRefine(FrameworkElement container, object item, string? _)`

Writes one part of speech into its chip.

## `private static void QRosterUsageRefine(FrameworkElement container, object item, string? _)`

Fills one incoming row: its icon, headword, epithet, title, owner, flag and language.
The epithet keeps the en space its string format put before it.

## `private void QRosterCompassRefine(FrameworkElement container, object item, string? _)`

Fills one contents row with its indent, number and name.
The current row carries the `Chosen` cue, which `QLookDisplay` colours in the accent.
The row's click is subscribed once to `QRosterCompassObserve`.

## `private void QRosterCompassObserve(object sender, RoutedEventArgs e)`

Hands a contents row's click to the lectern's compass, which scrolls to the section the row names.
