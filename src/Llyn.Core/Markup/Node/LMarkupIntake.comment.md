# LMarkupIntake.cs

## `public sealed record LMarkupIntake(`

The reader's choice for one parsed entry: how it enters the workspace.
The import window builds one per entry and the engine imports under them.

**Parameters**

- `LMarkupIntakeIndex` — The position of the entry among the parsed entries.
- `LMarkupIntakeMode` — Whether the entry is created, merged into a target, or replaces one.
- `LMarkupIntakeTarget` — The id of the stored entry a merge or replacement lands on, zero for a new entry.
