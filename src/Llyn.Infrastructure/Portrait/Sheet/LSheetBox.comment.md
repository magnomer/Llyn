# LSheetBox.cs
Hash: `b32b74e843b28e08`

## `internal static class LSheetBox`

Writes the folded fanqie and script boxes of an exported entry body.
Each is a `details` element, so the reader opens and closes it without script.

## `internal static void LSheetFanqieAppend(StringBuilder sheet, LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label)`

Writes the fanqie section in a box placed as `PDisplayFanqie`, after the paradigm box.
It starts open only when `LLiveryPageOpened` holds `LFoldBoxFanqie`, the state the view restores.

## `internal static void LSheetScriptAppend(StringBuilder sheet, LLiveryPage livery, LPortraitPage portrait, LPortraitLabel label)`

Writes the script section in a box placed as `PDisplayScript`, after the note.
It starts open only when `LLiveryPageOpened` holds `LFoldBoxScript`.

## `private static void LSheetBoxAppend(StringBuilder sheet, LPortraitSection? section, string style, bool opened)`

The section's heading becomes the summary, so the folded box still names itself.
The section itself is written without its heading, so the band shows no second title.
A page without the section writes no box.
