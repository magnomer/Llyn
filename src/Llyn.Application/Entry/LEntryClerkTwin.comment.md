# LEntryClerkTwin.cs
Hash: `190654e34a95c306`

## `public static class LEntryClerkTwin`

The twin naming every catalog of entries or labels runs through.
Two rows sharing a name are numbered so a list never shows the same words twice unmarked.
Nothing here reads a store, so the helpers are static over the rows handed in.
The engine's vista and the markup clerk number through it.

## `public static string[] LTwinRead(IReadOnlyList<LEntry> entries)`

The twin name of each entry, by position, numbered by entry id.
So the older entry is `(1)` in every view.
Only entries of one language are twins, so one headword in two languages stays bare.

## `public static string[] LTwinRead<LTwinRow>(IReadOnlyList<LTwinRow> rows, Func<LTwinRow, string> name, Func<LTwinRow, string> group, Func<LTwinRow, long> id)`

The same numbering over any row kind, given how to read a row's name, group and id.
Only rows of one group are twins.

## `public static IReadOnlyList<string> LTwinNameResolve(IReadOnlyList<string> labels)`

The labels made distinct in their given order, numbered where two share a name, for the compass rows.
Labels carry no language, so they all form one group.

## `public static string LTwinNameRead(LStateValue value, string unknown, string fallback)`

The wording a catalog row shows for a state value.
An unknown value shows `unknown`, an empty one shows `fallback`, and any other shows its text.
