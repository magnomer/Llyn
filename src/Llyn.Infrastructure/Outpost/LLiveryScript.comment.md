# LLiveryScript.cs
Hash: `ff9a864059db6a9f`

## `internal static class LLiveryScript`

Writes the script card of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage` and the `lookup` it is handed.

## `public static void LLiveryScriptAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

`LLiverySheet.LLiveryFormat` calls it right after `LLiveryRime.LLiveryRimeAppend`.
Every group of `LLiveryPageScript` goes into one `llyn-card` div, with a blank line inside both ends.
A group's heading character, when set, opens it as a `llyn-heading` chip.
The group's style follows as a `llyn-style` chip, then its images in one row.
Each image goes through `LLiveryFigureFormat`.
The group's gloss follows as a `llyn-quote` line.
The variant group is a style like any other, so its glyphs follow in the same card.
No groups write nothing.

## `private static string LLiveryFigureFormat(LScriptImage image, Func<string, string> lookup)`

A `llyn-figure` span holding the image and its caption under it.
The image is an `<img>` with a PNG data address, which `LLiverySheet.LLiveryImageApply` turns into a resource.
An image without bytes writes no `<img>`.
The caption holds the epoch through its `Epoch.` key, then the stored caption.
An image without epoch or caption writes no caption.
