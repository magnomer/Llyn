# LForm.cs

## `public sealed record LForm(`

A written form of an entry, ordered within it. Identity is `(entry_id, position)`: the form is subordinate to its `LFormEntryId` parent, and reordering changes `LFormPosition` only.

**Parameters**

- `LFormEntryId` — Parent entry id.
- `LFormPosition` — Order within the parent entry.
- `LFormText` — The written form.
- `LFormLocal` — Optional local representation.
- `LFormRole` — Stable role identifier.
