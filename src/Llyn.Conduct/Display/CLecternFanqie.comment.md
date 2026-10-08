# CLecternFanqie.cs
Hash: `d5583ba94123fa95`

## `public sealed record CLecternFanqie(IReadOnlyList<CFanqieGroup> CLecternFanqieGroups, bool CLecternFanqiePending, string CLecternFanqieReading, CFont CLecternFanqieFont)`

The rime-book block of the reading view for the shown entry, ready to show.

**Parameters**

- `CLecternFanqieGroups`: the fanqie blocks as the engine divides them.
- `CLecternFanqiePending`: whether the rows are still being fetched.
- `CLecternFanqieReading`: the headword's representative reading, empty when refused.
- `CLecternFanqieFont`: the pack's glyph typography for the shown language.
