# LScriptGroup.cs

## `public sealed record LScriptGroup(`

One block of the script box: the images of one character in one style.
The engine groups the images, so the shell only draws the blocks it is handed.

**Parameters**

- `LScriptGroupHeading` — The character, printed once at its first block when the entry has several.
- `LScriptGroupStyle` — The style name the images share.
- `LScriptGroupGloss` — The first gloss any image of the block carries.
- `LScriptGroupImages` — The images of the block, as the archive stores them.

## `public static IReadOnlyList<LScriptGroup> LScriptGroupScan(`

Groups the images by character and then by style, in the order the pack lists the styles.
