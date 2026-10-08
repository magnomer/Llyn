# CDisplayAccent.cs
Hash: `9fddfe5a415f4f7e`

## `public sealed class CDisplayAccent`

The reading view's pronunciation area, holding the reads of its accent block and the contour scale.
It is split from [CDisplaySound](CDisplaySound.comment.md) by concern, since the block reads through the language port alone.
The header area [CDisplay](CDisplay.comment.md) builds one over its rules.
So every driver over that area hears the same shown draft.
It holds no state of its own.
The shown draft stays in [LDisplaySound](LDisplaySound.comment.md).
Every read answers the mute block while nothing is shown, so a late notice paints what a close painted.

## `internal CDisplayAccent(LDisplay display, LLanguagePort languages, LSettingsPort settings, CEnvoy envoy)`

Only the header area builds its pronunciation area, over the port and the envoy the atelier handed down.
It takes the sound half and the atelier's repaint memory from `display`.
Every read below shows its failure through that memory, since it runs on every repaint.

## `public IReadOnlyList<int> CDisplayAccentScale`

The pitch levels a tone contour draws, highest first, one guide line each.
It comes from the language port, so the scale keeps the one owner that parses the levels.

## `public CLecternAccent CDisplayAccentRead()`

The shown entry's pronunciation block, ready to draw.
The engine answers the readings, the brackets and the pack's verdicts in one read.
Conduct only chooses each variety's label key through `CVariety.CVarietyRead`.
Nothing shown answers the mute block, and a refused read shows `Sound.LoadFailed` once and answers it too.

## `public async Task<CLecternAccent?> CDisplayAccentLoad(Func<IReadOnlyList<CEnsignRow>, Action<string, Exception>, Action> store)`

The load a driver starts once an entry opened, for the flags its pronunciation block draws.
The engine picks the varieties and loads nothing when the pack shows no flags.
`store` is the driver's own image store, handed the rows through the one flag map.
It answers the block again once the flags are in, so the driver repaints its rows.
Another entry shown meanwhile wins, so a late load answers null and paints nothing.
A failed load shows `Sound.LoadFailed` once and answers null, like `CDisplayAccentRead`.
