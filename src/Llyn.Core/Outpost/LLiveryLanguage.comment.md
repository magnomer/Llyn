# LLiveryLanguage.cs
Hash: `2d5b77e8f979c4cd`

## `public sealed record LLiveryLanguage(string LLiveryLanguageName, IReadOnlyList<LCatalogPronunciation> LLiveryLanguagePronunciation, IReadOnlyList<LLiveryStem> LLiveryLanguageStem, IReadOnlyList<LLiveryDiwei> LLiveryLanguageDiwei)`

The whole reconstruction side of one language, the platform-free model of its Joplin notes.
`LLiveryClerk.LLiveryClerkBuild` constructs it from what `LLiveryFacade.LEngineLiveryRead` reads for the language.
The Joplin push renders its sound note and every series and category note from it.

**Parameters**

- `LLiveryLanguageName` — The language the read was asked for.
- `LLiveryLanguagePronunciation` — Only the pronunciation rows whose entry belongs to the language, in name order.
- `LLiveryLanguageStem` — Every series of the language in name order, empty without a series rule.
- `LLiveryLanguageDiwei` — The initials, then the rimes, then the tones, empty without a fanqie book.
