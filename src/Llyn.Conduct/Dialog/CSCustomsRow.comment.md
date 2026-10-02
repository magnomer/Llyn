# CSCustomsRow.cs
Hash: `423a98142d4c7c7a`

## `public sealed record CSCustomsRow(`

One row of the customs gate as a driver shows it, read in one call.

**Parameters**

- `CSCustomsRowMode`: how the entry enters.
- `CSCustomsRowTarget`: the stored entry the row is joined to, or zero before one is chosen.
- `CSCustomsRowTargeted`: whether the mode wants a target, true for Merge and Replace.
- `CSCustomsRowLoss`: the wording key of what a Replace would drop, or null when nothing is lost.
- `CSCustomsRowMeaning`: the meaning cards a Replace would drop, zero when nothing is lost.
- `CSCustomsRowCollocation`: the collocation cards a Replace would drop, zero when nothing is lost.
