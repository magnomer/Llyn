# LLiveryPage.cs
Hash: `42ba306be5ca3197`

## `public sealed record LLiveryPage(LEntryDraft LLiveryPageDraft, bool LLiveryPageFavorite, int LLiveryPageGrasp, string LLiveryPageCreated, string LLiveryPageUpdated, LAccentSheet LLiveryPageAccent, IReadOnlyList<LReflexGuise> LLiveryPageGuise, IReadOnlyList<string> LLiveryPageFolded, IReadOnlySet<long> LLiveryPageFold, IReadOnlyList<LTranscriptionDraft> LLiveryPageTranscription, LGlyph? LLiveryPageGlyph, IReadOnlyList<LGlyphCell> LLiveryPageCell, IReadOnlyList<LFrequency> LLiveryPageFrequency, IReadOnlyList<LParadigmRow> LLiveryPageParadigm, LParadigmView? LLiveryPageInflection, IReadOnlyList<LFanqieGroup> LLiveryPageFanqie, IReadOnlyList<LScriptGroup> LLiveryPageScript, IReadOnlyDictionary<long, IReadOnlyList<LTranslationTarget>> LLiveryPageTarget, IReadOnlyDictionary<long, string> LLiveryPageSource, IReadOnlyList<LUsage> LLiveryPageIncoming, IReadOnlyList<LTranslationTarget> LLiveryPageEtymon, IReadOnlyDictionary<string, string> LLiveryPageBanner, IReadOnlyDictionary<string, string> LLiveryPageEnsign)`

`LLiveryClerk.LLiveryClerkBuild` constructs it from the answers `LLiveryFacade.LEngineLiveryRead` gathers.
`LCourierClerk.LCourierClerkSend` passes the read on, and `LCourierNote.LCourierEntrySend` hands one per entry to `LLivery.LLiveryFormat`.

**Parameters**

- `LLiveryPageDraft` comes from `LEngineEntryLoad`.
- `LLiveryPageFavorite` comes from `LEngineFavoriteCheck`.
- `LLiveryPageGrasp` comes from `LEngineGraspRead`.
- `LLiveryPageCreated` is the second answer of `LEngineStampRead`.
- `LLiveryPageUpdated` is the third answer of `LEngineStampRead`.
- `LLiveryPageAccent` comes from `LEngineAccentRead` on the draft.
- `LLiveryPageGuise` comes from `LEngineGuiseRead`, one guise per reflex of the draft in draft order.
- `LLiveryPageFolded` comes from `LReflexClerk.LReflexFoldedRead` for the draft's language.
- `LLiveryPageFold` holds the ids of the folded meaning and collocation cards, from `LFoldClerkRead`.
  `LLiveryPageFold` holds card ids and `LLiveryPageFolded` holds reflex languages.
  A fold toggle changes the note, so the next push re-sends it.
- `LLiveryPageTranscription` comes from `LEngineTranscriptionRead` on the draft.
- `LLiveryPageGlyph` comes from `LEngineGlyphRead` for the draft's language.
- `LLiveryPageCell` comes from `LEngineGlyphDivide` on the draft.
- `LLiveryPageFrequency` holds the stored rows from `LFrequencyClerk.LFrequencyClerkRead` with fetch off.
- `LLiveryPageParadigm` comes from `LEngineParadigmScan`.
- `LLiveryPageInflection` comes from `LEngineInflectionRead`, null when the language has no inflection layout.
  The paradigm rows drop the layout's part of speech, so only this table carries those forms.
- `LLiveryPageFanqie` comes from `LEngineFanqieDivide`.
- `LLiveryPageScript` comes from `LEngineScriptDivide`.
- `LLiveryPageTarget` comes from `LEngineTranslationRead` on the draft.
- `LLiveryPageSource` comes from `LEngineCitationRead` on the draft.
- `LLiveryPageIncoming` comes from `LEngineIncomingRead`.
- `LLiveryPageEtymon` comes from `LEngineEtymonRead` on the draft.
- `LLiveryPageBanner` maps each language the page names to its stored flag file path from `LLanguageFlagFind`.
  A language with no stored flag file has no entry in it.
- `LLiveryPageEnsign` maps each flagged variety to its stored flag file path from `LVarietyFlagScan`.
