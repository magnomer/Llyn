# LLiveryClerk.cs
Hash: `925286e30775c0d2`

## `public static class LLiveryClerk`

Holds the builders `LLiveryFacade` calls to turn engine answers into an `LLiveryPage` or an `LLiveryLanguage`.

## `public static LLiveryPage LLiveryClerkBuild(LEntryDraft draft, bool favorite, int grasp, string created, string updated, LAccentSheet accent, IReadOnlyList<LReflexGuise> guise, IReadOnlyList<string> folded, IReadOnlyList<LTranscriptionDraft> transcription, LGlyph? glyph, IReadOnlyList<LGlyphCell> cell, IReadOnlyList<LFrequency> frequency, IReadOnlyList<LParadigmRow> paradigm, IReadOnlyList<LFanqieGroup> fanqie, IReadOnlyList<LScriptGroup> script, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> target, IReadOnlyDictionary<long, string> source, IReadOnlyList<LUsage> incoming, IReadOnlyList<LTranslationTarget> etymon, Func<string, string?> flag, IReadOnlyDictionary<string, string> ensign)`

`LLiveryFacade.LEngineLiveryRead` calls it once per stored entry.
It hands each answer unchanged to the `LLiveryPage` constructor, in member order, except `flag`.
It collects every language the page names, from the draft, its reflexes, targets, usages, etymons and glosses.
Glosses are read from every card and every nested child card.
It asks `flag` once per distinct language and keeps each path it answers as `LLiveryPageBanner`.
A language `flag` answers null for gets no entry.

## `public static LLiveryLanguage LLiveryClerkBuild(string language, IReadOnlyList<LCatalogPronunciation> pronunciation, LStemClerk? stem, LDiweiClerk? diwei, bool switched, bool tallied, Func<string, string?> localize)`

`LLiveryFacade.LEngineLiveryRead` calls it once per language, holding the engine gate.
It keeps the pronunciation rows whose entry belongs to the language, in the order they arrive.
An entry's language is trimmed before the match, since the courier passes trimmed names.
A null `stem` means the language has no series rule, so the series list stays empty.
Otherwise it lists every series in name order with an empty query.
Each series carries its `LStemPageRead` page and the entries `LStemEntryScan` finds for it alone.
A null `diwei` means the language has no fanqie book, so the category list stays empty.
Otherwise it lists the initials, then the rimes, then the tones, each kind in name order.
Each category carries its kind, its `LDiweiPageRead` page and the entries `LDiweiEntryScan` finds for it alone.
`switched`, `tallied` and `localize` pass unchanged to `LDiweiPageRead`, as `LEngineDiweiResolve` passes them.
