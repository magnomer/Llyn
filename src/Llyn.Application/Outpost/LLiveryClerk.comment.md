# LLiveryClerk.cs
Hash: `be40808ecb01245b`

## `public static class LLiveryClerk`

Holds the builders `LLiveryFacade` calls to turn engine answers into an `LLiveryPage` or an `LLiveryLanguage`.

## `public static LLiveryPage LLiveryClerkBuild(LEntryDraft draft, bool favorite, int grasp, string created, string updated, LAccentSheet accent, IReadOnlyList<LReflexGuise> guise, IReadOnlyList<string> folded, IReadOnlySet<long> fold, IReadOnlyList<LTranscriptionDraft> transcription, LGlyph? glyph, IReadOnlyList<LGlyphCell> cell, IReadOnlyList<LFrequency> frequency, IReadOnlyList<LParadigmRow> paradigm, bool pending, bool enabled, LParadigmView? inflection, IReadOnlyList<LFanqieGroup> fanqie, IReadOnlyList<LScriptGroup> script, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> target, IReadOnlyDictionary<long, string> source, IReadOnlyList<LUsage> incoming, IReadOnlyList<LTranslationTarget> etymon, Func<string, string?> flag, IReadOnlyDictionary<string, string> ensign, IReadOnlyDictionary<LFontRole, LFont> font, IReadOnlySet<LFoldBox> opened, string reading)`

`LLiveryFacade.LEngineLiveryRead` calls it once per stored entry.
It hands each answer unchanged to the `LLiveryPage` constructor, except `flag`, `pending` and `enabled`.
It builds `LLiveryPageShown` with `LParadigmClerk.LParadigmClerkCheck`, one per paradigm row in row order.
Each check passes `pending` and `enabled` and keeps `held` off, since a page never holds a fill.
That is the check the view's `LEngineParadigmCheck` makes, so both show the same row text.
It collects every language the page names, from the draft, its reflexes, targets, usages, etymons and glosses.
Glosses are read from every card and every nested child card.
It asks `flag` once per distinct language and keeps each path it answers as `LLiveryPageBanner`.
A language `flag` answers null for gets no entry.

## `public static LLiveryLanguage LLiveryClerkBuild(string language, LStemClerk? stem, LDiweiClerk? diwei, bool switched, bool respelled, bool tallied, Func<string, string?> localize)`

`LLiveryFacade.LEngineLiveryRead` calls it once per language, holding the engine gate.
A null `stem` means the language has no series rule, so the series list stays empty.
Otherwise it lists every series in name order with an empty query.
Each series carries its `LStemPageRead` page and the entries `LStemEntryScan` finds for it alone.
The page is read with folds, so each member carries its stored Series fold as `LStemMemberOpened`.
The sheet writes that state as an open or closed readings block.
`respelled` is the raw respelling setting, so each member reads its guises as the entry page does.
Those entries are in headword order, since the export has no panel setting to follow.
A null `diwei` means the language has no fanqie book, so the category list stays empty.
Otherwise it lists the initials, then the rimes, then the tones, each kind in name order.
Each category carries its kind, its `LDiweiPageRead` page and the entries `LDiweiEntryScan` finds for it alone.
Those entries are in headword order too.
`switched`, `tallied` and `localize` pass unchanged to `LDiweiPageRead`, as `LEngineDiweiResolve` passes them.
