# TEngineLiveryCard.cs
Hash: `cbcc615c59c43e13`

## `public sealed class TEngineLiveryCard`

Writes Joplin bodies through the `TLiveryFormat` relay, one fact per script, card or incoming part.

## `private const string TLiveryPicture`

A one-pixel PNG in base64, so the picture parser reads a real image.

## `public void LiveryFormat_ScriptImage_AddsImageParcel()`

Gives a stored page one script group with one image, caption and quote.
Checks that the image becomes one PNG parcel.
Checks that the style, image, caption and quote print in that order.

## `public void LiveryFormat_MeaningCard_KeepsContentOrder()`

Gives a stored page one meaning card holding every content part, a translation target and a byline.
Checks that the heading, title, meaning, situation, register, translation, child, example, tag, image and video print in order.

## `public void LiveryFormat_StoredBanner_DrawsLanguageFlags()`

Gives a page one Korean translation target, one Korean incoming usage and one Korean gloss.
Its `LLiveryPageBanner` maps Korean to a stored PNG file.
Checks that the target chip, the incoming language and the gloss each open with that flag.

## `public void LiveryFormat_IncomingUsage_LinksEntryNote()`

Gives a stored page one incoming usage from a meaning.
Checks that the usage links its entry's note under the incoming heading and carries the owner chip.

## `public void LiveryFormat_VideoFile_AddsVideoParcel()`

Gives a stored page one meaning card with a stored video file.
Checks that the file becomes one video parcel behind a Markdown link, with no `<video>` tag left.

## `private static LLiveryPage TLiveryPageRead(LEngine engine, string headword)`

Saves an English entry with one meaning under `headword` and reads its page through `TLiveryRead`.
