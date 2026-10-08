# LPronunciationPort.cs
Hash: `56a634253dc58350`

## `public interface LPronunciationPort`

The slice of the engine a deportment sees when it lists pronunciations or shows how often an entry is heard.
It also reads the IPA charts the input aid lays out.
`LPronunciationFacade` implements it.

## `IReadOnlyList<LCatalogPronunciation> LEnginePronunciationFind(LVista vista);`

The pronunciation rows of a vista, twinned names and the chosen mark applied.

## `LFrequencyGauge? LEngineFrequencyResolve(long entryId, string once);`

The entry's frequency gathered into one answer, or null when it has none.
The `once` text words a word interval.

## `LArticulation LEngineConsonantRead();`

The IPA consonant chart, which needs no workspace, so the input aid reads it at any time.

## `LArticulation LEngineVowelRead();`

The IPA vowel chart, which needs no workspace, so the input aid reads it at any time.
