# LLiveryHeader.cs
Hash: `fe042a92f569e64a`

## `internal static class LLiveryHeader`

Writes the head and the chips of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage` and the `lookup` it is handed.

## `private const string LLiveryEscaped = "\\\u0060*_[]|~#!";`

The characters `LLiveryTextFormat` escapes with a backslash, so stored text never turns into Markdown.

## `public static void LLiveryHeaderAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes one level-one heading holding the headword, the language chip, the heart, the stars and the rating.
The language chip is left out when the draft names no language.
The heart, the stars and the rating sit in one `llyn-regard` span.
The style note lays that span out as a flex row, which drops the spaces between them.
So no 40-pixel heading space widens a gap between two stars.
The heart carries `llyn-on` when the entry is a favorite.
The grasp is clamped through `LGrasp.LGraspNormalize` before the stars and the rating key are read.
The rating text comes from `LGrasp.LGraspKeyRead` of the clamped grasp.
The language chip opens with the flag from `LLiveryBannerFormat` for the draft's language, when one reads.

## `public static void LLiveryChipAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes the speech chips as one paragraph, then the frequency chip as another.
The speech paragraph is written only when the draft is marked.
The lexical unit leads the speech paragraph as a `llyn-unit` chip, named through `lookup`.
The freedom follows it as a `llyn-freedom` chip, named the same way.
Each part is left out when the page holds no data for it.
The trailing space is trimmed only when this method wrote a chip.
A marked draft with no unit, freedom or named speech writes nothing at all.
So text written before this method is never cut.
`LFrequencyGauge.LFrequencyGaugeResolve` reads the stored rows with the `Frequency.Once` text.
The chip names its band through the `Frequency.` key of the gauge rank.
A ranked gauge adds a band class such as `llyn-frequency-core`, so the style note colours each band.
An unranked gauge adds no band class.
Its title holds the gauge source, one line per row.
Each band step draws one pip, and the steps the band reaches carry `llyn-on`.

## `internal static string LLiveryTextFormat(string text)`

Escapes stored text for Markdown inside the body.
Line breaks become spaces, so a value never splits a table row or a heading.
Markdown characters get a backslash first, then HTML characters are encoded.
`LLiverySound` writes every stored value through it too.

## `internal static string LLiveryFlagFormat(string? path)`

`LLiveryPictureFormat` over `path` with class `llyn-flag`.
`LLiverySound` uses it for the flags of variety rows.

## `internal static string LLiveryBannerFormat(LLiveryPage page, string language)`

`LLiveryFlagFormat` over the path `LLiveryPageBanner` holds for `language`.
A language with no entry answers empty.
`LLiveryCard` uses it for the flags of translation chips, incoming rows and gloss lines.

## `internal static string LLiveryPictureFormat(string? path, string style)`

An `<img>` with class `style` whose source is the stored file at `path` as a base64 data address.
`LLiverySheet.LLiveryImageApply` later turns that address into a Joplin resource.
The file is read through `LPortraitAsset.LPortraitAssetLoad`, the loader the export uses.
So the media type rule and the size ceiling are one copy shared with the export.
A null path, or a file that loader refuses, answers empty.
`LLiveryCard` uses it for the stored image files of a card.

## `private static void LLiveryStarAppend(StringBuilder sheet, int grasp)`

Writes one star span per fill `LGrasp.LGraspStarScan` answers.
Core owns the fill rule, so the view's stars and these agree.
A full star carries `llyn-on` and a half star carries `llyn-half`.
At grasp zero every star carries `llyn-unrated`, so the style note tells unrated from rated.
