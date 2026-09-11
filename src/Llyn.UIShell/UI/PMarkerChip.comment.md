# PMarkerChip.cs

## `internal sealed class PMarkerChip`

Presentation item for one part-of-speech chip under the field.
It carries the name shown and the id of the value row the name stands for (`PMarkerChipValue`).
The id is `0` when the chip is text the engine did not declare as a value.
The chip is immutable, because chips are added and removed rather than edited.
