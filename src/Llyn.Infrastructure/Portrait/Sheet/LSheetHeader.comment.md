# LSheetHeader.cs
Hash: `51370f27bc7b37bc`

## `public static class LSheetHeader`

Writes the header of an exported entry body, as `PDisplayHeader` lays it out.
Every value comes from the Livery page, and every decision from a shared Core helper.

## `public static void LSheetHeaderAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<string> ensigns, Func<string, string> lookup)`

Writes the headword, the reading line, the language pill, the heart, the stars and the grasp label.
The headword takes the face and size of the headword font, so it matches the view's lettering.
Only those two values go inline, and every other look is left to the style.
A quote or backslash in the face is escaped, so the CSS string stays closed.
The reading line prints only when `LLiveryPageReading` holds text.
The grasp is clamped through `LGrasp.LGraspNormalize` first, the same clamp the Livery uses.
The stars come from `LGrasp.LGraspStarScan`, so the export never counts them itself.
The label is the localized text of `LGrasp.LGraspKeyRead`.
The heart is an empty span marked `on` for a favourite.
The style paints the embedded `favorite` or `favorite-marked` icon into it.

## `internal static void LSheetLanguageAppend(StringBuilder sheet, LLiveryPage page, IReadOnlyList<string> ensigns)`

Writes the language pill with the banner's ensign class, or the globe when no flag reads.
This is the pick `QLecternHeader` makes between the flag image and the globe.
An entry without a language writes no pill.

## `private static void LSheetStarAppend(StringBuilder sheet, int grasp)`

Writes one empty star per scanned value, marking it full, half or empty.
The star carries no glyph, since the style masks the embedded `star` icon over its colour.
