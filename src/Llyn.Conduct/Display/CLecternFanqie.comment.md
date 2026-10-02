# CLecternFanqie.cs
Hash: `2e9b18a944919b86`

## `public sealed record CLecternFanqie(`

The rime-book block of the reading view for the shown entry, ready to show.

**Parameters**

- `CLecternFanqieGroups`: the fanqie blocks as the engine divides them.
- `CLecternFanqiePending`: whether the rows are still being fetched.
- `CLecternFanqieReading`: the headword's representative reading, empty when refused.
- `CLecternFanqieAnchor`: the reflex anchors again, since new fanqie rows can change them.
- `CLecternFanqieFont`: the pack's glyph typography for the shown language.
