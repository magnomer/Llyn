# TBerthSeat.cs
Hash: `75eb74163de8d120`

## `public sealed class TBerthSeat`

Covers how a chip field's panel seats the typing entry among the chips.
The panel lives on its own STA thread, since a WPF control demands one.

## `public void BerthAnchor_SeatsTheEntryBeforeItsChipWithoutTouchingTheChips(string? anchor, int seat)`

The entry stands right before the chip the caret is anchored to.
No anchor, or an anchor no chip carries, seats the entry at the end.
The chips keep their order around the entry.

## `private static QBerth? TBerthFind(DependencyObject root)`

The panel the list built from its items panel template, found down the visual tree.
