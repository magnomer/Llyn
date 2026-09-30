# CLeafChip.cs

## `public sealed record CLeafChip(`

One situation, register or tag chip of a reading card, ready to paint and to open.

**Parameters**

- `CLeafChipId`: the record the chip names.
- `CLeafChipWording`: the chip's text, with the unknown mark's key while unknown.
- `CLeafChipSubject`: the kind of record, which picks the tab a click opens.
- `CLeafChipStored`: whether the record is stored, since a record never saved opens nothing.

## `internal static CLeafChip LLeafChipRead(LSituationDraft situation)`

A situation chip, worded from the situation's title.

## `internal static CLeafChip LLeafChipRead(LRegisterDraft register)`

A register chip, worded from the register's name.

## `internal static CLeafChip LLeafChipRead(LTagDraft tag)`

A tag chip, worded from the tag's text, which has no unknown state.
