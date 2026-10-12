# QRoster.cs
Hash: `b2df99db6021bbc1`

## `internal sealed class QRoster`

The item fills of the shared read-only entry view's lists.
The [QLectern](../QLectern.comment.md) constructor builds it, and it paints each row from a ready item.
The card and row fills live in [PLeaf](../Template/PLeaf.comment.md), which paints the ready items the lectern hands the lists.
The meaning list takes the card fill and the Other collocations list the row fill.

## `internal QRoster(FrameworkElement surface, QCompass compass)`

Pulls the speech, meaning, collocation, incoming and contents lists from the page by contract ID and attaches their item fills.
It keeps the compass, since a contents row's click goes to it.

## `private static void QRosterSpeechRefine(FrameworkElement container, object item, string? _)`

Writes one part of speech into its chip.

## `private static void QRosterUsageRefine(FrameworkElement container, object item, string? _)`

Fills one incoming row: its icon, headword, epithet, title, owner, flag and language.
The epithet is read through `QLookEpithetRead`, which keeps the en space before it.

## `private void QRosterCompassRefine(FrameworkElement container, object item, string? _)`

Fills one contents row with its indent, number and name.
A muted row's name takes `Theme.Muted`.
Any other row clears it, so the name follows the row's own ink.
The current row carries the `Chosen` cue, which `QLookDisplay` colours in the accent.
The row's click is subscribed once to `QRosterCompassObserve`.

## `private void QRosterCompassObserve(object sender, RoutedEventArgs e)`

Hands a contents row's click to the compass, which scrolls to the section or card the row names.
