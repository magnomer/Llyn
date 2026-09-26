# CSCustomsRow.cs

## `public sealed record CSCustomsRow(`

One row of the customs gate as a driver shows it, read in one call.

**Parameters**

- `CSCustomsRowMode`: how the entry enters.
- `CSCustomsRowTarget`: the stored entry the row is joined to, or zero before one is chosen.
- `CSCustomsRowTargeted`: whether the mode wants a target, true for Merge and Replace.
- `CSCustomsRowLoss`: the stored entry a Replace would overwrite, or zero when nothing is lost.
