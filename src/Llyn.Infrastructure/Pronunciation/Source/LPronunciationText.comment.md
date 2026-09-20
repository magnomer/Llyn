# LPronunciationText.cs

## `internal static class LPronunciationText`

Extracts and normalizes pronunciation text scraped from HTML.
Tolerant on purpose.
Given a pattern that matches an IPA element's opening tag, it reads the full inner content.
It walks nested spans to the matching close.
It strips any inner markup and returns every non-empty phonetic form.
This survives real markup such as Cambridge's `...əl`, where naive "up to the first " matching would truncate the value.

## `public static IReadOnlyList<string> LPronunciationTextScan(string html, Regex openPattern, int skip)`

Returns the inner text of every element `openPattern` opens in `html`, from the skipped match onward, in page order.
`openPattern` must match an IPA element's opening tag.
Each text still carries its delimiters and entities, so the caller splits and normalizes it.
An element whose close is never found, or whose text is blank, is passed over.

## `public static string LPronunciationTextNormalize(string raw)`

Decodes HTML entities and strips enclosing IPA delimiters and a stray comma, so the caller stores just the phonetic characters.
The UI supplies its own surrounding brackets.

## `private static string? LPronunciationInnerRead(string html, int start)`

Reads a span's inner HTML starting just after its opening tag.
It returns the substring up to the matching `` while accounting for nested spans.
It returns `null` when the close is never found.
