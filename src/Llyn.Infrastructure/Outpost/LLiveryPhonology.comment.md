# LLiveryPhonology.cs
Hash: `586fd790c3aeccc7`

## `internal static class LLiveryPhonology`

Writes the body of a language's sound note for `LLiverySheet.LLiveryFormat`.
It reads only `LLiveryLanguage` and the `note` map it is handed.

## `public static void LLiveryPhonologyAppend(StringBuilder sheet, LLiveryLanguage language, Func<long, string> note)`

Writes one two-column Markdown table row per row of `LLiveryLanguagePronunciation`, in record order.
The left cell links the headword through `LLiveryEtymology.LLiveryLinkFormat`.
The right cell is `LCatalogPronunciationText`, so a missing sound shows as `[ ]`.
No other part of a row is written.
The table sits inside a `llyn-phonology` div, which keeps it apart from the entry sound rows.
No rows write nothing.
