# LPronunciationText.cs

## `internal static class LPronunciationText`

Extracts and normalizes pronunciation text scraped from HTML. Tolerant on purpose: given a pattern that matches an IPA element's opening tag, it reads the element's full inner content by walking nested spans to the matching close, strips any inner markup, and returns the first non-empty phonetic form. This survives real markup such as Cambridge's `...əl`, where naive "up to the first " matching would truncate the value.

## `public static string? LPronunciationTextRead(string html, Regex openPattern)`

Returns the first non-empty normalized phonetic found by `openPattern` (which must match an IPA element's opening tag) across all matches in `html`, or `null` when none qualifies.

## `public static string LPronunciationTextNormalize(string raw)`

Decodes HTML entities and strips enclosing IPA delimiters, so the caller stores just the phonetic characters. The UI supplies its own surrounding brackets.

## `private static string? LPronunciationTextScan(string html, int start)`

Reads a span's inner HTML starting just after its opening tag, returning the substring up to the matching `` while accounting for nested spans, or `null` when the close is never found.
