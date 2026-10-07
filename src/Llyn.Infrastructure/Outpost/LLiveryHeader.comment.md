# LLiveryHeader.cs
Hash: `4e421f227da79359`

## `internal static class LLiveryHeader`

Writes the head and the chips of a Joplin entry body for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryPage` and the `lookup` it is handed.

## `private const int LLiveryStarCount = 5;`

The number of stars the rating row draws.

## `private const int LLiveryGraspCeiling = 10;`

The highest grasp, which fills every star.
A grasp is clamped to it before the stars and the rating key are read.

## `private const string LLiveryEscaped = "\\\u0060*_[]|~#!";`

The characters `LLiveryTextFormat` escapes with a backslash, so stored text never turns into Markdown.

## `public static void LLiveryHeaderAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes one level-one heading holding the headword, the language chip, the heart, the stars and the rating.
The language chip is left out when the draft names no language.
The heart carries `llyn-on` when the entry is a favorite.
The rating text comes from the `Grasp.Level` key of the clamped grasp.
The language chip opens with the flag from `LLiveryBannerFormat` for the draft's language, when one reads.

## `public static void LLiveryChipAppend(StringBuilder sheet, LLiveryPage page, Func<string, string> lookup)`

Writes the speech chips as one paragraph, then the frequency chip as another.
Each part is left out when the page holds no data for it.
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
The media type follows the file suffix, and an unknown suffix answers empty.
A null path, or a file that throws `IOException` or `UnauthorizedAccessException`, answers empty.
`LLiveryCard` uses it for the stored image files of a card.

## `private static void LLiveryStarAppend(StringBuilder sheet, int grasp)`

Writes five star spans, each worth two grasp steps.
A full star carries `llyn-on` and a star worth one step carries `llyn-half`.
At grasp zero every star carries `llyn-unrated`, so the style note tells unrated from rated.
