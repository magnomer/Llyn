# CSoundingScript.cs
Hash: `1433c6b014e79b07`

## `public sealed record CSoundingScript(IReadOnlyList<CScriptGroup> CSoundingScriptGroups, bool CSoundingScriptPending, bool CSoundingScriptRebuildable, CFont CSoundingScriptFont)`

The script block of the editor for the stored entry, ready to show.

**Parameters**

- `CSoundingScriptGroups`: the script images grouped by style.
- `CSoundingScriptPending`: whether the images are still being fetched.
- `CSoundingScriptRebuildable`: whether the user may fetch the images again.
- `CSoundingScriptFont`: the pack's glyph typography for the draft's language.
