# CLecternScript.cs
Hash: `b793b552979ff773`

## `public sealed record CLecternScript(IReadOnlyList<CScriptGroup> CLecternScriptGroups, bool CLecternScriptPending, CFont CLecternScriptFont)`

The script block of the reading view for the shown entry, ready to show.

**Parameters**

- `CLecternScriptGroups`: the script images grouped by style.
- `CLecternScriptPending`: whether the images are still being fetched.
- `CLecternScriptFont`: the pack's glyph typography for the shown language.
