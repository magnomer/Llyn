# QRoster.cs
Hash: `32b76d65c7197441`

## `internal sealed class QRoster`

The item fills of the shared read-only entry view's lists.
The [QLectern](../QLectern.comment.md) constructor builds it, and it paints each row from a ready item.
The card fills live in [PLeaf](../Template/PLeaf.comment.md), which paints the ready cards the lectern hands the lists.

## `internal QRoster(FrameworkElement surface, QCompass compass)`

Pulls the speech, card, incoming and contents lists from the page by contract ID and attaches their item fills.
It keeps the compass, since a contents row's click goes to it.

## `private static void QRosterSpeechRefine(FrameworkElement container, object item, string? _)`

Writes one part of speech into its chip.

## `private static void QRosterUsageRefine(FrameworkElement container, object item, string? _)`

Fills one incoming row: its icon, headword, epithet, title, owner, flag and language.
The epithet is read through `QLookEpithetRead`, which keeps the en space before it.

## `private void QRosterCompassRefine(FrameworkElement container, object item, string? _)`

Fills one contents row with its indent, number and name.
The current row carries the `Chosen` cue, which `QLookDisplay` colours in the accent.
The row's click is subscribed once to `QRosterCompassObserve`.

## `private void QRosterCompassObserve(object sender, RoutedEventArgs e)`

Hands a contents row's click to the compass, which scrolls to the section the row names.
